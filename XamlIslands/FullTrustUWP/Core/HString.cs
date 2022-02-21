using System;
using System.Runtime.InteropServices;

namespace FullTrustUWP.Core
{
    public sealed class HString
    {
        public IntPtr Ptr { get; private set; }

        public HString(IntPtr ptr)
            => this.Ptr = ptr;

        public HString(string value)
        {
            Marshal.ThrowExceptionForHR(WindowsCreateString(value, (uint)value.Length, out IntPtr ptr));
            this.Ptr = ptr;
        }

        [DllImport("combase.dll", CharSet = CharSet.Unicode), PreserveSig]
        private static extern int WindowsCreateString(
            string WindowsCreateString,
            uint length,
            out IntPtr hstring
        );

        public string Value
        {
            get
            {
                return Marshal.PtrToStringUni(WindowsGetStringRawBuffer(this.Ptr, out _))!;
            }
        }

        [DllImport("combase.dll"), PreserveSig]
        private static extern IntPtr WindowsGetStringRawBuffer(
            IntPtr hstring,
            out uint count
        );
    }
}
