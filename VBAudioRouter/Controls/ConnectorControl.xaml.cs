using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using VBAudioRouter.Controls.Nodes;
using VBAudioRouter.Dialogs;
using VBAudioRouter.GraphControl;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Media.Audio;

namespace VBAudioRouter.Controls;

[ObservableObject]
internal sealed partial class ConnectorControl : UserControl
{
    public ConnectorControl()
    {
        InitializeComponent();
    }

    public bool IsOutgoing { get; set; } = false;
    public IAudioNodeControl<IAudioNode> AttachedNode => (IAudioNodeControl<IAudioNode>)((INodeControl)DataContext).NodeContent;

    public bool AllowMultipleConnections { get; set; } = false;

    public List<NodeConnection> Connections { get; set; } = [];
    public bool IsConnected => Connections.Count > 0;

    [ObservableProperty]
    Point _connectorPosition;

    private void Grid_DragStarting(UIElement sender, DragStartingEventArgs args)
    {
        if (IsOutgoing)
        {
            args.AllowedOperations = DataPackageOperation.Link;
            args.Data.Properties.Add("AttachedNode", AttachedNode);
            args.Data.Properties.Add("Connector", this);
        }
        else
            args.Cancel = true;
    }

    private void Grid_DragOver(object sender, DragEventArgs e)
    {
        if (IsOutgoing)
            e.AcceptedOperation = DataPackageOperation.None;
        else
            e.AcceptedOperation = DataPackageOperation.Link;
    }

    private async void Grid_Drop(object sender, DragEventArgs e)
    {
        if (!IsOutgoing && e.DataView != null && e.DataView.Properties != null)
        {
            // If we may only create a single connection remove the previous one
            if (IsConnected && !AllowMultipleConnections)
                ConnectionHelper.DeleteConnection(Connections.First());

            var remoteConnector = (ConnectorControl)e.DataView.Properties["Connector"];

            try
            {
                CreateConnection(remoteConnector);
            }
            catch (Exception ex)
            {
                // Call New MessageDialog(ex.Message, "Failed to add connection").ShowAsync()
                ErrorDialog dialog = new(ex)
                {
                    Title = "Failed to add connection"
                };
                await dialog.ShowAsync();
            }
        }
        e.AcceptedOperation = DataPackageOperation.None;
    }

    private void CreateConnection(ConnectorControl remoteConnector)
    {
        // Create Connection
        NodeConnection connection = new();
        connection.SourceConnector = remoteConnector;
        connection.DestinationConnector = this;

        // Add graph connection
        if (remoteConnector.AttachedNode.GraphNode != null)
            ((IAudioInputNode)remoteConnector.AttachedNode.GraphNode).AddOutgoingConnection(AttachedNode.GraphNode);

        // Only create line if connection could be established (AudioGraph)
        connection.Line = CreateConnectionVisual(remoteConnector);

        // Add connection to list
        Connections.Add(connection);
        remoteConnector.Connections.Add(connection);
    }

    private Line CreateConnectionVisual(ConnectorControl remoteConnector)
    {
        Line line = new()
        {
            StrokeThickness = 1,
            Stroke = new SolidColorBrush(Colors.White)
        };
        BindingOperations.SetBinding(line, Line.X1Property, new Binding()
        {
            Source = remoteConnector,
            Path = new PropertyPath("ConnectorPosition.X")
        });
        BindingOperations.SetBinding(line, Line.Y1Property, new Binding()
        {
            Source = remoteConnector,
            Path = new PropertyPath("ConnectorPosition.Y")
        });
        BindingOperations.SetBinding(line, Line.X2Property, new Binding()
        {
            Source = this,
            Path = new PropertyPath("ConnectorPosition.X")
        });
        BindingOperations.SetBinding(line, Line.Y2Property, new Binding()
        {
            Source = this,
            Path = new PropertyPath("ConnectorPosition.Y")
        });
        AttachedNode.Canvas.Children.Add(line);
        return line;
    }

    private void UserControl_LayoutUpdated(object sender, object e)
    {
        if (DesignMode.DesignModeEnabled | DesignMode.DesignMode2Enabled)
            return;
        if (AttachedNode.Canvas == null)
            return;
        ConnectorPosition = TransformToVisual(AttachedNode.Canvas)
            .TransformPoint(new Point(ActualWidth / 2, ActualHeight / 2));
    }
}
