using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using XamlApplication = Windows.UI.Xaml.Application;
using XamlElement = Windows.UI.Xaml.UIElement;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlApplicationWrapper : IDisposable
    {
        public XamlApplication Application { get; private set; }

        public XamlApplicationWrapper(Func<XamlApplication> callback)
        {
            if (Current != null)
                throw new InvalidOperationException($"Only one instance of \"{nameof(XamlApplicationWrapper)}\" is allowed!");

            Application = callback();
            Current = this;
        }

        public void Dispose()
        {
            Application = null;
        }


        public static XamlApplicationWrapper? Current { get; private set; }

        public static void Run<TApp, TContent>() where TApp : XamlApplication, new() where TContent : XamlElement, new()
        {
            using (XamlApplicationWrapper appWrapper = new(() => new TApp()))
            {
                string windowTitle = Process.GetCurrentProcess().ProcessName;
                try
                {
                    windowTitle = Package.Current?.DisplayName ?? windowTitle;
                }
                catch { }
                var window = XamlWindowActivator.CreateNewWindow(new(windowTitle));
                window.Content = new TContent();

                // Run
                window.Dispatcher.ProcessEvents(Windows.UI.Core.CoreProcessEventsOption.ProcessUntilQuit);
            }
        }
    }
}
