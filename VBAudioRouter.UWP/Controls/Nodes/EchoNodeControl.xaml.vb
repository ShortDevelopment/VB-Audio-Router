
Imports VBAudioRouter.AudioGraphControl
Imports VBAudioRouter.AudioGraphControl.Serialization
Imports Windows.Media.Audio

Namespace Controls.Nodes

    Public NotInheritable Class EchoNodeControl
        Inherits UserControl
        Implements IAudioNodeControl, IAudioNodeControlInput, IAudioNodeControlEffect, IAudioNodeControlOutput, IAudioNodeSerializable

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
        Public ReadOnly Property IncomingConnector As ConnectorControl Implements IAudioNodeControlInput.IncomingConnector
            Get
                Return IncomingConnectorControl
            End Get
        End Property
#End Region

        Public Property EchoEffect As EchoEffectDefinition

        Dim isInitialized As Boolean = False
        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            _BaseAudioNode = graph.CreateSubmixNode()
            EchoEffect = New EchoEffectDefinition(graph)
            BaseAudioNode.EffectDefinitions.Add(EchoEffect)

            ReloadSettings()

            ' Important because the value changed event will otherwise override default settings!
            isInitialized = True
        End Function

        Public Sub ReloadSettings() Implements IAudioNodeSerializable.ReloadSettings
            DelayTimeRadialGauge.Value = EchoEffect.Delay
            FeedbackTimeRadialGauge.Value = EchoEffect.Feedback

            DryWetSlider.Value = EchoEffect.WetDryMix * 100
        End Sub

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub

        Private Sub RadialGauge_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If EchoEffect Is Nothing Or Not isInitialized Then Exit Sub

            EchoEffect.Delay = DelayTimeRadialGauge.Value
            EchoEffect.Feedback = FeedbackTimeRadialGauge.Value

            EchoEffect.WetDryMix = DryWetSlider.Value / 100
        End Sub
    End Class

End Namespace
