using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [ComImport]
	[Guid("372E1D3B-38D3-42E4-A15B-8AB2B178F513")]
	[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
	public interface IApplicationView
	{
		int SetFocus();

		int SwitchTo();

		int TryInvokeBack(IntPtr /* IAsyncCallback* */ callback);

		int GetThumbnailWindow(out IntPtr hwnd);

		int GetMonitor(out IntPtr /* IImmersiveMonitor */ immersiveMonitor);

		int GetVisibility(out int visibility);

		int SetCloak(ApplicationViewCloakType cloakType, bool removeFlag);

		int GetPosition(ref Guid guid /* GUID for IApplicationViewPosition */, out IntPtr /* IApplicationViewPosition** */ position);

		int SetPosition(ref IntPtr /* IApplicationViewPosition* */ position);

		int InsertAfterWindow(IntPtr hwnd);

		int GetExtendedFramePosition(out Rect rect);

		int GetAppUserModelId([MarshalAs(UnmanagedType.LPWStr)] out string id);

		int SetAppUserModelId(string id);

		int IsEqualByAppUserModelId(string id, out int result);

		int GetViewState(out uint state);

		int SetViewState(uint state);

		int GetNeediness(out int neediness);

		int GetLastActivationTimestamp(out ulong timestamp);

		int SetLastActivationTimestamp(ulong timestamp);

		int GetVirtualDesktopId(out Guid guid);

		int SetVirtualDesktopId(ref Guid guid);

		int GetShowInSwitchers(out int flag);

		int SetShowInSwitchers(int flag);

		int GetScaleFactor(out int factor);

		int CanReceiveInput(out bool canReceiveInput);

		int GetCompatibilityPolicyType(out ApplicationViewCompatibilityPolicy flags);

		int SetCompatibilityPolicyType(ApplicationViewCompatibilityPolicy flags);

		int GetPositionPriority(out IntPtr /* IShellPositionerPriority** */ priority);

		int SetPositionPriority(IntPtr /* IShellPositionerPriority* */ priority);

		int GetSizeConstraints(IntPtr /* IImmersiveMonitor* */ monitor, out Size size1, out Size size2);

		int GetSizeConstraintsForDpi(uint uint1, out Size size1, out Size size2);

		int SetSizeConstraintsForDpi(ref uint uint1, ref Size size1, ref Size size2);

		int QuerySizeConstraintsFromApp();

		int OnMinSizePreferencesUpdated(IntPtr hwnd);

		int ApplyOperation(IntPtr /* IApplicationViewOperation* */ operation);

		int IsTray(out bool isTray);

		int IsInHighZOrderBand(out bool isInHighZOrderBand);

		int IsSplashScreenPresented(out bool isSplashScreenPresented);

		int Flash();

		int GetRootSwitchableOwner(out IApplicationView rootSwitchableOwner);

		int EnumerateOwnershipTree(out IObjectArray ownershipTree);

		/*** (Windows 10 Build 10584 or later?) ***/

		int GetEnterpriseId([MarshalAs(UnmanagedType.LPWStr)] out string enterpriseId);

		int IsMirrored(out bool isMirrored);
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Rect
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Size
	{
		public int X;
		public int Y;
	}

	[Flags]
	public enum ApplicationViewCloakType
	{
		NONE = 0,
		DEFAULT = 1,
		VIRTUAL_DESKTOP = 2
	}

	public enum ApplicationViewCompatibilityPolicy
	{
		NONE = 0,
		SMALL_SCREEN = 1,
		TABLET_SMALL_SCREEN = 2,
		VERY_SMALL_SCREEN = 3,
		HIGH_SCALE_FACTOR = 4
	}
}
