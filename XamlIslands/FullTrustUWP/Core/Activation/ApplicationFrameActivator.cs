using FullTrustUWP.Core.Interfaces;

namespace FullTrustUWP.Core.Activation
{
    public static class ApplicationFrameActivator
    {
        public static IApplicationFrameManager CreateApplicationFrameManager()
        {
            // CLSID_ApplicationFrameManagerPriv = ddc05a5a-351a-4e06-8eaf-54ec1bc2dcea
            // CLSID_ApplicationFrameManager = b9b05098-3e30-483f-87f7-027ca78da287 // b9b05098_3e30_483f_87f7_027ca78da287
            return InteropHelper.ComCreateInstance<IApplicationFrameManager>("b9b05098-3e30-483f-87f7-027ca78da287")!;
        }
    }
}
