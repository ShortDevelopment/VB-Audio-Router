Imports Newtonsoft.Json

Namespace AudioGraphControl.Serialization

    Public Class SerializationHelper

        Public Shared Sub WriteGraphToStream(stream As Stream, nodes As IEnumerable(Of Controls.Nodes.INodeControl))
            Dim data As IEnumerable(Of NodeInfo) = nodes.Select(Function(x) New NodeInfo(x))
            Using writer As New StreamWriter(stream)
                writer.Write(JsonConvert.SerializeObject(data))
            End Using
        End Sub

        Private Class NodeInfo
            Public Sub New() : End Sub
            Public Sub New(node As Controls.Nodes.INodeControl)
                Dim nodeContent As IAudioNodeControl = DirectCast(node.NodeContent, IAudioNodeControl)
                Me.Type = nodeContent.GetType()
                Me.Position = node.NodePosition
                If GetType(IAudioNodeSerializable).IsAssignableFrom(Me.Type) Then
                    Me.Payload = JsonConvert.SerializeObject(nodeContent.BaseAudioNode)
                End If
            End Sub

            Public Property Type As Type
            Public Property Position As Point
            Public Property Payload As String

            Public Sub PopulateNodeControl(node As Controls.Nodes.INodeControl)
                If Not node.GetType().FullName = Me.Type.FullName Then Throw New ArgumentException("Wrong node type")
                node.NodePosition = Me.Position
                If GetType(IAudioNodeSerializable).IsAssignableFrom(Me.Type) Then
                    Dim nodeContent As IAudioNodeSerializable = DirectCast(node.NodeContent, IAudioNodeSerializable)
                    JsonConvert.PopulateObject(Payload, nodeContent.BaseAudioNode)
                    nodeContent.ReloadSettings()
                End If
            End Sub
        End Class

    End Class

End Namespace