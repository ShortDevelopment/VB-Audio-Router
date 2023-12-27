using Microsoft.UI.Xaml.Controls;
using VBAudioRouter.GraphControl;
using Windows.Media.Audio;

namespace VBAudioRouter.Controls.Nodes;

internal sealed partial class OutputNodeControl : UserControl, IAudioOutputNodeControl<IAudioNode>, IAudioNodeControlFactory<OutputNodeControl>
{
    public IAudioNode GraphNode { get; }
    public OutputNodeControl(IAudioNode node)
    {
        GraphNode = node;

        InitializeComponent();
    }

    public Canvas? Canvas { get; set; }

    public ConnectorControl IncomingConnector => IncomingConnectorControl;

    public static async ValueTask<OutputNodeControl> CreateAsync(AudioGraph graph)
    {
        var result = await graph.CreateDeviceOutputNodeAsync();
        if (result.Status != AudioDeviceNodeCreationStatus.Success)
            throw result.ExtendedError;

        return new(result.DeviceOutputNode);
    }
}
