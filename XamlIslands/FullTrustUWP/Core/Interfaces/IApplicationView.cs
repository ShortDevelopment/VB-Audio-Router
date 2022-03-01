using FullTrustUWP.Core.Types;
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

		int GetExtendedFramePosition(out Win32Rect rect);

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

		int GetShowInSwitchers(out bool flag);

		int SetShowInSwitchers(bool flag);

		int GetScaleFactor(out int factor);

		int CanReceiveInput(out bool canReceiveInput);

		int GetCompatibilityPolicyType(out ApplicationViewCompatibilityPolicy flags);

		int SetCompatibilityPolicyType(ApplicationViewCompatibilityPolicy flags);

		int GetPositionPriority(out IntPtr /* IShellPositionerPriority** */ priority);

		int SetPositionPriority(IntPtr /* IShellPositionerPriority* */ priority);

		int GetSizeConstraints(IntPtr /* IImmersiveMonitor* */ monitor, out Win32Size size1, out Win32Size size2);

		int GetSizeConstraintsForDpi(uint uint1, out Win32Size size1, out Win32Size size2);

		int SetSizeConstraintsForDpi(ref uint uint1, ref Win32Size size1, ref Win32Size size2);

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
}
