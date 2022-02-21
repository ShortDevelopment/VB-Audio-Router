using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace App1
{
    public static class Program
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, int ordinal);

        static void Main(string[] args)
            => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            var hook = EasyHook.LocalHook.Create(
                EasyHook.LocalHook.GetProcAddress("sechost.dll", "CapabilityCheck"),
                new CapabilityCheck_Sig(CapabilityCheckImpl),
                null); ;
            hook.ThreadACL.SetInclusiveACL(new int[] { 0 });

            var accessStatus = await Geolocator.RequestAccessAsync();

            global::Windows.UI.Xaml.Application.Start((p) => new App());
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        delegate int CapabilityCheck_Sig(
            IntPtr hUnknown,
            string capabilityName,
            out bool hasCapability
        );

        static unsafe int CapabilityCheckImpl(
            IntPtr hUnknown,
            string capabilityName,
            out bool hasCapability
        )
        {
            hasCapability = true;
            return 0;
        }
    }
}
