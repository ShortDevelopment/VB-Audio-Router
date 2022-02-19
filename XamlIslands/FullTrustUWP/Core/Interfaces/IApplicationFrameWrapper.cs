using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("1404252c-1e59-462d-b5c1-9cbe30e85f19")] // ??
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IApplicationFrameWrapper
    {
        [PreserveSig]
        bool IsEqualByFrame(ref IApplicationFrame frame);

        [PreserveSig]
        int GetFrame(out IApplicationFrame frame);

        [PreserveSig]
        int SetShellCloak(bool cloaked);
    }
}
