
Imports VBAudioRooter.AudioGraphControl
Imports VBAudioRooter.Controls
Imports Windows.Media
Imports Windows.Media.Audio
Imports Windows.Media.MediaProperties
Imports Windows.UI
Imports Windows.UI.Xaml.Shapes

Public NotInheritable Class MainPage
    Inherits Page

    Private Async Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim settings As New AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media)
        settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.LowestLatency
        'settings.EncodingProperties = New AudioEncodingProperties()
        'settings.EncodingProperties.Subtype = "Float"
        'settings.EncodingProperties.SampleRate = 48000
        'settings.EncodingProperties.ChannelCount = 2
        'settings.EncodingProperties.BitsPerSample = 32
        'settings.EncodingProperties.Bitrate = 3072000
        Dim graphResult = Await AudioGraph.CreateAsync(settings)
        If Not graphResult.Status = AudioGraphCreationStatus.Success Then Throw graphResult.ExtendedError
        _CurrentAudioGraph = graphResult.Graph

        ' https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/background-audio
        ' https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/system-media-transport-controls#use-the-system-media-transport-controls-for-background-audio
        _MediaTransportControl = SystemMediaTransportControls.GetForCurrentView()
        MediaTransportControl.IsEnabled = True
        MediaTransportControl.IsPlayEnabled = True
        MediaTransportControl.IsPlayEnabled = True
        MediaTransportControl.IsStopEnabled = True
        MediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Stopped
        AddHandler MediaTransportControl.ButtonPressed, Sub(s As SystemMediaTransportControls, args As SystemMediaTransportControlsButtonPressedEventArgs)
                                                            Select Case args.Button
                                                                Case SystemMediaTransportControlsButton.Play
                                                                    MediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Playing
                                                                Case SystemMediaTransportControlsButton.Pause
                                                                    MediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Stopped
                                                                Case SystemMediaTransportControlsButton.Stop
                                                                    MediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Stopped
                                                            End Select
                                                        End Sub
    End Sub

    Public ReadOnly Property CurrentAudioGraph As AudioGraph = Nothing

    Public ReadOnly Property MediaTransportControl As SystemMediaTransportControls

    Private Async Sub PlayButton_Click(sender As Object, e As RoutedEventArgs) Handles PlayButton.Click
        MediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Playing
        For Each child In NodeContainer.Children
            If GetType(NodeControl) = child.GetType() Then
                Dim control As NodeControl = DirectCast(child, NodeControl)
                If control.NodeContent IsNot Nothing AndAlso GetType(IAudioNodeControl).IsAssignableFrom(control.NodeContent.GetType()) Then
                    Dim node As IAudioNodeControl = DirectCast(control.NodeContent, IAudioNodeControl)
                    ' Ignore not connected nodes
                    If node.OutgoingConnector IsNot Nothing AndAlso Not node.OutgoingConnector.IsConnected Then Continue For
                    Await DirectCast(control.NodeContent, IAudioNodeControl).Initialize(CurrentAudioGraph)
                End If
            End If
        Next

        For Each child In NodeContainer.Children
            If GetType(NodeControl) = child.GetType() Then
                Dim control As NodeControl = DirectCast(child, NodeControl)
                If control.NodeContent IsNot Nothing AndAlso GetType(IAudioNodeControl).IsAssignableFrom(control.NodeContent.GetType()) Then
                    Dim node As IAudioNodeControl = DirectCast(control.NodeContent, IAudioNodeControl)
                    If node.OutgoingConnector IsNot Nothing AndAlso Not node.OutgoingConnector.IsConnected Then Continue For
                    If node.OutgoingConnector IsNot Nothing AndAlso node.OutgoingConnector.IsConnected Then
                        For Each connection In node.OutgoingConnector.Connections
                            node.AddOutgoingConnection(connection.DestinationConnector.AttachedNode)
                        Next
                    End If
                    node.OnStateChanged(GraphState.Started)
                End If
            End If
        Next

        CurrentAudioGraph.Start()
        PlayButton.IsEnabled = False
        StopButton.IsEnabled = True
    End Sub

    Private Sub StopButton_Click(sender As Object, e As RoutedEventArgs) Handles StopButton.Click
        MediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Stopped
        CurrentAudioGraph.Stop()
        PlayButton.IsEnabled = True
        StopButton.IsEnabled = False
    End Sub

#Region "Context Menu"
    Private Sub MenuFlyoutItem_Click(sender As Object, e As RoutedEventArgs)
        Dim tag As String = DirectCast(DirectCast(sender, MenuFlyoutItem).Tag, String)
        If Not String.IsNullOrEmpty(tag) Then
            Dim nodeEle As New NodeControl()
            Dim contentEle = DirectCast(Activator.CreateInstance(Me.GetType().Assembly.GetType($"VBAudioRooter.Controls.{tag}")), IAudioNodeControl)
            contentEle.Canvas = ConnectionCanvas
            nodeEle.NodeContent = contentEle
            nodeEle.HorizontalAlignment = HorizontalAlignment.Left
            nodeEle.VerticalAlignment = VerticalAlignment.Top
            NodeContainer.Children.Add(nodeEle)
        End If
    End Sub
#End Region

End Class
