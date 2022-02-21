using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
                new PrivateCreateCoreWindow_Sig(RoGetServerActivatableClassesImpl),
                null); ;
            hook.ThreadACL.SetInclusiveACL(new int[] { 0 });

            global::Windows.UI.Xaml.Application.Start((p) => new App());
        }

        [DllImport("combase.dll"), PreserveSig]
        static extern int RoGetServerActivatableClasses(
            [MarshalAs(UnmanagedType.HString)] string serverName,
            out IntPtr activatableClassIds,
            out uint count
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate int PrivateCreateCoreWindow_Sig(
            int windowType,
            [MarshalAs(UnmanagedType.BStr)] string windowTitle,
            int x,
            int y,
            uint width,
            uint height,
            uint dwAttributes,
            IntPtr hOwnerWindow,
            Guid riid,
            out object windowRef
        );

        static unsafe int RoGetServerActivatableClassesImpl(
            int windowType,
            [MarshalAs(UnmanagedType.BStr)] string windowTitle,
            int x,
            int y,
            uint width,
            uint height,
            uint dwAttributes,
            IntPtr hOwnerWindow,
            Guid riid,
            out object windowRef
        )
        {
            windowRef = null;
            return 0;
        }
    }
}
