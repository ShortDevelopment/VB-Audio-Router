using Microsoft.Toolkit.Forms.UI.XamlHost;
using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.Xaml;
using Application = System.Windows.Forms.Application;

namespace VBAudioRouter.Host
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            XamlWindow<WelcomePage> window = new();
            window.Show();
            using (new App())
            {
                window.InitializeXamlContent();
                Application.Run(window);
            }
        }
    }

    class XamlWindow<T> : Form where T : UIElement
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
