using FullTrustUWP;
using System;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            XamlHostApplication<App>.Run<WelcomePage>();
        }
    }
}
