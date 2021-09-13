Imports VBAudioRouter.Controls
Imports Windows.UI.Xaml.Shapes

Namespace AudioGraphControl

    Public Structure NodeConnection
        Public Property SourceConnector As ConnectorControl
        Public Property DestinationConnector As ConnectorControl
        Public Property Line As Line
    End Structure


End Namespace