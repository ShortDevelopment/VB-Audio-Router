Imports Windows.System

Public NotInheritable Class WelcomePage
    Inherits Page

    Private Sub NavigationView_SelectionChanged(sender As Microsoft.UI.Xaml.Controls.NavigationView, args As Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs)
        Dim navOptions As New FrameNavigationOptions()
        navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo

        Select Case DirectCast(DirectCast(args.SelectedItem, Microsoft.UI.Xaml.Controls.NavigationViewItem).Tag, String)
            Case "Home"
                Throw New NotImplementedException()
            Case "Edit"
                ContentFrame.NavigateToType(GetType(GraphViewPage), Nothing, navOptions)
            Case "AudioControl"
                ContentFrame.NavigateToType(GetType(AudioControlPage), Nothing, navOptions)
            Case "VirtualDevices"
                Throw New NotImplementedException()
            Case "Help"
                ContentFrame.NavigateToType(GetType(HelpPage), Nothing, navOptions)
        End Select
    End Sub

    Private Async Sub NewInstance_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Await Launcher.LaunchUriAsync(New Uri("vb-audio-router://"))
    End Sub
End Class
