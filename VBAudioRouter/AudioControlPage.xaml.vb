Imports VBAudioRouter.Interop
Imports Windows.Media.Devices

Public NotInheritable Class AudioControlPage
    Inherits Page

    Private Property VolumeManager As IAudioEndpointVolume
    Private Async Sub AudioControlPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim deviceId As String = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default)
        VolumeManager = Await AudioInterfaceActivator.ActivateAudioInterfaceAsync(Of IAudioEndpointVolume)(deviceId)
    End Sub

    Private Async Sub Slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
        If VolumeManager Is Nothing Then Exit Sub
        VolumeManager.SetMasterVolumeLevelScalar(Convert.ToSingle(e.NewValue / 100), Guid.Empty)
    End Sub

    Dim isMuted As Boolean = False
    Private Sub MuteToggleButton_Click(sender As Object, e As RoutedEventArgs)
        If VolumeManager Is Nothing Then Exit Sub

        isMuted = Not isMuted
        VolumeManager.SetMute(isMuted, Guid.Empty)

        If isMuted Then
            MuteButton.Icon = New SymbolIcon(Symbol.Mute)
        Else
            MuteButton.Icon = New SymbolIcon(Symbol.Volume)
        End If
    End Sub
End Class
