using Microsoft.UI.Xaml.Controls;
using System.Text;
using VBAudioRouter.Controls;
using Windows.Media.Audio;

namespace VBAudioRouter.GraphControl;

internal interface IAudioNodeControl<out TNode> where TNode : IAudioNode
{
    TNode? GraphNode { get; }
    Canvas? Canvas { get; set; }
}

internal interface IAudioNodeControlFactory<TSelf> where TSelf : IAudioNodeControl<IAudioNode>
{
    static abstract ValueTask<TSelf> CreateAsync(AudioGraph graph);
    static virtual string DisplayName
    {
        get
        {
            var typeName = typeof(TSelf).GetType().Name.AsSpan();

            StringBuilder builder = new();
            for (int i = 0; i < typeName.Length; i++)
            {
                char c = typeName[i];

                if (c == 'N') // ToDo: Better check for "NodeControl"
                    break;

                if (char.IsUpper(c))
                    builder.Append(' ');

                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}

internal interface IAudioInputNodeControl<out TNode> : IAudioNodeControl<TNode>
    where TNode : IAudioInputNode
{
    ConnectorControl OutgoingConnector { get; }
}

internal interface IAudioEffectNodeControl : IAudioNodeControl<AudioSubmixNode>
{
}

internal interface IAudioOutputNodeControl<out TNode> : IAudioNodeControl<TNode>
    where TNode : IAudioNode
{
    ConnectorControl IncomingConnector { get; }
}