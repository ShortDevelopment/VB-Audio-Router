
Imports VBAudioRouter.AudioGraphControl
Imports Windows.Media.Audio

Namespace Controls

    Public NotInheritable Class ReverbNodeControl
        Inherits UserControl
        Implements IAudioNodeControl

        Public Sub New()
            InitializeComponent()
        End Sub

#Region "Identity"
        
        Public ReadOnly Property NodeType As NodeTypeEnum Implements IAudioNodeControl.NodeType
            Get
                Return NodeTypeEnum.Effect
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

        Public Property ReverbEffect As ReverbEffectDefinition

        Dim isInitialized As Boolean = False
        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            _BaseAudioNode = graph.CreateSubmixNode()
            ReverbEffect = New ReverbEffectDefinition(graph)
            BaseAudioNode.EffectDefinitions.Add(ReverbEffect)

            HighCutRadialGauge.Value = ReverbEffect.HighEQCutoff
            LowCutRadialGauge.Value = ReverbEffect.LowEQCutoff
            EarlyDiffusionRadialGauge.Value = ReverbEffect.EarlyDiffusion
            LateDiffusionRadialGauge.Value = ReverbEffect.LateDiffusion
            DecayTimeRadialGauge.Value = ReverbEffect.DecayTime
            DensityRadialGauge.Value = ReverbEffect.Density

            RearDelayRadialGauge.Value = ReverbEffect.RearDelay
            ReflectionsDelayRadialGauge.Value = ReverbEffect.ReflectionsDelay
            ReverbDelayRadialGauge.Value = ReverbEffect.ReverbDelay

            DryWetSlider.Value = ReverbEffect.WetDryMix
            RoomSizeSlider.Value = ReverbEffect.RoomSize

            ' Important because the value changed event will otherwise override default settings!
            isInitialized = True
        End Function

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub

        Private Sub RadialGauge_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If ReverbEffect Is Nothing Or Not isInitialized Then Exit Sub

            ReverbEffect.HighEQCutoff = HighCutRadialGauge.Value
            ReverbEffect.LowEQCutoff = LowCutRadialGauge.Value
            ReverbEffect.EarlyDiffusion = EarlyDiffusionRadialGauge.Value
            ReverbEffect.LateDiffusion = LateDiffusionRadialGauge.Value
            ReverbEffect.DecayTime = DecayTimeRadialGauge.Value
            ReverbEffect.Density = DensityRadialGauge.Value

            ReverbEffect.RearDelay = RearDelayRadialGauge.Value
            ReverbEffect.ReflectionsDelay = ReflectionsDelayRadialGauge.Value
            ReverbEffect.ReverbDelay = ReverbDelayRadialGauge.Value

            ReverbEffect.WetDryMix = DryWetSlider.Value
            ReverbEffect.RoomSize = RoomSizeSlider.Value
        End Sub

    End Class

End Namespace
