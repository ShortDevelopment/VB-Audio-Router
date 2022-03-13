using FullTrustUWP.Core.Types;
using System;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("c45f3f8c-61e6-4f9a-be88-fe4fe6e64f5f"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IFrameworkApplicationStaticsPrivate
    {
        [PreserveSig]
        int StartInCoreWindowHostingMode(WindowCreationParameters @params, ApplicationInitializationCallback callback);
    }
}
