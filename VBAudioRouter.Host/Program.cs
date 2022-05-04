using ShortDev.Uwp.FullTrust.Core.Xaml;
using System;
using Windows.UI.Xaml;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            XamlApplicationWrapper.Run<App, WelcomePage>(() =>
            {
                Window.Current.GetSubclass().CloseRequested += Program_CloseRequested;
            });
        }

        private static async void Program_CloseRequested(object sender, ShortDev.Uwp.FullTrust.Core.Xaml.Navigation.XamlWindowCloseRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();
            e.Handled = await App.HandleCloseRequest();
            deferral.Complete();
        }
    }
}
