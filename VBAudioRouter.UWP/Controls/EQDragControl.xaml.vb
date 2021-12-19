
Namespace Controls
    Public NotInheritable Class EQDragControl
        Inherits UserControl

        Public Property Canvas As Canvas
        Public Property Index As Integer

        Public Event ValueChanged As EventHandler(Of Point)

        Public Sub SetPosition(p As Point)
            Dim maxX = Canvas.ActualWidth - Me.ActualWidth
            Dim maxY = Canvas.ActualHeight - Me.ActualHeight
            positionTransform.TranslateX = p.X * maxX
            positionTransform.TranslateY = (1 - p.Y) * maxY
        End Sub

        Private Sub Grid_ManipulationDelta(sender As Object, e As ManipulationDeltaRoutedEventArgs)
            Dim maxX = Canvas.ActualWidth - Me.ActualWidth
            Dim maxY = Canvas.ActualHeight - Me.ActualHeight
            positionTransform.TranslateX = Math.Min(Math.Max(positionTransform.TranslateX + e.Delta.Translation.X, 0), maxX)
            positionTransform.TranslateY = Math.Min(Math.Max(positionTransform.TranslateY + e.Delta.Translation.Y, 0), maxY)
            RaiseEvent ValueChanged(Me, New Point(positionTransform.TranslateX / maxX, 1 - (positionTransform.TranslateY / maxY)))
        End Sub
    End Class

End Namespace
