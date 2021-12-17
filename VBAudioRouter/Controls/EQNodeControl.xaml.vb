
Imports VBAudioRouter.AudioGraphControl
Imports VBAudioRouter.AudioGraphControl.Serialization
Imports VBAudioRouter.Utils
Imports VBAudioRouter.Utils.GainControl
Imports Windows.Media.Audio

Namespace Controls

    Public NotInheritable Class EQNodeControl
        Inherits UserControl
        Implements IAudioNodeControl, IAudioNodeControlInput, IAudioNodeControlEffect, IAudioNodeControlOutput, IAudioNodeSerializable

#Region "Indentity"
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

        Dim EQEffect As EqualizerEffectDefinition

        Public Const MinFreq As Integer = 100
        Public Const MaxFreq As Integer = 12000

        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            _BaseAudioNode = graph.CreateSubmixNode()
            EQEffect = New EqualizerEffectDefinition(graph)
            BaseAudioNode.EffectDefinitions.Add(EQEffect)
            DirectCast(BaseAudioNode, AudioSubmixNode).EnableEffectsByDefinition(EQEffect)

            ReloadSettings()
        End Function

        Public Sub ReloadSettings() Implements IAudioNodeSerializable.ReloadSettings
            EQDrag1.SetPosition(New Point(EQEffect.Bands(0).FrequencyCenter.Map(MinFreq, MaxFreq, 0.0, 1.0), EQEffect.Bands(0).Gain.Map(fxeq_min_gain, fxeq_max_gain, 0.0, 1.0)))
            EQDrag2.SetPosition(New Point(EQEffect.Bands(1).FrequencyCenter.Map(MinFreq, MaxFreq, 0.0, 1.0), EQEffect.Bands(1).Gain.Map(fxeq_min_gain, fxeq_max_gain, 0.0, 1.0)))
            EQDrag3.SetPosition(New Point(EQEffect.Bands(2).FrequencyCenter.Map(MinFreq, MaxFreq, 0.0, 1.0), EQEffect.Bands(2).Gain.Map(fxeq_min_gain, fxeq_max_gain, 0.0, 1.0)))
            EQDrag4.SetPosition(New Point(EQEffect.Bands(3).FrequencyCenter.Map(MinFreq, MaxFreq, 0.0, 1.0), EQEffect.Bands(3).Gain.Map(fxeq_min_gain, fxeq_max_gain, 0.0, 1.0)))
        End Sub

        Private Sub EQDrag_ValueChanged(sender As Object, e As Point)
            If EQEffect Is Nothing Then Exit Sub
            Dim band = EQEffect.Bands(DirectCast(sender, EQDragControl).Index)
            band.FrequencyCenter = e.X.Map(0.0, 1.0, MinFreq, MaxFreq)
            band.Gain = e.Y.Map(0.0, 1.0, fxeq_min_gain, fxeq_max_gain)
        End Sub

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub

    End Class

End Namespace
