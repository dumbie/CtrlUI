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
        //Check if controller output needs to be forwarded
        async Task<bool> ControllerOutput(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Activated && !Controller.BlockOutput)
                {
                    //Check if keyboard is visible
                    if (App.vWindowKeyboard.vWindowVisible)
                    {
                        await App.vWindowKeyboard.ControllerInteractionMouse(Controller.InputCurrent);
                        await App.vWindowKeyboard.ControllerInteractionKeyboard(Controller.InputCurrent);
                        return true;
                    }
                    else if (App.vWindowKeypad.vWindowVisible)
                    {
                        App.vWindowKeypad.ControllerInteractionKeyboard(Controller.InputCurrent);
                        return true;
                    }
                    else if (vProcessCtrlUI != null && vProcessCtrlUIActivated)
                    {
                        await OutputAppCtrlUI(Controller);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

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
                    Controller.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayNanoTicks;
                }
            }
            catch { }
        }
    }
}