using FullTrustUWP.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using XamlWindow = Windows.UI.Xaml.Window;

namespace FullTrustUWP.Core.Xaml
{
    public static class XamlWindowExtensions
    {
        private const int animationDurationMs = 1000;

        public static void ShowAsFlyout(this XamlWindow window)
        {
            IntPtr hwnd = window.GetHwnd();
            if (AnimateWindow(hwnd, animationDurationMs, AnimateWindowFlags.ACTIVATE | AnimateWindowFlags.SLIDE | AnimateWindowFlags.HOR_POSITIVE) != 0)
                throw new Win32Exception();
        }

        public static void HideAsFlyout(this XamlWindow window)
        {
            IntPtr hwnd = window.GetHwnd();
            if (AnimateWindow(hwnd, animationDurationMs, AnimateWindowFlags.HIDE | AnimateWindowFlags.SLIDE | AnimateWindowFlags.HOR_POSITIVE) != 0)
                throw new Win32Exception();
        }

        [DllImport("user32", SetLastError = true), PreserveSig]
        static extern int AnimateWindow(IntPtr hwnd, int time, AnimateWindowFlags flags);

        [Flags]
        enum AnimateWindowFlags
        {
            HOR_POSITIVE = 0x00000001,
            HOR_NEGATIVE = 0x00000002,
            VER_POSITIVE = 0x00000004,
            VER_NEGATIVE = 0x00000008,
            CENTER = 0x00000010,
            HIDE = 0x00010000,
            ACTIVATE = 0x00020000,
            SLIDE = 0x00040000,
            BLEND = 0x00080000
        }

        public static IntPtr GetHwnd(this XamlWindow window)
            => window.CoreWindow.GetHwnd();

        public static IntPtr GetHwnd(this Windows.UI.Core.CoreWindow window)
            => (window as object as ICoreWindowInterop)!.WindowHandle;

        const int GWL_STYLE = -16;
        public static void ShowWin32Frame(this XamlWindow window)
            => SetWindowLong(window.GetHwnd(), GWL_STYLE, 0x94CF0000, notifyWindow: true);

        public static void HideWin32Frame(this XamlWindow window)
            => SetWindowLong(window.GetHwnd(), GWL_STYLE, 0x94000000, notifyWindow: true);

        #region SetWindowLong
        static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong, bool notifyWindow = true)
        {
            IntPtr result;
            if (IntPtr.Size == 8)
                result = SetWindowLongPtr64(hWnd, nIndex, new IntPtr(dwNewLong));
            else
                result = SetWindowLong32(hWnd, nIndex, dwNewLong);

            if (notifyWindow)
            {
                // https://github.com/strobejb/winspy/blob/03887c8ab1ebc9abad6865743eba15b94c9e9dbc/src/StyleEdit.c#L143
                SetWindowPos(
                    hWnd, IntPtr.Zero,
                    0, 0, 0, 0,
                    SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize | SetWindowPosFlags.IgnoreZOrder | SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.FrameChanged
                );
                // InvalidateRect(hWnd, IntPtr.Zero, true);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        #endregion

        #region SetWindowPos
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [Flags]
        private enum SetWindowPosFlags : uint
        {
            AsynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }
        #endregion

        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
    }
}
