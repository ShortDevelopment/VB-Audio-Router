
Imports VBAudioRouter.AudioGraphControl
Imports VBAudioRouter.Capture
Imports VBAudioRouter.Utils
Imports Windows.Media.Audio

Namespace Controls.Nodes

    Public NotInheritable Class ProcessInputNodeControl
        Inherits UserControl
        Implements IAudioNodeControl, IAudioNodeControlOutput

        ReadOnly Property Processes As Process()
        Public Sub New()
            InitializeComponent()
        End Sub

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

            _Processes = Process.GetProcesses().OrderBy(Function(x) x.ProcessName).ToArray()
            InputDevices.ItemsSource = Processes.Select(Function(device) device.ProcessName).ToList()
        End Function

        Dim Capture As ProcessAudioCapture
        Private Sub CreateAudioNode()
            If GraphState = GraphState.Started AndAlso BaseAudioNode IsNot Nothing Then Me.DisposeAudioNode()

            Capture = New ProcessAudioCapture(Processes(InputDevices.SelectedIndex))
            _BaseAudioNode = Capture.CreateAudioNode(Graph)

            If GraphState = GraphState.Started Then Me.ReconnectAudioNode()

            DirectCast(BaseAudioNode, AudioFrameInputNode).OutgoingGain = GainSlider.Value
            DirectCast(BaseAudioNode, AudioFrameInputNode).ConsumeInput = Not isMuted

            GainSlider.Value = GainSlider.Maximum
        End Sub

#Region "State"
        Dim GraphState As GraphState = GraphState.Stopped
        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged
            Me.GraphState = state
            ' Ensure Initialized
            If state = GraphState.Started AndAlso BaseAudioNode Is Nothing Then CreateAudioNode()
        End Sub
#End Region

        Private Sub InputDevices_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Graph Is Nothing Then Exit Sub
            CreateAudioNode()
        End Sub

        Dim isMuted As Boolean = False
        Private Sub MuteToggleButton_Click(sender As Object, e As RoutedEventArgs)
            If BaseAudioNode Is Nothing Then Exit Sub

            isMuted = Not isMuted
            DirectCast(BaseAudioNode, AudioFrameInputNode).ConsumeInput = Not isMuted

            If isMuted Then
                MuteButton.Icon = New SymbolIcon(Symbol.Mute)
            Else
                MuteButton.Icon = New SymbolIcon(Symbol.Volume)
            End If
        End Sub

        Private Sub Slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If BaseAudioNode Is Nothing Then Exit Sub
            DirectCast(BaseAudioNode, AudioFrameInputNode).OutgoingGain = GainSlider.Value.Map(0, 100, 0, GainControl.fxeq_max_gain)
        End Sub


    End Class

End Namespace
