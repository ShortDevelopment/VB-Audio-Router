using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("06636c29-5a17-458d-8ea2-2422d997a922"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IWindowPrivate
    {
        bool TransparentBackground { get; set; }

        [PreserveSig]
        int Show();

        [PreserveSig]
        int Hide();

        [PreserveSig]
        int MoveWindow(int x, int h, int width, int height);
    }
}
