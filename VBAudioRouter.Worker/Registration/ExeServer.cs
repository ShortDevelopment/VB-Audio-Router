using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VBAudioRouter.Worker.Registration
{
    /// <summary>
    /// <see href="https://github.com/dotnet/samples/blob/main/core/extensions/OutOfProcCOM/COMRegistration/LocalServer.cs">LocalServer.cs</see>
    /// </summary>
    internal sealed class ExeServer : IDisposable
    {
        private readonly List<int> registrationCookies = new();

        public void RegisterClass<T>(Guid clsid) where T : new()
        {
            int cookie;
            int hr = Ole32.CoRegisterClassObject(
                ref clsid,
                new GenericClassFactory<T>(),
                Ole32.CLSCTX_LOCAL_SERVER, Ole32.REGCLS_MULTIPLEUSE | Ole32.REGCLS_SUSPENDED,
                out cookie
            );
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);

            registrationCookies.Add(cookie);

            hr = Ole32.CoResumeClassObjects();
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);
        }

        public void Dispose()
        {
            foreach (int cookie in registrationCookies)
                Ole32.CoRevokeClassObject(cookie);
        }

        private class Ole32
        {
            // https://docs.microsoft.com/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx
            public const int CLSCTX_LOCAL_SERVER = 0x4;

            // https://docs.microsoft.com/windows/win32/api/combaseapi/ne-combaseapi-regcls
            public const int REGCLS_MULTIPLEUSE = 1;
            public const int REGCLS_SUSPENDED = 4;

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-coregisterclassobject
            [DllImport(nameof(Ole32))]
            public static extern int CoRegisterClassObject(ref Guid guid, [MarshalAs(UnmanagedType.IUnknown)] object obj, int context, int flags, out int register);

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-coresumeclassobjects
            [DllImport(nameof(Ole32))]
            public static extern int CoResumeClassObjects();

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-corevokeclassobject
            [DllImport(nameof(Ole32))]
            public static extern int CoRevokeClassObject(int register);
        }
    }
}
