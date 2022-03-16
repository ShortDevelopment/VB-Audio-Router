using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Markup;
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

        public static void RunOnNewThread(Action<XamlWindow> callback, out Thread windowThread)
        {
            //TaskCompletionSource<XamlWindow> taskCompletion = new();
            Thread thread = new Thread(() =>
            {
                RunOnCurrentThread(callback);
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start();

            windowThread = thread;

            //return taskCompletion.Task;
        }

        public static void RunOnCurrentThread(Action<XamlWindow> callback)
        {
            //if (!IsAppInitialized)
            //    new XamlApplicationImpl();
            //IsAppInitialized = true;

            //Windows.UI.Xaml.Hosting.WindowsXamlManager.InitializeForCurrentThread();

            CoreWindow coreWindow = CoreWindow.GetForCurrentThread();

            if (coreWindow == null)
            {
                coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "", IntPtr.Zero, 30, 30, 1024, 768, 0);
                //coreWindow.Activate();
            }

            CoreApplicationViewImpl coreView = new(coreWindow);

            XamlFrameworkView view = new();
            (view as object as IFrameworkView)!.Initialize(coreView);
            view.SetWindow(coreWindow);

            XamlWindow window = XamlWindow.Current;
            window.Activate();

            callback?.Invoke(window);

            view.Run();
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



        /// <summary>
        /// <see href="https://github.com/CommunityToolkit/Microsoft.Toolkit.Win32/blob/master/Microsoft.Toolkit.Win32.UI.XamlApplication/XamlApplication.cpp">XamlApplication.cpp</see>
        /// </summary>
        class XamlApplicationImpl : XamlApplication, IXamlMetadataProvider
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
    }
}
