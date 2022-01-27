using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static FullTrustUWP.Core.InteropHelper;

namespace FullTrustUWP.Core
{
    internal static class AppxActivatorTest
    {

        [DllImport("daxexec.dll", SetLastError = true)]
        public static extern IntPtr TryActivateDesktopAppXApplication(DesktopAppxActivationParams activationParams);

        public enum DesktopAppxActivationParams
        {
            None = 0x0
        }

        public struct DESKTOP_APPX_ACTIVATION_RESULT { }

        public static void Test()
        {
            int hRes;
            IActivationFactory appActivationFactory;
            Guid guid = new(Const.IID_ICoreApplicationPrivate2);
            ThrowOnError(GetActivationFactory("Windows.ApplicationModel.Core.CoreApplication", ref guid, out appActivationFactory));
            ICoreApplicationPrivate2 app = appActivationFactory as ICoreApplicationPrivate2;

            guid = new(Const.IID_ICoreWindowStatic);
            ThrowOnError(GetActivationFactory("Windows.UI.Core.CoreWindow", ref guid, out appActivationFactory));

            Debugger.Break();
        }
    }

    [Guid(Const.IID_ICoreApplicationPrivate2), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface ICoreApplicationPrivate2
    {

    }

    [Guid(Const.IID_ICoreWindowStatic), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface ICoreWindowStatic
    {

    }
}
