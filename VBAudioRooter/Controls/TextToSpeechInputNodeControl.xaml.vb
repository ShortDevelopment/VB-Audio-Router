
Imports VBAudioRouter.AudioGraphControl
Imports Windows.Media.Audio
Imports Windows.Media.Core
Imports Windows.Media.SpeechSynthesis

Namespace Controls

    Public NotInheritable Class TextToSpeechInputNodeControl
        Inherits UserControl
        Implements IAudioNodeControl

        Public ReadOnly Property AllVoices As New Dictionary(Of String, VoiceInformation)
        Public Sub New()
            InitializeComponent()

            AllVoices = SpeechSynthesizer.AllVoices.ToDictionary(Function(x) x.Id)
            VoiceSelection.ItemsSource = AllVoices.Values.Select(Function(x) $"{x.DisplayName} | {x.Gender} | {x.Language}").ToArray()
            VoiceSelection.SelectedIndex = 0
        End Sub

#Region "Identity"

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
#End Region

        Dim Graph As AudioGraph
        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            Me.Graph = graph
        End Function

        Public ReadOnly Property Synthesizer As New SpeechSynthesizer()
        Private Async Function CreateAudioNode() As Task
            If BaseAudioNode IsNot Nothing Then Me.DisposeAudioNode()

            Synthesizer.Voice = AllVoices.Values(VoiceSelection.SelectedIndex)
            Synthesizer.Options.SpeakingRate = SpeedSlider.Value
            Synthesizer.Options.AudioVolume = VolumeSlider.Value / 100.0

            Dim source As MediaSource
            Using stream = Await Synthesizer.SynthesizeTextToStreamAsync(InputTextBox.Text)
                source = MediaSource.CreateFromStream(stream, stream.ContentType)
            End Using

            Dim result = Await Graph.CreateMediaSourceAudioInputNodeAsync(source)
            If Not result.Status = AudioDeviceNodeCreationStatus.Success Then Throw result.ExtendedError
            _BaseAudioNode = result.Node

            Me.ReconnectAudioNode()
        End Function

        Public Async Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub

        Private Async Sub PlayButton_Click(sender As Object, e As RoutedEventArgs) Handles PlayButton.Click
            Await CreateAudioNode()
        End Sub
    End Class

End Namespace
