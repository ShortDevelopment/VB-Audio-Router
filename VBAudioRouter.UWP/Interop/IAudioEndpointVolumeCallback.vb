Imports System.Runtime.InteropServices

Namespace Interop

    ''' <summary>
    ''' <see href="https://docs.microsoft.com/en-us/windows/win32/api/endpointvolume/nn-endpointvolume-iaudioendpointvolumecallback"/> <br/>
    ''' <see href="https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Interfaces/IAudioEndpointVolumeCallback.cs"/>
    ''' </summary>
    <Guid("657804FA-D6AD-4496-8A60-352752AF4F89"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IAudioEndpointVolumeCallback
        Sub OnNotify(ByVal notifyData As IntPtr)
    End Interface

End Namespace