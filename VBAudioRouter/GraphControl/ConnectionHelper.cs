using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VBAudioRouter.Controls.Nodes;
using Windows.Media.Audio;

namespace VBAudioRouter.GraphControl;
internal static class ConnectionHelper
{
    public static void DeleteConnection(NodeConnection connection)
    {
        // Get graph nodes
        var sourceNode = connection.SourceConnector.AttachedNode.GraphNode as IAudioInputNode;
        var destinationNode = connection.DestinationConnector.AttachedNode.GraphNode;
        // Remove graph connection
        if (destinationNode != null)
            sourceNode?.RemoveOutgoingConnection(destinationNode);

        // Remove visual
        ((Canvas)connection.Line.Parent).Children.Remove(connection.Line);
        // Remove reference
        connection.SourceConnector.Connections.Remove(connection);
        connection.DestinationConnector.Connections.Remove(connection);
    }

    public static void DisposeNode(INodeControl nodeControl)
    {
        var node = (IAudioNodeControl<IAudioNode>)nodeControl.NodeContent;

        List<NodeConnection> connections = [];

        if (nodeControl.NodeContent is IAudioOutputNodeControl<IAudioNode> inputNode)
            connections.AddRange(inputNode.IncomingConnector.Connections);

        if (nodeControl.NodeContent is IAudioInputNodeControl<IAudioInputNode> outputNode)
            connections.AddRange(outputNode.OutgoingConnector.Connections);

        foreach (var connection in connections)
            DeleteConnection(connection);

        // Dispose audio node
        node?.DisposeAudioNode();

        // Remove visual node
        ((Panel)nodeControl.Parent).Children.Remove((UIElement)nodeControl);
    }

    public static void DisposeAudioNode(this IAudioNodeControl<IAudioNode> nodeControl)
    {
        using var node = nodeControl.GraphNode;
        node?.Stop();

        if (nodeControl.GraphNode is not IAudioInputNode inputNode)
            return;

        foreach (var connection in inputNode.OutgoingConnections)
            inputNode.RemoveOutgoingConnection(connection.Destination);
    }

    public static void ReconnectAudioNode(this IAudioInputNodeControl<IAudioInputNode> nodeControl)
    {
        foreach (var connection in nodeControl.OutgoingConnector.Connections)
        {
            var node = connection.DestinationConnector.AttachedNode.GraphNode;
            if (node is null)
                continue;

            nodeControl.GraphNode?.AddOutgoingConnection(node);
        }
    }
}