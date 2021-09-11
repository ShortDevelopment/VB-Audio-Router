
Imports VBAudioRooter.Controls
Imports Windows.Media.Audio

Namespace AudioGraphControl

    Public Interface IAudioNodeControl
#Region "Identity"

        ReadOnly Property ID As Guid
        ReadOnly Property NodeType As NodeTypeEnum
        ReadOnly Property BaseAudioNode As IAudioNode
#End Region

#Region "State"
        Function Initialize(graph As AudioGraph) As Task
        Sub OnStateChanged(state As GraphState)
#End Region

        ReadOnly Property OutgoingConnector As ConnectorControl

        Property Canvas As Canvas
    End Interface

End Namespace
