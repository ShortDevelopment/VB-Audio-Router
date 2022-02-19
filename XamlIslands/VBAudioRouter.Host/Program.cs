using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.ApplicationFrame;
using FullTrustUWP.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using IServiceProvider = FullTrustUWP.Core.Interfaces.IServiceProvider;

namespace VBAudioRouter.Host
{
    static class Program
    {
        static Form MainForm;
        [STAThread]
        static void Main()
        {
            MainForm = new();
            MainForm.Show();

            // https://raw.githubusercontent.com/fboldewin/COM-Code-Helper/master/code/interfaces.txt
            // GOOGLE: "IApplicationViewCollection" site:lise.pnfsoftware.com

            // Marshal.ThrowExceptionForHR(CoreWindowActivator.CoreUICreateICoreWindowFactory(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, out var factory));

            // https://github.com/tpn/winsdk-10/blob/master/Include/10.0.16299.0/winrt/Windows.UI.Core.CoreWindowFactory.h
            Guid iid = typeof(ICoreWindowFactory).GUID;
            Marshal.ThrowExceptionForHR(ActivationManager_GetActivationFactory("Windows.ApplicationModel.Activation.LaunchActivatedEventArgs", out var factory));
            ICoreWindowFactory coreWindowFactory = Marshal.GetObjectForIUnknown(factory.ActivateInstance()) as ICoreWindowFactory;
            coreWindowFactory.CreateCoreWindow("Test", out var window);
            window.Activate();

            // Marshal.ThrowExceptionForHR(AppxActivatorTest.TryActivateDesktopAppXApplication("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App", out var pid));

            var frameManager = ApplicationFrameActivator.CreateApplicationFrameManager();
            ListAllFrames(frameManager);

            Application.Run(MainForm);

            // XamlHostApplication<App>.Run<WelcomePage>();
        }

        [DllImport("twinapi.appcore.dll", EntryPoint = "DllGetActivationFactory")]
        public static extern int ActivationManager_GetActivationFactory([MarshalAs(UnmanagedType.HString)] string activatableClassId, out IActivationFactory activationFactory);

        [DllImport("Ole32.dll")]
        public static extern int CoLoadLibrary(string lpszLibName, bool bAutoFree);

        static IntPtr hwndNewFrame;
        private static void CreateNewFrame(IApplicationFrameManager frameManager)
        {
            Marshal.ThrowExceptionForHR(frameManager.CreateFrame(out var frame));
            Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out hwndNewFrame));

            Marshal.ThrowExceptionForHR(frame.SetChromeOptions(97, 97));
            Marshal.ThrowExceptionForHR(frame.SetBackgroundColor(System.Drawing.Color.Blue.ToArgb()));
            // Marshal.ThrowExceptionForHR(frame.SetPresentedWindow(form.Handle));

            Marshal.ThrowExceptionForHR(frame.GetTitleBar(out var titleBar));
            Marshal.ThrowExceptionForHR(titleBar.SetWindowTitle($"LK Window - {DateTime.Now}"));
        }

        private static void TryGetFrameFactory()
        {
            var serviceProvider = ImmersiveShellActivator.CreateImmersiveShellServiceProvider();

            Guid iid;
            iid = new Guid("bf63999f-7411-40da-861c-df72c0ffee84");
            // var x = Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("50fdbb99-5c92-495e-9e81-e2c2f48cddae"))) as IUncloakWindowService;
            Marshal.ThrowExceptionForHR(serviceProvider.QueryService(ref iid, ref iid, out object ptr2));
            IServiceProvider uncloakWindowService = (IServiceProvider)ptr2; // IFrameFactory
            iid = typeof(IFrameFactory).GUID;
            Guid iidIUnkown = new Guid("00000000-0000-0000-C000-000000000046");
            Guid serviceId = new Guid("d8c26227-b75e-4d8b-ac8c-c463a34ed11e");
            Marshal.ThrowExceptionForHR(uncloakWindowService.QueryService(ref serviceId, ref iidIUnkown, out object ptr3));
            IFrameFactory frameFactory = (IFrameFactory)ptr3;
        }

        private static void UncloakTests(IntPtr hwnd)
        {
            CloakingHelper.AcquireIAMKey();
            CloakingHelper.EnableIAMAccess(true);
            RemoteThread.UnCloakWindow(hwnd);
            int value = 0;
            // Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(hwnd, (DwmWindowAttribute.Cloak), ref value, Marshal.SizeOf<int>()));
            CloakingHelper.EnableIAMAccess(false);
        }

        #region List Frames
        private static void ListAllFrames(IApplicationFrameManager frameManager)
        {
            #region Immersive Shell
            var serviceProvider = ImmersiveShellActivator.CreateImmersiveShellServiceProvider();

            Guid iid;

            iid = typeof(IApplicationViewCollection).GUID;
            Marshal.ThrowExceptionForHR(serviceProvider.QueryService(ref iid, ref iid, out object ptr));
            IApplicationViewCollection viewCollection = (IApplicationViewCollection)ptr;
            #endregion

            Marshal.ThrowExceptionForHR(frameManager.GetFrameArray(out var frameArray));
            Marshal.ThrowExceptionForHR(frameArray.GetCount(out var count));
            bool alreadyGotVictim = false;
            for (uint i = 0; i < count; i++)
            {
                Guid iid2 = typeof(IApplicationFrame).GUID;
                Marshal.ThrowExceptionForHR(frameArray.GetAt(i, ref iid2, out object frameUnk));
                IApplicationFrame frame = frameUnk as IApplicationFrame;
                Marshal.ThrowExceptionForHR(frame.GetChromeOptions(out var options));
                Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out var hwndHost));
                frame.GetPresentedWindow(out var hwndContent);

                var view = GetApplicationViewForFrame(viewCollection, frame);
                string appUserModelId = "";
                view?.GetAppUserModelId(out appUserModelId);
                if (view != null && !alreadyGotVictim)
                {
                    Marshal.ThrowExceptionForHR(view.SetCloak(ApplicationViewCloakType.DEFAULT, false));
                    Marshal.ThrowExceptionForHR(view.Flash());
                    // Marshal.ThrowExceptionForHR(view.SetCloak(ApplicationViewCloakType.VIRTUAL_DESKTOP, false));
                    //Marshal.ThrowExceptionForHR(frame.SetPresentedWindow(MainForm.Handle));

                    //IntPtr newHwnd = MainForm.Handle;
                    //Marshal.ThrowExceptionForHR(SetWindowLong(newHwnd, -20, GetWindowLong(hwndHost, -20)));
                    //Marshal.ThrowExceptionForHR(SetWindowLong(newHwnd, -16, GetWindowLong(hwndHost, -16)));
                    //if (SetParent(hwndContent, hwndNewFrame) == IntPtr.Zero)
                    //{
                    //    throw new Win32Exception(Marshal.GetLastWin32Error());
                    //}

                    //Marshal.ThrowExceptionForHR(frame.SetBackgroundColor(System.Drawing.Color.Green.ToArgb()));
                    //Marshal.ThrowExceptionForHR(frame.GetTitleBar(out var titleBar));
                    //Marshal.ThrowExceptionForHR(frame.SetApplicationId("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"));
                    alreadyGotVictim = true;
                }

                Debug.Print(
                    $"HWND: {hwndHost}; TITLE: {GetWindowTitle(hwndHost)};\r\n" +
                    $"CONTENT: {hwndContent}; TITLE: {GetWindowTitle(hwndContent)};\r\n" +
                    $"OPTIONS: {options}\r\n" +
                    $"ID: {appUserModelId}\r\n"
                );
            }
        }

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [Flags]
        public enum test
        {
            a = 0x1,
            b = 0x2,
            c = 0x3
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
        #endregion

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

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
