using System;
using System.Runtime.InteropServices;
using static FullTrustUWP.Core.InteropHelper;

namespace FullTrustUWP.Core.ApplicationFrame
{
    public static class CloakingHelper
    {
        private delegate void EnableIAMAccess_Sig(
            [In, Out] IntPtr srwLock,
            UInt64 enabled
        );
        private static EnableIAMAccess_Sig EnableIAMAccess_Ref;

        public static void EnableIAMAccess(IntPtr SRWLock, bool enabled)
        {
            if (EnableIAMAccess_Ref == null)
                EnableIAMAccess_Ref = DynamicLoad<EnableIAMAccess_Sig>("user32.dll", 2501);

            EnableIAMAccess_Ref(SRWLock, (UInt64)1);
        }

        private delegate int AcquireIAMKey_Sig();
        private static AcquireIAMKey_Sig AcquireIAMKey_Ref;

        public static void AcquireIAMKey()
        {
            if (AcquireIAMKey_Ref == null)
                AcquireIAMKey_Ref = DynamicLoad<AcquireIAMKey_Sig>("user32.dll", 2509);

            Marshal.ThrowExceptionForHR(AcquireIAMKey_Ref());
        }

        [DllImport("kernel32")]
        public static extern void InitializeSRWLock([Out] IntPtr SRWLock);

        [DllImport("kernel32")]
        public static extern void AcquireSRWLockExclusive([In] IntPtr SRWLock);

        [DllImport("kernel32")]
        public static extern void ReleaseSRWLockExclusive([In] IntPtr SRWLock);

        [StructLayout(LayoutKind.Sequential)]
        public struct RTL_SRWLOCK
        {
            public IntPtr Ptr;
        }

        static RTL_SRWLOCK srwLock;
        public static unsafe void EnableIAMAccess()
        {
            fixed (RTL_SRWLOCK* ptr = &srwLock)
            {
                InitializeSRWLock((IntPtr)ptr);
                AcquireSRWLockExclusive((IntPtr)ptr);
                EnableIAMAccess((IntPtr)ptr, true);
            }
        }
    }
}
