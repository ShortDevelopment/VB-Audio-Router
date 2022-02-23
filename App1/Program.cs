using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;

namespace App1
{
    public static class Program
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, int ordinal);

        static void Main(string[] args)
        {
            IntPtr hLib = LoadLibrary("windows.ui.dll");
            IntPtr hProc = GetProcAddress(hLib, 0x5dc);
            var hook = EasyHook.LocalHook.Create(
                hProc,
                new PrivateCreateCoreWindowSig(PrivateCreateCoreWindowImpl),
                null); ;
            // hook.ThreadACL.SetInclusiveACL(new int[] { 0 });
            hook.ThreadACL.SetExclusiveACL(new[] { 12345678 });

            global::Windows.UI.Xaml.Application.Start((p) => new App());
        }

        [DllImport("combase.dll"), PreserveSig]
        static extern int RoGetServerActivatableClasses(
            [MarshalAs(UnmanagedType.HString)] string serverName,
            out IntPtr activatableClassIds,
            out uint count
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int PrivateCreateCoreWindowSig(
            int windowType,
            string windowTitle,
            int x,
            int y,
            uint width,
            uint height,
            uint dwAttributes,
            IntPtr hOwnerWindow,
            Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out CoreWindow windowRef
        );

        static unsafe int PrivateCreateCoreWindowImpl(
            int windowType,
            string windowTitle,
            int x,
            int y,
            uint width,
            uint height,
            uint dwAttributes,
            IntPtr hOwnerWindow,
            Guid riid,
            out CoreWindow windowRef
        )
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            windowRef = null;
            return 0;
        }
    }
}
