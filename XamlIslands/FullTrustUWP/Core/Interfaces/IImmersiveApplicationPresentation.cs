using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("b706dded-208c-4795-b610-e7c002c31edc"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImmersiveApplicationPresentation
    {
        [PreserveSig]
        int SetCloak(IntPtr hWnd, bool cloak);
    }

    [Guid("d8c26227-b75e-4d8b-ac8c-c463a34ed11e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] // 94ea2b94-e9cc-49e0-c0ff-ee64ca8f5b90 : 02ee93d4-448e-469e-9799-0a8a1f70f171
    public interface IApplicationFrameFactory
    {
        [PreserveSig]
        int CreateFrameWithWrapper(out IApplicationFrameWrapper frame);

        [PreserveSig]
        int DestroyFrameWithWrapper(ref IApplicationFrameWrapper frame);

        [PreserveSig]
        int RegisterForFrameEvents(ref object handler, out int cookie);

        [Obsolete]
        void UnregisterForFrameEvents();
    }
}
