Imports ShortDev.Uwp.FullTrust.Xaml

Partial Public Class Program

    <MTAThread()>
    Shared Sub Main(ByVal args() As String)
        Application.Start(Function(p) New App())
    End Sub

    Public Shared Sub Win32Main(args As String())
        FullTrustApplication.Start(Function(p) New App(), New XamlWindowConfig("VBAudioRouter"))
    End Sub

End Class