Imports VBAudioRouter.Utils
Imports Windows.UI

NotInheritable Class App
    Inherits Application

    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim titlebar = ApplicationView.GetForCurrentView().TitleBar
        Dim themeColor = ColorTranslator.FromHex("#E87D0D")
        Dim darkThemeColor = ColorTranslator.FromHex("#232323")
        ' Active TitleBar
        titlebar.ForegroundColor = Colors.White
        titlebar.BackgroundColor = darkThemeColor
        titlebar.ButtonBackgroundColor = darkThemeColor
        ' Inactive TitleBar
        titlebar.InactiveForegroundColor = Colors.LightGray
        titlebar.InactiveBackgroundColor = darkThemeColor
        titlebar.ButtonInactiveBackgroundColor = darkThemeColor

        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)
        If rootFrame Is Nothing Then
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: Zustand von zuvor angehaltener Anwendung laden
            End If
            Window.Current.Content = rootFrame
        End If

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                rootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If

            Window.Current.Activate()
        End If
    End Sub

    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
        deferral.Complete()
    End Sub

End Class
