Imports System.Runtime.InteropServices
Imports NAudio.CoreAudioApi.Interfaces
Imports VBAudioRouter.Interop
Imports WinUI.Interop
Imports WinUI.Interop.AppContainer

Public NotInheritable Class SpeakerControlPage
    Inherits Page
    Implements IAudioEndpointVolumeCallback, IAudioSessionNotification

    Public ReadOnly Property VolumeManager As IAudioEndpointVolume
    Public ReadOnly Property MeterInformation As IAudioMeterInformation
    Public ReadOnly Property AudioSessionManager As IAudioSessionManager2

    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim deviceId As String = DirectCast(e.Parameter, String)
        _VolumeManager = Await AudioInterfaceActivator.ActivateAudioInterfaceAsync(Of IAudioEndpointVolume)(deviceId)
        _MeterInformation = DirectCast(VolumeManager, IAudioMeterInformation)

        VolumeManager.RegisterControlChangeNotify(Me)
        OnNotify(IntPtr.Zero)

        Dim timer As New Timers.Timer()
        timer.Interval = 30
        AddHandler timer.Elapsed, Sub()
                                      Dim unused = Dispatcher?.RunIdleAsync(Sub()
                                                                                Dim meters As Single() = New Single(MeterInformation.GetMeteringChannelCount() - 1) {}
                                                                                Dim metersRef = GCHandle.Alloc(meters, GCHandleType.Pinned)
                                                                                MeterInformation.GetChannelsPeakValues(meters.Length, metersRef.AddrOfPinnedObject)
                                                                                metersRef.Free()
                                                                                LeftMeter.ScaleY = meters(0)
                                                                                RightMeter.ScaleY = meters(1)
                                                                            End Sub)
                                  End Sub
        timer.Enabled = True

        ' UWP apps don't have permission to do the following stuff 😥
        If InteropHelper.IsUWP() Then Exit Sub

        _AudioSessionManager = DirectCast(Await AudioInterfaceActivator.ActivateAudioInterfaceAsync(Of IAudioSessionManager)(deviceId), IAudioSessionManager2)
        AudioSessionManager.RegisterSessionNotification(Me)
        Dim sessionEnumerator = AudioSessionManager.GetSessionEnumerator()
        For sessionIndex = 0 To sessionEnumerator.GetCount() - 1
            AddAudioSession(sessionEnumerator.GetSession(sessionIndex))
        Next
    End Sub

    Private Sub AudioControlPage_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
        VolumeManager?.UnregisterControlChangeNotify(Me)
        AudioSessionManager?.UnregisterSessionNotification(Me)
    End Sub

    Public ReadOnly AudioSessions As New ObservableCollection(Of Controls.AudioSessionControl)
    Private Sub AddAudioSession(session As IAudioSessionControl)
        AudioSessions.Add(New Controls.AudioSessionControl(Me, session))
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

    Public Function OnSessionCreated(newSession As IAudioSessionControl) As Integer Implements IAudioSessionNotification.OnSessionCreated
        AddAudioSession(newSession)
    End Function
End Class
