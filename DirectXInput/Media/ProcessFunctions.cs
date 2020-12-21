using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;

namespace DirectXInput.MediaCode
{
    partial class WindowMedia
    {
        //Launch or close the Fps Overlayer
        async Task LaunchCloseFpsOverlayer()
        {
            try
            {
                if (CheckRunningProcessByNameOrTitle("FpsOverlayer", false))
                {
                    //Close the Fps Overlayer
                    await CloseFpsOverlayer();
                }
                else
                {
                    //Launch the Fps Overlayer
                    await LaunchFpsOverlayer(true);
                }
            }
            catch { }
        }

        //Close the Fps Overlayer
        async Task CloseFpsOverlayer()
        {
            try
            {
                App.vWindowOverlay.Notification_Show_Status("Fps", "Hiding Fps Overlayer");
                Debug.WriteLine("Closing Fps Overlayer");

                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "ApplicationExit";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vSocketServerIp, vArnoldVinkSockets.vSocketServerPort + 2, vArnoldVinkSockets.vSocketTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vSocketTimeout, false);
            }
            catch { }
        }

        //Launch the Fps Overlayer
        async Task LaunchFpsOverlayer(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !CheckRunningProcessByNameOrTitle("FpsOverlayer", false))
                {
                    App.vWindowOverlay.Notification_Show_Status("Fps", "Showing Fps Overlayer");
                    Debug.WriteLine("Showing Fps Overlayer");
                    await ProcessLauncherWin32Async("FpsOverlayer-Admin.exe", "", "", true, false);
                }
            }
            catch { }
        }
    }
}