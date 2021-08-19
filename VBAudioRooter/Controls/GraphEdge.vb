Imports Windows.UI
Imports PathShape = Windows.UI.Xaml.Shapes.Path

Namespace Controls

    ''' <summary>
    ''' https://stackoverflow.com/a/2824595/15213858
    ''' </summary>
    Public NotInheritable Class GraphEdge
        Inherits UserControl

        Public Shared ReadOnly SourceProperty As DependencyProperty = DependencyProperty.Register("Source", GetType(Point), GetType(GraphEdge), Nothing)

        Public Property Source As Point
            Get
                Return CType(Me.GetValue(SourceProperty), Point)
            End Get
            Set(ByVal value As Point)
                Me.SetValue(SourceProperty, value)
            End Set
        End Property

        Public Shared ReadOnly DestinationProperty As DependencyProperty = DependencyProperty.Register("Destination", GetType(Point), GetType(GraphEdge), Nothing)

        Public Property Destination As Point
            Get
                Return CType(Me.GetValue(DestinationProperty), Point)
            End Get
            Set(ByVal value As Point)
                Me.SetValue(DestinationProperty, value)
            End Set
        End Property

        Public Sub New()
            Dim segment As LineSegment = New LineSegment()
            Dim figure As PathFigure = New PathFigure()
            figure.Segments.Add(segment)
            Dim geometry As PathGeometry = New PathGeometry()
            geometry.Figures.Add(figure)
            Dim sourceBinding As BindingBase = New Binding With {
                .Source = Me,
                .Path = New PropertyPath("SourceProperty")
            }
            Dim destinationBinding As BindingBase = New Binding With {
                .Source = Me,
                .Path = New PropertyPath("DestinationProperty")
            }
            BindingOperations.SetBinding(figure, PathFigure.StartPointProperty, sourceBinding)
            BindingOperations.SetBinding(segment, LineSegment.PointProperty, destinationBinding)
            Content = New PathShape() With {
                .Data = geometry,
                .StrokeThickness = 5,
                .Stroke = New SolidColorBrush(Colors.White),
                .MinWidth = 1,
                .MinHeight = 1
            }
        End Sub
    End Class


End Namespace