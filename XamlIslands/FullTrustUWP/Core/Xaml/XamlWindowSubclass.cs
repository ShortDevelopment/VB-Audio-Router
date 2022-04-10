using FullTrustUWP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using XamlWindow = Windows.UI.Xaml.Window;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlWindowSubclass : IDisposable
    {
        #region LiveTime
        static Dictionary<XamlWindow, XamlWindowSubclass> _subclassRegistry = new();

        /// <summary>
        /// Attaches a <see cref="XamlWindowSubclass"/> to a given <see cref="XamlWindow"/>. <br/>
        /// Only one subclass ist allowed per window!
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        public static XamlWindowSubclass Attach(XamlWindow window)
        {
            if (_subclassRegistry.ContainsKey(window))
                throw new ArgumentException($"{nameof(window)} already has a subclass!");

            XamlWindowSubclass subclass = new(window);
            subclass.Install();
            _subclassRegistry.Add(window, subclass);

            return subclass;
        }

        /// <summary>
        /// Returns an existing <see cref="XamlWindowSubclass"/> for a given <see cref="XamlWindow"/>
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="KeyNotFoundException" />
        public static XamlWindowSubclass ForWindow(XamlWindow window)
            => _subclassRegistry[window];

        /// <inheritdoc cref="ForWindow(XamlWindow)"/>
        public static XamlWindowSubclass ForCurrentWindow()
            => ForWindow(XamlWindow.Current);
        #endregion

        #region Instance
        public XamlWindow Window { get; }
        public IWindowPrivate? WindowPrivate { get; }

        public IntPtr Hwnd
            => Window.GetHwnd();

        private XamlWindowSubclass(XamlWindow window)
        {
            this.Window = window;
            this.WindowPrivate = window as object as IWindowPrivate;
            Debug.Assert(WindowPrivate != null, $"\"{nameof(WindowPrivate)}\" is null");
        }

        bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(XamlWindowSubclass));
            _disposed = true;

            Uninstall();
            _subclassRegistry.Remove(this.Window);

            GC.KeepAlive(this);
        }
        #endregion

        #region Registration
        SubclassProc? _subclassProc;
        IntPtr? _subclassProcPtr;
        void Install()
        {
            if (_subclassProc != null)
                throw new InvalidOperationException();

            _subclassProc = XamlWindowSubclassProc;
            _subclassProcPtr = Marshal.GetFunctionPointerForDelegate(_subclassProc);
            SetWindowSubclass(Hwnd, _subclassProc, IntPtr.Zero, IntPtr.Zero);
        }

        void Uninstall()
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
        static extern IntPtr DefSubclassProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        #endregion
        #endregion

        IntPtr XamlWindowSubclassProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data)
        {
            const uint WM_NCHITTEST = 0x0084;
            if (msg == WM_NCHITTEST)
            {
                if (CursorIsInTitleBar)
                    return (IntPtr)2;
                // return (IntPtr)(-1); // Nowhere
            }

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

        #region WinApi
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

        #region Settings
        /// <summary>
        /// If <see langword="true" />, the window will be dragable as if the mouse would be on the titlebar
        /// </summary>
        public bool CursorIsInTitleBar { get; set; } = false;

        bool _hasWin32TitleBar = true;
        /// <summary>
        /// If <see langword="false"/>, the window will have no (win32) titlebar
        /// </summary>
        public bool HasWin32TitleBar
        {
            get => _hasWin32TitleBar;
            set
            {
                _hasWin32TitleBar = value;
                NotifyFrameChanged(Hwnd);
            }
        }

        bool _isTopMost = false;
        public bool IsTopMost
        {
            get => _isTopMost;
            set
            {
                const int HWND_TOPMOST = -1;
                const int HWND_NOTOPMOST = -2;
                SetWindowPos(Hwnd,
                    value ? (IntPtr)HWND_TOPMOST : (IntPtr)HWND_NOTOPMOST,
                    0, 0, 0, 0,
                    SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize
                );
            }
        }
        #endregion

        const int GWL_STYLE = -16;
        public void ShowWin32Frame()
            => SetWindowLong(Hwnd, GWL_STYLE, 0x94CF0000, notifyWindow: true);

        public void HideWin32Frame()
            => SetWindowLong(Hwnd, GWL_STYLE, 0x94000000, notifyWindow: true);

        static void NotifyFrameChanged(IntPtr hWnd)
        {
            // https://github.com/strobejb/winspy/blob/03887c8ab1ebc9abad6865743eba15b94c9e9dbc/src/StyleEdit.c#L143
            SetWindowPos(
                hWnd, IntPtr.Zero,
                0, 0, 0, 0,
                SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize | SetWindowPosFlags.IgnoreZOrder | SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.FrameChanged
            );
            // InvalidateRect(hWnd, IntPtr.Zero, true);
        }

        #region SetWindowLong
        static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong, bool notifyWindow = true)
        {
            IntPtr result;
            if (IntPtr.Size == 8)
                result = SetWindowLongPtr64(hWnd, nIndex, new IntPtr(dwNewLong));
            else
                result = SetWindowLong32(hWnd, nIndex, dwNewLong);

            if (notifyWindow)
                NotifyFrameChanged(hWnd);

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
