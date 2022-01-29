using FullTrustUWP.Core.Activation;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static FullTrustUWP.Core.Activation.ApplicationFrameActivator;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Form form = new();
            form.Show();

            var frameManager = ApplicationFrameActivator.CreateApplicationFrameManager();

            Marshal.ThrowExceptionForHR(frameManager.GetFrameArray(out var frameArray));
            Marshal.ThrowExceptionForHR(frameArray.GetCount(out var count));
            for (uint i = 0; i < count; i++)
            {
                Guid iid = typeof(IApplicationFrame).GUID;
                Marshal.ThrowExceptionForHR(frameArray.GetAt(i, ref iid, out object frameUnk));
                IApplicationFrame frame = frameUnk as IApplicationFrame;
                Marshal.ThrowExceptionForHR(frame.GetChromeOptions(out var options));
                Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out var hwnd));
                Debug.Print($"HWND: {hwnd}; OPTIONS: {options}");
            }

            //Marshal.ThrowExceptionForHR(frameManager.CreateFrame(out var frame));
            //Marshal.ThrowExceptionForHR(frame.GetFrameWindow(out IntPtr hwnd));
            //var coreWindow = CoreWindowActivator.CreateCoreWindow(CoreWindowActivator.WindowType.NOT_IMMERSIVE, "Test", hwnd);
            //IntPtr contentHwnd = (coreWindow as object as ICoreWindowInterop).WindowHandle;
            //contentHwnd = (IntPtr)0x1107F8;
            // Marshal.ThrowExceptionForHR(frame.SetPresentedWindow(ref contentHwnd));            
            //Marshal.ThrowExceptionForHR(frame.SetBackgroundColor(System.Drawing.Color.Red.ToArgb()));
            // XamlHostApplication<App>.Run<WelcomePage>();
            // Marshal.ThrowExceptionForHR(frameManager.DestroyFrame(ref frame));

            Application.Run();
        }
    }
}
