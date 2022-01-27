using System;
using System.Runtime.InteropServices;
using static FullTrustUWP.Core.InteropHelper;

namespace FullTrustUWP.Core
{
    public static class CapabilityChecker
    {
        // CapabilityCheck(0i64, L"shellExperienceComposer", &v58)
        public static bool HasCapability(string capability)
        {
            CapabilityCheck(IntPtr.Zero, capability, out bool result);
            ThrowOnError();
            return result;
        }

        [DllImport("sechost.dll", SetLastError = true)]
        private static extern void CapabilityCheck(IntPtr ptr, string capability, out bool result);
    }
}
