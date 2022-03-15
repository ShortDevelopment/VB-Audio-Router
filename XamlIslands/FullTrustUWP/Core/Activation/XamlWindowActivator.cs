using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using XamlApplication = Windows.UI.Xaml.Application;
using XamlWindow = Windows.UI.Xaml.Window;
using XamlFrameworkView = Windows.UI.Xaml.FrameworkView;

namespace FullTrustUWP.Core.Activation
{
    public sealed class XamlWindowActivator
    {
        public static bool IsAppInitialized { get; private set; } = false;

        public static void UseUwp()
            => IsAppInitialized = true;

        public static Task<XamlWindow> ActivateXamlWindowAsync(string title)
            => ActivateXamlWindowAsync(title, out _);

        public static Task<XamlWindow> ActivateXamlWindowAsync(string title, out Thread windowThread)
        {
            TaskCompletionSource<XamlWindow> taskCompletion = new();
            Thread thread = new Thread(() =>
            {
                try
                {
                    if (!IsAppInitialized)
                        new XamlApplicationImpl();
                    IsAppInitialized = true;

                    CoreWindow coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, title, IntPtr.Zero, 30, 30, 1024, 768, 0);
                    coreWindow.Activate();

                    CoreApplicationViewImpl coreView = new(coreWindow);

                    XamlFrameworkView view = new();
                    (view as object as IFrameworkView)!.Initialize(coreView);
                    view.SetWindow(coreWindow);

                    XamlWindow window = XamlWindow.Current;
                    window.Activate();
                    window.Content = new Windows.UI.Xaml.Controls.Button()
                    {
                        Content = new Windows.UI.Xaml.Controls.TextBlock()
                        {
                            Text = "Hallo!"
                        }
                    };
                    // taskCompletion.SetResult(window);

                    view.Run();
                }
                catch (Exception ex)
                {
                    taskCompletion.SetException(ex);
                }
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start();

            windowThread = thread;

            return taskCompletion.Task;
        }

        class XamlApplicationImpl : XamlApplication
        {

        }

        #region CoreApplicationView
        [ComVisible(true)]
        class CoreApplicationViewImpl : ICoreApplicationView
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
