
Imports VBAudioRooter.AudioGraphControl
Imports Windows.Devices.Enumeration
Imports Windows.Media.Audio
Imports Windows.Media.Devices

Namespace Controls

    Public NotInheritable Class InputNodeControl
        Inherits UserControl
        Implements IAudioNodeControl

        Property AudioCaptureDevices As DeviceInformationCollection
        Private Async Sub InputNodeControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If DesignMode.DesignModeEnabled Or DesignMode.DesignMode2Enabled Then Exit Sub
            AudioCaptureDevices = Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioCaptureSelector())
            InputDevices.ItemsSource = AudioCaptureDevices.Select(Function(device) device.Name).ToList()
            For i As Integer = 0 To AudioCaptureDevices.Count - 1
                If AudioCaptureDevices.Item(i).IsDefault Then
                    InputDevices.SelectedIndex = i
                    Exit For
                End If
            Next
        End Sub

        Public ReadOnly Property ID As Guid = Guid.NewGuid() Implements IAudioNodeControl.ID
        Public Property Canvas As Canvas Implements IAudioNodeControl.Canvas
        Public ReadOnly Property Node As IAudioNode Implements IAudioNodeControl.Node

        Public ReadOnly Property OutgoingConnector As ConnectorControl Implements IAudioNodeControl.OutgoingConnector
            Get
                Return OutgoingConnectorControl
            End Get
        End Property

        Public Sub AddOutgoingConnection(node As IAudioNodeControl) Implements IAudioNodeControl.AddOutgoingConnection
            DirectCast(Me.Node, AudioDeviceInputNode).AddOutgoingConnection(node.Node)
        End Sub

        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            Me.Graph = graph
            Await CreateAudioNode(graph)
        End Function
        Dim Graph As AudioGraph
        Private Async Function CreateAudioNode(graph As AudioGraph, Optional reconnect As Boolean = False) As Task
            If Node IsNot Nothing Then
                Node.Stop()
                _Node = Nothing
            End If
            Dim result = Await graph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Other, graph.EncodingProperties, AudioCaptureDevices.Item(InputDevices.SelectedIndex))
            If Not result.Status = AudioDeviceNodeCreationStatus.Success Then Throw result.ExtendedError
            _Node = result.DeviceInputNode
            DirectCast(Node, AudioDeviceInputNode).OutgoingGain = GainSlider.Value
            DirectCast(Node, AudioDeviceInputNode).ConsumeInput = Not MuteToggleButton.IsChecked
        End Function

        Private Async Sub InputDevices_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If Graph Is Nothing Then Exit Sub
            Await CreateAudioNode(Graph, True)
        End Sub

        Private Sub MuteToggleButton_Click(sender As Object, e As RoutedEventArgs)
            If Node Is Nothing Then Exit Sub
            DirectCast(Node, AudioDeviceInputNode).ConsumeInput = Not MuteToggleButton.IsChecked
        End Sub

        Private Sub Slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
            If Node Is Nothing Then Exit Sub
            DirectCast(Node, AudioDeviceInputNode).OutgoingGain = GainSlider.Value
        End Sub

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub
    End Class

End Namespace
