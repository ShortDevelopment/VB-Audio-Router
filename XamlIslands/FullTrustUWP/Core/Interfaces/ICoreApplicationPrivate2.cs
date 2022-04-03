using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("6090202d-2843-4ba5-9b0d-fc88eecd9ce5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface ICoreApplicationPrivate2
    {
        [PreserveSig]
        int InitializeForAttach();

        [PreserveSig]
        int WaitForActivate(out CoreWindow coreWindow);

        [PreserveSig]
        int CreateNonImmersiveView(out CoreApplicationView coreWindow);
    }

    //[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    //public interface IFrameworkApplicationPrivate2
    //{
    //    [PreserveSig]
    //    int StartOnCurrentThread(ApplicationInitializationCallback callback);
    //}
}
