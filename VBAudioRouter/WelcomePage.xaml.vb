Imports Windows.System

Public NotInheritable Class WelcomePage
    Inherits Page

    Private Sub NavigationView_SelectionChanged(sender As Microsoft.UI.Xaml.Controls.NavigationView, args As Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs)
        Select Case DirectCast(DirectCast(args.SelectedItem, Microsoft.UI.Xaml.Controls.NavigationViewItem).Tag, String)
            Case "Home"
                Throw New NotImplementedException()
            Case "Edit"
                ContentFrame.Navigate(GetType(GraphViewPage))
            Case "AudioControl"
                ContentFrame.Navigate(GetType(AudioControlPage))
            Case "VirtualDevices"
                Throw New NotImplementedException()
            Case "Help"
                ContentFrame.Navigate(GetType(HelpPage))
        End Select
    End Sub

    Private Async Sub NewInstance_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Await Launcher.LaunchUriAsync(New Uri("vb-audio-router://"))
    End Sub
End Class
