using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.Interfaces;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Form form = new();
            form.Show();

            var frameManager = ApplicationFrameActivator.CreateApplicationFrameManager();

            #region Immersive Shell
            var serviceProvider = ImmersiveShellActivator.CreateImmersiveShellServiceProvider();
            Guid iid = typeof(IApplicationViewCollection).GUID;
            Marshal.ThrowExceptionForHR(serviceProvider.QueryService(ref iid, ref iid, out object ptr));
            IApplicationViewCollection viewCollection = (IApplicationViewCollection)ptr;
            #endregion

            Marshal.ThrowExceptionForHR(frameManager.GetFrameArray(out var frameArray));
            Marshal.ThrowExceptionForHR(frameArray.GetCount(out var count));
            for (uint i = 0; i < count; i++)
            {
                Guid iid2 = typeof(IApplicationFrame).GUID;
                Marshal.ThrowExceptionForHR(frameArray.GetAt(i, ref iid2, out object frameUnk));
                IApplicationFrame frame2 = frameUnk as IApplicationFrame;
                Marshal.ThrowExceptionForHR(frame2.GetChromeOptions(out var options));
                Marshal.ThrowExceptionForHR(frame2.GetFrameWindow(out var hwndHost));
                frame2.GetPresentedWindow(out var hwndContent);

                frame2.SetBackgroundColor(System.Drawing.Color.Red.ToArgb());

                var view = GetApplicationViewForFrame(viewCollection, frame2);
                string appUserModelId = "";
                view?.GetAppUserModelId(out appUserModelId);
                if(view != null)
                {
                    Marshal.ThrowExceptionForHR(view.SetCloak(ApplicationViewCloakType.DEFAULT, false));
                    Marshal.ThrowExceptionForHR(view.SetCloak(ApplicationViewCloakType.VIRTUAL_DESKTOP, true));
                    Marshal.ThrowExceptionForHR(view.Flash());
                }

                Debug.Print(
                    $"HWND: {hwndHost}; TITLE: {GetWindowTitle(hwndHost)};\r\n" +
                    $"CONTENT: {hwndContent}; TITLE: {GetWindowTitle(hwndContent)};\r\n" +
                    $"OPTIONS: {options}\r\n" +
                    $"ID: {appUserModelId}\r\n"
                );
            }

            Marshal.ThrowExceptionForHR(frameManager.CreateFrame(out var frame));
            Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out IntPtr hwnd));

            //var coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", hwnd);
            //IntPtr contentHwnd = (coreWindow as object as ICoreWindowInterop).WindowHandle;

            Marshal.ThrowExceptionForHR(frame.SetChromeOptions(97, 97));
            Marshal.ThrowExceptionForHR(frame.SetBackgroundColor(System.Drawing.Color.Blue.ToArgb()));
            // Marshal.ThrowExceptionForHR(frame.SetPresentedWindow(form.Handle));

            // RemoteThread.UnCloakWindow2(hwnd);

            var desktopManager = VirtualDesktopManagerActivator.CreateVirtualDesktopManager();
            Marshal.ThrowExceptionForHR(desktopManager.GetWindowDesktopId(form.Handle, out Guid desktopId));
            // Marshal.ThrowExceptionForHR(desktopManager.MoveWindowToDesktop((IntPtr)0x30BEE, ref desktopId));

            Marshal.ThrowExceptionForHR(frame.GetTitleBar(out var titleBar));
            Marshal.ThrowExceptionForHR(titleBar.SetWindowTitle($"LK Window - {DateTime.Now}"));

            int value = 0;
            Marshal.ThrowExceptionForHR(DwmGetWindowAttribute(hwnd, (DwmWindowAttribute.Cloaked), out value, Marshal.SizeOf<int>()));

            Application.Run(form);

            // XamlHostApplication<App>.Run<WelcomePage>();
        }

        private static IApplicationView? GetApplicationViewForFrame(IApplicationViewCollection collection, IApplicationFrame frame)
        {
            try
            {
                Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out IntPtr hwnd));
                Marshal.ThrowExceptionForHR(collection.GetViewForHwnd(hwnd, out var view));
                return view;
            }
            catch
            {
                return null;
            }
        }

        private static string GetWindowTitle(IntPtr hWnd)
        {
            StringBuilder stringBuilder = new();
            Marshal.ThrowExceptionForHR(GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity));
            return stringBuilder.ToString();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DwmWindowAttribute attr, out int attrValue, int attrSize);

        /// <summary>
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute"/>
        /// </summary>
        public enum DwmWindowAttribute : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation,
            PASSIVE_UPDATE_MODE,
            USE_HOSTBACKDROPBRUSH,
            USE_IMMERSIVE_DARK_MODE,
            WINDOW_CORNER_PREFERENCE,
            BORDER_COLOR,
            CAPTION_COLOR,
            TEXT_COLOR,
            VISIBLE_FRAME_BORDER_THICKNESS,
            LAST
        }

        public enum DwmCloakedByValue
        {
            OwnerProcess = 1,
            Shell,
            ParentWindow
        }
    }
}
