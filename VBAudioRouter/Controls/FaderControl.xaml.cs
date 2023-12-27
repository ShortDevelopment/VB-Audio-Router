using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VBAudioRouter.GraphControl;
using VBAudioRouter.UI;

namespace VBAudioRouter.Controls;

[ObservableObject]
internal sealed partial class FaderControl : UserControl
{
    public FaderControl()
    {
        InitializeComponent();
    }

    [ObservableProperty]
    private FaderData? _faderData = null;

    private async void OpenGraphButton_Click(object sender, RoutedEventArgs e)
    {
        if (FaderData is null)
            throw new InvalidOperationException("No fader data assigned");

        await new ContentDialog()
        {
            XamlRoot = XamlRoot,
            Content = new GraphViewPage(FaderData),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        }.ShowAsync();
    }
}
