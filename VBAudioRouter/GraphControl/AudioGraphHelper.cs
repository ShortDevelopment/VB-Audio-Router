using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Render;

namespace VBAudioRouter.GraphControl;

internal static class AudioGraphHelper
{
    public static async ValueTask<AudioGraph> CreateGraphAsync(DeviceInformation? renderDevice = null, AudioRenderCategory category = AudioRenderCategory.Media)
    {
        AudioGraphSettings settings = new(category)
        {
            QuantumSizeSelectionMode = QuantumSizeSelectionMode.LowestLatency,
            PrimaryRenderDevice = renderDevice
        };
        return await CreateGraphAsync(settings);
    }

    public static async ValueTask<AudioGraph> CreateGraphAsync(AudioGraphSettings settings)
    {
        var result = await AudioGraph.CreateAsync(settings);
        if (result.Status != AudioGraphCreationStatus.Success)
            throw result.ExtendedError;

        var graph = result.Graph;
        graph.Start();
        return graph;
    }
}
