using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("c8e34820-d46a-41bc-8c5c-5bc9fdee243d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IApplicationFrameTitleBar
    {
        [PreserveSig]
        int SetWindowTitle([MarshalAs(UnmanagedType.LPWStr)] string title);

        [PreserveSig]
        int SetIsVisible(bool visible);

        [PreserveSig]
        int GetIsVisible(out bool visible);
    }
}
