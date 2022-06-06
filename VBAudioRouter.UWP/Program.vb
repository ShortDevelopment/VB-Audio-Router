Imports ShortDev.Uwp.FullTrust.Core.Xaml

Partial Public Class Program

    Public Shared Sub WinMain(args As String())
        XamlApplicationWrapper.Run(Of App, WelcomePage)(Sub()
                                                            Dim subclass = XamlWindowSubclass.ForCurrentWindow()
                                                            subclass.UseDarkMode = True
                                                            AddHandler Window.Current.GetSubclass().CloseRequested, AddressOf Program_CloseRequested
                                                        End Sub)
    End Sub

    Private Shared Async Sub Program_CloseRequested(ByVal sender As Object, ByVal e As Navigation.XamlWindowCloseRequestedEventArgs)
        Dim deferral = e.GetDeferral()
        e.Handled = Await App.HandleCloseRequest()
        deferral.Complete()
    End Sub

End Class