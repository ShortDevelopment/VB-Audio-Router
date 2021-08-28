
Imports VBAudioRooter.AudioGraphControl
Imports VBAudioRooter.Utils
Imports Windows.Media.Audio
Imports Windows.Storage
Imports Windows.Storage.Pickers

Namespace Controls

    Public NotInheritable Class FileInputNodeControl
        Inherits UserControl
        Implements IAudioNodeControl

        Public Property CurrentFile As StorageFile

        Public ReadOnly Property ID As Guid = Guid.NewGuid() Implements IAudioNodeControl.ID
        Public ReadOnly Property NodeType As NodeTypeEnum Implements IAudioNodeControl.NodeType
            Get
                Return NodeTypeEnum.Input
            End Get
        End Property
        Public Property Canvas As Canvas Implements IAudioNodeControl.Canvas
        Public ReadOnly Property BaseAudioNode As IAudioNode Implements IAudioNodeControl.BaseAudioNode

        Public ReadOnly Property OutgoingConnector As ConnectorControl Implements IAudioNodeControl.OutgoingConnector
            Get
                Return OutgoingConnectorControl
            End Get
        End Property

        Public Sub AddOutgoingConnection(node As IAudioNodeControl) Implements IAudioNodeControl.AddOutgoingConnection
            DirectCast(Me.BaseAudioNode, AudioFileInputNode).AddOutgoingConnection(node.BaseAudioNode)
        End Sub

        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            Me.Graph = graph
            Await CreateAudioNode(graph)
        End Function
        Dim Graph As AudioGraph
        Private Async Function CreateAudioNode(graph As AudioGraph) As Task
            If CurrentFile Is Nothing Then Throw New Exception("Not file has been choosen!")
            If BaseAudioNode IsNot Nothing Then
                BaseAudioNode.Stop()
                _BaseAudioNode = Nothing
            End If
            Dim result = Await graph.CreateFileInputNodeAsync(CurrentFile)
            If Not result.Status = AudioDeviceNodeCreationStatus.Success Then Throw result.ExtendedError
            _BaseAudioNode = result.FileInputNode
            DirectCast(BaseAudioNode, AudioFileInputNode).OutgoingGain = GainSlider.Value
            DirectCast(BaseAudioNode, AudioFileInputNode).ConsumeInput = Not MuteToggleButton.IsChecked
            AddHandler DirectCast(BaseAudioNode, AudioFileInputNode).FileCompleted, Sub()
                                                                                        If Dispatcher Is Nothing Then Exit Sub
#Disable Warning BC42358 ' Da auf diesen Aufruf nicht gewartet wird, wird die Ausführung der aktuellen Methode vor Abschluss des Aufrufs fortgesetzt.
                                                                                        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                                                                                               If BaseAudioNode Is Nothing Then Exit Sub
                                                                                                                                                               If LoopToggleButton.IsChecked Then
                                                                                                                                                                   BaseAudioNode.Reset()
                                                                                                                                                                   BaseAudioNode.Start()
                                                                                                                                                               End If
                                                                                                                                                           End Sub)
#Enable Warning BC42358 ' Da auf diesen Aufruf nicht gewartet wird, wird die Ausführung der aktuellen Methode vor Abschluss des Aufrufs fortgesetzt.
                                                                                    End Sub
            ' UI
            PositionSlider.Maximum = DirectCast(BaseAudioNode, AudioFileInputNode).Duration.TotalMilliseconds
            GainSlider.Value = GainSlider.Maximum
        End Function

        Private Async Sub InputDevices_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Graph Is Nothing Then Exit Sub
            Await CreateAudioNode(Graph)
        End Sub

        Private Sub MuteToggleButton_Click(sender As Object, e As RoutedEventArgs)
            If BaseAudioNode Is Nothing Then Exit Sub
            DirectCast(BaseAudioNode, AudioFileInputNode).ConsumeInput = Not MuteToggleButton.IsChecked
        End Sub

        Private Sub Slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If BaseAudioNode Is Nothing Then Exit Sub
            DirectCast(BaseAudioNode, AudioFileInputNode).OutgoingGain = GainSlider.Value.Map(0, 100, 0, GainControl.fxeq_max_gain)
        End Sub

        Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
            Dim filePicker As New FileOpenPicker()
            filePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary
            filePicker.FileTypeFilter.Add(".mp3")
            filePicker.FileTypeFilter.Add(".wav")
            filePicker.FileTypeFilter.Add(".wma")
            filePicker.FileTypeFilter.Add(".m4a")
            filePicker.ViewMode = PickerViewMode.Thumbnail
            Dim file = Await filePicker.PickSingleFileAsync()
            If file IsNot Nothing Then
                PathDisplay.Text = file.Path
                CurrentFile = file
            End If
        End Sub

        Private Sub PlayButton_Click(sender As Object, e As RoutedEventArgs) Handles PlayButton.Click
            If DirectCast(PlayButton.Tag, String) = "play" Then
                PlayButton.Tag = "pause"
                PlayButton.Icon = New SymbolIcon(Symbol.Pause)
                StopButton.IsEnabled = True
                DirectCast(BaseAudioNode, AudioFileInputNode).Start()
            Else
                PlayButton.Tag = "play"
                PlayButton.Icon = New SymbolIcon(Symbol.Play)
                DirectCast(BaseAudioNode, AudioFileInputNode).Stop()
            End If
        End Sub

        Private Sub StopButton_Click(sender As Object, e As RoutedEventArgs) Handles StopButton.Click
            StopButton.IsEnabled = False
            PlayButton.Tag = "play"
            PlayButton.Icon = New SymbolIcon(Symbol.Play)
            DirectCast(BaseAudioNode, AudioFileInputNode).Stop()
            DirectCast(BaseAudioNode, AudioFileInputNode).Reset()
        End Sub

#Region "Position Slider"
        Dim SliderAdjustedByHand As Boolean = False
        Private Sub PositionSlider_ManipulationStarting(sender As Object, e As ManipulationStartingRoutedEventArgs)
            SliderAdjustedByHand = True
        End Sub

        Private Sub PositionSlider_Tapped(sender As Object, e As TappedRoutedEventArgs)
            DirectCast(BaseAudioNode, AudioFileInputNode).Seek(TimeSpan.FromMilliseconds(PositionSlider.Value))
            SliderAdjustedByHand = False
        End Sub

        Private Sub PositionSlider_ManipulationCompleted(sender As Object, e As ManipulationCompletedRoutedEventArgs)
            DirectCast(BaseAudioNode, AudioFileInputNode).Seek(TimeSpan.FromMilliseconds(PositionSlider.Value))
            SliderAdjustedByHand = False
        End Sub

        Private Sub FileInputNodeControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Dim timer As New DispatcherTimer()
            timer.Interval = New TimeSpan(100)
            timer.Start()
            AddHandler timer.Tick, Sub()
                                       If BaseAudioNode Is Nothing Or SliderAdjustedByHand Then Exit Sub
                                       PositionSlider.Value = DirectCast(BaseAudioNode, AudioFileInputNode).Position.TotalMilliseconds
                                   End Sub
        End Sub
#End Region

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged
            PlayButton_Click(Nothing, Nothing)
        End Sub

    End Class

End Namespace
