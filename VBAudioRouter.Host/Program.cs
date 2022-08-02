using Sentry;
using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            using (SentrySdk.Init(o =>
            {
                o.Dsn = "https://ba91eccb16f94401895d04e20e0db0f0@o646413.ingest.sentry.io/6409713";
                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = true;
                // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;
            }))
            {
                try
                {
                    VBAudioRouter.Program.WinMain(args);
                }
                catch (Exception ex)
                {
                    MessageBox(IntPtr.Zero, ex.Message, "Error", MB_ICONERROR);
                    throw;
                }
            }
        }

        const int MB_OKCANCEL = 0x00000001;
        const int MB_ICONERROR = 0x00000010;

        [DllImport("user32.dll")]
        static extern void MessageBox(
            IntPtr hWnd,
            string text,
            string caption,
            uint type
        );
    }
}