using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using static FullTrustUWP.Core.Activation.ApplicationFrameActivator;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //var provider = Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("3480a401-bde9-4407-bc02-798a866ac051")));
            //var factory = provider as ICoreWindowFactory;

            // ApplicationView.GetForCurrentView().TitleBar.

            Form form = new();
            form.Show();

            var frameManager = ApplicationFrameActivator.CreateApplicationFrameManager();

            Marshal.ThrowExceptionForHR(frameManager.GetFrameArray(out var frameArray));
            Marshal.ThrowExceptionForHR(frameArray.GetCount(out var count));
            for (uint i = 0; i < count; i++)
            {
                Guid iid = typeof(IApplicationFrame).GUID;
                Marshal.ThrowExceptionForHR(frameArray.GetAt(i, ref iid, out object frameUnk));
                IApplicationFrame frame2 = frameUnk as IApplicationFrame;
                Marshal.ThrowExceptionForHR(frame2.GetChromeOptions(out var options));
                Marshal.ThrowExceptionForHR(frame2.GetFrameWindow(out var hwndHost));
                frame2.GetPresentedWindow(out var hwndContent);
                Debug.Print(
                    $"HWND: {hwndHost}; TITLE: {GetWindowTitle(hwndHost)};\r\n" +
                    $"CONTENT: {hwndContent}; TITLE: {GetWindowTitle(hwndContent)};\r\n" +
                    $"OPTIONS: {options}"
                );
                frame2.SetBackgroundColor(System.Drawing.Color.Red.ToArgb());
            }

            Marshal.ThrowExceptionForHR(frameManager.CreateFrame(out var frame));
            Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out IntPtr hwnd));

            //var coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", hwnd);
            //IntPtr contentHwnd = (coreWindow as object as ICoreWindowInterop).WindowHandle;

            Marshal.ThrowExceptionForHR(frame.SetChromeOptions(97, 97));
            Marshal.ThrowExceptionForHR(frame.SetBackgroundColor(System.Drawing.Color.Blue.ToArgb()));
            // Marshal.ThrowExceptionForHR(frame.SetPresentedWindow(form.Handle));

            Marshal.ThrowExceptionForHR(frame.GetTitleBar(out var titleBar));

            SetProp(hwnd, "LastSetGlomIdForReconstitution", (IntPtr)1);
            // SetProp(hwnd, "{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3} 5", (IntPtr)0x0000C174);
            SetProp(hwnd, "ApplicationViewCloakType", (IntPtr)2);
            SetProp(hwnd, "43290 (Atom)", (IntPtr)0x2011DDC5BF0);
            RemoteThread.UnCloakWindow(hwnd);

            SetProp(hwnd, "ApplicationViewCloakType", (IntPtr)0);

            Marshal.ThrowExceptionForHR(titleBar.SetWindowTitle($"LK Window - {DateTime.Now}"));

            int value = 0;
            Marshal.ThrowExceptionForHR(DwmGetWindowAttribute(hwnd, (DwmWindowAttribute.Cloaked), out value, Marshal.SizeOf<int>()));

            var x = CoreApplication.Views.ToArray();

            Application.Run(form);

            // XamlHostApplication<App>.Run<WelcomePage>();
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetPropW")]
        static extern bool SetProp(IntPtr hWnd, string lpString, IntPtr hData);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

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
