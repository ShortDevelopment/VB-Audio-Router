using System.Windows.Forms;
using VBAudioRouter.Worker;
using VBAudioRouter.Worker.Registration;

if (args.Length != 0)
    return;

using (ExeServer server = new())
{
    server.RegisterClass<DesktopBridge>(typeof(DesktopBridge).GUID);
    Application.Run();
}