Imports System.Runtime.InteropServices
Imports AudioVisualizer
Imports VBAudioRouter.Interop
Imports Windows.Media.Devices
Imports WinUI.Interop.AppContainer

Public NotInheritable Class SpeakerControlPage
    Inherits Page
    Implements IAudioEndpointVolumeCallback

    Public Property VolumeManager As IAudioEndpointVolume
    Public Property MeterInformation As IAudioMeterInformation

    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim deviceId As String = DirectCast(e.Parameter, String)
        VolumeManager = Await AudioInterfaceActivator.ActivateAudioInterfaceAsync(Of IAudioEndpointVolume)(deviceId)
        VolumeManager.RegisterControlChangeNotify(Me)
        OnNotify(IntPtr.Zero)

        MeterInformation = DirectCast(VolumeManager, IAudioMeterInformation)
        Dim timer As New Timers.Timer()
        timer.Interval = 30
        AddHandler timer.Elapsed, Sub()
                                      Dim unused = Dispatcher.RunIdleAsync(Sub()
                                                                               Dim meters As Single() = New Single(MeterInformation.GetMeteringChannelCount() - 1) {}
                                                                               Dim metersRef = GCHandle.Alloc(meters, GCHandleType.Pinned)
                                                                               MeterInformation.GetChannelsPeakValues(meters.Length, metersRef.AddrOfPinnedObject)
                                                                               metersRef.Free()
                                                                               LeftMeter.ScaleY = meters(0)
                                                                               RightMeter.ScaleY = meters(1)
                                                                           End Sub)
                                  End Sub
        timer.Enabled = True
    End Sub

    Private Sub AudioControlPage_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
        VolumeManager?.UnregisterControlChangeNotify(Me)
    End Sub

    Dim oldValue As Double = -1
    Private Sub Slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
        If VolumeManager Is Nothing Or oldValue = GainSlider.Value Then Exit Sub
        oldValue = GainSlider.Value
        VolumeManager.SetMasterVolumeLevelScalar(Convert.ToSingle(GainSlider.Value / 100), Guid.Empty)
    End Sub

    Dim isMuted As Boolean = False
    Private Sub MuteToggleButton_Click(sender As Object, e As RoutedEventArgs)
        If VolumeManager Is Nothing Then Exit Sub

        If sender IsNot Nothing Then
            isMuted = Not isMuted
            VolumeManager.SetMute(isMuted, Guid.Empty)
        End If

        If isMuted Then
            MuteButton.Icon = New SymbolIcon(Symbol.Mute)
        Else
            MuteButton.Icon = New SymbolIcon(Symbol.Volume)
        End If
    End Sub

    Public Sub OnNotify(notifyData As IntPtr) Implements IAudioEndpointVolumeCallback.OnNotify
        Dim unused = Dispatcher.RunIdleAsync(Sub()
                                                 GainSlider.Value = VolumeManager.GetMasterVolumeLevelScalar() * 100
                                                 isMuted = VolumeManager.GetMute()
                                                 MuteToggleButton_Click(Nothing, Nothing)
                                             End Sub)
    End Sub
End Class
