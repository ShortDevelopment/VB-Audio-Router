using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using VBAudioRouter.GraphControl;
using Windows.Media.Audio;

namespace VBAudioRouter.UI;

internal sealed partial class MixViewPage : Page
{
    public MixViewPage()
    {
        InitializeComponent();
    }

    AudioGraph _graph = null!;
    AudioDeviceOutputNode _outputNode = null!;
    private async void MixViewPage_Loaded(object sender, RoutedEventArgs e)
    {
        _graph = await AudioGraphHelper.CreateGraphAsync();
        var result = await _graph.CreateDeviceOutputNodeAsync();
        if (result.Status != AudioDeviceNodeCreationStatus.Success)
            throw result.ExtendedError;

        _outputNode = result.DeviceOutputNode;
    }

    public ObservableCollection<FaderData> Faders { get; } = [];
    private void AddFaderButton_Click(object sender, RoutedEventArgs e)
    {
        FaderData faderData = new(_graph);
        faderData.ConnectionNode.AddOutgoingConnection(_outputNode);
        Faders.Add(faderData);
    }
}
