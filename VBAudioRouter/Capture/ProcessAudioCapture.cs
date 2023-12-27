using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Variant;
using Windows.Win32.System.WinRT;
using WinRT;
using static Windows.Win32.PInvoke;
using IActivateAudioInterfaceAsyncOperation = Windows.Win32.Media.Audio.IActivateAudioInterfaceAsyncOperation;
using IActivateAudioInterfaceCompletionHandler = Windows.Win32.Media.Audio.IActivateAudioInterfaceCompletionHandler;

namespace VBAudioRouter.Capture;

public sealed partial class ProcessAudioCapture(Process process, bool include = true) : IDisposable
{
    public Process Process
    {
        get;
    } = process;
    public bool IncludeProcessTree
    {
        get;
    } = include;

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

    private unsafe void AudioNode_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
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
                    byte* targetBuffer = default;
                    reference.As<IMemoryBufferByteAccess>().GetBuffer(&targetBuffer, out _);
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

    unsafe IAudioClient ActivateAudioClientInternal()
    {
        const string VIRTUAL_AUDIO_DEVICE_PROCESS_LOOPBACK = @"VAD\Process_Loopback";

        ActivateAudioInterfaceCompletionHandler<IAudioClient> completionHandler = new();

        ActivationParameters @params = new()
        {
            Type = ActivationType.ProcessLoopback,
            ProcessLoopbackParams = new()
            {
                TargetProcessId = Process.Id,
                ProcessLoopbackMode = IncludeProcessTree ? LoopbackMode.IncludeProcessTree : LoopbackMode.ExcludeProcessTree
            }
        };

        PROPVARIANT propVariant = new()
        {
            Anonymous =
            {
                Anonymous =
                {
                    vt = VARENUM.VT_BLOB,
                    Anonymous =
                    {
                        blob =
                        {
                            cbSize = (uint)sizeof(ActivationParameters),
                            pBlobData = (byte*)(&@params)
                        }
                    }
                }
            }
        };

        ActivateAudioInterfaceAsync(
            VIRTUAL_AUDIO_DEVICE_PROCESS_LOOPBACK,
            typeof(IAudioClient).GUID,
            propVariant,
            completionHandler,
            out var resultHandler
        ).ThrowOnFailure();

        completionHandler.WaitForCompletion();

        HRESULT hresult = default;
        resultHandler.GetActivateResult(&hresult, out var result);

        return (IAudioClient)result;
    }
}

public sealed class ActivateAudioInterfaceCompletionHandler<T> : IActivateAudioInterfaceCompletionHandler where T : class
{
    readonly AutoResetEvent _completionEvent = new(false);
    void IActivateAudioInterfaceCompletionHandler.ActivateCompleted(IActivateAudioInterfaceAsyncOperation activateOperation)
        => _completionEvent.Set();

    public void WaitForCompletion()
        => _completionEvent.WaitOne();
}

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