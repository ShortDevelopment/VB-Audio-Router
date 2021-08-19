
Imports VBAudioRooter.AudioGraphControl

Namespace Controls

    Public NotInheritable Class ConnectorControl
        Inherits UserControl
        Implements INotifyPropertyChanged

        Public Property IsOutgoing As Boolean = False
        Public ReadOnly Property AttachedNode As IAudioNodeControl
            Get
                Return DirectCast(DirectCast(DataContext, NodeControl).NodeContent, IAudioNodeControl)
            End Get
        End Property
        Public Property LinkedNode As IAudioNodeControl = Nothing
        Public ReadOnly Property IsConnected As Boolean
            Get
                Return LinkedNode IsNot Nothing
            End Get
        End Property

#Region "INotifyPropertyChanged"
        Private Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
#End Region

        Private Sub Grid_DragStarting(sender As UIElement, args As DragStartingEventArgs)
            If IsOutgoing Then
                args.AllowedOperations = DataTransfer.DataPackageOperation.Link
                args.Data.Properties.Add("AttachedNode", AttachedNode)
                args.Data.Properties.Add("Connector", Me)

                Exit Sub
            ElseIf LinkedNode IsNot Nothing Then
            End If
            args.Cancel = True
        End Sub

        Private Sub Grid_DragOver(sender As Object, e As DragEventArgs)
            If IsOutgoing Then
                e.AcceptedOperation = DataTransfer.DataPackageOperation.None
            Else
                e.AcceptedOperation = DataTransfer.DataPackageOperation.Link
            End If
        End Sub

        Private Sub Grid_Drop(sender As Object, e As DragEventArgs)
            If Not IsOutgoing AndAlso e.DataView IsNot Nothing AndAlso e.DataView.Properties IsNot Nothing Then
                LinkedNode = DirectCast(e.DataView.Properties("AttachedNode"), IAudioNodeControl)
                DirectCast(e.DataView.Properties("Connector"), ConnectorControl).LinkedNode = AttachedNode
            End If
            e.AcceptedOperation = DataTransfer.DataPackageOperation.None
        End Sub
    End Class

End Namespace

