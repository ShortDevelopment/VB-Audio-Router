using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.Interfaces;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
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

        /// <summary>
        /// Creates a new <see cref="XamlWindow"/>, loads xaml from a <see cref="Stream"/> and sets it as <see cref="XamlWindow.Content"/>. <br/>
        /// The <see cref="Stream"/> will be disposed automatically!
        /// </summary>
        public static XamlWindow CreateNewFromXaml(XamlWindowConfig config, Stream xamlStream)
        {
            using (xamlStream)
            using (StreamReader reader = new(xamlStream))
                return CreateNewFromXaml(config, reader.ReadToEnd());
        }

        /// <summary>
        /// Creates a new <see cref="XamlWindow"/> and sets xaml as <see cref="XamlWindow.Content"/>. <br/>
        /// </summary>
        public static XamlWindow CreateNewFromXaml(XamlWindowConfig config, string xaml)
        {
            var window = CreateNewWindow(config);
            UIElement content = (UIElement)XamlReader.Load(xaml);
            window.Content = content;
            return window;
        }

        public static void CreateNewThread(Action callback)
        {
            Thread thread = new(() => callback());
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
