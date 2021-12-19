Namespace Utils

    Public Class Utils

        Public Shared Sub CloseAllDialogs()
            For Each popup In VisualTreeHelper.GetOpenPopups(Window.Current)
                Dim legacyDialog As ContentDialog = TryCast(popup?.Child, ContentDialog)
                If legacyDialog IsNot Nothing Then
                    legacyDialog.Hide()
                End If
            Next
        End Sub

    End Class

End Namespace