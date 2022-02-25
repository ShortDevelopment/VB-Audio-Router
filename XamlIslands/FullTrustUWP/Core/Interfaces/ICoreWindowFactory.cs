using System;
using System.Runtime.InteropServices;
using Windows.UI.Core;

namespace FullTrustUWP.Core.Interfaces
{
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    [Guid("CD292360-2763-4085-8A9F-74B224A29175")] // CD292360_2763_4085_8A9F_74B224A29175
    public interface ICoreWindowFactory
    {
        void CreateCoreWindow([MarshalAs(UnmanagedType.HString)] string windowTitle, out CoreWindow window);
        bool WindowReuseAllowed { get; }
    }
}
