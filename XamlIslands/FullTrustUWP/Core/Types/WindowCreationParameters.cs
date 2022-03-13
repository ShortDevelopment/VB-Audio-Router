using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowCreationParameters
    {
        public uint Left;
        public uint Top;
        public uint Width;
        public uint Height;
        public bool TransparentBackground;
        public bool IsCoreNavigationClient;
    }
}
