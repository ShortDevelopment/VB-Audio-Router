using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using XamlWindow = Windows.UI.Xaml.Window;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlWindowTransparencyHook
    {
        public XamlWindowTransparencyHook(XamlWindow window)
        {
            internalWndProc = SetWndProc(window, CustomWndProc);
        }

        private IntPtr internalWndProc = IntPtr.Zero;
        private IntPtr CustomWndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            // Debug.Print($"msg: {msg}");
            const uint WM_NCHITTEST = 0x0084;

            if (msg == WM_NCHITTEST)
            {
                return (IntPtr)(-1);
            }

            if (internalWndProc != IntPtr.Zero)
                return CallWindowProc(internalWndProc, hwnd, msg, wParam, lParam);
            return (IntPtr)0;
        }

        IntPtr newWndProcPtr = IntPtr.Zero;

        #region API
        private const int GWLP_WNDPROC = -4;
        IntPtr SetWndProc(XamlWindow window, WndProcDelegate newProc)
        {
            IntPtr hwnd = window.GetHwnd();

            newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newProc);
            return SetWindowLong(hwnd, GWLP_WNDPROC, newWndProcPtr);
        }

        private delegate IntPtr WndProcDelegate(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLong64(hWnd, nIndex, dwNewLong);
            return SetWindowLong32(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong"), PreserveSig]
        static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr"), PreserveSig]
        static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        #endregion
    }
}
