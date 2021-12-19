
Imports System.Runtime.InteropServices
Imports NAudio.CoreAudioApi.Interfaces

Namespace Interop

    ''' <summary>
    ''' <see href="https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Interfaces/IAudioSessionManager.cs"/>
    ''' </summary>
    <Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IAudioSessionManager
        <PreserveSig>
        Function GetAudioSessionControl(
        <[In], [Optional]>
        <MarshalAs(UnmanagedType.LPStruct)> ByVal sessionId As Guid,
        <[In]>
        <MarshalAs(UnmanagedType.U4)> ByVal streamFlags As UInt32, <Out>
        <MarshalAs(UnmanagedType.[Interface])> ByRef sessionControl As IAudioSessionControl) As Integer
        <PreserveSig>
        Function GetSimpleAudioVolume(
        <[In], [Optional]>
        <MarshalAs(UnmanagedType.LPStruct)> ByVal sessionId As Guid,
        <[In]>
        <MarshalAs(UnmanagedType.U4)> ByVal streamFlags As UInt32, <Out>
        <MarshalAs(UnmanagedType.[Interface])> ByRef audioVolume As Object) As Integer 'ISimpleAudioVolume
    End Interface

    <Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IAudioSessionManager2
        Inherits IAudioSessionManager

        <PreserveSig>
        Overloads Function GetAudioSessionControl(
        <[In], [Optional]>
        <MarshalAs(UnmanagedType.LPStruct)> ByVal sessionId As Guid,
        <[In]>
        <MarshalAs(UnmanagedType.U4)> ByVal streamFlags As UInt32, <Out>
        <MarshalAs(UnmanagedType.[Interface])> ByRef sessionControl As IAudioSessionControl) As Integer
        <PreserveSig>
        Overloads Function GetSimpleAudioVolume(
        <[In], [Optional]>
        <MarshalAs(UnmanagedType.LPStruct)> ByVal sessionId As Guid,
        <[In]>
        <MarshalAs(UnmanagedType.U4)> ByVal streamFlags As UInt32, <Out>
        <MarshalAs(UnmanagedType.[Interface])> ByRef audioVolume As Object) As Integer ' ISimpleAudioVolume

        Function GetSessionEnumerator() As IAudioSessionEnumerator
        <PreserveSig>
        Function RegisterSessionNotification(ByVal sessionNotification As IAudioSessionNotification) As Integer
        <PreserveSig>
        Function UnregisterSessionNotification(ByVal sessionNotification As IAudioSessionNotification) As Integer
        <PreserveSig>
        Function RegisterDuckNotification(ByVal sessionId As String, ByVal audioVolumeDuckNotification As IAudioSessionNotification) As Integer
        <PreserveSig>
        Function UnregisterDuckNotification(ByVal audioVolumeDuckNotification As IntPtr) As Integer
    End Interface


End Namespace