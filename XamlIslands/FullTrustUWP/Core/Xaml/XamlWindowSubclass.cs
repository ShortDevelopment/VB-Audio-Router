using System;
using System.Runtime.InteropServices;
using XamlWindow = Windows.UI.Xaml.Window;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlWindowSubclass : IDisposable
    {
        public XamlWindow Window { get; set; }

        public IntPtr Hwnd
            => Window.GetHwnd();

        public XamlWindowSubclass(XamlWindow window)
        {
            this.Window = window;

            InstallSubclass();
        }

        ~XamlWindowSubclass()
        {
            RemoveSubclass();
        }

        public void Dispose()
        {
            GC.KeepAlive(this);
        }


        SubclassProc? _subclassProc;
        IntPtr? _subclassProcPtr;
        void InstallSubclass()
        {
            if (_subclassProc != null)
                throw new InvalidOperationException();

            _subclassProc = XamlWindowSubclassProc;
            _subclassProcPtr = Marshal.GetFunctionPointerForDelegate(_subclassProc);
            SetWindowSubclass(Hwnd, _subclassProc, IntPtr.Zero, IntPtr.Zero);
        }

        void RemoveSubclass()
        {
            if (_subclassProc == null)
                throw new InvalidOperationException();

            RemoveWindowSubclass(Hwnd, _subclassProc, IntPtr.Zero);
            _subclassProc = null;
        }

        #region WinApi
        [DllImport("comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetWindowSubclass(IntPtr hwnd, SubclassProc callback, IntPtr id, IntPtr data);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool RemoveWindowSubclass(IntPtr hwnd, SubclassProc callback, IntPtr id);

        private delegate IntPtr SubclassProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr DefSubclassProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        #endregion

        public bool CursorIsInTitleBar { get; set; } = false;
        public bool HasWin32TitleBar { get; set; } = true;

        IntPtr XamlWindowSubclassProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data)
        {
            const uint WM_NCHITTEST = 0x0084;
            if (CursorIsInTitleBar && msg == WM_NCHITTEST)
                return (IntPtr)(2);

            const int WM_NCCALCSIZE = 0x83;
            if (!HasWin32TitleBar && msg == WM_NCCALCSIZE)
            {
                // https//github.com/microsoft/terminal/blob/ff8fdbd2431f1cfd8211833815be481dfdec4420/src/cascadia/WindowsTerminal/NonClientIslandWindow.cpp#L405
                var topOld = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam).rgrc0.top;

                // Run default processing
                var result = DefSubclassProc(hwnd, msg, wParam, lParam);

                var nccsp = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam);
                // Rest to old top (remove title bar)
                nccsp.rgrc0.top = topOld;
                Marshal.StructureToPtr(nccsp, lParam, true);
                return result;
            }

            return DefSubclassProc(hwnd, msg, wParam, lParam);
        }

        #region NCCALCSIZE_PARAMS
        [StructLayout(LayoutKind.Sequential)]
        struct NCCALCSIZE_PARAMS
        {
            public RECT rgrc0, rgrc1, rgrc2;
            public WINDOWPOS lppos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndinsertafter;
            public int x, y, cx, cy;
            public int flags;
        }
        #endregion
    }
}
