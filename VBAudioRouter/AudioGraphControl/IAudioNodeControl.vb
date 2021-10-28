
Imports VBAudioRouter.Controls
Imports Windows.Media.Audio

Namespace AudioGraphControl

    Public Interface IAudioNodeControl
        ReadOnly Property BaseAudioNode As IAudioNode

#Region "State"
        Function Initialize(graph As AudioGraph) As Task
        Sub OnStateChanged(state As GraphState)
#End Region

        Property Canvas As Canvas
    End Interface

    Public Interface IAudioNodeControlInput
        Inherits IAudioNodeControl

        ReadOnly Property IncomingConnector As ConnectorControl
    End Interface

    Public Interface IAudioNodeControlEffect
        Inherits IAudioNodeControl
    End Interface

    Public Interface IAudioNodeControlOutput
        Inherits IAudioNodeControl

        ReadOnly Property OutgoingConnector As ConnectorControl
    End Interface

End Namespace
