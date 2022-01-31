using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("1841C6D7-4F9D-42C0-AF41-8747538F10E5")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IApplicationViewCollection
	{
		int GetViews(out IObjectArray array);

		int GetViewsByZOrder(out IObjectArray array);

		int GetViewsByAppUserModelId(string id, out IObjectArray array);

		int GetViewForHwnd(IntPtr hwnd, out IApplicationView view);

		int GetViewForApplication(object application, out IApplicationView view);

		int GetViewForAppUserModelId(string id, out IApplicationView view);

		int GetViewInFocus(out IntPtr view);

		void RefreshCollection();

		int RegisterForApplicationViewChanges(object listener, out int cookie);

		int RegisterForApplicationViewPositionChanges(object listener, out int cookie);

		int UnregisterForApplicationViewChanges(int cookie);
	}
}
