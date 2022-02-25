using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.Interfaces;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace App1
{
    public static class Program
    {

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int GetProcessUIContextInformationDelegate(
            uint processToken,
            ref Int64 info
        );

        [DllImport("user32")]
        static extern int GetProcessUIContextInformation(
            uint processToken,
            ref Int64 info
        );

        static unsafe int GetProcessUIContextInformationImpl(
            uint processToken,
            ref Int64 info
        )
        {
            GetProcessUIContextInformation(processToken, ref info);
            info = 2;
            return 0;
        }

        static void Main(string[] args)
        {
            {
                var hook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("user32.dll", "GetProcessUIContextInformation"),
                    new GetProcessUIContextInformationDelegate(GetProcessUIContextInformationImpl),
                    null);
                hook.ThreadACL.SetExclusiveACL(new int[] { 12345 });
            }

            //var windowFactory1 = CoreWindowFactoryActivator.CreateInstance();
            //windowFactory1.CreateCoreWindow("Test2", out var coreWindow2);
            //coreWindow2.Activate();

            //Window.Current.Activate();

            global::Windows.UI.Xaml.Application.Start((p) => new App());
        }
    }
}
