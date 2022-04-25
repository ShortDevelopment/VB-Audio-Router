Imports VBAudioRouter.AudioGraphControl
Imports VBAudioRouter.Capture
Imports Windows.Devices.Enumeration
Imports Windows.Media.Audio
Imports Windows.Media.Devices

Namespace Controls.Nodes

    Public NotInheritable Class OutputNodeControl
        Inherits UserControl
        Implements IAudioNodeControl, IAudioNodeControlInput

        Property AudioRenderDevices As DeviceInformationCollection
        Private Async Sub OutputNodeControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If DesignMode.DesignModeEnabled Or DesignMode.DesignMode2Enabled Then Exit Sub
            AudioRenderDevices = Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector())
            OutputDevices.ItemsSource = AudioRenderDevices.Select(Function(device) device.Name).ToList()
            For i As Integer = 0 To AudioRenderDevices.Count - 1
                If AudioRenderDevices.Item(i).IsDefault Then
                    OutputDevices.SelectedIndex = i
                    Exit For
                End If
            Next
        End Sub

#Region "Identity"
        Public Property Canvas As Canvas Implements IAudioNodeControl.Canvas
        Public ReadOnly Property BaseAudioNode As IAudioNode Implements IAudioNodeControl.BaseAudioNode
        Public ReadOnly Property IncomingConnector As ConnectorControl Implements IAudioNodeControlInput.IncomingConnector
            Get
                Return IncomingConnectorControl
            End Get
        End Property
#End Region

        Dim Graph As AudioGraph
        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            Me.Graph = graph
            If BaseAudioNode IsNot Nothing Then
                BaseAudioNode.Stop()
                _BaseAudioNode = Nothing
            End If
            Dim result = Await graph.CreateDeviceOutputNodeAsync()
            If Not result.Status = AudioDeviceNodeCreationStatus.Success Then Throw result.ExtendedError
            _BaseAudioNode = result.DeviceOutputNode

            Dim processCapture As New AudioProcessCapture(Process.GetProcessesByName("Microsoft.Media.Player")(0))
            Dim audioNode = processCapture.CreateAudioNode(graph)
            audioNode.AddOutgoingConnection(BaseAudioNode)
        End Function

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub

        Private Sub OutputDevices_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        End Sub
    End Class

End Namespace