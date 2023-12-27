using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Diagnostics;
using VBAudioRouter.Capture;
using VBAudioRouter.GraphControl;
using VBAudioRouter.Utils;
using Windows.Media.Audio;

namespace VBAudioRouter.Controls.Nodes;

internal sealed partial class ProcessInputNodeControl : UserControl, IAudioInputNodeControl<AudioFrameInputNode>, IAudioNodeControlFactory<ProcessInputNodeControl>
{
    public IReadOnlyList<Process> Processes { get; private set; }

    readonly AudioGraph _graph;
    private ProcessInputNodeControl(AudioGraph graph)
    {
        _graph = graph;
        Processes = Process.GetProcesses().OrderBy(x => x.ProcessName).ToArray();

        InitializeComponent();
        InputDevices.ItemsSource = Processes.Select(device => device.ProcessName).ToList();
    }

    public Canvas? Canvas { get; set; }
    public ConnectorControl OutgoingConnector => OutgoingConnectorControl;

    public AudioFrameInputNode? GraphNode { get; private set; }

    private ProcessAudioCapture? _capture;
    private void CreateAudioNode()
    {
        if (InputDevices.SelectedIndex < 0)
            return;

        _capture = new ProcessAudioCapture(Processes[InputDevices.SelectedIndex]);
        GraphNode = _capture.CreateAudioNode(_graph);

        GraphNode.OutgoingGain = GainSlider.Value;

        this.ReconnectAudioNode();
    }

    private void InputDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_graph == null)
            return;

        CreateAudioNode();
    }

    private void MuteToggleButton_Click(object sender, RoutedEventArgs e)
    {
        if (GraphNode == null)
            return;

        GraphNode.ConsumeInput = !GraphNode.ConsumeInput;

        if (GraphNode.ConsumeInput)
            MuteButton.Icon = new SymbolIcon(Symbol.Mute);
        else
            MuteButton.Icon = new SymbolIcon(Symbol.Volume);
    }

    private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (GraphNode == null)
            return;

        GraphNode.OutgoingGain = GainSlider.Value.Map(0, 100, 0, GainControl.fxeq_max_gain);
    }

    public static ValueTask<ProcessInputNodeControl> CreateAsync(AudioGraph graph)
        => new(new ProcessInputNodeControl(graph));
}
