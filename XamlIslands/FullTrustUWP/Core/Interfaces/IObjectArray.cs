using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Interfaces
{

    [Guid("92CA9DCD-5622-4bba-A805-5E9F541BD8C9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IObjectArray
    {
        [PreserveSig]
        int GetCount(out uint count);

        [PreserveSig]
        int GetAt(uint uiIndex, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object count);
    }


    public static class IObjectArrayExtensions
    {
        public static T GetAt<T>(this IObjectArray array, int index)
            => GetAt<T>(array, (uint)index);

        public static T GetAt<T>(this IObjectArray array, uint index)
        {
            Guid iid = typeof(T).GUID;
            Marshal.ThrowExceptionForHR(array.GetAt(index, ref iid, out object result));
            return (T)result;
        }
    }
}
