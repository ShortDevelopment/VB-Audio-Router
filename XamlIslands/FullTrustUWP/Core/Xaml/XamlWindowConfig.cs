using Windows.UI;
using Windows.UI.Xaml.Media;

namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlWindowConfig
    {
        public XamlWindowConfig(string title)
            => this.Title = title;

        public string Title { get; private set; } = "";
        public bool TransparentBackground { get; set; } = true;
        public bool HasWin32Frame { get; set; } = true;
        public bool TopMost { get; set; } = false;

        public int SplashScreenTime { get; set; } = 1_000;
        public Color SplashScreenBackground { get; set; } = Color.FromArgb(255, 58, 57, 55);
        public ImageSource? SplashScreenImage { get; set; }
    }
}
