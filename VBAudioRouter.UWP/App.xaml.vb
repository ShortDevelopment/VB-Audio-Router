Imports Microsoft.Toolkit.Win32.UI.XamlHost
Imports VBAudioRouter.Dialogs
Imports VBAudioRouter.Utils
Imports Windows.UI
Imports Windows.UI.Core.Preview

NotInheritable Class App
    Inherits XamlApplication

    Public Sub New()
        Initialize()
        InitializeComponent()
    End Sub

    Protected Overrides Async Sub OnLaunched(e As LaunchActivatedEventArgs)
        LaunchApp(e)
    End Sub

    Protected Overrides Sub OnActivated(args As IActivatedEventArgs)
        LaunchApp(args)
    End Sub

    Private Sub LaunchApp(args As IActivatedEventArgs)
#Region "TitleBar"
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
#End Region

#Region "Close confirm"
        AddHandler SystemNavigationManagerPreview.GetForCurrentView().CloseRequested, Async Sub(sender As Object, ev As SystemNavigationCloseRequestedPreviewEventArgs)
                                                                                          Dim deferral As Deferral = ev.GetDeferral()
                                                                                          Try
                                                                                              ' Force all legacy dialogs to close
                                                                                              Utils.Utils.CloseAllDialogs()
                                                                                              ' Show confirmation dialog
                                                                                              Dim dialog As New CloseConfirmDialog()
                                                                                              ev.Handled = (Await dialog.ShowAsync()) = ContentDialogResult.Secondary
                                                                                          Finally
                                                                                              deferral.Complete()
                                                                                          End Try
                                                                                      End Sub
#End Region

#Region "Exception handling"
        AddHandler Me.UnhandledException, Sub(sender As System.Object, ev As Xaml.UnhandledExceptionEventArgs)
                                              ev.Handled = True
                                              Call (Async Sub()
                                                        Try
                                                            ' Force all legacy dialogs to close
                                                            Utils.Utils.CloseAllDialogs()
                                                            ' Show error dialog
                                                            Dim dialog As New ErrorDialog(ev.Exception)
                                                            Await dialog.ShowAsync()
                                                        Catch : End Try
                                                    End Sub)()
                                          End Sub
#End Region

        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)
        If rootFrame Is Nothing Then
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: Zustand von zuvor angehaltener Anwendung laden
            End If
            Window.Current.Content = rootFrame
        End If

        'If args.PrelaunchActivated = False Then
        If rootFrame.Content Is Nothing Then
            rootFrame.Navigate(GetType(WelcomePage))
        End If

        Window.Current.Activate()
        'End If
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
