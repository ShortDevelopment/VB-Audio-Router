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

        [Guid("92CA9DCD-5622-4bba-A805-5E9F541BD8C9")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IObjectArray
        {
            [PreserveSig]
            int GetCount(out uint count);

            [PreserveSig]
            int GetAt(uint uiIndex, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object count);
        }

        [Guid("d6defab3-dbb9-4413-8af9-554586fdff94")] // d6defab3_dbb9_4413_8af9_554586fdff94
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IApplicationFrameManager
        {
            [PreserveSig]
            int CreateFrame(out IApplicationFrame frame);

            [PreserveSig]
            int DestroyFrame(ref IApplicationFrame frame);

            [PreserveSig]
            int GetFrameArray(out IObjectArray array);

            [Obsolete("Wrong signature")]
            void RegisterForFrameEvents();
            [Obsolete("Wrong signature")]
            void UnregisterForFrameEvents();
            [Obsolete("Wrong signature")]
            void EnableLayoutFrames();
        }

        [Guid("143715d9-a015-40ea-b695-d5cc267e36ee")] // 143715d9_a015_40ea_b695_d5cc267e36ee
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IApplicationFrame
        {
            [PreserveSig]
            int GetFrameWindow(out IntPtr hWnd);

            [Obsolete("Wrong signature")]
            void SetPosition();

            [PreserveSig]
            int GetPresentedWindow(out IntPtr hWnd);

            [PreserveSig]
            int SetPresentedWindow(IntPtr hWnd);

            [Obsolete("Wrong signature")]
            void SetSystemVisual();
            [Obsolete("Wrong signature")]
            void GetSystemVisual();
            [Obsolete("Wrong signature")]
            void SetApplicationId();
            [Obsolete("Wrong signature")]
            void SetMinimumSize();
            [Obsolete("Wrong signature")]
            void SetMaximumSize();

            [PreserveSig]
            int FitToWorkArea();

            [PreserveSig]
            int GetChromeOptions(out APPLICATION_FRAME_CHROME_OPTIONS options);

            [PreserveSig]
            int SetChromeOptions(int options1, int options2);

            [Obsolete("Wrong signature")]
            void GetChromeOffsets();

            [PreserveSig]
            int GetTitleBarDrawnByApp(out bool value);

            [PreserveSig]
            int InvokeActionsMenu();

            [PreserveSig]
            int GetTitleBar(out IApplicationFrameTitleBar titleBar);

            [PreserveSig]
            int GetBackgroundColor(out int color);

            [PreserveSig]
            int SetBackgroundColor(int color);

            [PreserveSig]
            int GetSystemVisualFadeTime(out uint time);

            [PreserveSig]
            int SetOperatingMode(FRAME_OPERATING_MODE mode);

            [PreserveSig]
            int SetSizeConstraintOverridesPhysical(ref tagSIZE size1, ref tagSIZE size2);

            [PreserveSig]
            int SetSizeConstraintOverridesLogical(ref tagSIZE size1, ref tagSIZE size2);

            [PreserveSig]
            int SetPreferredAspectRatioHint(ref tagSIZE ration);

            [Obsolete("Wrong signature")]
            void SetSystemVisualAnimation();
            [Obsolete("Wrong signature")]
            void GetPropertyValue();

            [PreserveSig]
            int EnsureSizeConstraints();

            [Obsolete("Wrong signature")]
            void OnCommand();
            [Obsolete("Wrong signature")]
            void OnCommand2();
            [Obsolete("Wrong signature")]
            void OnCloseCommand();

            [PreserveSig]
            bool IsEqual(IApplicationFrame frame);

            [PreserveSig]
            int Destroy();

            [PreserveSig]
            int NotifyChromeChange(NotifyChromeChangeFlags flags);

            [PreserveSig]
            int NotifyVisibleButtonsChange();

            [Obsolete("Wrong signature")]
            void GetMinimumSize(out tagSIZE size);
        }

        [Guid("c8e34820-d46a-41bc-8c5c-5bc9fdee243d")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IApplicationFrameTitleBar
        {
            [PreserveSig]
            int SetWindowTitle([MarshalAs(UnmanagedType.LPWStr)] string title);

            [PreserveSig]
            int SetIsVisible(bool visible);

            [PreserveSig]
            int GetIsVisible(out bool visible);
        }

        public struct tagSIZE
        {
            public int cx;
            public int cy;
        }

        public enum APPLICATION_FRAME_CHROME_OPTIONS
        {
            ZERO = 0,
            OTHER = 97
        }

        public enum NotifyChromeChangeFlags
        {
            ZERO = 0
        }

        public enum FRAME_OPERATING_MODE
        {
            ZERO = 0
        }
    }
}
