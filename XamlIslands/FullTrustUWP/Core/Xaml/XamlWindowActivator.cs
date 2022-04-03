using FullTrustUWP.Core.Activation;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XamlFrameworkView = Windows.UI.Xaml.FrameworkView;
using XamlWindow = Windows.UI.Xaml.Window;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlWindowActivator
    {
        public static XamlWindow CreateNewWindow(XamlWindowConfig config)
        {
            if (XamlApplicationWrapper.Current == null)
                throw new InvalidOperationException($"No instance of \"{nameof(XamlApplicationWrapper)}\" was found!");

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
    }
}
