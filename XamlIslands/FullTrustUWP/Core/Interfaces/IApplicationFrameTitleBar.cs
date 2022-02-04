using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{
    [Guid("c8e34820-d46a-41bc-8c5c-5bc9fdee243d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IApplicationFrameTitleBar
    {
        [PreserveSig]
        int SetWindowTitle([MarshalAs(UnmanagedType.LPWStr)] string title);

        [PreserveSig]
        int SetIsVisible(bool visible);

        [PreserveSig]
        int GetIsVisible(out bool visible);

        [PreserveSig]
        int SetShowOptions(int flag1, int falgs2);

        [PreserveSig]
        int SetEnabledSystemMenuItems(int flag);

        [PreserveSig]
        void OnTitleBarColorUpdated();

        [PreserveSig]
        void OnTitleBarDrawnByAppUpdated();

        [PreserveSig]
        void OnTitleBarHitTestVisualUpdated();

        [PreserveSig]
        int SetVisibleButtons(int flag1, int falgs2);

        [PreserveSig]
        int GetVisibleButtons(int flag);

        [Obsolete("Wrong signature")]
        void SetDragOptions();

        [PreserveSig]
        int GetDrawnByApp(out bool value);

        [Obsolete("Wrong signature")]
        void SetHideOptions();
        [Obsolete("Wrong signature")]
        void SetFrameHoverState();
        [Obsolete("Wrong signature")]
        void SetSuppressedButtons();
        [Obsolete("Wrong signature")]
        void OnDesiredTitlebarOverlayStateUpdated();
        [Obsolete("Wrong signature")]
        void OnTitleBarIconUpdated();
        [Obsolete("Wrong signature")]
        void SetPinButtonHandler();

        [PreserveSig]
        void ShowActionsMenu();

        [PreserveSig]
        int GetState();

        [PreserveSig]
        IntPtr GetFrameWindow();
    }

    public static class ApplicationFrameTitleBarExtensions
    {
        public static void SetVisibleButtons(this IApplicationFrameTitleBar titleBar, VisibleButtons buttons)
        {
            if (buttons == (VisibleButtons.Min | VisibleButtons.Close))
                titleBar.SetVisibleButtons(2, 0);
            else if (buttons == (VisibleButtons.Max | VisibleButtons.Close))
                titleBar.SetVisibleButtons(0, 1);
            else
                titleBar.SetVisibleButtons(2, 2); // All
        }
    }

    public enum VisibleButtons
    {
        Min,
        Max,
        Close
    }
}
