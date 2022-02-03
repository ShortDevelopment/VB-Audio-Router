using System;
using System.Runtime.InteropServices;
using static FullTrustUWP.Core.InteropHelper;

namespace FullTrustUWP.Core.ApplicationFrame
{
    public static class CloakingHelper
    {
        private delegate int EnableIAMAccess_Sig(
            IntPtr srwLock,
            bool enabled
        );
        private static EnableIAMAccess_Sig EnableIAMAccess_Ref;

        public static void EnableIAMAccess(bool enabled)
        {
            if (EnableIAMAccess_Ref == null)
                EnableIAMAccess_Ref = DynamicLoad<EnableIAMAccess_Sig>("user32.dll", 2510);

            Marshal.ThrowExceptionForHR(EnableIAMAccess_Ref(IntPtr.Zero, enabled));
        }

        private delegate int AcquireIAMKey_Sig();
        private static AcquireIAMKey_Sig AcquireIAMKey_Ref;

        public static void AcquireIAMKey()
        {
            if (AcquireIAMKey_Ref == null)
                AcquireIAMKey_Ref = DynamicLoad<AcquireIAMKey_Sig>("user32.dll", 2509);

            Marshal.ThrowExceptionForHR(AcquireIAMKey_Ref());
        }
    }
}
