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

    [Guid("d8c26227-b75e-4d8b-ac8c-c463a34ed11e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFrameFactory
    {
    }
}
