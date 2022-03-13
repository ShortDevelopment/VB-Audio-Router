using CoreWindowExample;
using FullTrustUWP.Core;
using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.Interfaces;
using FullTrustUWP.Core.Types;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Application = System.Windows.Forms.Application;

namespace VBAudioRouter.Host
{
    static class Program
    {
        

        static Form MainForm;
        [MTAThread]
        static void Main()
        {
            // https://raw.githubusercontent.com/fboldewin/COM-Code-Helper/master/code/interfaces.txt
            // GOOGLE: "IApplicationViewCollection" site:lise.pnfsoftware.com

            //var windowFactory1 = CoreWindowFactoryActivator.CreateInstance();
            //windowFactory1.CreateCoreWindow("Test2", out var coreWindow2);
            //coreWindow2.Activate();

            var applicationFactory = InteropHelper.GetActivationFactory<IFrameworkApplicationStaticsPrivate>("Windows.UI.Xaml.Application");
            Marshal.ThrowExceptionForHR(applicationFactory.StartInCoreWindowHostingMode(new() {
                Left = 10,
                Top = 10,
                Width = 100,
                Height = 100,
                TransparentBackground = true,
                IsCoreNavigationClient = true
            }, (x) =>
            {
                Debugger.Break();
            }));

            #region CoreWindow
            CoreWindow coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", (IntPtr)0, 30, 30, 1024, 768, 0);
            coreWindow.Activate();

            {
                var applicationView = ApplicationView.GetForCurrentView(); // ✔                
                var visualizationSettings = PointerVisualizationSettings.GetForCurrentView(); // ✔
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += Program_CloseRequested;
            }

            var hWnd = (coreWindow as object as ICoreWindowInterop).WindowHandle;

            Window.Current?.Activate();

            MainForm = new();
            MainForm.Show();

            #endregion

            #region ApplicationFrame
            var frameManager = ApplicationFrameActivator.CreateApplicationFrameManager();
            var immersiveShell = ImmersiveShellActivator.CreateImmersiveShellServiceProvider();

            var uncloakService = immersiveShell.QueryService<IImmersiveApplicationManager>() as IUncloakWindowService;
            var frameService = immersiveShell.QueryService<IImmersiveApplicationManager>() as IApplicationFrameService;
            var applicationPresentation = immersiveShell.QueryService<IImmersiveApplicationManager>() as IImmersiveApplicationPresentation;
            // ListAllFrames(frameManager);

            //var frameFactory = immersiveShell.QueryService<IApplicationFrameFactory>();
            //Marshal.ThrowExceptionForHR(frameFactory.CreateFrameWithWrapper(out var frameWrapper));

            var frame = CreateNewFrame(frameManager);

            Win32Size minSize = new()
            {
                X = 100,
                Y = 50
            };
            Marshal.ThrowExceptionForHR(frame.SetMinimumSize(ref minSize));

            { // Show frame
                Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out IntPtr frameHwnd));
                RemoteThread.UnCloakWindowShell(frameHwnd);
            }

            Marshal.ThrowExceptionForHR(frame.SetPresentedWindow((IntPtr)0x1E0838));

            //var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            //titleBar.BackgroundColor = Windows.UI.Colors.Red
            // var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            // IApplicationFrameTitleBarPersistenceInternal GUID_1f4df06b_6e3b_46ab_9365_55568e176b53
            #endregion

            Application.Run(MainForm);

            // Marshal.ThrowExceptionForHR(frame.Destroy());

            // XamlHostApplication<App>.Run<WelcomePage>();
        }

        private static void Program_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static IApplicationFrame CreateNewFrame(IApplicationFrameManager frameManager)
        {
            Marshal.ThrowExceptionForHR(frameManager.CreateFrame(out var frame));

            Marshal.ThrowExceptionForHR(frame.SetChromeOptions(97, 97));
            Marshal.ThrowExceptionForHR(frame.SetOperatingMode(1, 1));
            Marshal.ThrowExceptionForHR(frame.SetBackgroundColor(65535));

            Marshal.ThrowExceptionForHR(frame.GetTitleBar(out var titleBar));
            Marshal.ThrowExceptionForHR(titleBar.SetWindowTitle($"LK Window - {DateTime.Now}"));
            Marshal.ThrowExceptionForHR(titleBar.SetVisibleButtons(2, 2));
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
                IApplicationFrame frame = frameArray.GetAt<IApplicationFrame>(i);
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
