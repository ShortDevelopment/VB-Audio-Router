
Imports System.Runtime.InteropServices
Imports NAudio.CoreAudioApi.Interfaces
Imports VBAudioRouter.Interop

Namespace Controls

    Public NotInheritable Class AudioSessionControl
        Inherits UserControl
        Implements IAudioEndpointVolumeCallback, IAudioSessionEvents

        Public ReadOnly Property VolumeManager As IAudioEndpointVolume
        Public ReadOnly Property MeterInformation As IAudioMeterInformation
        Public ReadOnly Property AudioSession As IAudioSessionControl

        Public ReadOnly SpeakerControlPageInstance As SpeakerControlPage

        Friend Sub New(parent As SpeakerControlPage, audioSession As IAudioSessionControl)
            InitializeComponent()

            Me.SpeakerControlPageInstance = parent
            Me.AudioSession = audioSession

            VolumeManager = DirectCast(audioSession, IAudioEndpointVolume)
            MeterInformation = DirectCast(audioSession, IAudioMeterInformation)

            VolumeManager.RegisterControlChangeNotify(Me)
            OnNotify(IntPtr.Zero)

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

            audioSession.RegisterAudioSessionNotification(Me)
        End Sub

        Private Sub AudioSessionControl_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
            VolumeManager?.UnregisterControlChangeNotify(Me)
            AudioSession?.UnregisterAudioSessionNotification(Me)
        End Sub

        Dim oldValue As Double = -1
        Private Sub VolumeSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If VolumeManager Is Nothing Or oldValue = VolumeSlider.Value Then Exit Sub
            oldValue = VolumeSlider.Value
            VolumeManager.SetMasterVolumeLevelScalar(Convert.ToSingle(VolumeSlider.Value / 100), Guid.Empty)
        End Sub

        Dim isMuted As Boolean = False
        Private Sub MuteButton_Click(sender As Object, e As RoutedEventArgs)
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
                                                     VolumeSlider.Value = VolumeManager.GetMasterVolumeLevelScalar() * 100
                                                     isMuted = VolumeManager.GetMute()
                                                     MuteButton_Click(Nothing, Nothing)
                                                 End Sub)
        End Sub

        Public Function OnDisplayNameChanged(displayName As String, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnDisplayNameChanged

        End Function

        Public Function OnIconPathChanged(iconPath As String, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnIconPathChanged

        End Function

        Public Function OnSimpleVolumeChanged(volume As Single, isMuted As Boolean, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnSimpleVolumeChanged

        End Function

        Public Function OnChannelVolumeChanged(channelCount As UInteger, newVolumes As IntPtr, channelIndex As UInteger, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnChannelVolumeChanged

        End Function

        Public Function OnGroupingParamChanged(ByRef groupingId As Guid, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnGroupingParamChanged

        End Function

        Public Function OnStateChanged(state As AudioSessionState) As Integer Implements IAudioSessionEvents.OnStateChanged

        End Function

        Public Function OnSessionDisconnected(disconnectReason As AudioSessionDisconnectReason) As Integer Implements IAudioSessionEvents.OnSessionDisconnected
            SpeakerControlPageInstance.AudioSessions.Remove(Me)
        End Function
    End Class

End Namespace
