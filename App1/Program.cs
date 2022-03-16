using System.Threading.Tasks;

namespace App1
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            //var applicationFactory = InteropHelper.GetActivationFactory<IFrameworkApplicationStaticsPrivate>("Windows.UI.Xaml.Application");
            //Marshal.ThrowExceptionForHR(applicationFactory.StartInCoreWindowHostingMode(new WindowCreationParameters()
            //{
            //    Left = 10,
            //    Top = 10,
            //    Width = 100,
            //    Height = 100,
            //    TransparentBackground = true,
            //    IsCoreNavigationClient = true
            //}, (x) =>
            //{
            //    Debugger.Break();
            //    new App();
            //}));

            global::Windows.UI.Xaml.Application.Start((p) => new App());
        }
    }
}
