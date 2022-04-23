using FullTrustUWP.Core.Xaml;
using System;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            XamlApplicationWrapper.Run<App, WelcomePage>();
        }
    }
}
