using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Windows.Media.Audio;

namespace VBAudioRouter.GraphControl;
internal sealed class FaderData(AudioGraph graph) : ObservableObject
{
    public AudioGraph AudioGraph { get; } = graph;
    public AudioSubmixNode ConnectionNode { get; } = graph.CreateSubmixNode();

    public ObservableCollection<IAudioNodeControl<IAudioNode>> AudioNodes { get; } = [];
}
