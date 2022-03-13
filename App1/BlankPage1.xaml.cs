using CoreWindowExample;
using FullTrustUWP.Core.Activation;
using FullTrustUWP.Core.Interfaces;
using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var coreView = App.CoreApplicationView;
            Thread thread = new Thread(() =>
            {
                CoreWindow coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", (IntPtr)0, 30, 30, 1024, 768, 0);
                coreWindow.Activate();

                // coreView = new CustomCoreApplicationView(coreWindow) as ICoreApplicationView as object as CoreApplicationView;
                
                var view = new FrameworkView();
                view.Initialize(coreView);
                view.SetWindow(coreWindow);

                Window window = Window.Current;

                window.Activate();
                window.Content = new Button()
                {
                    Content = new TextBlock()
                    {
                        Text = "Hallo!"
                    }
                };
                window.Content = new BlankPage1();
                view.Run();

                //Renderer renderer = new Renderer(coreWindow);

                ////var windowFactory1 = CoreWindowFactoryActivator.CreateInstance();
                ////windowFactory1.CreateCoreWindow("Test2", out var coreWindow2);
                ////coreWindow2.Activate();

                //while (true)
                //{
                //    coreWindow.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
                //    renderer.Render();
                //}
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start();
        }

        [Guid("638bb2db-451d-4661-b099-414f34ffb9f1"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        internal interface ICoreApplicationView
        {
            CoreWindow CoreWindow { get; }
            bool IsHosted { get; }
            bool IsMain { get; }

            event TypedEventHandler<CoreApplicationView, IActivatedEventArgs> Activated;
        }

        [Windows.Foundation.Metadata.Guid(1760262879, 37247, 18667, 154, 235, 125, 229, 62, 8, 106, 177)]
        internal interface ICoreApplicationView2
        {
            CoreDispatcher Dispatcher { get; }
        }

        [Windows.Foundation.Metadata.Guid(3239695514, 1657, 18874, 128, 63, 183, 156, 92, 243, 76, 202)]
        internal interface ICoreApplicationView6
        {
            DispatcherQueue DispatcherQueue { get; }
        }

        [ComVisible(true)]
        class CustomCoreApplicationView : ICoreApplicationView
        {
            public CustomCoreApplicationView(CoreWindow coreWindow)
            {
                this._coreWindow = coreWindow;
            }

            CoreWindow _coreWindow;

            public CoreWindow CoreWindow => _coreWindow;

            public bool IsHosted => false;

            public bool IsMain => true;

            public CoreDispatcher Dispatcher => _coreWindow.Dispatcher;

            public DispatcherQueue DispatcherQueue => _coreWindow.DispatcherQueue;

            public event TypedEventHandler<CoreApplicationView, IActivatedEventArgs> Activated;
        }
    }
}
