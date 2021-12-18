Imports VBAudioRouter.Interop
Imports Windows.Media.Devices
Imports WinUI.Interop.AppContainer

Public NotInheritable Class AudioControlPage
    Inherits Page
    Implements IAudioEndpointVolumeCallback

    Private Property VolumeManager As IAudioEndpointVolume
    Private Async Sub AudioControlPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim deviceId As String = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default)
        VolumeManager = Await AudioInterfaceActivator.ActivateAudioInterfaceAsync(Of IAudioEndpointVolume)(deviceId)
        VolumeManager.RegisterControlChangeNotify(Me)
        OnNotify(IntPtr.Zero)
    End Sub

    Private Sub AudioControlPage_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
        VolumeManager.UnregisterControlChangeNotify(Me)
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
