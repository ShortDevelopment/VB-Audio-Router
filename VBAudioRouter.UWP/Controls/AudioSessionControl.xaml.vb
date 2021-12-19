Imports NAudio.CoreAudioApi.Interfaces

Namespace Controls

    Public NotInheritable Class AudioSessionControl
        Inherits UserControl
        Implements IAudioSessionEvents

        Public ReadOnly Property AudioSession As IAudioSessionControl
        Public ReadOnly Property AudioSessionControl As NAudio.CoreAudioApi.AudioSessionControl
        Public ReadOnly Property AudioMeterInformation As NAudio.CoreAudioApi.AudioMeterInformation
        Public ReadOnly Property SimpleAudioVolume As NAudio.CoreAudioApi.SimpleAudioVolume

        Public ReadOnly SpeakerControlPageInstance As SpeakerControlPage

        Friend Sub New(parent As SpeakerControlPage, audioSession As IAudioSessionControl)
            InitializeComponent()

            Me.SpeakerControlPageInstance = parent
            Me.AudioSessionControl = New NAudio.CoreAudioApi.AudioSessionControl(audioSession)
            AudioMeterInformation = AudioSessionControl.AudioMeterInformation
            SimpleAudioVolume = AudioSessionControl.SimpleAudioVolume

            VolumeSlider.Value = SimpleAudioVolume.Volume * 100
            DisplayNameTextBlock.Text = AudioSessionControl.DisplayName

            Dim timer As New Timers.Timer()
            timer.Interval = 30
            AddHandler timer.Elapsed, Sub()
                                          Dim unused = Dispatcher?.RunIdleAsync(Sub()
                                                                                    Dim peakValues = AudioMeterInformation.PeakValues
                                                                                    If peakValues.Count > 0 Then
                                                                                        LeftMeter.ScaleX = peakValues(0)
                                                                                    End If
                                                                                    If peakValues.Count > 1 Then
                                                                                        RightMeter.ScaleX = peakValues(1)
                                                                                    End If
                                                                                End Sub)
                                      End Sub
            timer.Enabled = True

            audioSession.RegisterAudioSessionNotification(Me)
        End Sub

        Private Sub AudioSessionControl_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
            AudioSession?.UnregisterAudioSessionNotification(Me)
        End Sub

        Dim oldValue As Double = -1
        Private Sub VolumeSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If SimpleAudioVolume Is Nothing Or oldValue = VolumeSlider.Value Then Exit Sub
            oldValue = VolumeSlider.Value
            SimpleAudioVolume.Volume = CType(VolumeSlider.Value / 100.0F, Single)
        End Sub

        Dim isMuted As Boolean = False
        Private Sub MuteButton_Click(sender As Object, e As RoutedEventArgs)
            If SimpleAudioVolume Is Nothing Then Exit Sub

            If sender IsNot Nothing Then
                isMuted = Not isMuted
                SimpleAudioVolume.Mute = isMuted
            End If

            If isMuted Then
                MuteButton.Icon = New SymbolIcon(Symbol.Mute)
            Else
                MuteButton.Icon = New SymbolIcon(Symbol.Volume)
            End If
        End Sub

        Public Function OnDisplayNameChanged(displayName As String, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnDisplayNameChanged
            DisplayNameTextBlock.Text = AudioSessionControl.DisplayName

            Return 0
        End Function

        Public Function OnIconPathChanged(iconPath As String, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnIconPathChanged
            Return 0
        End Function

        Public Function OnSimpleVolumeChanged(volume As Single, isMuted As Boolean, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnSimpleVolumeChanged
            Dispatcher.RunIdleAsync(Sub()
                                        If Not oldValue = volume * 100 Then
                                            oldValue = volume * 100
                                            VolumeSlider.Value = volume * 100
                                        End If
                                        Me.isMuted = isMuted
                                        MuteButton_Click(Nothing, Nothing)
                                    End Sub)

            Return 0
        End Function

        Public Function OnChannelVolumeChanged(channelCount As UInteger, newVolumes As IntPtr, channelIndex As UInteger, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnChannelVolumeChanged
            Return 0
        End Function

        Public Function OnGroupingParamChanged(ByRef groupingId As Guid, ByRef eventContext As Guid) As Integer Implements IAudioSessionEvents.OnGroupingParamChanged
            Return 0
        End Function

        Public Function OnStateChanged(state As AudioSessionState) As Integer Implements IAudioSessionEvents.OnStateChanged
            Return 0
        End Function

        Public Function OnSessionDisconnected(disconnectReason As AudioSessionDisconnectReason) As Integer Implements IAudioSessionEvents.OnSessionDisconnected
            Dispatcher.RunIdleAsync(Sub() SpeakerControlPageInstance.AudioSessions.Remove(Me))

            Return 0
        End Function
    End Class

End Namespace
