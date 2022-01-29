using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.ApplicationModel.Activation;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Form form = new();
            form.Show();

            //int hres = InteropHelper.GetActivationFactory("App", ref IID_IActivatableApplication, out var factory);
            //if (hres != 0)
            //    Marshal.ThrowExceptionForHR(hres);
            // IActivatableApplication activatableApplication = new App() as object as IActivatableApplication;

            //var splashScreen = SplashScreenActivator.CreateSplashScreen();
            //splashScreen.TitleBarButtonPressedTextColorOverride = Windows.UI.Colors.Red;

            var frameManager = ApplicationFrameActivator.CreateApplicationFrameManager();
            Marshal.ThrowExceptionForHR(frameManager.CreateFrame(out var frame));
            Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out IntPtr hwnd));

            var coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", hwnd);
            IntPtr contentHwnd = (coreWindow as object as ICoreWindowInterop).WindowHandle;
            Marshal.ThrowExceptionForHR(frame.SetPresentedWindow(ref contentHwnd));

            //ICoreWindowFactory coreWindowFactory = new CoreWindowFactory();
            //IActivatedEventArgs activatedEventArgs = new ActivatedEventArgsImpl();
            //CoreWindowActivator.CoreUICreateICoreWindowFactory(0, IntPtr.Zero, IntPtr.Zero, out var coreWindowFactory);
            //coreWindowFactory.CreateCoreWindow("Test2", out var test);
            // activatableApplication.Activate(ref coreWindowFactory, "App", ref activatedEventArgs);

            // XamlHostApplication<App>.Run<WelcomePage>();
            Application.Run();
        }
    }
}
