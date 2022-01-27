using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace FullTrustUWP.Core.Activation
{
    public static class CoreApplicationActivator
    {
        public static ICoreApplication ActivateCoreApplication()
        {
            Guid IID_ICoreApplication = new Guid("0aacf7a4-5e1d-49df-8034-fb6a68bc5ed1");
            InteropHelper.GetActivationFactory("Windows.ApplicationModel.Core.CoreApplication", ref IID_ICoreApplication, out var factory);
            return factory as ICoreApplication;
        }

        public interface ICoreApplication
        {
            CoreApplicationView GetCurrentView();
            void Run(IFrameworkViewSource viewSource);
            void RunWithActivationFactories(IGetActivationFactory activationFactoryCallback);

            string Id { get; }
            IPropertySet Properties { get; }

            event EventHandler<object> Resuming;
            event EventHandler<SuspendingEventArgs> Suspending;
        }
    }
}
