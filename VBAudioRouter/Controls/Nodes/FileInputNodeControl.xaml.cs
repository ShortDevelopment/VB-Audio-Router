using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using VBAudioRouter.GraphControl;
using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace VBAudioRouter.Controls.Nodes;

[ObservableObject]
internal sealed partial class FileInputNodeControl : UserControl, IAudioInputNodeControl<MediaSourceAudioInputNode>, IAudioNodeControlFactory<FileInputNodeControl>
{
    readonly AudioGraph _graph;
    private FileInputNodeControl(AudioGraph graph)
    {
        _graph = graph;

        InitializeComponent();
    }

    public MediaSourceAudioInputNode? GraphNode { get; private set; }

    [ObservableProperty]
    MediaSource? _mediaSource;

    public Canvas? Canvas { get; set; }

    public ConnectorControl OutgoingConnector => OutgoingConnectorControl;

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var file = await CreatePicker().PickSingleFileAsync();
        if (file == null)
            return;

        PathDisplay.Text = file.Path;
        BrowseButton.IsEnabled = false;

        MediaSource = MediaSource.CreateFromStorageFile(file);
        var result = await _graph.CreateMediaSourceAudioInputNodeAsync(MediaSource);
        if (result.Status != MediaSourceAudioInputNodeCreationStatus.Success)
            throw result.ExtendedError;

        GraphNode = result.Node;
        GraphNode.Start();

        this.ReconnectAudioNode();
    }

    static FileOpenPicker CreatePicker()
    {
        FileOpenPicker picker = new()
        {
            SuggestedStartLocation = PickerLocationId.MusicLibrary,
            ViewMode = PickerViewMode.Thumbnail,
            FileTypeFilter =
            {
                ".mp3",
                ".wav",
                ".wma",
                ".m4a",
            }
        };
        InitializeWithWindow.Initialize(picker, Process.GetCurrentProcess().MainWindowHandle);
        return picker;
    }

    public static ValueTask<FileInputNodeControl> CreateAsync(AudioGraph graph)
        => new(new FileInputNodeControl(graph));
}

