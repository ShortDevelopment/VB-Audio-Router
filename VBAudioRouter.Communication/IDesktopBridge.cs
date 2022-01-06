using System;
using System.Runtime.InteropServices;

namespace VBAudioRouter.Communication
{
    [ComVisible(true), Guid(Consts.IDesktopBridge_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDesktopBridge
    {
        string Version { get; }
    }
}
