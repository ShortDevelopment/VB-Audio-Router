using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace VBAudioRouter.UI;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {

    }

    private async void NewInstanceNavigationViewItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("vb-audio-router://"));
    }
}
