using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.UI.Core;
using XamlFrameworkView = Windows.UI.Xaml.FrameworkView;
using XamlWindow = Windows.UI.Xaml.Window;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlWindowActivator
    {
        /// <summary>
        /// Creates new <see cref="XamlWindow"/> on current thread. <br />
        /// Only one window is allowed per thread! <br/>
        /// A <see cref="XamlWindowSubclass"/> will be attached automatically.
        /// </summary>
        public static XamlWindow CreateNewWindow(XamlWindowConfig config)
        {
            if (XamlApplicationWrapper.Current == null)
                throw new InvalidOperationException($"No instance of \"{nameof(XamlApplicationWrapper)}\" was found!");

            CoreWindow coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, config.Title, IntPtr.Zero, 30, 30, 1024, 768, 0);

            // Create CoreApplicationView
            var coreApplicationPrivate = InteropHelper.RoGetActivationFactory<ICoreApplicationPrivate2>("Windows.ApplicationModel.Core.CoreApplication");
            Marshal.ThrowExceptionForHR(coreApplicationPrivate.CreateNonImmersiveView(out var coreView));

            // Mount Xaml rendering
            XamlFrameworkView view = new();
            view.Initialize(coreView);
            view.SetWindow(coreWindow);

            // Get xaml window & activate
            XamlWindow window = XamlWindow.Current;

            // Enable async / await
            SynchronizationContext.SetSynchronizationContext(new XamlSynchronizationContext(coreWindow));

            XamlWindowSubclass subclass = XamlWindowSubclass.Attach(window);
            if (subclass.WindowPrivate != null)
            {
                // A XamlWindow inside a Win32 process is transparent by default
                // (See Windows.UI.Xaml.dll!DirectUI::DXamlCore::ConfigureCoreWindow)
                // This is to provide a consistent behavior across platforms
                subclass.WindowPrivate.TransparentBackground = config.TransparentBackground;
            }

            // Show win32 frame if requested
            if (config.HasWin32Frame)
                subclass.ShowWin32Frame();

            subclass.IsTopMost = config.TopMost;
            subclass.HasWin32TitleBar = config.HasWin32TitleBar;

            //coreWindow.Closed += (CoreWindow window, CoreWindowEventArgs args) =>
            //{
            //    subclass.Dispose();
            //};

            // Show window
            window.Activate();

            return window;
        }
    }
}
