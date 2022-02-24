using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("94ea2b94-e9cc-49e0-c0ff-ee64ca8f5b90")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IApplicationFrameService
    {
        [Obsolete("Not implemented")]
        void CompleteInitialization();

        [PreserveSig]
        int Uninitialize();

        [PreserveSig]
        int EnsureFramePool();

        [PreserveSig]
        int UnensureFramePool();

        [PreserveSig]
        int BeginFrameRecovery();

        [PreserveSig]
        int EndFrameRecovery();

        [PreserveSig]
        int GetFrame([MarshalAs(UnmanagedType.LPWStr)] string a, IntPtr glomId, out IApplicationFrameProxy frameProxy);

        [PreserveSig]
        int GetFrameByWindow(IntPtr hWnd, out IApplicationFrame frameProxy);
    }
}
