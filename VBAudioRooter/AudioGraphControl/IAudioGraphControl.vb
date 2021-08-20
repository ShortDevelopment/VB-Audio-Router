
Imports VBAudioRooter.Controls
Imports Windows.Media.Audio

Namespace AudioGraphControl

    Public Interface IAudioNodeControl
        ReadOnly Property ID As Guid
        ReadOnly Property Node As IAudioNode
        Sub AddOutgoingConnection(node As IAudioNodeControl)
        Function Initialize(graph As AudioGraph) As Task
        Sub OnStateChanged(state As GraphState)
        ReadOnly Property OutgoingConnector As ConnectorControl
        Property Canvas As Canvas
    End Interface

End Namespace
