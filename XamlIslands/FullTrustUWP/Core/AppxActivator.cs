using FullTrustUWP.Core.Activation;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Activation;

namespace FullTrustUWP.Core
{
    public sealed class AppxActivator
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    [Guid("92696c00-7578-48e1-ac1a-2ca909e2c8cf")]
    public interface IActivatableApplication
    {
        void Activate(ref ICoreWindowFactory windowFactory, [MarshalAs(UnmanagedType.HString)] string serverName, ref IActivatedEventArgs eventArgs);
    }

    public sealed class ActivatedEventArgsImpl : IActivatedEventArgs, IPrelaunchActivatedEventArgs, ILaunchActivatedEventArgs
    {
        public ActivationKind Kind => ActivationKind.Launch;

        public ApplicationExecutionState PreviousExecutionState => ApplicationExecutionState.NotRunning;

        public SplashScreen SplashScreen => null;

        public bool PrelaunchActivated => false;

        public string Arguments => "";

        public string TileId => "";
    }
}
