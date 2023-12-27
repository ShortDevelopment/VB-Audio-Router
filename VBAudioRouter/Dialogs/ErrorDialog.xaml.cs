using Microsoft.UI.Xaml.Controls;

namespace VBAudioRouter.Dialogs;

internal sealed partial class ErrorDialog : ContentDialog
{
    public Exception Exception
    {
        get;
    }

    public ErrorDialog(Exception exception)
    {
        InitializeComponent();

        Exception = exception;
        DataContext = this;
    }
}
