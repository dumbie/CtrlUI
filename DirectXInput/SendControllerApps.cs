using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Send controller output to CtrlUI
        async Task OutputAppCtrlUI(ControllerStatus Controller)
        {
            try
            {
                if (Environment.TickCount >= Controller.Delay_CtrlUIOutput)
                {
                    //Check if socket server is running
                    if (vArnoldVinkSockets == null)
                    {
                        Debug.WriteLine("The socket server is not running.");
                        return;
                    }

                    //Prepare socket data
                    SocketSendContainer socketSend = new SocketSendContainer();
                    socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                    socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
                    socketSend.Object = Controller.InputCurrent;
                    byte[] SerializedData = SerializeObjectToBytes(socketSend);

                    //Send socket data
                    TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vTcpListenerIp, vArnoldVinkSockets.vTcpListenerPort - 1, vArnoldVinkSockets.vTcpClientTimeout);
                    await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vTcpClientTimeout, false);

                    //Update delay time
                    Controller.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayPollingTicks;
                }
            }
            catch { }
        }

        //Send controller output to Keyboard Controller
        async Task OutputAppKeyboardController(ControllerStatus Controller)
        {
            try
            {
                if (Environment.TickCount >= Controller.Delay_KeyboardControllerShortcut)
                {
                    //Check if socket server is running
                    if (vArnoldVinkSockets == null)
                    {
                        Debug.WriteLine("The socket server is not running.");
                        return;
                    }

                    //Prepare socket data
                    SocketSendContainer socketSend = new SocketSendContainer();
                    socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                    socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
                    socketSend.Object = Controller.InputCurrent;
                    byte[] SerializedData = SerializeObjectToBytes(socketSend);

                    //Send socket data
                    TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vTcpListenerIp, vArnoldVinkSockets.vTcpListenerPort + 1, vArnoldVinkSockets.vTcpClientTimeout);
                    await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vTcpClientTimeout, false);

                    //Update delay time
                    Controller.Delay_KeyboardControllerShortcut = Environment.TickCount + vControllerDelayPollingTicks;
                }
            }
            catch { }
        }
    }
}