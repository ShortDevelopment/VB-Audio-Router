using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using VBAudioRouter.Controls.Nodes;
using VBAudioRouter.GraphControl;
using Windows.Media.Audio;
using NodeFactoryProc = System.Func<
    Windows.Media.Audio.AudioGraph,
    System.Threading.Tasks.Task<(
        string displayName,
        VBAudioRouter.GraphControl.IAudioNodeControl<Windows.Media.Audio.IAudioNode>
    )>
>;

namespace VBAudioRouter.UI;
internal sealed partial class GraphViewPage : Page
{
    public AudioGraph CurrentAudioGraph => _faderData.AudioGraph;

    readonly FaderData _faderData;
    public GraphViewPage(FaderData faderData)
    {
        _faderData = faderData;
        InitializeComponent();

        AddNode("Output", new OutputNodeControl(faderData.ConnectionNode));
    }

    static readonly Dictionary<string, NodeFactoryProc> FactoryLookup = new() {
        { nameof(FileInputNodeControl), PrepareNodeFactory<FileInputNodeControl>() },
        { nameof(ProcessInputNodeControl), PrepareNodeFactory<ProcessInputNodeControl>() }
    };

    static NodeFactoryProc PrepareNodeFactory<TFactory>()
        where TFactory : IAudioNodeControlFactory<TFactory>, IAudioNodeControl<IAudioNode>
    {
        return static async (AudioGraph graph) => (TFactory.DisplayName, await TFactory.CreateAsync(graph));
    }

    private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string tag = (string)((MenuFlyoutItem)sender).Tag;
            if (string.IsNullOrEmpty(tag) || CurrentAudioGraph is null)
                return;

            var factory = FactoryLookup[tag] ?? throw new InvalidOperationException("Could not find audio node factory");
            var (displayName, contentEle) = await factory.Invoke(CurrentAudioGraph);

            AddNode(displayName, contentEle);
        }
        catch (Exception ex)
        {
            Dialogs.ErrorDialog dialog = new(ex)
            {
                Title = "Failed to add node"
            };
            await dialog.ShowAsync();
        }
    }

    void AddNode(string displayName, IAudioNodeControl<IAudioNode> nodeControl)
    {
        NodeControl nodeContainer = new()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Title = displayName
        };

        nodeControl.Canvas = ConnectionCanvas;

        // Assign content to container ("window")
        nodeContainer.NodeContent = (UIElement)nodeControl;
        NodeContainer.Children.Add(nodeContainer);
    }

    private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        return;
        if (e.OriginalSource != ViewPort)
            return;
        ViewPortTransform.TranslateX += e.Delta.Translation.X;
        ViewPortTransform.TranslateY += e.Delta.Translation.Y;
    }
}

