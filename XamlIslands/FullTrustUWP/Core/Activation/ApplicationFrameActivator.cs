using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core.Activation
{
    public static class ApplicationFrameActivator
    {
        public static IApplicationFrameManager CreateApplicationFrameManager()
        {
            // CLSID_ApplicationFrameManagerPriv = ddc05a5a-351a-4e06-8eaf-54ec1bc2dcea
            // CLSID_ApplicationFrameManager = b9b05098-3e30-483f-87f7-027ca78da287
            return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("b9b05098-3e30-483f-87f7-027ca78da287"))) as IApplicationFrameManager;
        }

        [Guid("d6defab3-dbb9-4413-8af9-554586fdff94")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IApplicationFrameManager
        {
            [PreserveSig]
            int CreateFrame(out IApplicationFrame frame);

            [PreserveSig]
            int DestroyFrame(ref IApplicationFrame frame);

            [Obsolete("Wrong signature")]
            void GetFrameArray();
            [Obsolete("Wrong signature")]
            void RegisterForFrameEvents();
            [Obsolete("Wrong signature")]
            void UnregisterForFrameEvents();
            [Obsolete("Wrong signature")]
            void EnableLayoutFrames();
        }

        [Guid("143715d9-a015-40ea-b695-d5cc267e36ee")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IApplicationFrame
        {
            [PreserveSig]
            int GetFrameWindow(out IntPtr hWnd);

            [Obsolete("Wrong signature")]
            int SetPosition();

            [PreserveSig]
            int GetPresentedWindow(out IntPtr hWnd);

            [PreserveSig]
            int SetPresentedWindow(ref IntPtr hWnd);
        }
    }
}
