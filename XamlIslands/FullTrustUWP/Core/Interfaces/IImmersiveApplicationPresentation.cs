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

    [Guid("d8c26227-b75e-4d8b-ac8c-c463a34ed11e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] // 02ee93d4-448e-469e-9799-0a8a1f70f171
    public interface IApplicationFrameFactory
    {
        [PreserveSig]
        int CreateFrameWithWrapper(out IApplicationFrame frame);

        [PreserveSig]
        int DestroyFrameWithWrapper(ref IApplicationFrame frame);

        [Obsolete]
        void RegisterForFrameEvents(ref object handler);

        [Obsolete]
        void UnregisterForFrameEvents();
    }
}
