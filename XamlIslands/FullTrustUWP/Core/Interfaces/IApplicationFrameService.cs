using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("88b25c81-171b-48b0-91d6-c75846bcf035")] // 94ea2b94-e9cc-49e0-c0ff-ee64ca8f5b90 : 88b25c81-171b-48b0-91d6-c75846bcf035
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
        int GetFrameByWindow(IntPtr hWnd, out IApplicationFrameProxy frameProxy);

        [Obsolete("Not implemented")]
        void SynchronizeFrameInformation();

        [PreserveSig]
        int DestroyFrame(ref IApplicationFrameProxy frameProxy);

        [PreserveSig]
        int SimulateFrameManagerCrash();
    }
}
