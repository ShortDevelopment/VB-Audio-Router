using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.Interfaces;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
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
            var coreApplicationPrivate = InteropHelper.RoGetActivationFactory<ICoreApplicationPrivate2>("Windows.ApplicationModel.Core.CoreApplication");
            Marshal.ThrowExceptionForHR(coreApplicationPrivate.CreateNonImmersiveView(out var coreView));

            // Mount Xaml rendering
            XamlFrameworkView view = new();
            view.Initialize(coreView);
            view.SetWindow(coreWindow);

            // Get xaml window & activate
            XamlWindow window = XamlWindow.Current;

            // Show win32 frame if requested
            if (config.HasWin32Frame)
                window.ShowWin32Frame();

            // Show window
            window.Activate();

            IWindowPrivate? windowPrivate = window as object as IWindowPrivate;
            Debug.Assert(windowPrivate != null, $"\"{nameof(windowPrivate)}\" is null");
            if (windowPrivate != null)
            {
                // A XamlWindow inside a Win32 process is transparent by default
                // (See Windows.UI.Xaml.dll!DirectUI::DXamlCore::ConfigureCoreWindow)
                // This is to provide a consistent behavior across platforms
                windowPrivate.TransparentBackground = config.TransparentBackground;
            }

            // Enable async / await
            SynchronizationContext.SetSynchronizationContext(new XamlSynchronizationContext(coreWindow));

            // SplashScreen
            Image splashScreenImage = new()
            {
                Source = config.SplashScreenImage
            };
            Frame frame = new();
            frame.Background = new SolidColorBrush(config.SplashScreenBackground);
            frame.Content = splashScreenImage;
            // window.Content = frame;

            new XamlWindowSubclass(window);

            return window;
        }
    }
}
