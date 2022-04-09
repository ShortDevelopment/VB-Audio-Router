using FullTrustUWP.Core.Interfaces;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlRuntimeSettings
    {
        public static IXamlRuntimeStatics RuntimeSettings
            => InteropHelper.RoGetActivationFactory<IXamlRuntimeStatics>("Windows.UI.Xaml.Hosting.XamlRuntime");
    }
}
