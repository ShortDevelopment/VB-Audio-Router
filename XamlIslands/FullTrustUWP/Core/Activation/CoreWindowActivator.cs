using FullTrustUWP.Core.Interfaces;
using System;
using System.Runtime.InteropServices;
using Windows.UI.Core;

namespace FullTrustUWP.Core.Activation
{
    public static class CoreWindowActivator
    {
        public enum WindowType : int
        {
            IMMERSIVE_BODY = 0,
            IMMERSIVE_DOCK,
            IMMERSIVE_HOSTED,
            IMMERSIVE_TEST,
            IMMERSIVE_BODY_ACTIVE,
            IMMERSIVE_DOCK_ACTIVE,
            NOT_IMMERSIVE
        }

        [DllImport("windows.ui.dll", EntryPoint = "#1500")]
        static extern int PrivateCreateCoreWindow(
            WindowType windowType,
            [MarshalAs(UnmanagedType.BStr)] string windowTitle,
            int x,
            int y,
            uint width,
            uint height,
            uint dwAttributes,
            ref IntPtr hOwnerWindow,
            ref Guid riid,
            out ICoreWindowInterop windowRef
        );

        public static CoreWindow CreateCoreWindow(WindowType windowType, string windowTitle, IntPtr hOwnerWindow, int x = 0, int y = 0, uint width = 10, uint height = 10, uint dwAttributes = 0)
        {
            Guid iid = typeof(ICoreWindowInterop).GUID;
            Marshal.ThrowExceptionForHR(PrivateCreateCoreWindow(windowType, windowTitle, x, y, width, height, dwAttributes, ref hOwnerWindow, ref iid, out ICoreWindowInterop windowRef));
            return (windowRef as object as CoreWindow)!;
        }
    }
}
