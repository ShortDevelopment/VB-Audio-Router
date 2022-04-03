using System.Threading;
using Windows.UI.Core;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlSynchronizationContext : SynchronizationContext
    {
        public CoreWindow CoreWindow { get; }
        public XamlSynchronizationContext(CoreWindow coreWindow)
            => this.CoreWindow = coreWindow;

        public override void Post(SendOrPostCallback d, object? state)
            => _ = CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => d?.Invoke(state));

        public override void Send(SendOrPostCallback d, object? state)
            => _ = CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => d?.Invoke(state));
    }
}
