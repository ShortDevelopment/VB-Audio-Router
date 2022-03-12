using CoreWindowExample;
using FullTrustUWP.Core.Activation;
using Microsoft.Graphics.Canvas;
using System;
using System.Threading;
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
            Thread thread = new Thread(() =>
            {
                CoreWindow coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", (IntPtr)0, 30, 30, 1024, 768, 0);
                coreWindow.Activate();

                Renderer renderer = new Renderer(coreWindow);

                //var windowFactory1 = CoreWindowFactoryActivator.CreateInstance();
                //windowFactory1.CreateCoreWindow("Test2", out var coreWindow2);
                //coreWindow2.Activate();

                while (true)
                {
                    coreWindow.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
                    renderer.Render();
                }
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
