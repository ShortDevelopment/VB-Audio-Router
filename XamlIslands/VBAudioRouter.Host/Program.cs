using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.ApplicationFrame;
using FullTrustUWP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Application = System.Windows.Forms.Application;
using IServiceProvider = FullTrustUWP.Core.Interfaces.IServiceProvider;

namespace VBAudioRouter.Host
{
    static class Program
    {
        class App : Windows.UI.Xaml.Application
        {
            protected override void OnWindowCreated(WindowCreatedEventArgs args)
            {

            }

            protected override void OnLaunched(LaunchActivatedEventArgs args)
            {
                base.OnLaunched(args);
            }
        }

        class Test : IGetActivationFactory
        {
            public object GetActivationFactory(string activatableClassId)
            {
                throw new NotImplementedException();
            }
        }

        [DllImport("combase.dll"), PreserveSig]
        static extern int RoGetServerActivatableClasses(
            [MarshalAs(UnmanagedType.HString)] string serverName,
            out IntPtr activatableClassIds,
            out uint count
        );

        [DllImport("combase.dll"), PreserveSig]
        static extern IntPtr WindowsGetStringRawBuffer(
            IntPtr hstring,
            out uint count
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int RoGetServerActivatableClassesDelegate(
            [MarshalAs(UnmanagedType.HString)] string serverName,
            IntPtr activatableClassIds,
            out uint count
        );

        static unsafe int RoGetServerActivatableClassesImpl(
            string serverName,
            IntPtr activatableClassIds,
            out uint count
        )
        {
            List<string> classIds = new List<string>();
            Marshal.ThrowExceptionForHR(RoGetServerActivatableClasses(serverName, out IntPtr test, out count));
            IntPtr* hstringPtr = (IntPtr*)test;
            for (int i = 0; i < count; i++)
            {
                string classId = Marshal.PtrToStringUni(WindowsGetStringRawBuffer(hstringPtr[i], out _));
                classIds.Add(classId);
            }
            return 0;
        }

        static Form MainForm;
        [MTAThread]
        static void Main()
        {
            // https://raw.githubusercontent.com/fboldewin/COM-Code-Helper/master/code/interfaces.txt
            // GOOGLE: "IApplicationViewCollection" site:lise.pnfsoftware.com

            new App();

            IntPtr classId = IntPtr.Zero;
            // Marshal.ThrowExceptionForHR(RoGetServerActivatableClasses("App.AppXfda2ecmmv7vpeq0mkwrj7mkgkg8efffm.mca", classId, out var count));

            var hook = EasyHook.LocalHook.Create(
                EasyHook.LocalHook.GetProcAddress("combase.dll", "RoGetServerActivatableClasses"),
                new RoGetServerActivatableClassesDelegate(RoGetServerActivatableClassesImpl),
                null);
            hook.ThreadACL.SetInclusiveACL(new int[] { 0 });

            //var hook2 = EasyHook.LocalHook.Create(
            //    EasyHook.LocalHook.GetProcAddress("Kernel32.dll", "GetCommandLineW"),
            //    new RoGetServerActivatableClassesDelegate(RoGetServerActivatableClassesImpl),
            //    null);

            CoreApplication.RunWithActivationFactories(new Test());

            //var windowFactory1 = Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("B243A9FD-C57A-4D3E-A7CF-21CAED64CB5A"))) as ICoreWindowFactory;
            //windowFactory1.CreateCoreWindow("Test2", out var coreWindow2);
            //coreWindow2.Activate();

            #region CoreWindow
            CoreWindow coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", IntPtr.Zero, 30, 30, 1024, 768, 0);
            coreWindow.Activate();

            _ = coreWindow.Dispatcher.RunIdleAsync((x) =>
            {
                MessageBox.Show("Hallo!");
            });

            MainForm = new();
            MainForm.Show();

            var hWnd = (coreWindow as object as ICoreWindowInterop).WindowHandle;

            if (SetParent(MainForm.Handle, hWnd) == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            SetWindowLongPtr(hWnd, -16, (IntPtr)0x95CF0000);

            {
                var applicationView = ApplicationView.GetForCurrentView(); // ✔
                var visualizationSettings = PointerVisualizationSettings.GetForCurrentView(); // ✔
            }
            #endregion

            //FolderPicker picker = new();
            //picker.FileTypeFilter.Add("*");
            //_ = picker.PickSingleFolderAsync();

            #region ApplicationFrame
            var frameManager = ApplicationFrameActivator.CreateApplicationFrameManager();
            // ListAllFrames(frameManager);

            // var frame = CreateNewFrame(frameManager);
            //Marshal.ThrowExceptionForHR(frame.SetPresentedWindow(hWnd));

            #endregion

            Window.Current?.Activate();

            DataTransferManager.GetForCurrentView().DataRequested += Program_DataRequested;
            DataTransferManager.ShowShareUI();

            Application.Run(MainForm);
            // XamlHostApplication<App>.Run<WelcomePage>();
        }

        private static void Program_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            throw new NotImplementedException();
        }

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
            IApplicationViewCollectionManagement viewCollectionManagement = (IApplicationViewCollectionManagement)ptr;
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
