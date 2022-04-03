using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Activation
{
    public static class ImmersiveShellActivator
    {
        public static Interfaces.IServiceProvider CreateImmersiveShellServiceProvider()
            => InteropHelper.ComCreateInstance<Interfaces.IServiceProvider>("c2f03a33-21f5-47fa-b4bb-156362a2f239")!;

        public static T QueryService<T>(this Interfaces.IServiceProvider serviceProvider)
        {
            Guid iid = typeof(T).GUID;
            Marshal.ThrowExceptionForHR(serviceProvider.QueryService(ref iid, ref iid, out object ptr));
            return (T)ptr;
        }
    }
}
