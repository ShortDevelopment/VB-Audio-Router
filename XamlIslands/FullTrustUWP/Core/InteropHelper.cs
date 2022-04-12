using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core
{
    public static class InteropHelper
    {
        #region RoGetActivationFactory
        [DllImport("combase.dll", EntryPoint = "RoGetActivationFactory", CharSet = CharSet.Unicode, SetLastError = true), PreserveSig]
        public static extern int RoGetActivationFactory([MarshalAs(UnmanagedType.HString)] string activatableClassId, ref Guid iid, out IWinRTActivationFactory factory);

        public static T RoGetActivationFactory<T>(string activatableClassId)
        {
            Guid iid = typeof(T).GUID;
            Marshal.ThrowExceptionForHR(RoGetActivationFactory(activatableClassId, ref iid, out var ptr));
            return (T)ptr;
        }
        #endregion

        [DllImport("Ole32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int CoCreateInstance(
            ref Guid rclsid,
            [MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
            uint context,
            ref Guid iid,
            [MarshalAs(UnmanagedType.Interface)] out object result
        );

        public static T? ComCreateInstance<T>(string clsid)
            => ComCreateInstance<T>(new Guid(clsid));

        public static T? ComCreateInstance<T>(Guid clsid)
        {
            Type? type = Type.GetTypeFromCLSID(clsid);
            if (type == null)
                return default(T);
            return (T?)Activator.CreateInstance(type);
        }
    }

    [Guid("00000035-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IWinRTActivationFactory
    {
        IntPtr ActivateInstance();
    }

    public class WindowBandHelper
    {
        public enum ZBandID : int
        {
            Default = 0x0,
            Desktop = 0x1,
            UIAccess = 0x2,
            ImmersiveIHM = 0x3,
            ImmersiveNotification = 0x4,
            ImmersiveAppChrome = 0x5,
            ImmersiveMogo = 0x6,
            ImmersiveEdgy = 0x7,
            ImmersiveInActiveMOBODY = 0x8,
            ImmersiveInActiveDock = 0x9,
            ImmersiveActiveMOBODY = 0xA,
            ImmersiveActiveDock = 0xB,
            ImmersiveBackground = 0xC,
            ImmersiveSearch = 0xD,
            GenuineWindows = 0xE,
            ImmersiveRestricted = 0xF,
            SystemTools = 0x10,
            Lock = 0x11,
            AboveLockUX = 0x12,
        };

        [DllImport("user32.dll"), PreserveSig]
        public static extern int GetWindowBand(IntPtr hWnd, out ZBandID bandId);

        [DllImport("user32.dll"), PreserveSig, Obsolete("Needs IAMAccess!")]
        public static extern int SetWindowBand(IntPtr hWnd, IntPtr hwndInsertAfter, ZBandID bandId);
    }
}
