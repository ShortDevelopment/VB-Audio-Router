using System.Windows.Forms;
using VBAudioRouter.Worker;
using VBAudioRouter.Worker.Registration;

using (ExeServer server = new())
{
    server.RegisterClass<DesktopBridge>(typeof(DesktopBridge).GUID);    
    Application.Run();
}