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

    [Guid("02ee93d4-448e-469e-9799-0a8a1f70f171"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] // 
    public interface IFrameFactory
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
