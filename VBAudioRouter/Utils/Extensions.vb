Imports Windows.UI

Namespace Utils
    Module Extensions
        ''' <summary>
        ''' https://stackoverflow.com/a/14353572/15213858
        ''' </summary>
        ''' <returns></returns>
        <Extension()>
        Public Function Map(ByVal value As Double, ByVal fromSource As Double, ByVal toSource As Double, ByVal fromTarget As Double, ByVal toTarget As Double) As Double
            Return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget
        End Function

        <Extension>
        Public Function FindNameRecursive(Of T As FrameworkElement)(ByRef ele As FrameworkElement, name As String) As T
            For child_index = 0 To VisualTreeHelper.GetChildrenCount(ele) - 1
                Dim child As FrameworkElement = DirectCast(VisualTreeHelper.GetChild(ele, child_index), FrameworkElement)

                Dim search As T = DirectCast(child.FindName(name), T)
                If search IsNot Nothing Then Return search

                Dim recursion As T = FindNameRecursive(Of T)(child, name)
                If recursion IsNot Nothing Then Return recursion
            Next
            Return Nothing
        End Function

        <Extension>
        Public Function FindNameRecursive(ByRef ele As FrameworkElement, name As String) As FrameworkElement
            Return FindNameRecursive(Of FrameworkElement)(ele, name)
        End Function

        ''' <summary>
        ''' https://stackoverflow.com/a/24120993/15213858
        ''' </summary>
        ''' <param name="this"></param>
        <Extension>
        Public Sub BringToFront(ByRef this As FrameworkElement)
            Try
                Dim parent As Panel = DirectCast(this.Parent, Panel)
                Dim currentIndex As Integer = Canvas.GetZIndex(this)
                Dim zIndex As Integer = 0
                Dim maxZ As Integer = 0
                Dim child As FrameworkElement

                For i As Integer = 0 To parent.Children.Count - 1
                    If TypeOf parent.Children(i) Is UserControl AndAlso parent.Children(i) IsNot this Then
                        child = TryCast(parent.Children(i), UserControl)
                        zIndex = Canvas.GetZIndex(child)
                        maxZ = Math.Max(maxZ, zIndex)

                        If zIndex >= currentIndex Then
                            Canvas.SetZIndex(child, zIndex - 1)
                        End If
                    End If
                Next

                Canvas.SetZIndex(this, maxZ)
            Catch : End Try
        End Sub
    End Module

    Public Class ColorTranslator
        ''' <summary>
        ''' http://joeljoseph.net/converting-hex-to-color-in-universal-windows-platform-uwp/
        ''' </summary>
        Public Shared Function FromHex(hex As String) As Color
            hex = hex.Replace("#", String.Empty)
            Dim a As Byte = 255
            Dim r As Byte = CType(Convert.ToUInt32(hex.Substring(0, 2), 16), Byte)
            Dim g As Byte = CType(Convert.ToUInt32(hex.Substring(2, 2), 16), Byte)
            Dim b As Byte = CType(Convert.ToUInt32(hex.Substring(4, 2), 16), Byte)
            Return Color.FromArgb(a, r, g, b)
        End Function
    End Class

End Namespace