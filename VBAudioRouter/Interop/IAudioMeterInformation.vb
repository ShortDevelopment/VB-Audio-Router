Imports System.Runtime.InteropServices

Namespace Interop

    ''' <summary>
    ''' <see href="https://github.com/naudio/NAudio/blob/master/NAudio.Wasapi/CoreAudioApi/Interfaces/IAudioMeterInformation.cs"/>
    ''' </summary>
    <Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IAudioMeterInformation
        Function GetPeakValue() As Single
        Function GetMeteringChannelCount() As Integer
        Function GetChannelsPeakValues(ByVal u32ChannelCount As Integer, <[In]> ByVal afPeakValues As IntPtr) As Integer
        Function QueryHardwareSupport(<Out> ByRef pdwHardwareSupportMask As Integer) As Integer
    End Interface

End Namespace