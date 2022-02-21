using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FullTrustUWP.Core
{
    public sealed class RemoteThread
    {
        #region standard imports from kernel32
        [DllImport("kernel32")]
        public static extern IntPtr CreateRemoteThread(
          IntPtr hProcess,
          IntPtr lpThreadAttributes,
          uint dwStackSize,
          IntPtr lpStartAddress,
          IntPtr lpParameter,
          uint dwCreationFlags,
          out uint lpThreadId
        );

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

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

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeThread(IntPtr hThread, out int lpExitCode);

        #endregion // standard imports from kernel32

        static void WaitForThreadToExit(IntPtr hThread)
        {
            WaitForSingleObject(hThread, unchecked((uint)-1));

            int exitCode;
            GetExitCodeThread(hThread, out exitCode);
            //if (exitCode != 0)
            //    throw new Win32Exception(exitCode);
        }

        private static void LoadLibraryRemote(uint pid, string dllName)
        {
            IntPtr hModule = GetModuleHandle("kernel32.dll");
            IntPtr hProc = GetProcAddress(hModule, "LoadLibraryA");
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);

            LoadLibrary(dllName);

            IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((dllName.Length + 1) * sizeof(char)), AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);

            WriteProcessMemory(hProcess, allocMemAddress, Encoding.Default.GetBytes(dllName), (dllName.Length + 1) * sizeof(char), out _);

            IntPtr hThread = CreateRemoteThread(
                hProcess,
                IntPtr.Zero,
                0,
                hProc, allocMemAddress,
                0,
                out _);
            WaitForThreadToExit(hThread);
            CloseHandle(hThread);
            CloseHandle(hProcess);
        }

        const string libName = @"D:\Programmieren\Visual Studio Projects\VBAudioRouter\x64\Debug\UncloakHelper.dll";

        public static void CloakWindow(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out var pid);
            LoadLibraryRemote(pid, libName);
            CallFunctionRemote(pid, "UncloakHelper.dll", "CloakWindow", hWnd);
        }

        public static void UnCloakWindow(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out var pid);
            LoadLibraryRemote(pid, libName);
            CallFunctionRemote(pid, "UncloakHelper.dll", "UnCloakWindow", hWnd);
        }

        public static void MoveWindowToCurrentDesktop(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out var pid);
            LoadLibraryRemote(pid, libName);
            CallFunctionRemote(pid, "UncloakHelper.dll", "MoveWindowToCurrentDesktop", hWnd);
        }

        public static void CallFunctionRemote(uint pid, string library, string functionName, IntPtr arg)
        {
            IntPtr hModule = GetModuleHandle(library);
            IntPtr hProc = GetProcAddress(hModule, functionName);
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);

            IntPtr hThread = CreateRemoteThread(
                hProcess,
                IntPtr.Zero,
                0,
                hProc, arg,
                0,
                out _);
            WaitForThreadToExit(hThread);
            CloseHandle(hThread);
            CloseHandle(hProcess);
        }
    }
}