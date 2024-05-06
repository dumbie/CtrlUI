using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVSettings;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    public partial class SocketServer
    {
        //Enable the socket server
        public static async Task EnableSocketServer()
        {
            try
            {
                int SocketServerPort = SettingLoad(vConfiguration, "ServerPort", typeof(int));
                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort, false, true);
                vArnoldVinkSockets.vSocketTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
                await vArnoldVinkSockets.SocketServerEnable();
            }
            catch { }
        }

        //Enable the pipes server
        public static void EnablePipesServer()
        {
            try
            {
                vArnoldVinkPipes = new ArnoldVinkPipes("ScreenCaptureTool");
                vArnoldVinkPipes.PipeServerEnable();
                vArnoldVinkPipes.EventStringReceived += ReceivedPipesHandler;
            }
            catch { }
        }
    }
}