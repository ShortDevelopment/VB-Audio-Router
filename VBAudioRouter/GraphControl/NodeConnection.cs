using Microsoft.UI.Xaml.Shapes;
using VBAudioRouter.Controls;

namespace VBAudioRouter.GraphControl;
internal struct NodeConnection
{
    public ConnectorControl SourceConnector { get; set; }
    public ConnectorControl DestinationConnector { get; set; }
    public Line Line { get; set; }
}

