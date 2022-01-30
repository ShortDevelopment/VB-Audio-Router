' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.UI.WindowManagement
Imports Windows.UI.Xaml.Hosting
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class HelpPage
    Inherits Page

    Private Async Sub HelpPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim window = Await AppWindow.TryCreateAsync()
        ElementCompositionPreview.SetAppWindowContent(window, New Button())
        window.TryShowAsync()
    End Sub
End Class
