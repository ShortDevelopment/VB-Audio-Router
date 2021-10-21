
Imports VBAudioRouter.AudioGraphControl
Imports Windows.Media.Audio
Imports Windows.UI
Imports Windows.UI.Popups
Imports Windows.UI.Xaml.Shapes

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

        Public Property AllowMultipleConnections As Boolean = False

        Public Property Connections As New List(Of NodeConnection)
        Public ReadOnly Property IsConnected As Boolean
            Get
                Return Connections.Count > 0
            End Get
        End Property

        Public Shared Property ConnectorPositionProperty As DependencyProperty = DependencyProperty.Register("ConnectorPosition", GetType(Point), GetType(ConnectorControl), Nothing)
        Public Property ConnectorPosition As Point
            Get
                Return GetValue(ConnectorPositionProperty)
            End Get
            Set(value As Point)
                SetValue(ConnectorPositionProperty, value)
                OnPropertyChanged()
            End Set
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
            Else
                args.Cancel = True
            End If
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
                ' If we may only create a single connection remove the previous one
                If IsConnected AndAlso Not AllowMultipleConnections Then ConnectionHelper.DeleteConnection(Connections.First())

                Dim remoteConnector = DirectCast(e.DataView.Properties("Connector"), ConnectorControl)

                Try
                    CreateConnection(remoteConnector)
                Catch ex As Exception
                    'Call New MessageDialog(ex.Message, "Failed to add connection").ShowAsync()
                    Dim dialog As New Dialogs.ErrorDialog(ex)
                    dialog.Title = "Failed to add connection"
                    Call dialog.ShowAsync()
                End Try
            End If
            e.AcceptedOperation = DataTransfer.DataPackageOperation.None
        End Sub

        Private Sub CreateConnection(remoteConnector As ConnectorControl)
            ' Create Connection
            Dim connection As New NodeConnection()
            connection.SourceConnector = remoteConnector
            connection.DestinationConnector = Me

            ' Add graph connection
            If remoteConnector.AttachedNode.BaseAudioNode IsNot Nothing Then
                DirectCast(remoteConnector.AttachedNode.BaseAudioNode, IAudioInputNode).AddOutgoingConnection(AttachedNode.BaseAudioNode)
            End If

            ' Only create line if connection could be established (AudioGraph)
            connection.Line = CreateConnectionVisual(remoteConnector)

            ' Add connection to list
            Connections.Add(connection)
            remoteConnector.Connections.Add(connection)
        End Sub

        Private Function CreateConnectionVisual(remoteConnector As ConnectorControl) As Line
            Dim line As New Line()
            line.StrokeThickness = 1
            line.Stroke = New SolidColorBrush(Colors.White)
            BindingOperations.SetBinding(line, Line.X1Property, New Binding() With {
                .Source = remoteConnector,
                .Path = New PropertyPath("ConnectorPosition.X")
            })
            BindingOperations.SetBinding(line, Line.Y1Property, New Binding() With {
                .Source = remoteConnector,
                .Path = New PropertyPath("ConnectorPosition.Y")
            })
            BindingOperations.SetBinding(line, Line.X2Property, New Binding() With {
                .Source = Me,
                .Path = New PropertyPath("ConnectorPosition.X")
            })
            BindingOperations.SetBinding(line, Line.Y2Property, New Binding() With {
                .Source = Me,
                .Path = New PropertyPath("ConnectorPosition.Y")
            })
            AttachedNode.Canvas.Children.Add(line)
            Return line
        End Function

        Private Sub UserControl_LayoutUpdated(sender As Object, e As Object)
            If DesignMode.DesignModeEnabled Or DesignMode.DesignMode2Enabled Then Exit Sub
            If AttachedNode.Canvas Is Nothing Then Exit Sub
            ConnectorPosition = Me.TransformToVisual(AttachedNode.Canvas).TransformPoint(New Point(ActualWidth / 2, ActualHeight / 2))
        End Sub
    End Class

End Namespace

