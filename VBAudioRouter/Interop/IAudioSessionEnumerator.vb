Imports System.Runtime.InteropServices
Imports NAudio.CoreAudioApi.Interfaces

Namespace Interop

    ''' <summary>
    ''' <see href="https://docs.microsoft.com/en-us/windows/win32/api/audiopolicy/nn-audiopolicy-iaudiosessionenumerator">Documentation</see> <br/>
    ''' <see href="https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Interfaces/IAudioSessionEnumerator.cs">Implementation</see>
    ''' </summary>
    <Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IAudioSessionEnumerator
        Function GetCount() As Integer
        Function GetSession(ByVal sessionCount As Integer) As IAudioSessionControl
    End Interface

End Namespace