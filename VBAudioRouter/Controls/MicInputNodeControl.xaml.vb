
Imports VBAudioRouter.AudioGraphControl
Imports VBAudioRouter.Utils
Imports Windows.Devices.Enumeration
Imports Windows.Media.Audio
Imports Windows.Media.Devices

Namespace Controls

    Public NotInheritable Class MicInputNodeControl
        Inherits UserControl
        Implements IAudioNodeControl, IAudioNodeControlOutput

        Property AudioCaptureDevices As DeviceInformationCollection
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

            AudioCaptureDevices = Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioCaptureSelector())
            InputDevices.ItemsSource = AudioCaptureDevices.Select(Function(device) device.Name).ToList()

            Dim foundDefault As Boolean = False
            For i As Integer = 0 To AudioCaptureDevices.Count - 1
                If AudioCaptureDevices.Item(i).IsDefault Then
                    InputDevices.SelectedIndex = i
                    foundDefault = True
                    Exit For
                End If
            Next

            If Not foundDefault Then
                InputDevices.SelectedIndex = 0
            End If
        End Function

        Private Async Function CreateAudioNode() As Task
            If GraphState = GraphState.Started AndAlso BaseAudioNode IsNot Nothing Then Me.DisposeAudioNode()

            Dim result = Await Graph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Other, Graph.EncodingProperties, AudioCaptureDevices.Item(InputDevices.SelectedIndex))
            If Not result.Status = AudioDeviceNodeCreationStatus.Success Then Throw result.ExtendedError
            _BaseAudioNode = result.DeviceInputNode

            If GraphState = GraphState.Started Then Me.ReconnectAudioNode()

            DirectCast(BaseAudioNode, AudioDeviceInputNode).OutgoingGain = GainSlider.Value
            DirectCast(BaseAudioNode, AudioDeviceInputNode).ConsumeInput = Not isMuted

            GainSlider.Value = GainSlider.Maximum
        End Function

#Region "State"
        Dim GraphState As GraphState = GraphState.Stopped
        Public Async Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged
            Me.GraphState = state
            ' Ensure Initialized
            If state = GraphState.Started AndAlso BaseAudioNode Is Nothing Then Await CreateAudioNode()
        End Sub
#End Region

        Private Async Sub InputDevices_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Graph Is Nothing Then Exit Sub
            Await CreateAudioNode()
        End Sub

        Dim isMuted As Boolean = False
        Private Sub MuteToggleButton_Click(sender As Object, e As RoutedEventArgs)
            If BaseAudioNode Is Nothing Then Exit Sub

            isMuted = Not isMuted
            DirectCast(BaseAudioNode, AudioDeviceInputNode).ConsumeInput = Not isMuted

            If isMuted Then
                MuteButton.Icon = New SymbolIcon(Symbol.Mute)
            Else
                MuteButton.Icon = New SymbolIcon(Symbol.Volume)
            End If
        End Sub

        Private Sub Slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If BaseAudioNode Is Nothing Then Exit Sub
            DirectCast(BaseAudioNode, AudioDeviceInputNode).OutgoingGain = GainSlider.Value.Map(0, 100, 0, GainControl.fxeq_max_gain)
        End Sub


    End Class

End Namespace
