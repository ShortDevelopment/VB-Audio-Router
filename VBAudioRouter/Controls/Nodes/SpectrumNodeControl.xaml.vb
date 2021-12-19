Imports AudioVisualizer
Imports VBAudioRouter.AudioGraphControl
Imports Windows.Media.Audio

Namespace Controls.Nodes

    Public NotInheritable Class SpectrumNodeControl
        Inherits UserControl
        Implements IAudioNodeControl, IAudioNodeControlInput

#Region "Identity"
        Public Property Canvas As Canvas Implements IAudioNodeControl.Canvas
        Public ReadOnly Property BaseAudioNode As IAudioNode Implements IAudioNodeControl.BaseAudioNode

        Public ReadOnly Property IncomingConnector As ConnectorControl Implements IAudioNodeControlInput.IncomingConnector
            Get
                Return IncomingConnectorControl
            End Get
        End Property
#End Region

        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            _BaseAudioNode = graph.CreateSubmixNode()

            ' https://github.com/clarkezone/audiovisualizer/blob/47938f7cf592daedd705c125b1e218f93d0bbc4b/samples/VisualizationPlayer/AudioNodePage.xaml.cs#L64
            Dim _source As PlaybackSource = PlaybackSource.CreateFromAudioNode(BaseAudioNode)
            Dim _converter As New SourceConverter()
            _converter.Source = _source.Source
            _converter.MinFrequency = EQNodeControl.MinFreq
            _converter.MaxFrequency = EQNodeControl.MaxFreq
            _converter.FrequencyCount = 12 * 5 * 5 ' 5 octaves, 5 bars per note
            _converter.FrequencyScale = ScaleType.Logarithmic
            _converter.SpectrumRiseTime = TimeSpan.FromMilliseconds(20)
            _converter.SpectrumFallTime = TimeSpan.FromMilliseconds(200)
            _converter.RmsRiseTime = TimeSpan.FromMilliseconds(20) ' Use RMS To gate noise, fast rise slow fall
            _converter.RmsFallTime = TimeSpan.FromMilliseconds(500)
            _converter.ChannelCount = 1
            SpectrumVisualizer.Source = _converter
        End Function

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub
    End Class

End Namespace