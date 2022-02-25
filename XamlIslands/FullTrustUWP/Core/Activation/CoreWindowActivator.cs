using FullTrustUWP.Core.Interfaces;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;

namespace FullTrustUWP.Core.Activation
{
    public static class CoreWindowActivator
    {
        public enum WindowType : long
        {
            IMMERSIVE_BODY = 0,
            IMMERSIVE_DOCK,
            IMMERSIVE_HOSTED,
            IMMERSIVE_TEST,
            IMMERSIVE_BODY_ACTIVE,
            IMMERSIVE_DOCK_ACTIVE,
            NOT_IMMERSIVE
        }

        #region CreateCoreWindow
        [DllImport("windows.ui.dll", EntryPoint = "#1500")]
        static extern int PrivateCreateCoreWindow(
            WindowType windowType,
            [MarshalAs(UnmanagedType.BStr)] string windowTitle,
            int x,
            int y,
            uint width,
            uint height,
            uint dwAttributes,
            IntPtr hOwnerWindow,
            Guid riid,
            out ICoreWindowInterop windowRef
        );

        public static CoreWindow CreateCoreWindow(WindowType windowType, string windowTitle, IntPtr hOwnerWindow, int x = 0, int y = 0, uint width = 10, uint height = 10, uint dwAttributes = 0)
        {
            Marshal.ThrowExceptionForHR(PrivateCreateCoreWindow(windowType, windowTitle, x, y, width, height, dwAttributes, hOwnerWindow, typeof(ICoreWindowInterop).GUID, out ICoreWindowInterop windowRef));
            return windowRef as object as CoreWindow;
        }
        #endregion

        #region CreateCoreApplicationViewTitleBar
        [DllImport("twinapi.appcore.dll", EntryPoint = "#501")]
        static extern int CreateCoreApplicationViewTitleBar(
            CoreWindow titleBarClientAdapter,
            IntPtr hWnd,
            out CoreApplicationViewTitleBar titleBar
        );

        public static CoreApplicationViewTitleBar CreateCoreApplicationViewTitleBar(CoreWindow coreWindow, IntPtr hWnd)
        {
            Marshal.ThrowExceptionForHR(CreateCoreApplicationViewTitleBar(coreWindow, hWnd, out var titleBar));
            return titleBar;
        }
        #endregion

        #region CreateApplicationViewTitleBar
        [DllImport("twinapi.appcore.dll", EntryPoint = "#502")]
        static extern int CreateApplicationViewTitleBar(
            AppWindow titleBarClientAdapter,
            IntPtr hWnd,
            out ApplicationViewTitleBar titleBar
        );

        public static ApplicationViewTitleBar CreateApplicationViewTitleBar(AppWindow titleBarClientAdapter, IntPtr hWnd)
        {
            Marshal.ThrowExceptionForHR(CreateApplicationViewTitleBar(titleBarClientAdapter, hWnd, out var titleBar));
            return titleBar;
        }
        #endregion

        /// <summary>
        /// Not really working?!
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("twinapi.appcore.dll", EntryPoint = "#12")]
        public static extern bool IsImmersiveWindow(IntPtr hWnd);

        [DllImport("CoreUIComponents.dll", SetLastError = true)]
        public static extern int CoreUICreateICoreWindowFactory(IntPtr a, IntPtr reserved1, IntPtr reserved2, out ICoreWindowFactory coreWindowFactory);
    }
}
