Namespace Utils

    Public Class MediaTransportControlsWrapper
        Inherits ElementWrapper(Of MediaTransportControls)

        Public Sub New(control As MediaTransportControls)
            MyBase.New(control)
        End Sub

#Region "Control Wrapper Fields"

#Region "Play Button"
        <ElementChildReference>
        Private WithEvents PlayPauseButton As AppBarButton
#End Region

#Region "Position & Duration"
        <ElementChildReference>
        Private WithEvents TimeElapsedElement As TextBlock

        <ElementChildReference>
        Private WithEvents ProgressSlider As Slider

        <ElementChildReference>
        Private WithEvents TimeRemainingElement As TextBlock
#End Region

#Region "Volume"

#Region "Button"
        <ElementChildReference>
        Private WithEvents VolumeMuteButton As AppBarButton
#End Region

#Region "Flyout"
        <ElementChildReference>
        Private WithEvents AudioMuteButton As AppBarButton

        <ElementChildReference>
        Private WithEvents VolumeSlider As Slider
#End Region

#End Region
#End Region


#Region "Exposed Properties & Events"

#Region "Muted"
        Dim _IsMuted As Boolean = False
        Public Property IsMuted As Boolean
            Get
                Return _IsMuted
            End Get
            Set(muted As Boolean)
                If muted Then
                    VolumeMuteButton.Icon = New SymbolIcon(Symbol.Mute)
                    AudioMuteButton.Icon = New SymbolIcon(Symbol.Mute)
                Else
                    VolumeMuteButton.Icon = New SymbolIcon(Symbol.Volume)
                    AudioMuteButton.Icon = New SymbolIcon(Symbol.Volume)
                End If
                If Not Me._IsMuted = muted Then
                    Me._IsMuted = muted
                    RaiseEvent MutedChanged(Me, muted)
                End If
            End Set
        End Property

        Public Event MutedChanged As TypedEventHandler(Of MediaTransportControlsWrapper, Boolean)

        Private Sub AudioMuteButton_Click(sender As Object, e As RoutedEventArgs) Handles AudioMuteButton.Click
            IsMuted = Not IsMuted
        End Sub
#End Region

#Region "Volume"
        Public Property Volume As Double
            Get
                Return VolumeSlider.Value
            End Get
            Set(volume As Double)
                If Not VolumeSlider.Value = volume Then
                    ' Set from code
                    VolumeSlider.Value = volume
                Else
                    ' Set manually
                    RaiseEvent VolumeChanged(Me, volume)
                End If
            End Set
        End Property

        Public Event VolumeChanged As TypedEventHandler(Of MediaTransportControlsWrapper, Double)

        Private Sub VolumeSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles VolumeSlider.ValueChanged
            Me.Volume = VolumeSlider.Value
        End Sub
#End Region

#Region "IsPlaying"
        Dim _IsPlaying As Boolean
        Public Property IsPlaying As Boolean
            Get
                Return _IsPlaying
            End Get
            Set(playing As Boolean)
                If Not Me._IsPlaying = playing Then
                    Me._IsPlaying = playing
                    If playing Then
                        PlayPauseButton.Icon = New SymbolIcon(Symbol.Pause)
                    Else
                        PlayPauseButton.Icon = New SymbolIcon(Symbol.Play)
                    End If
                    RaiseEvent PlayStateChanged(Me, playing)
                End If
            End Set
        End Property

        Public Event PlayStateChanged As TypedEventHandler(Of MediaTransportControlsWrapper, Boolean)

        Private Sub PlayPauseButton_Click(sender As Object, e As RoutedEventArgs) Handles PlayPauseButton.Tapped
            IsPlaying = Not IsPlaying
        End Sub
#End Region

#Region "Position"
        Dim _Position As TimeSpan
        Public Property Position As TimeSpan
            Get
                Return _Position
            End Get
            Set(position As TimeSpan)
                ' Prevent to set property multiple times with same value
                If _Position = position Then Exit Property
                _Position = position

                If Not ProgressSlider.Value = position.TotalMilliseconds Then
                    ' Set by code
                    ProgressSlider.Value = position.TotalMilliseconds
                Else
                    ' Set manually
                    RaiseEvent PositionChanged(Me, position)
                End If
                TimeElapsedElement.Text = position.ToString("hh\:mm\:ss")
            End Set
        End Property

        Public Event PositionChanged As TypedEventHandler(Of MediaTransportControlsWrapper, TimeSpan)

        Private Sub ProgressSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles ProgressSlider.ValueChanged
            Position = e.NewValue / ProgressSlider.Maximum * Duration
        End Sub
#End Region

#Region "Duration"
        Dim _Duration As TimeSpan
        Public Property Duration As TimeSpan
            Get
                Return _Duration
            End Get
            Set(duration As TimeSpan)
                If Not Me._Duration = duration Then
                    Me._Duration = duration
                    ProgressSlider.Maximum = duration.TotalMilliseconds
                    TimeRemainingElement.Text = duration.ToString("hh\:mm\:ss")
                End If
            End Set
        End Property
#End Region

#End Region

    End Class

End Namespace