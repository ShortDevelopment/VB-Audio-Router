namespace FullTrustUWP.Core.Xaml
{
    public sealed class XamlWindowConfig
    {
        public XamlWindowConfig(string title)
            => this.Title = title;

        public string Title { get; }
        public bool TransparentBackground { get; set; } = true;
        public bool HasWin32Frame { get; set; } = true;
        public bool HasWin32TitleBar { get; set; } = true;
        public bool TopMost { get; set; } = false;
    }
}
