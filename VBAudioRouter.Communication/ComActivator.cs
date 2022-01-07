using System;
using System.Runtime.InteropServices;

namespace VBAudioRouter.Communication
{
    public static class ComActivator
    {
        public static T ActivateInterface<T>(string clsid) => ActivateInterface<T>(new Guid(clsid));
        public static T ActivateInterface<T>(Guid clsid)
        {
            Guid riid = typeof(T).GUID;
            int hr = CoCreateInstance(ref clsid, IntPtr.Zero, 2, ref riid, out IntPtr interfacePtr);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);

            return (T)Marshal.GetObjectForIUnknown(interfacePtr);
        }

        [DllImport("Ole32"), PreserveSig]
        private static extern int CoCreateInstance(ref Guid rclsid, IntPtr pUnkOuter, uint dwClsContext, ref Guid riid, out IntPtr ppv);
    }
}
