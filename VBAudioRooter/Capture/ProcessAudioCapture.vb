Imports System.Runtime.InteropServices
Imports NAudio.CoreAudioApi
Imports NAudio.CoreAudioApi.Interfaces
Imports Windows.Devices.Enumeration
Imports Windows.Media.Devices

Namespace Capture

    Public Class AudioProcessCapture

        Public Shared Async Sub Test()
            Dim capture As New AudioProcessCapture()
            Dim client = Await capture.GetAudioClient()
            Debugger.Break()
        End Sub

        Public ReadOnly Property Process As Process
        Public Sub New(process As Process)
            Me.Process = process
        End Sub

        Public Sub New() : End Sub

        Public Async Function GetAudioClientForDevice() As Task(Of NAudio.CoreAudioApi.AudioClient)
            Dim deviceID = (Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector())).FirstOrDefault().Id

            Dim IID_IAudioClient As Guid = New Guid("726778CD-F60A-4eda-82DE-E47610CD78AA") ' GetType(IAudioClient).GUID
            Dim completionHandler As New ActivateAudioInterfaceCompletionHandler(Of IAudioClient)

            Dim result As IActivateAudioInterfaceAsyncOperation = Nothing
            Dim result2 = Helper.ActivateAudioInterfaceAsync(deviceID, IID_IAudioClient, Nothing, completionHandler, result)

            Return New NAudio.CoreAudioApi.AudioClient(Await completionHandler)
        End Function

        Public Async Function GetAudioClient() As Task(Of NAudio.CoreAudioApi.AudioClient)
            Dim IID_IAudioClient As Guid = New Guid("726778CD-F60A-4eda-82DE-E47610CD78AA") ' GetType(IAudioClient).GUID
            Dim completionHandler As New ActivateAudioInterfaceCompletionHandler(Of IAudioClient)

            Dim params = Helper.GetActivationParameters(11424, True)
            Dim propVariant = Helper.GetPropVariant(params)

            ' Helper.ActivateAudioInterfaceAsync(Helper.VIRTUAL_AUDIO_DEVICE_PROCESS_LOOPBACK, IID_IAudioClient, params, completionHandler)
            Dim result As IActivateAudioInterfaceAsyncOperation = Nothing
            Dim result2 = Helper.ActivateAudioInterfaceAsync(Helper.VIRTUAL_AUDIO_DEVICE_PROCESS_LOOPBACK, IID_IAudioClient, propVariant, completionHandler, result)

            Return New NAudio.CoreAudioApi.AudioClient(Await completionHandler)
        End Function

    End Class

    Public Class Helper
        Public Shared Function GetActivationParameters(pid As Integer, includeTree As Boolean) As AudioClient.ActivationParameters
            Dim params As New AudioClient.ActivationParameters()
            params.ActivationType = AudioClient.ActivationType.ProcessLoopback
            params.ProcessLoopbackParams.TargetProcessId = pid
            params.ProcessLoopbackParams.ProcessLoopbackMode = If(includeTree, AudioClient.LoopbackMode.IncludeProcessTree, AudioClient.LoopbackMode.ExcludeProcessTree)
            Return params
        End Function

        Public Const VT_BLOB As Integer = 65
        Public Const VIRTUAL_AUDIO_DEVICE_PROCESS_LOOPBACK As String = "VAD\\Process_Loopback"

        Public Shared Function GetPropVariant(params As AudioClient.ActivationParameters) As PropVariant
            Dim propVariant As New PropVariant()
            propVariant.vt = VT_BLOB

            Dim size = Marshal.SizeOf(params)
            Dim ptr As IntPtr = Marshal.AllocHGlobal(size)
            Marshal.StructureToPtr(params, ptr, False)

            propVariant.blobVal.Data = ptr
            propVariant.blobVal.Length = size

            Return propVariant
        End Function

        <DllImport("Mmdevapi", SetLastError:=True, CharSet:=CharSet.Unicode), PreserveSig>
        Public Shared Function ActivateAudioInterfaceAsync(<[In], MarshalAs(UnmanagedType.LPWStr)> deviceInterfacePath As String, <[In], MarshalAs(UnmanagedType.LPStruct)> riid As Guid, <[In]> ByRef activationParams As PropVariant, <[In]> completionHandler As IActivateAudioInterfaceCompletionHandler, <Out> ByRef result As IActivateAudioInterfaceAsyncOperation) As Integer : End Function

    End Class

    ''' <summary>
    ''' Represents an asynchronous operation activating a WASAPI interface and provides a method to retrieve the results of the activation.
    ''' </summary>
    <ComImport, Guid("72A22D78-CDE4-431D-B8CC-843A71199B6D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IActivateAudioInterfaceAsyncOperation
        Sub GetActivateResult(<Out> ByRef activateResult As Integer, <Out, MarshalAs(UnmanagedType.IUnknown)> ByRef activatedInterface As Object)
    End Interface

    ''' <summary>
    ''' Provides a callback to indicate that activation of a WASAPI interface is complete.
    ''' </summary>
    <ComImport, Guid("41D949AB-9862-444A-80F6-C261334DA5EB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IActivateAudioInterfaceCompletionHandler
        ''' <summary>
        ''' Indicates that activation of a WASAPI interface is complete and results are available.
        ''' </summary>
        ''' <param name="activateOperation"></param>
        Sub ActivateCompleted(activateOperation As IActivateAudioInterfaceAsyncOperation)
    End Interface

    Public Class ActivateAudioInterfaceCompletionHandler(Of T)
        Implements IActivateAudioInterfaceCompletionHandler

#Region "Await / Async"
        Dim taskFactory As New TaskCompletionSource(Of T)
        Public Function GetAwaiter() As TaskAwaiter(Of T)
            Return taskFactory.Task.GetAwaiter()
        End Function
#End Region

        Private Sub ActivateCompleted(activateOperation As IActivateAudioInterfaceAsyncOperation) Implements IActivateAudioInterfaceCompletionHandler.ActivateCompleted
            Try
                Dim result As Object = Nothing
                Dim hr As Integer
                activateOperation.GetActivateResult(hr, result)
                If hr = 0 Then
                    taskFactory.SetResult(DirectCast(result, T))
                Else
                    taskFactory.SetException(New Win32Exception(hr))
                End If
            Catch ex As Exception
                taskFactory.SetException(ex)
                Throw
            End Try
        End Sub
    End Class

    Namespace AudioClient
        ''' <summary>
        ''' Specifies the activation parameters for a call to <see cref="Helper.ActivateAudioInterfaceAsync(String, Guid, ByRef PropVariant, ByRef IActivateAudioInterfaceCompletionHandler)"/>.
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure ActivationParameters
            ''' <summary>
            ''' A member of the <see cref="ActivationType">AUDIOCLIENT_ACTIVATION_TYPE</see> specifying the type of audio interface activation. <br/>
            ''' Currently default activation and loopback activation are supported.
            ''' </summary>
            Public ActivationType As ActivationType
            ''' <summary>
            ''' A <see cref="ProcessLoopbackParams">AUDIOCLIENT_PROCESS_LOOPBACK_PARAMS</see> specifying the loopback parameters for the audio interface activation.
            ''' </summary>
            Public ProcessLoopbackParams As ProcessLoopbackParams
        End Structure

        ''' <summary>
        ''' Specifies the activation type for an <see cref="ActivationParameters">AUDIOCLIENT_ACTIVATION_PARAMS</see> structure passed into a call to <see cref="Helper.ActivateAudioInterfaceAsync(String, Guid, ByRef PropVariant, ByRef IActivateAudioInterfaceCompletionHandler)" />.
        ''' </summary>
        Public Enum ActivationType
            ''' <summary>
            ''' Default activation.
            ''' </summary>
            [Default]
            ''' <summary>
            ''' Process loopback activation, allowing for the inclusion or exclusion of audio rendered by the specified process and its child processes. <br/>
            ''' For sample code that demonstrates the process loopback capture scenario, see the Application Loopback <see href="https://docs.microsoft.com/en-us/samples/microsoft/windows-classic-samples/applicationloopbackaudio-sample/">API Capture Sample.</see>
            ''' </summary>
            ProcessLoopback
        End Enum

        ''' <summary>
        ''' Specifies parameters for a call to <see cref="Helper.ActivateAudioInterfaceAsync(String, Guid, ByRef PropVariant, ByRef IActivateAudioInterfaceCompletionHandler)"/> where loopback activation is requested.
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure ProcessLoopbackParams
            ''' <summary>
            ''' The ID of the process for which the render streams, and the render streams of its child processes, will be included or excluded when activating the process loopback stream.
            ''' </summary>
            Public TargetProcessId As Integer
            ''' <summary>
            ''' A value from the <see cref="LoopbackMode">PROCESS_LOOPBACK_MODE</see> enumeration specifying whether the render streams for the process and child processes specified in the TargetProcessId field should be included or excluded when activating the audio interface. <br />
            ''' For sample code that demonstrates the process loopback capture scenario, see the <see href="https://docs.microsoft.com/en-us/samples/microsoft/windows-classic-samples/applicationloopbackaudio-sample/">Application Loopback API Capture Sample</see>.
            ''' </summary>
            Public ProcessLoopbackMode As LoopbackMode
        End Structure

        ''' <summary>
        ''' Specifies the loopback mode for an <see cref="ActivationParameters">AUDIOCLIENT_ACTIVATION_PARAMS</see> structure passed into a call to <see cref="Helper.ActivateAudioInterfaceAsync(String, Guid, ByRef PropVariant, ByRef IActivateAudioInterfaceCompletionHandler)"/>.
        ''' </summary>
        Public Enum LoopbackMode
            ''' <summary>
            ''' Render streams from the specified process and its child processes are included in the activated process loopback stream.
            ''' </summary>
            IncludeProcessTree
            ''' <summary>
            ''' Render streams from the specified process and its child processes are excluded from the activated process loopback stream.
            ''' </summary>
            ExcludeProcessTree
        End Enum

    End Namespace
End Namespace