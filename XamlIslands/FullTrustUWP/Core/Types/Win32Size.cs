using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Size
    {
        public int X;
        public int Y;
    }
}
