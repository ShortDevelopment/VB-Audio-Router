﻿
Imports VBAudioRooter.AudioGraphControl
Imports Windows.UI
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
        Public Property LinkedNode As IAudioNodeControl = Nothing
        Public ReadOnly Property IsConnected As Boolean
            Get
                Return LinkedNode IsNot Nothing
            End Get
        End Property

        Public Shared Property ConnectorPositionProperty As DependencyProperty = DependencyProperty.Register("ConnectorPosition", GetType(ConnectorControl), GetType(NodeControl), Nothing)
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
                Dim remoteNode = DirectCast(e.DataView.Properties("AttachedNode"), IAudioNodeControl)
                LinkedNode = remoteNode
                Dim remoteConnector = DirectCast(e.DataView.Properties("Connector"), ConnectorControl)
                remoteConnector.LinkedNode = AttachedNode

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
            End If
            e.AcceptedOperation = DataTransfer.DataPackageOperation.None
        End Sub

        Private Sub UserControl_LayoutUpdated(sender As Object, e As Object)
            RecalculatePosition()
        End Sub

        Public Sub RecalculatePosition()
            If AttachedNode.Canvas Is Nothing Then Exit Sub
            ConnectorPosition = Me.TransformToVisual(AttachedNode.Canvas).TransformPoint(New Point(ActualWidth / 2, ActualHeight / 2))
            Debug.Print(ConnectorPosition.ToString())
        End Sub
    End Class

End Namespace

