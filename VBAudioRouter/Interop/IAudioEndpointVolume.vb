Imports System.Runtime.InteropServices

Namespace Interop
    <Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IAudioEndpointVolume
        <Obsolete("Not implemented!")>
        Sub Reserved1()
        <Obsolete("Not implemented!")>
        Sub Reserved2()
        Function GetChannelCount(<Out> ByRef pnChannelCount As Integer) As Integer
        Function SetMasterVolumeLevel(ByVal fLevelDB As Single, ByVal pguidEventContext As Guid) As Integer
        Function SetMasterVolumeLevelScalar(ByVal fLevel As Single, ByVal pguidEventContext As Guid) As Integer
        Function GetMasterVolumeLevel(<Out> ByRef pfLevelDB As Single) As Integer
        Function GetMasterVolumeLevelScalar(<Out> ByRef pfLevel As Single) As Integer
        Function SetChannelVolumeLevel(ByVal nChannel As UInteger, ByVal fLevelDB As Single, ByVal pguidEventContext As Guid) As Integer
        Function SetChannelVolumeLevelScalar(ByVal nChannel As UInteger, ByVal fLevel As Single, ByVal pguidEventContext As Guid) As Integer
        Function GetChannelVolumeLevel(ByVal nChannel As UInteger, <Out> ByRef pfLevelDB As Single) As Integer
        Function GetChannelVolumeLevelScalar(ByVal nChannel As UInteger, <Out> ByRef pfLevel As Single) As Integer
        Function SetMute(<MarshalAs(UnmanagedType.Bool)> ByVal bMute As Boolean, ByVal pguidEventContext As Guid) As Integer
        Function GetMute(<Out> ByRef pbMute As Boolean) As Integer
        Function GetVolumeStepInfo(<Out> ByRef pnStep As UInteger, <Out> ByRef pnStepCount As UInteger) As Integer
        Function VolumeStepUp(ByVal pguidEventContext As Guid) As Integer
        Function VolumeStepDown(ByVal pguidEventContext As Guid) As Integer
        Function QueryHardwareSupport(<Out> ByRef pdwHardwareSupportMask As UInteger) As Integer
        Function GetVolumeRange(<Out> ByRef pflVolumeMindB As Single, <Out> ByRef pflVolumeMaxdB As Single, <Out> ByRef pflVolumeIncrementdB As Single) As Integer
    End Interface
End Namespace