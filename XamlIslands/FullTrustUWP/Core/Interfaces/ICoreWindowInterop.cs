using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [ComImport, Guid(Const.IID_ICoreWindowInterop)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICoreWindowInterop
    {
        IntPtr WindowHandle { get; }
        bool MessageHandled { get; }
    }
}
