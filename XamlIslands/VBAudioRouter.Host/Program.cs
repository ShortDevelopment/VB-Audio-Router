using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using System;
using System.Windows.Forms;
using Windows.ApplicationModel.Activation;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Form form = new();
            //form.Show();

            //int hres = InteropHelper.GetActivationFactory("App", ref IID_IActivatableApplication, out var factory);
            //if (hres != 0)
            //    Marshal.ThrowExceptionForHR(hres);
            // IActivatableApplication activatableApplication = new App() as object as IActivatableApplication;

            ICoreWindowFactory coreWindowFactory = new CoreWindowFactory();
            IActivatedEventArgs activatedEventArgs = new ActivatedEventArgsImpl();
            coreWindowFactory.CreateCoreWindow("Test2", out var test);
            // activatableApplication.Activate(ref coreWindowFactory, "App", ref activatedEventArgs);

            // XamlHostApplication<App>.Run<WelcomePage>();
            Application.Run();
        }
    }
}
