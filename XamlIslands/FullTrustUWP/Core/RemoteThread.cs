using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core
{
    public sealed class RemoteThread
    {
        #region standard imports from kernel32

        // CreateRemoteThread, since ThreadProc is in remote process, we must use a raw function-pointer.
        [DllImport("kernel32")]
        public static extern IntPtr CreateRemoteThread(
          IntPtr hProcess,
          IntPtr lpThreadAttributes,
          uint dwStackSize,
          IntPtr lpStartAddress, // raw Pointer into remote process
          IntPtr lpParameter,
          uint dwCreationFlags,
          out uint lpThreadId
        );

        const uint PROCESS_ALL_ACCESS = 0x000F0000 | 0x00100000 | 0xFFF;
        [DllImport("kernel32")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(
             IntPtr hProcess,
             IntPtr lpBaseAddress,
             byte[] lpBuffer,
             Int32 nSize,
             out IntPtr lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32")]
        public static extern
        uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeThread(IntPtr hThread, out int lpExitCode);

        #endregion // standard imports from kernel32

        // Helper to wait for a thread to exit and print its exit code
        static void WaitForThreadToExit(IntPtr hThread)
        {
            WaitForSingleObject(hThread, unchecked((uint)-1));

            int exitCode;
            GetExitCodeThread(hThread, out exitCode);
            Marshal.ThrowExceptionForHR(exitCode);
        }

        struct DwmSetWindowAttribute_Attributes
        {
            public IntPtr hwnd;
            public uint attr;
            public IntPtr attrValue;
            public uint attrSize;
        }

        struct MessageBox_Attributes
        {
            // public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPStr)] public string lpText;
            [MarshalAs(UnmanagedType.LPStr)] public string lpCaption;
            public uint uType;
        }

        public enum DwmWindowAttribute : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation,
            PASSIVE_UPDATE_MODE,
            USE_HOSTBACKDROPBRUSH,
            USE_IMMERSIVE_DARK_MODE,
            WINDOW_CORNER_PREFERENCE,
            BORDER_COLOR,
            CAPTION_COLOR,
            TEXT_COLOR,
            VISIBLE_FRAME_BORDER_THICKNESS,
            LAST
        }

        public static void UnCloakWindow(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out var pid);

            IntPtr hModule = GetModuleHandle("dwmapi.dll");
            IntPtr fpProc = GetProcAddress(hModule, "DwmSetWindowAttribute");
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);

            DwmSetWindowAttribute_Attributes attribute_Attributes = new();
            attribute_Attributes.hwnd = hWnd;
            attribute_Attributes.attr = (uint)DwmWindowAttribute.NCRenderingEnabled;

            IntPtr valuePtr = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(valuePtr, 1);
            attribute_Attributes.attrValue = valuePtr;
            attribute_Attributes.attrSize = sizeof(int);

            byte[] data = Struct2Bytes(attribute_Attributes, out int size);

            //IntPtr hModule = GetModuleHandle("User32.dll");
            //IntPtr fpProc = GetProcAddress(hModule, "MessageBoxA");
            //IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);

            //MessageBox_Attributes attribute_Attributes = new();
            //// attribute_Attributes.hwnd = hWnd;
            //attribute_Attributes.lpCaption = "Hallo!";
            //attribute_Attributes.lpText = "Test!";
            //attribute_Attributes.uType = 0;

            //byte[] data = Struct2Bytes(attribute_Attributes, out int size);

            IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)size, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);

            WriteProcessMemory(hProcess, allocMemAddress, data, size, out var _);

            // DwmSetWindowAttribute_Attributes test = Marshal.PtrToStructure<DwmSetWindowAttribute_Attributes>(allocMemAddress);

            uint dwThreadId;
            // Create a thread in the first process.
            IntPtr hThread = CreateRemoteThread(
                hProcess,
                IntPtr.Zero,
                0,
                fpProc, allocMemAddress,
                0,
                out dwThreadId);
            WaitForThreadToExit(hThread);

            //Marshal.FreeHGlobal(valuePtr);
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        static byte[] Struct2Bytes<T>(T str, out int size)
        {
            size = Marshal.SizeOf(str);
            byte[] data = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, false);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);
            return data;
        }
    }
}