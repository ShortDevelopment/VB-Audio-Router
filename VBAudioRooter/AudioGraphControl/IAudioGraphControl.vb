
Imports VBAudioRooter.Controls
Imports Windows.Media.Audio

Namespace AudioGraphControl

    Public Interface IAudioNodeControl
        ReadOnly Property ID As Guid
        ReadOnly Property Node As IAudioNode
        Sub AddOutgoingConnection(node As IAudioNodeControl)
        Function Initialize(graph As AudioGraph) As Task
        Sub OnStartNotify()
        ReadOnly Property OutgoingConnector As ConnectorControl
        ReadOnly Property IngoingConnector As ConnectorControl
    End Interface

End Namespace
