using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Audio;

namespace VBAudioRouter.Capture
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public class AudioProcessCapture : IDisposable
    {
        public Process Process { get; }
        public bool IncludeProcessTree { get; }
        public AudioProcessCapture(Process process, bool include = true)
        {
            this.Process = process;
            this.IncludeProcessTree = include;
        }

        AudioClient client;
        AudioCaptureClient captureClient;
        WaveFormat format;

        // https://github.dev/microsoft/Windows-classic-samples/blob/main/Samples/ApplicationLoopback/cpp/LoopbackCapture.cpp
        public AudioFrameInputNode CreateAudioNode(AudioGraph graph)
        {
            var audioNode = graph.CreateFrameInputNode();

            var nodeFormat = audioNode.EncodingProperties;
            int blockAlign = (int)(nodeFormat.ChannelCount * nodeFormat.BitsPerSample / 8);
            format = WaveFormat.CreateCustomFormat(WaveFormatEncoding.IeeeFloat, (int)nodeFormat.SampleRate, (int)nodeFormat.ChannelCount, (int)nodeFormat.SampleRate * blockAlign, blockAlign, (int)nodeFormat.BitsPerSample);

            audioNode.QuantumStarted += AudioNode_QuantumStarted;
            return audioNode;
        }

        unsafe void AudioNode_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            if (disposed)
                return;

            // Client need to be activated in here so that we don't run into threading problems
            // This method get's called on an MTA worker thread from native code
            if (client == null)
            {
                const uint AUDCLNT_STREAMFLAGS_AUTOCONVERTPCM = 0x80000000;
                client = new(ActivateAudioClientInternal());
                client.Initialize(
                    AudioClientShareMode.Shared,
                    AudioClientStreamFlags.Loopback,
                    5 * 10_000_000,
                    AUDCLNT_STREAMFLAGS_AUTOCONVERTPCM,
                    format,
                    Guid.Empty);

                captureClient = client.AudioCaptureClient;
                client.Start();
            }

            int availablePackages = captureClient.GetNextPacketSize();
            if (availablePackages != 0)
            {
                uint bytesToCapture = (uint)(format.BlockAlign * availablePackages);
                AudioFrame frame = new(bytesToCapture);
                using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
                using (var reference = buffer.CreateReference())
                {
                    unsafe
                    {
                        ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* targetBuffer, out _);
                        byte* srcBuffer = (byte*)captureClient.GetBuffer(out var numFrames, out _);
                        Buffer.MemoryCopy(srcBuffer, targetBuffer, bytesToCapture, bytesToCapture);
                        captureClient.ReleaseBuffer(numFrames);
                    }
                }
                sender.AddFrame(frame);
            }
        }

        bool disposed = false;
        void IDisposable.Dispose()
        {
            disposed = true;
            client.Dispose();
            client = null;
            captureClient.Dispose();
            captureClient = null;
        }

        IAudioClient ActivateAudioClientInternal()
        {
            const string VIRTUAL_AUDIO_DEVICE_PROCESS_LOOPBACK = @"VAD\Process_Loopback";

            ActivateAudioInterfaceCompletionHandler<IAudioClient> completionHandler = new();

            AudioClientHelper.ActivationParameters @params = new AudioClientHelper.ActivationParameters();
            @params.Type = AudioClientHelper.ActivationType.ProcessLoopback;
            @params.ProcessLoopbackParams.TargetProcessId = Process.Id;
            @params.ProcessLoopbackParams.ProcessLoopbackMode = IncludeProcessTree ? AudioClientHelper.LoopbackMode.IncludeProcessTree : AudioClientHelper.LoopbackMode.ExcludeProcessTree;

            PropVariant propVariant = new PropVariant();
            propVariant.vt = (short)VarEnum.VT_BLOB;

            var size = Marshal.SizeOf(@params);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(@params, ptr, false);

            propVariant.blobVal.Data = ptr;
            propVariant.blobVal.Length = size;

            Marshal.ThrowExceptionForHR(ActivateAudioInterfaceAsync(VIRTUAL_AUDIO_DEVICE_PROCESS_LOOPBACK, typeof(IAudioClient).GUID, ref propVariant, completionHandler, out var resultHandler));
            completionHandler.WaitForCompletion();
            Marshal.ThrowExceptionForHR(resultHandler.GetActivateResult(out _, out var result));
            return (IAudioClient)result;
        }

        [PreserveSig]
        [DllImport("Mmdevapi", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern int ActivateAudioInterfaceAsync(
            [MarshalAs(UnmanagedType.LPWStr)] string deviceInterfacePath,
            Guid riid,
            ref PropVariant activationParams,
            IActivateAudioInterfaceCompletionHandler completionHandler,
            out IActivateAudioInterfaceAsyncOperation result
        );
    }

    /// <summary>
    /// Represents an asynchronous operation activating a WASAPI interface and provides a method to retrieve the results of the activation.
    /// </summary>
    [ComImport]
    [Guid("72A22D78-CDE4-431D-B8CC-843A71199B6D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IActivateAudioInterfaceAsyncOperation
    {
        [PreserveSig]
        int GetActivateResult(out int activateResult, [MarshalAs(UnmanagedType.IUnknown)] out object activatedInterface);
    }

    /// <summary>
    /// Provides a callback to indicate that activation of a WASAPI interface is complete.
    /// </summary>
    [ComImport]
    [Guid("41D949AB-9862-444A-80F6-C261334DA5EB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IActivateAudioInterfaceCompletionHandler
    {
        /// <summary>
        /// Indicates that activation of a WASAPI interface is complete and results are available.
        /// </summary>
        /// <param name="activateOperation"></param>
        void ActivateCompleted(IActivateAudioInterfaceAsyncOperation activateOperation);
    }

    public class ActivateAudioInterfaceCompletionHandler<T> : IActivateAudioInterfaceCompletionHandler where T : class
    {
        AutoResetEvent _completionEvent = new(false);
        void IActivateAudioInterfaceCompletionHandler.ActivateCompleted(IActivateAudioInterfaceAsyncOperation activateOperation)
            => _completionEvent.Set();

        public void WaitForCompletion()
            => _completionEvent.WaitOne();
    }

    namespace AudioClientHelper
    {
        /// <summary>
        /// Specifies the activation parameters for a call to <see cref="Helper.ActivateAudioInterfaceAsync(String, Guid, ByRef PropVariant, ByRef IActivateAudioInterfaceCompletionHandler)"/>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ActivationParameters
        {
            /// <summary>
            /// A member of the <see cref="ActivationType">AUDIOCLIENT_ACTIVATION_TYPE</see> specifying the type of audio interface activation. <br/>
            /// Currently default activation and loopback activation are supported.
            /// </summary>
            public ActivationType Type;
            /// <summary>
            /// A <see cref="ProcessLoopbackParams">AUDIOCLIENT_PROCESS_LOOPBACK_PARAMS</see> specifying the loopback parameters for the audio interface activation.
            /// </summary>
            public ProcessLoopbackParams ProcessLoopbackParams;
        }

        /// <summary>
        /// Specifies the activation type for an <see cref="ActivationParameters">AUDIOCLIENT_ACTIVATION_PARAMS</see> structure passed into a call to <see cref="Helper.ActivateAudioInterfaceAsync(String, Guid, ByRef PropVariant, ByRef IActivateAudioInterfaceCompletionHandler)" />.
        /// </summary>
        public enum ActivationType
        {
            /// <summary>
            /// Default activation.
            /// </summary>
            Default,
            /// <summary>
            /// Process loopback activation, allowing for the inclusion or exclusion of audio rendered by the specified process and its child processes. <br/>
            /// For sample code that demonstrates the process loopback capture scenario, see the Application Loopback <see href="https://docs.microsoft.com/en-us/samples/microsoft/windows-classic-samples/applicationloopbackaudio-sample/">API Capture Sample.</see>
            /// </summary>
            ProcessLoopback
        }

        /// <summary>
        /// Specifies parameters for a call to <see cref="Helper.ActivateAudioInterfaceAsync(String, Guid, ByRef PropVariant, ByRef IActivateAudioInterfaceCompletionHandler)"/> where loopback activation is requested.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ProcessLoopbackParams
        {
            /// <summary>
            /// The ID of the process for which the render streams, and the render streams of its child processes, will be included or excluded when activating the process loopback stream.
            /// </summary>
            public int TargetProcessId;
            /// <summary>
            /// A value from the <see cref="LoopbackMode">PROCESS_LOOPBACK_MODE</see> enumeration specifying whether the render streams for the process and child processes specified in the TargetProcessId field should be included or excluded when activating the audio interface. <br />
            /// For sample code that demonstrates the process loopback capture scenario, see the <see href="https://docs.microsoft.com/en-us/samples/microsoft/windows-classic-samples/applicationloopbackaudio-sample/">Application Loopback API Capture Sample</see>.
            /// </summary>
            public LoopbackMode ProcessLoopbackMode;
        }

        /// <summary>
        /// Specifies the loopback mode for an <see cref="ActivationParameters">AUDIOCLIENT_ACTIVATION_PARAMS</see> structure passed into a call to <see cref="Helper.ActivateAudioInterfaceAsync(String, Guid, ByRef PropVariant, ByRef IActivateAudioInterfaceCompletionHandler)"/>.
        /// </summary>
        public enum LoopbackMode
        {
            /// <summary>
            /// Render streams from the specified process and its child processes are included in the activated process loopback stream.
            /// </summary>
            IncludeProcessTree,
            /// <summary>
            /// Render streams from the specified process and its child processes are excluded from the activated process loopback stream.
            /// </summary>
            ExcludeProcessTree
        }
    }
}