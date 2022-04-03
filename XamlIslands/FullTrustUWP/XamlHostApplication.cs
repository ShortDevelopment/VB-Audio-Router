#if NETCOREAPP3_1
using Microsoft.Toolkit.Forms.UI.XamlHost;
using Microsoft.Toolkit.Win32.UI.XamlHost;
using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Application = System.Windows.Forms.Application;

namespace FullTrustUWP
{
    // https://github.com/CommunityToolkit/Microsoft.Toolkit.Win32/blob/master/Microsoft.Toolkit.Win32.UI.XamlApplication/XamlApplication.cpp

    public class XamlHostApplication<TApp> where TApp : XamlApplication
    {
        static XamlHostApplication()
        {
            try
            {
                Application.SetCompatibleTextRenderingDefault(false);
            }
            catch { }
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
        }

        public static void Run<TUiElement>() where TUiElement : UIElement => Run<TUiElement>(new());

        public static void Run<T>(XamlWindow<T> window) where T : UIElement
        {
            window.Show();
            using (TApp app = Activator.CreateInstance<TApp>())
            {
                window.InitializeXamlContent();
                Application.Run(window);
            }
        }
    }

    public class XamlWindow<T> : Form where T : UIElement
    {
        public WindowsXamlHost XamlHost { get; private set; }
        public void InitializeXamlContent()
        {
            this.SuspendLayout();

            XamlHost = new();
            XamlHost.Dock = DockStyle.Fill;
            XamlHost.Child = Activator.CreateInstance<T>();
            this.Controls.Add(XamlHost);

            this.ResumeLayout(false);
            this.PerformLayout();

            SplashScreenVisible = false;
            this.Refresh();
        }

        #region SplashScreen
        public bool SplashScreenVisible { get; private set; } = true;
        public void ShowSplashScreen()
        {
            SplashScreenVisible = true;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (SplashScreenVisible)
                using (Graphics g = e.Graphics)
                {
                    g.Clear(Color.Red);
                }
            else
                base.OnPaint(e);
        }
        #endregion
    }
}

#endif