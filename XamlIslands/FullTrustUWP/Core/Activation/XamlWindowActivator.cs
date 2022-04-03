using FullTrustUWP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using XamlApplication = Windows.UI.Xaml.Application;
using XamlFrameworkView = Windows.UI.Xaml.FrameworkView;
using XamlWindow = Windows.UI.Xaml.Window;

namespace FullTrustUWP.Core.Activation
{
    public sealed class XamlWindowActivator
    {
        public static bool IsAppInitialized { get; private set; } = false;

        public static void UseUwp()
            => IsAppInitialized = true;

        public static XamlWindow CreateNewWindow(XamlWindowConfig config)
        {
            //Windows.UI.Xaml.Hosting.WindowsXamlManager.InitializeForCurrentThread();

            CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
            if (coreWindow == null)
                coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, config.Title, IntPtr.Zero, 30, 30, 1024, 768, 0);

            // Create CoreApplicationView
            var coreApplicationPrivate = InteropHelper.RoGetActivationFactory<Interfaces.ICoreApplicationPrivate2>("Windows.ApplicationModel.Core.CoreApplication");
            Marshal.ThrowExceptionForHR(coreApplicationPrivate.CreateNonImmersiveView(out var coreView));

            // Mount Xaml rendering
            XamlFrameworkView view = new();
            view.Initialize(coreView);
            view.SetWindow(coreWindow);

            // Get xaml window & activate
            XamlWindow window = XamlWindow.Current;
            window.Activate();

            SynchronizationContext.SetSynchronizationContext(new XamlSynchronizationContext(coreWindow));

            // SplashScreen
            Image splashScreenImage = new()
            {
                Source = config.SplashScreenImage
            };
            Frame frame = new();
            frame.Background = new SolidColorBrush(config.SplashScreenBackground);
            frame.Content = splashScreenImage;
            window.Content = frame;

            return window;
        }

        public sealed class XamlWindowConfig
        {
            public XamlWindowConfig(string title)
                => this.Title = title;

            public string Title { get; private set; } = "";

            public int SplashScreenTime { get; set; } = 1_000;
            public Color SplashScreenBackground { get; set; } = Color.FromArgb(255, 58, 57, 55);
            public ImageSource? SplashScreenImage { get; set; }
        }

        #region CoreApplicationView
        [ComVisible(true)]
        sealed class CoreApplicationViewImpl : ICoreApplicationView
        {
            CoreWindow _coreWindow;
            public CoreApplicationViewImpl(CoreWindow coreWindow)
            {
                this._coreWindow = coreWindow;
            }

            public CoreWindow CoreWindow => _coreWindow;

            public bool IsHosted => false;

            public bool IsMain => true;

            public event TypedEventHandler<CoreApplicationView, IActivatedEventArgs> Activated;
        }

        [ComVisible(true)]
        [Guid("638bb2db-451d-4661-b099-414f34ffb9f1"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        interface ICoreApplicationView
        {
            CoreWindow CoreWindow { get; }

            event TypedEventHandler<CoreApplicationView, IActivatedEventArgs> Activated;

            bool IsMain { get; }

            bool IsHosted { get; }
        }
        #endregion

        [Guid("faab5cd0-8924-45ac-ad0f-a08fae5d0324"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        interface IFrameworkView
        {
            [PreserveSig]
            int Initialize(ICoreApplicationView applicationView);
        }

        /// <summary>
        /// <see href="https://github.com/CommunityToolkit/Microsoft.Toolkit.Win32/blob/master/Microsoft.Toolkit.Win32.UI.XamlApplication/XamlApplication.cpp">XamlApplication.cpp</see>
        /// </summary>
        sealed class XamlApplicationImpl : XamlApplication, IXamlMetadataProvider
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, int dwFlags);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool FreeLibrary(IntPtr hModule);

            List<IntPtr> _loadedModules = new List<IntPtr>();
            public XamlApplicationImpl()
            {
                // Workaround a bug where twinapi.appcore.dll and threadpoolwinrt.dll gets loaded after it has been unloaded
                // because of a call to GetActivationFactory
                foreach (var lib in new[] { "twinapi.appcore.dll", "threadpoolwinrt.dll" })
                {
                    IntPtr hModule = LoadLibraryEx(lib, IntPtr.Zero, 0);
                    if (hModule == IntPtr.Zero)
                        throw new Win32Exception();
                    _loadedModules.Add(hModule);
                }

                // Provider = new ReflectionXamlMetadataProvider();
            }

            protected override void OnLaunched(LaunchActivatedEventArgs args)
            {
                // base.OnLaunched(args);
            }

            ~XamlApplicationImpl()
            {
                foreach (IntPtr hModule in _loadedModules)
                    FreeLibrary(hModule);
            }

            #region IXamlMetadataProvider
            IXamlMetadataProvider Provider;
            public IXamlType GetXamlType(Type type)
                => Provider.GetXamlType(type);

            public IXamlType GetXamlType(string fullName)
                => Provider.GetXamlType(fullName);

            public XmlnsDefinition[] GetXmlnsDefinitions()
                => Provider.GetXmlnsDefinitions();
            #endregion
        }

        public sealed class XamlSynchronizationContext : SynchronizationContext
        {
            public CoreWindow CoreWindow { get; }
            public XamlSynchronizationContext(CoreWindow coreWindow)
                => this.CoreWindow = coreWindow;

            public override void Post(SendOrPostCallback d, object? state)
                => _ = CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => d?.Invoke(state));

            public override void Send(SendOrPostCallback d, object? state)
                => _ = CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => d?.Invoke(state));
        }

        public sealed class XamlApplicationWrapper : IDisposable
        {
            public static XamlApplicationWrapper? Current { get; private set; }
            public XamlApplication Application { get; private set; }

            public XamlApplicationWrapper(Func<XamlApplication> callback)
            {
                if (Current != null)
                    throw new InvalidOperationException("Only one App is allowed!");

                Application = callback();
                Current = this;
            }

            public void Dispose() { }
        }
    }

    public static class XamlWindowAnimationExtensions
    {
        private const int animationDurationMs = 1000;

        public static void ShowAsFlyout(this XamlWindow window)
        {
            IntPtr hwnd = (window.CoreWindow as object as ICoreWindowInterop)!.WindowHandle;
            if (AnimateWindow(hwnd, animationDurationMs, AnimateWindowFlags.ACTIVATE | AnimateWindowFlags.SLIDE | AnimateWindowFlags.HOR_POSITIVE) != 0)
                throw new Win32Exception();
        }

        public static void HideAsFlyout(this XamlWindow window)
        {
            IntPtr hwnd = (window.CoreWindow as object as ICoreWindowInterop)!.WindowHandle;
            if (AnimateWindow(hwnd, animationDurationMs, AnimateWindowFlags.HIDE | AnimateWindowFlags.SLIDE | AnimateWindowFlags.HOR_POSITIVE) != 0)
                throw new Win32Exception();
        }

        [DllImport("user32", SetLastError = true), PreserveSig]
        static extern int AnimateWindow(IntPtr hwnd, int time, AnimateWindowFlags flags);

        [Flags]
        enum AnimateWindowFlags
        {
            HOR_POSITIVE = 0x00000001,
            HOR_NEGATIVE = 0x00000002,
            VER_POSITIVE = 0x00000004,
            VER_NEGATIVE = 0x00000008,
            CENTER = 0x00000010,
            HIDE = 0x00010000,
            ACTIVATE = 0x00020000,
            SLIDE = 0x00040000,
            BLEND = 0x00080000
        }
    }
}
