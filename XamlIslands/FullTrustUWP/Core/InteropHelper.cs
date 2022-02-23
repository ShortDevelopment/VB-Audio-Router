using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core
{
    public static class InteropHelper
    {
        #region ThrowOnError
        public static void ThrowOnError() => ThrowOnError(Marshal.GetLastWin32Error());

        public static void ThrowOnError(int error)
        {
            if (error != 0)
                throw new Win32Exception(error);
        }
        #endregion

        #region DynamicLoad
        public static T DynamicLoad<T>(string libraryName, int ordinal) where T : Delegate
        {
            IntPtr hLib = LoadLibrary(libraryName);
            IntPtr hProc = GetProcAddress(hLib, ordinal);
            return Marshal.GetDelegateForFunctionPointer<T>(hProc);
        }

        public static T DynamicLoad<T>(string libraryName, string procName) where T : Delegate
        {
            IntPtr hLib = LoadLibrary(libraryName);
            IntPtr hProc = GetProcAddress(hLib, procName);
            return Marshal.GetDelegateForFunctionPointer<T>(hProc);
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, int ordinal);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        #endregion

        [DllImport("combase.dll", EntryPoint = "RoGetActivationFactory", CharSet = CharSet.Unicode, SetLastError = true), PreserveSig]
        public static extern int GetActivationFactory([MarshalAs(UnmanagedType.HString)] string activatableClassId, ref Guid iid, out IActivationFactory factory);

        [DllImport("Ole32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int CoCreateInstance(
            ref Guid rclsid,
            [MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
            uint context,
            ref Guid iid,
            [MarshalAs(UnmanagedType.Interface)] out object result
        );
    }

    [Guid("00000035-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IActivationFactory
    {
        IntPtr ActivateInstance();
    }

    public static class Const
    {
        public const string IID_ICoreApplicationPrivate2 = "6090202d-2843-4ba5-9b0d-fc88eecd9ce5";
        public const string IID_ICoreWindowStatic = "4d239005-3c2a-41b1-9022-536bb9cf93b1";
        public const string IID_ICoreWindowInterop = "45d64a29-a63e-4cb6-b498-5781d298cb4f";
    }
}
