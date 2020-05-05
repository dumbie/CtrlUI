using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static KeyboardController.AppVariables;
using static LibraryShared.Classes;

namespace KeyboardController
{
    partial class WindowMain
    {
        //Send the notification status
        public async Task Notification_Send_Status(string targetIcon, string targetText)
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                //Create notification class
                NotificationDetails NotificationDetails = new NotificationDetails();
                NotificationDetails.Icon = targetIcon;
                NotificationDetails.Text = targetText;

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
                socketSend.Object = NotificationDetails;

                //Request controller status
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vTcpListenerIp, vArnoldVinkSockets.vTcpListenerPort - 1, vArnoldVinkSockets.vTcpClientTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vTcpClientTimeout, false);
            }
            catch { }
        }
    }
}