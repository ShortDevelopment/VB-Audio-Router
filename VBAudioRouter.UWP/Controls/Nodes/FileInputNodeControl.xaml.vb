﻿Imports VBAudioRouter.AudioGraphControl
Imports VBAudioRouter.Utils
Imports Windows.Media.Audio
Imports Windows.Media.Core
Imports Windows.Storage.Pickers
Imports WinUI.Interop
Imports WinUI.Interop.CoreWindow

Namespace Controls.Nodes

    Public NotInheritable Class FileInputNodeControl
        Inherits UserControl
        Implements IAudioNodeControl, IAudioNodeControlOutput

        Private WithEvents ControlsWrapper As Utils.MediaTransportControlsWrapper

        Public Property MediaSource As MediaSource

#Region "Identity"
        Public Property Canvas As Canvas Implements IAudioNodeControl.Canvas
        Public ReadOnly Property BaseAudioNode As IAudioNode Implements IAudioNodeControl.BaseAudioNode

        Public ReadOnly Property OutgoingConnector As ConnectorControl Implements IAudioNodeControlOutput.OutgoingConnector
            Get
                Return OutgoingConnectorControl
            End Get
        End Property
#End Region

        Dim Graph As AudioGraph
        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            Me.Graph = graph

            ControlsWrapper = New MediaTransportControlsWrapper(TransportControls)
            Dim unused = Dispatcher.RunIdleAsync(Sub() ControlsWrapper.Initialize())

            InitializePositionTimer()
        End Function

        Private Async Function CreateAudioNode() As Task
            If BaseAudioNode IsNot Nothing Then Me.DisposeAudioNode()

            Dim result = Await Graph.CreateMediaSourceAudioInputNodeAsync(MediaSource)
            If Not result.Status = AudioDeviceNodeCreationStatus.Success Then Throw result.ExtendedError
            _BaseAudioNode = result.Node

            Me.ReconnectAudioNode()

            ControlsWrapper.Duration = DirectCast(BaseAudioNode, MediaSourceAudioInputNode).Duration
        End Function

        Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
            Dim filePicker As New FileOpenPicker()
            If Not RuntimeInformation.IsUWP Then
                DirectCast(filePicker, IInitializeWithWindow).Initialize(Process.GetCurrentProcess().MainWindowHandle)
            End If
#Region "Picker Init"
            filePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary
            filePicker.FileTypeFilter.Add(".mp3")
            filePicker.FileTypeFilter.Add(".wav")
            filePicker.FileTypeFilter.Add(".wma")
            filePicker.FileTypeFilter.Add(".m4a")
            filePicker.ViewMode = PickerViewMode.Thumbnail
#End Region
            Dim file = Await filePicker.PickSingleFileAsync()
            If file IsNot Nothing Then
                PathDisplay.Text = file.Path
                BrowseButton.IsEnabled = False

                MediaSource = MediaSource.CreateFromStorageFile(file)
                Await CreateAudioNode()
            End If

        End Sub

        Public Async Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged
            If state = GraphState.Started Then
                ControlsWrapper.IsPlaying = True
                If MediaSource IsNot Nothing AndAlso BaseAudioNode Is Nothing Then Await CreateAudioNode()
            Else
                ControlsWrapper.IsPlaying = False
            End If
        End Sub

#Region "UI"
        Private Sub ControlsWrapper_MutedChanged(sender As MediaTransportControlsWrapper, args As Boolean) Handles ControlsWrapper.MutedChanged
            DirectCast(BaseAudioNode, MediaSourceAudioInputNode).ConsumeInput = Not ControlsWrapper.IsMuted
        End Sub

        Private Sub ControlsWrapper_PlayStateChanged(sender As MediaTransportControlsWrapper, playing As Boolean) Handles ControlsWrapper.PlayStateChanged
            If BaseAudioNode Is Nothing Then Exit Sub
            If playing Then
                DirectCast(BaseAudioNode, MediaSourceAudioInputNode).Start()
            Else
                DirectCast(BaseAudioNode, MediaSourceAudioInputNode).Stop()
            End If
        End Sub

        Private Sub ControlsWrapper_VolumeChanged(sender As MediaTransportControlsWrapper, args As Double) Handles ControlsWrapper.VolumeChanged
            DirectCast(BaseAudioNode, MediaSourceAudioInputNode).OutgoingGain = ControlsWrapper.Volume / 100.0
        End Sub

        Private Sub ControlsWrapper_PositionChanged(sender As MediaTransportControlsWrapper, position As TimeSpan) Handles ControlsWrapper.PositionChanged
            DirectCast(BaseAudioNode, MediaSourceAudioInputNode).Seek(position)
        End Sub

        Private Sub InitializePositionTimer()
            Dim timer As New DispatcherTimer()
            timer.Interval = New TimeSpan(100)
            timer.Start()
            AddHandler timer.Tick, Sub()
                                       If BaseAudioNode Is Nothing Then Exit Sub
                                       ControlsWrapper.Position = DirectCast(BaseAudioNode, MediaSourceAudioInputNode).Position
                                   End Sub
        End Sub
#End Region
    End Class

End Namespace
