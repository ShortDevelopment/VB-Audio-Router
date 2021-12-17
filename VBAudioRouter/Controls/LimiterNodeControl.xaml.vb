Option Strict Off

Imports VBAudioRouter.AudioGraphControl
Imports Windows.Media.Audio

Namespace Controls

    Public NotInheritable Class LimiterNodeControl
        Inherits UserControl
        Implements IAudioNodeControl, IAudioNodeControlInput, IAudioNodeControlEffect, IAudioNodeControlOutput

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

        Public Property LimiterEffect As LimiterEffectDefinition

        Dim isInitialized As Boolean = False
        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            _BaseAudioNode = graph.CreateSubmixNode()
            LimiterEffect = New LimiterEffectDefinition(graph)
            BaseAudioNode.EffectDefinitions.Add(LimiterEffect)

            LoudnessRadialGauge.Value = LimiterEffect.Loudness
            ReleaseRadialGauge.Value = LimiterEffect.Release

            ' Important because the value changed event will otherwise override default settings!
            isInitialized = True
        End Function

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub

        Private Sub RadialGauge_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If LimiterEffect Is Nothing Or Not isInitialized Then Exit Sub

            LimiterEffect.Loudness = LoudnessRadialGauge.Value
            LimiterEffect.Release = ReleaseRadialGauge.Value
        End Sub

    End Class

End Namespace
