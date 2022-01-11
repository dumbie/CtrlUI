using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Notify - DirectXInput setting changed
        async Task NotifyDirectXInputSettingChanged(string settingName)
        {
            try
            {
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
                socketSend.Object = "SettingChanged" + settingName;
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vSocketServerIp, vArnoldVinkSockets.vSocketServerPort + 1, vArnoldVinkSockets.vSocketTimeout);
                await vArnoldVinkSockets.TcpClientSendBytesServer(tcpClient, SerializedData, vArnoldVinkSockets.vSocketTimeout, false);
            }
            catch { }
        }

        //Notify - Fps Overlayer setting changed
        async Task NotifyFpsOverlayerSettingChanged(string settingName)
        {
            try
            {
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
                socketSend.Object = "SettingChanged" + settingName;
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vSocketServerIp, vArnoldVinkSockets.vSocketServerPort + 2, vArnoldVinkSockets.vSocketTimeout);
                await vArnoldVinkSockets.TcpClientSendBytesServer(tcpClient, SerializedData, vArnoldVinkSockets.vSocketTimeout, false);
            }
            catch { }
        }
    }
}