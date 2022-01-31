using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("d6defab3-dbb9-4413-8af9-554586fdff94")] // d6defab3_dbb9_4413_8af9_554586fdff94
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IApplicationFrameManager
    {
        [PreserveSig]
        int CreateFrame(out IApplicationFrame frame);

        [PreserveSig]
        int DestroyFrame(ref IApplicationFrame frame);

        [PreserveSig]
        int GetFrameArray(out IObjectArray array);

        [Obsolete("Wrong signature")]
        void RegisterForFrameEvents();
        [Obsolete("Wrong signature")]
        void UnregisterForFrameEvents();
        [Obsolete("Wrong signature")]
        void EnableLayoutFrames();
    }
}
