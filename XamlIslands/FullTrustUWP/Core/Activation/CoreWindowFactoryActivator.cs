using System;
using System.Runtime.InteropServices;
using IServiceProvider = FullTrustUWP.Core.Interfaces.IServiceProvider;

namespace FullTrustUWP.Core.Activation
{
    public static class CoreWindowFactoryActivator
    {
        public static ICoreWindowFactory CreateInstance()
        {
            const string CLSID_CoreUICoreWindowFactoryProxy = "B243A9FD-C57A-4D3E-A7CF-21CAED64CB5A";
            Guid clsid = new(CLSID_CoreUICoreWindowFactoryProxy);
            Guid iid = new("00000000-0000-0000-C000-000000000046");
            Marshal.ThrowExceptionForHR(InteropHelper.CoCreateInstance(ref clsid, null, 1026, ref iid, out object factoryPtr));
            return factoryPtr as ICoreWindowFactory;
        }
    }
}
