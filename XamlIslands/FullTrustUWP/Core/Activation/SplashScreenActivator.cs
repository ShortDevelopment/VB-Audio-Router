using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Activation;

namespace FullTrustUWP.Core.Activation
{
    public static class SplashScreenActivator
    {
        public static ISplashScreen CreateSplashScreen()
        {
            ISplashScreen screen = new SplashScreenActivation() as object as ISplashScreen;
            return screen;
        }

        [ComImport, Guid("329b80ec-2230-47b8-905d-a2dcf5171c6f")]
        internal sealed class SplashScreenActivation { }

        [Guid("0e0da070-2224-4934-8090-041f7ff223eb")]
        public interface ISplashScreen {
            Windows.UI.Color TitleBarButtonPressedTextColorOverride { set; }
            void OnActivationBegin(/* ushort const *,SPLASHSCREEN_FLAGS,HWND__ * * */);
            Windows.UI.Color TitleBarButtonPressedTextColorOverride2 { set; }
            Windows.UI.Color TitleBarButtonPressedTextColorOverride3 { set; }
            Windows.UI.Color TitleBarButtonPressedTextColorOverride4 { set; }
            void CreateSplashScreenEvents(/* _GUID const &,void** */);
        }
    }
}
