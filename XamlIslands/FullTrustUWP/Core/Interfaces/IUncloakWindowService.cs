using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("b706dded-208c-4795-b610-e7c002c31edc"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IUncloakWindowService
    {
        [PreserveSig]
        int UncloakWindow(IntPtr hWnd);
    }
}
