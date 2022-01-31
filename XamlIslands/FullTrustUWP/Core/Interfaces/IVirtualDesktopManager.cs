using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVirtualDesktopManager
	{
		[PreserveSig]
		int IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow, out bool result);

		[PreserveSig]
		int GetWindowDesktopId(IntPtr topLevelWindow, out Guid desktopId);

		[PreserveSig]
		int MoveWindowToDesktop(IntPtr topLevelWindow, ref Guid desktopId);
	}
}
