using System;
using System.Runtime.InteropServices;

namespace VBAudioRouter.Worker.Registration
{
    /// <summary>
    /// <see href="https://github.com/dotnet/samples/blob/main/core/extensions/OutOfProcCOM/COMRegistration/BasicClassFactory.cs">BasicClassFactory.cs</see>
    /// </summary>
    internal sealed class GenericClassFactory<T> : IClassFactory where T : new()
    {
        private static readonly Guid IID_IUnknown = Guid.Parse("00000000-0000-0000-C000-000000000046");

        public void CreateInstance([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, ref Guid riid, out IntPtr ppvObject)
        {
            Type interfaceType = GetInterfaceType(ref riid, pUnkOuter);

            object result = new();
            if (pUnkOuter == null)
                result = CreateAggregatedObject(pUnkOuter, result);

            ppvObject = GetInterfacePtr(result, interfaceType);
        }

        private static Type GetInterfaceType(ref Guid riid, object pUnkOuter)
        {
            if (riid == IID_IUnknown)
                return typeof(object);

            // Aggregation can only be done when requesting IUnknown.
            if (pUnkOuter != null)
            {
                const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
                throw new COMException(string.Empty, CLASS_E_NOAGGREGATION);
            }

            // Verify the class implements the desired interface
            foreach (Type i in typeof(T).GetInterfaces())
                if (i.GUID == riid)
                    return i;

            // E_NOINTERFACE
            throw new InvalidCastException();
        }

        private static IntPtr GetInterfacePtr(object instance, Type interfaceType)
        {
            if (interfaceType == typeof(object))
                return Marshal.GetIUnknownForObject(instance);

            IntPtr ptr = Marshal.GetComInterfaceForObject(instance, interfaceType, CustomQueryInterfaceMode.Ignore);
            if (ptr == IntPtr.Zero)
                throw new InvalidCastException(); // E_NOINTERFACE

            return ptr;
        }

        private static object CreateAggregatedObject(object pUnkOuter, object instance)
        {
            IntPtr outerPtr = Marshal.GetIUnknownForObject(pUnkOuter);

            try
            {
                IntPtr innerPtr = Marshal.CreateAggregatedObject(outerPtr, instance);
                return Marshal.GetObjectForIUnknown(innerPtr);
            }
            finally
            {
                // Decrement the above 'Marshal.GetIUnknownForObject()'
                Marshal.Release(outerPtr);
            }
        }

        public void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock) { }
    }
}
