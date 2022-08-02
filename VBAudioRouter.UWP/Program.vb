Partial Public Class Program

    <MTAThread()>
    Shared Sub Main(ByVal args() As String)
        Application.Start(Function(p) New App())
    End Sub

    Public Shared Sub WinMain(args As String())
        FullTrustApplication.Start(Function(p) New App())
    End Sub

End Class