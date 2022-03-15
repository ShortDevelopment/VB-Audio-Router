using FullTrustUWP.Core.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App1
{
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            XamlWindowActivator.UseUwp();
            var window = await XamlWindowActivator.ActivateXamlWindowAsync("Test", out var thread);
        }
    }
}
