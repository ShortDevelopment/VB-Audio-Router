using FullTrustUWP.Core.Interfaces;
using System;

namespace FullTrustUWP.Core.Activation
{
    public static class VirtualDesktopManagerActivator
    {
        public static IVirtualDesktopManager CreateVirtualDesktopManager()
        {
            Type type = Type.GetTypeFromCLSID(new Guid("aa509086-5ca9-4c25-8f95-589d3c07b48a"));
            return (IVirtualDesktopManager)Activator.CreateInstance(type);
        }
    }
}
