using System;
using System.Runtime.InteropServices;
using VBAudioRouter.Communication;

namespace VBAudioRouter.Worker
{
    [ComVisible(true), Guid(Consts.DesktopBridge_CLSID)]
    [ComDefaultInterface(typeof(IDesktopBridge))]
    internal sealed class DesktopBridge : IDesktopBridge
    {
        public string Version { get => "1.0.0"; }
    }
}
