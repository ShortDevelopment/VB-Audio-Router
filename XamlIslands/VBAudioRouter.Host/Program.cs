using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Application = System.Windows.Forms.Application;

namespace VBAudioRouter.Host
{
    static class Program
    {
        #region HookStuff
        [DllImport("combase.dll"), PreserveSig]
        static extern int RoGetServerActivatableClasses(
            [MarshalAs(UnmanagedType.HString)] string serverName,
            out IntPtr activatableClassIds,
            out uint count
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int RoGetServerActivatableClassesDelegate(
            [MarshalAs(UnmanagedType.HString)] string serverName,
            out IntPtr activatableClassIds,
            out uint count
        );

        static unsafe int RoGetServerActivatableClassesImpl(
            string serverName,
            out IntPtr activatableClassIds,
            out uint count
        )
        {
            //List<string> classIds = new List<string>();
            //Marshal.ThrowExceptionForHR(RoGetServerActivatableClasses(serverName, out activatableClassIds, out count));
            //IntPtr* hstringPtr = (IntPtr*)activatableClassIds;
            //for (int i = 0; i < count; i++)
            //{
            //    string classId = new HString(hstringPtr[i]).Value;
            //    classIds.Add(classId);
            //}
            IntPtr[] classIds = new[]
            {
                new HString("App").Ptr,
                new HString(serverName).Ptr
            };

            fixed (IntPtr* ptr = classIds)
            {
                activatableClassIds = (IntPtr)ptr;
            }
            count = 2;
            return 0;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int RoRegisterActivationFactoriesDelegate(
            IntPtr activatableClassIds,
            IntPtr activationFactoryCallbacks,
            uint count,
            out IntPtr cookie
        );

        static unsafe int RoRegisterActivationFactoriesImpl(
            IntPtr activatableClassIds,
            IntPtr activationFactoryCallbacks,
            uint count,
            out IntPtr cookie
        )
        {
            List<string> classIds = new List<string>();
            IntPtr* hstringPtr = (IntPtr*)activatableClassIds;
            for (int i = 0; i < count; i++)
            {
                string classId = new HString(hstringPtr[i]).Value;
                classIds.Add(classId);
            }
            cookie = (IntPtr)12345;
            return 0;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int RoGetActivationFactoryDelegate(
            [MarshalAs(UnmanagedType.HString)] string activatableClassId,
            ref Guid iid,
            out IActivationFactory factory
        );

        static unsafe int RoGetActivationFactoryImpl(
            [MarshalAs(UnmanagedType.HString)] string activatableClassId,
            ref Guid iid,
            out IActivationFactory factory
        )
        {
            InteropHelper.GetActivationFactory(activatableClassId, ref iid, out factory);
            Debug.Print(activatableClassId);
            return 0;
        }
        #endregion

        static Form MainForm;
        [MTAThread]
        static void Main()
        {
            // https://raw.githubusercontent.com/fboldewin/COM-Code-Helper/master/code/interfaces.txt
            // GOOGLE: "IApplicationViewCollection" site:lise.pnfsoftware.com

            //{
            //    var hook = EasyHook.LocalHook.Create(
            //    EasyHook.LocalHook.GetProcAddress("combase.dll", "RoGetServerActivatableClasses"),
            //    new RoGetServerActivatableClassesDelegate(RoGetServerActivatableClassesImpl),
            //    null);
            //    hook.ThreadACL.SetInclusiveACL(new int[] { 0 });
            //}
            //{
            //    var hook = EasyHook.LocalHook.Create(
            //    EasyHook.LocalHook.GetProcAddress("combase.dll", "RoRegisterActivationFactories"),
            //    new RoRegisterActivationFactoriesDelegate(RoRegisterActivationFactoriesImpl),
            //    null);
            //    hook.ThreadACL.SetInclusiveACL(new int[] { 0 });
            //}
            //{
            //    var hook = EasyHook.LocalHook.Create(
            //    EasyHook.LocalHook.GetProcAddress("combase.dll", "RoGetActivationFactory"),
            //    new RoGetActivationFactoryDelegate(RoGetActivationFactoryImpl),
            //    null);
            //    hook.ThreadACL.SetInclusiveACL(new int[] { 0 });
            //}

            //var hook2 = EasyHook.LocalHook.Create(
            //    EasyHook.LocalHook.GetProcAddress("Kernel32.dll", "GetCommandLineW"),
            //    new RoGetServerActivatableClassesDelegate(RoGetServerActivatableClassesImpl),
            //    null);

            //var windowFactory1 = CoreWindowFactoryActivator.CreateInstance();
            //windowFactory1.CreateCoreWindow("Test2", out var coreWindow2);
            //coreWindow2.Activate();

            #region CoreWindow
            CoreWindow coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", IntPtr.Zero, 30, 30, 1024, 768, 0);
            coreWindow.Activate();

            var hWnd = (coreWindow as object as ICoreWindowInterop).WindowHandle;

            _ = coreWindow.Dispatcher.RunIdleAsync((x) =>
            {
                MessageBox.Show("Hallo!");
            });

            MainForm = new();
            MainForm.Show();

            SetWndProc(coreWindow, WndProc);

            using (var g = Graphics.FromHwnd(hWnd))
            {
                g.Clear(Color.White);
            }


            SetWindowLongPtr(hWnd, -16, (IntPtr)0x95CF0000);

            if (SetParent(MainForm.Handle, hWnd) == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            //CoreApplication.RunWithActivationFactories(new Test());

            {
                var applicationView = ApplicationView.GetForCurrentView(); // ✔
                var visualizationSettings = PointerVisualizationSettings.GetForCurrentView(); // ✔
            }
            #endregion

            #region ApplicationFrame
            var frameManager = ApplicationFrameActivator.CreateApplicationFrameManager();
            var immersiveShell = ImmersiveShellActivator.CreateImmersiveShellServiceProvider();
            var uncloakService = immersiveShell.QueryService<IImmersiveApplicationManager>() as IUncloakWindowService;
            var frameService = immersiveShell.QueryService<IImmersiveApplicationManager>() as IApplicationFrameService;
            // ListAllFrames(frameManager);

            var frame = CreateNewFrame(frameManager);
            Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out IntPtr frameHwnd));

            // Marshal.ThrowExceptionForHR(frameService.GetFrame("Test", IntPtr.Zero, out var proxy));
            //Marshal.ThrowExceptionForHR(frameService.GetFrameByWindow((IntPtr)0x207E8, out var proxy));
            Marshal.ThrowExceptionForHR(uncloakService.UncloakWindow((IntPtr)0x207E8));

            #endregion

            Window.Current?.Activate();

            //DataTransferManager.GetForCurrentView().DataRequested += Program_DataRequested;
            //DataTransferManager.ShowShareUI();

            Application.Run(MainForm);
            // XamlHostApplication<App>.Run<WelcomePage>();
        }

        #region WndProc
        private const int GWLP_WNDPROC = -4;
        public delegate IntPtr WndProcDelegate(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

        public static IntPtr SetWndProc(CoreWindow coreWindow, WndProcDelegate newProc)
        {
            IntPtr hwnd = (coreWindow as object as ICoreWindowInterop).WindowHandle;
            IntPtr functionPointer = Marshal.GetFunctionPointerForDelegate(newProc);
            return (IntPtr)SetWindowLongPtr(hwnd, GWLP_WNDPROC, functionPointer);
        }

        [DllImport("user32.dll")]
        static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        static IntPtr WndProc(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProc(hwnd, message, wParam, lParam);
        }
        #endregion

        static IntPtr hwndNewFrame;
        private static IApplicationFrame CreateNewFrame(IApplicationFrameManager frameManager)
        {
            Marshal.ThrowExceptionForHR(frameManager.CreateFrame(out var frame));
            Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out hwndNewFrame));

            Marshal.ThrowExceptionForHR(frame.SetChromeOptions(97, 97));
            Marshal.ThrowExceptionForHR(frame.SetBackgroundColor(System.Drawing.Color.Blue.ToArgb()));
            // Marshal.ThrowExceptionForHR(frame.SetPresentedWindow(form.Handle));

            Marshal.ThrowExceptionForHR(frame.GetTitleBar(out var titleBar));
            Marshal.ThrowExceptionForHR(titleBar.SetWindowTitle($"LK Window - {DateTime.Now}"));
            return frame;
        }

        #region List Frames
        private static void ListAllFrames(IApplicationFrameManager frameManager)
        {
            #region Immersive Shell
            var serviceProvider = ImmersiveShellActivator.CreateImmersiveShellServiceProvider();

            IApplicationViewCollection viewCollection = serviceProvider.QueryService<IApplicationViewCollection>();
            IApplicationViewCollectionManagement viewCollectionManagement = (IApplicationViewCollectionManagement)viewCollection;
            #endregion

            Marshal.ThrowExceptionForHR(frameManager.GetFrameArray(out var frameArray));
            Marshal.ThrowExceptionForHR(frameArray.GetCount(out var count));
            bool alreadyGotVictim = false;
            for (uint i = 0; i < count; i++)
            {
                Guid iid = typeof(IApplicationFrame).GUID;
                Marshal.ThrowExceptionForHR(frameArray.GetAt(i, ref iid, out object frameUnk));
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
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        static extern int SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

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
