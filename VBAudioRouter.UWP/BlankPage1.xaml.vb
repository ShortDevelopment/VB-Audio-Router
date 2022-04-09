' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class BlankPage1
    Inherits Page

    Private Sub BlankPage1_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Window.Current.SetTitleBar(TitleBarElement)
    End Sub
End Class
