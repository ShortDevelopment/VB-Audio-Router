Namespace Dialogs

    Public NotInheritable Class ErrorDialog
        Inherits ContentDialog

        Public Property Exception As Exception

        Public Sub New(exception As Exception)
            InitializeComponent()

            Me.Exception = exception
            DataContext = Me
        End Sub

        Public Shadows Property Title As String
            Get
                Return DirectCast(MyBase.Title, String)
            End Get
            Set(value As String)
                MyBase.Title = value
                ' Call Dispatcher.RunIdleAsync(Sub() DirectCast(DirectCast(FindName("Title"), FrameworkElement).FindName("TitleTextBlock"), TextBlock).Text = value)
                'TitleTextBlock.Text = value
            End Set
        End Property

    End Class


End Namespace