
Imports VBAudioRooter.AudioGraphControl
Imports VBAudioRooter.Utils
Imports Windows.Media.Audio

Namespace Controls

    Public NotInheritable Class EQNodeControl
        Inherits UserControl
        Implements IAudioNodeControl

        Public ReadOnly Property ID As Guid = Guid.NewGuid() Implements IAudioNodeControl.ID

        Public ReadOnly Property Node As IAudioNode Implements IAudioNodeControl.Node

        Public ReadOnly Property OutgoingConnector As ConnectorControl Implements IAudioNodeControl.OutgoingConnector
            Get
                Return OutgoingConnectorControl
            End Get
        End Property

        Public ReadOnly Property IngoingConnector As ConnectorControl Implements IAudioNodeControl.IngoingConnector
            Get
                Return IngoingConnectorControl
            End Get
        End Property

        Public Sub AddOutgoingConnection(node As IAudioNodeControl) Implements IAudioNodeControl.AddOutgoingConnection
            DirectCast(Me.Node, AudioSubmixNode).AddOutgoingConnection(node.Node)
        End Sub

        Dim EQEffect As EqualizerEffectDefinition

        Private Const MinFreq As Integer = 100
        Private Const MaxFreq As Integer = 12000
        ' These are the same values as the ones in xapofx.h
        Private Const fxeq_min_gain As Double = 0.126
        Private Const fxeq_max_gain As Double = 7.94

        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            _Node = graph.CreateSubmixNode()
            EQEffect = New EqualizerEffectDefinition(graph)
            Node.EffectDefinitions.Add(EQEffect)
            EQDrag1.SetPosition(New Point(EQEffect.Bands(0).FrequencyCenter.Map(MinFreq, MaxFreq, 0.0, 1.0), EQEffect.Bands(0).Gain.Map(fxeq_min_gain, fxeq_max_gain, 0.0, 1.0)))
            EQDrag2.SetPosition(New Point(EQEffect.Bands(1).FrequencyCenter.Map(MinFreq, MaxFreq, 0.0, 1.0), EQEffect.Bands(1).Gain.Map(fxeq_min_gain, fxeq_max_gain, 0.0, 1.0)))
            EQDrag3.SetPosition(New Point(EQEffect.Bands(2).FrequencyCenter.Map(MinFreq, MaxFreq, 0.0, 1.0), EQEffect.Bands(2).Gain.Map(fxeq_min_gain, fxeq_max_gain, 0.0, 1.0)))
            EQDrag4.SetPosition(New Point(EQEffect.Bands(3).FrequencyCenter.Map(MinFreq, MaxFreq, 0.0, 1.0), EQEffect.Bands(3).Gain.Map(fxeq_min_gain, fxeq_max_gain, 0.0, 1.0)))
        End Function

        Private Sub EQDrag_ValueChanged(sender As Object, e As Point)
            If EQEffect Is Nothing Then Exit Sub
            Dim band = EQEffect.Bands(DirectCast(sender, EQDragControl).Index)
            band.FrequencyCenter = e.X.Map(0.0, 1.0, MinFreq, MaxFreq)
            band.Gain = e.Y.Map(0.0, 1.0, fxeq_min_gain, fxeq_max_gain)
        End Sub

        Public Sub OnStartNotify() Implements IAudioNodeControl.OnStartNotify : End Sub
    End Class

End Namespace
