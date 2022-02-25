using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core
{
    public static class CapabilityChecker
    {
        // CapabilityCheck(0i64, L"shellExperienceComposer", &v58)
        public static bool HasCapability(string capability)
        {
            Marshal.ThrowExceptionForHR(CapabilityCheck(IntPtr.Zero, capability, out bool result));
            return result;
        }

        [DllImport("sechost.dll", SetLastError = true), PreserveSig]
        private static extern int CapabilityCheck(IntPtr ptr, string capability, out bool result);
    }
}
