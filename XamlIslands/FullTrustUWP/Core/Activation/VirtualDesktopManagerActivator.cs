using FullTrustUWP.Core.Interfaces;

namespace FullTrustUWP.Core.Activation
{
    public static class VirtualDesktopManagerActivator
    {
        public static IVirtualDesktopManager CreateVirtualDesktopManager()
            => InteropHelper.ComCreateInstance<IVirtualDesktopManager>("aa509086-5ca9-4c25-8f95-589d3c07b48a")!;
    }
}
