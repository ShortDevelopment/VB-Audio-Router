Imports VBAudioRouter.Controls.Nodes
Imports Windows.Media.Audio

Namespace AudioGraphControl

    Public Class ConnectionHelper

        Public Shared Sub DeleteConnection(connection As NodeConnection)
            ' Get graph nodes
            Dim sourceNode As IAudioInputNode = DirectCast(connection.SourceConnector.AttachedNode.BaseAudioNode, IAudioInputNode)
            Dim destinationNode As IAudioNode = connection.DestinationConnector.AttachedNode.BaseAudioNode
            ' Remove graph connection
            If sourceNode IsNot Nothing Then
                sourceNode.RemoveOutgoingConnection(destinationNode)
            End If

            ' Remove visual
            DirectCast(connection.Line.Parent, Canvas).Children.Remove(connection.Line)
            ' Remove reference
            connection.SourceConnector.Connections.Remove(connection)
            connection.DestinationConnector.Connections.Remove(connection)
        End Sub

        Public Shared Sub DisposeNode(nodeControl As NodeControl)
            Dim node = DirectCast(nodeControl.NodeContent, IAudioNodeControl)

#Region "' Remove visual & graph connections"
            Dim connections As New List(Of NodeConnection)

            Dim inputNode = TryCast(nodeControl.NodeContent, IAudioNodeControlInput)
            If inputNode IsNot Nothing Then
                connections.AddRange(inputNode.IncomingConnector.Connections)
            End If

            Dim outputNode = TryCast(nodeControl.NodeContent, IAudioNodeControlOutput)
            If outputNode IsNot Nothing Then
                connections.AddRange(outputNode.OutgoingConnector.Connections)
            End If

            For Each connection In connections
                DeleteConnection(connection)
            Next
#End Region

            ' Dispose audio node
            If Not node.BaseAudioNode Is Nothing Then
                node.BaseAudioNode.Stop()
                node.BaseAudioNode.Dispose()
            End If

            ' Remove visual node
            DirectCast(nodeControl.Parent, Grid).Children.Remove(nodeControl)
        End Sub

    End Class

    Public Module ConnectionExtensions

        <Extension>
        Public Sub DisposeAudioNode(ByRef nodeControl As IAudioNodeControl)
            Dim node As IAudioInputNode = DirectCast(nodeControl.BaseAudioNode, IAudioInputNode)
            Try
                node.Stop()
                For Each connection In node.OutgoingConnections.ToArray()
                    node.RemoveOutgoingConnection(connection.Destination)
                Next
                node.Dispose()
            Catch : End Try
        End Sub

        <Extension>
        Public Sub ReconnectAudioNode(ByRef nodeControl As IAudioNodeControlOutput)
            For Each connection In nodeControl.OutgoingConnector.Connections
                Dim node As IAudioNode = connection.DestinationConnector.AttachedNode.BaseAudioNode
                DirectCast(nodeControl.BaseAudioNode, IAudioInputNode).AddOutgoingConnection(node)
            Next
        End Sub

    End Module

End Namespace