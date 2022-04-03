#if NETCOREAPP3_1
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Application = System.Windows.Forms.Application;

namespace FullTrustUWP.Core
{
    public class XamlIslandsApp
    {
        public static void Launch<TApp, TPage>() where TApp : IDisposable where TPage : UIElement
        {
            Form form = new();
            form.Show();
            // Default XamlIsland without Microsoft Toolkit Lib(self implemented)
            using (Activator.CreateInstance<TApp>())
            {
                DesktopWindowXamlSource xamlSource = new();
                xamlSource.Content = Activator.CreateInstance<TPage>();
                IDesktopWindowXamlSourceNative2 xamlSourceNative = (xamlSource as IDesktopWindowXamlSourceNative2)!;
                xamlSourceNative.AttachToWindow(form.Handle);
                IntPtr hWnd = xamlSourceNative.WindowHandle;
                int flags = GetWindowLong(hWnd, -16);
                SetWindowLong(hWnd, -16, flags | 0x10000000);

                Application.Run(form);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    }

    [ComImport, Guid("e3dcd8c7-3057-4692-99c3-7b7720afda31")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal partial interface IDesktopWindowXamlSourceNative2
    {
        void AttachToWindow(IntPtr parentWnd);
        IntPtr WindowHandle { get; }
        bool PreTranslateMessage(ref System.Windows.Forms.Message message);
    }
}

#endif