Imports VBAudioRooter.AudioGraphControl
Imports Windows.Devices.Enumeration
Imports Windows.Media.Audio
Imports Windows.Media.Devices

Namespace Controls

    Public NotInheritable Class OutputNodeControl
        Inherits UserControl
        Implements IAudioNodeControl

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

        Public ReadOnly Property ID As Guid = Guid.NewGuid() Implements IAudioNodeControl.ID
        Public Property Canvas As Canvas Implements IAudioNodeControl.Canvas
        Public ReadOnly Property Node As IAudioNode Implements IAudioNodeControl.Node

        Public ReadOnly Property OutgoingConnector As ConnectorControl = Nothing Implements IAudioNodeControl.OutgoingConnector

        Public Sub AddOutgoingConnection(node As IAudioNodeControl) Implements IAudioNodeControl.AddOutgoingConnection
            Throw New NotImplementedException()
        End Sub

        Dim Graph As AudioGraph
        Public Async Function Initialize(graph As AudioGraph) As Task Implements IAudioNodeControl.Initialize
            Me.Graph = graph
            If Node IsNot Nothing Then
                Node.Stop()
                _Node = Nothing
            End If
            Dim result = Await graph.CreateDeviceOutputNodeAsync()
            If Not result.Status = AudioDeviceNodeCreationStatus.Success Then Throw result.ExtendedError
            _Node = result.DeviceOutputNode
        End Function

        Public Sub OnStateChanged(state As GraphState) Implements IAudioNodeControl.OnStateChanged : End Sub

        Private Sub OutputDevices_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        End Sub
    End Class

End Namespace