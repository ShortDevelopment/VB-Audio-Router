#if NETCOREAPP3_1
using FullTrustUWP.Core.Activation;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.WindowManagement;

namespace FullTrustUWP.Core
{
    class Tests
    {
        static async void Test_AppWindow()
        {
            var instance = await AppWindow.TryCreateAsync();
            // var app = CoreApplication.CreateNewView();
            Debugger.Break();
        }

        static void Test_CreateCoreApplicationViewTitleBar()
        {
            Form form = new();
            form.Show();

            var titleBar = CoreWindowActivator.CreateCoreApplicationViewTitleBar(null, form.Handle /*windowInterop.WindowHandle*/);
            var x = titleBar.Height;

            Application.Run();
        }

        static void Test_CreateCoreWindow()
        {
            Form form = new();
            form.Show();

            CoreWindow window = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.IMMERSIVE_HOSTED, "TestWindow", form.Handle);
            // CoreWindow window = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.IMMERSIVE_BODY_ACTIVE, "TestWindow", IntPtr.Zero);                
            window.Activate();
        }

        [DllImport("daxexec.dll", SetLastError = true)]
        public static extern int TryActivateDesktopAppXApplication(string appUserModelId, out int processId);

        [StructLayout(LayoutKind.Sequential)]
        public struct AppxActivationParams
        {
            public object _1;
            public string launchMode; // Windows.Launch // Windows.Protocol // Windows.File // Windows.ShareTarget // Windows.StartupTask
            public IActivatedEventArgs activatedEventArgs; // cf651713-cd08-4fd8-b697-a281b6544e2e
            public object _2;
            public object _3;
            public object _4;
            public object _5;
            public object _6;
            public object _7;
            public bool _8;
        }
    }
}

#endif