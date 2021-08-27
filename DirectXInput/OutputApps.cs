using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVClassConverters;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if controller output needs to be forwarded
        async Task<bool> ControllerOutputApps(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Activated && !Controller.BlockInteraction)
                {
                    //Check if a popup is visible
                    if (App.vWindowKeyboard.vWindowVisible)
                    {
                        App.vWindowKeyboard.ControllerInteractionMouse(Controller.InputCurrent);
                        await App.vWindowKeyboard.ControllerInteractionKeyboard(Controller.InputCurrent);
                        return true;
                    }
                    else if (App.vWindowKeypad.vWindowVisible)
                    {
                        await App.vWindowKeypad.ControllerInteractionKeypadUpdate();
                        App.vWindowKeypad.ControllerInteractionMouse(Controller.InputCurrent);
                        App.vWindowKeypad.ControllerInteractionKeyboard(Controller.InputCurrent);
                        return true;
                    }
                    else if (App.vWindowMedia.vWindowVisible)
                    {
                        App.vWindowMedia.ControllerInteractionMouse(Controller.InputCurrent);
                        await App.vWindowMedia.ControllerInteractionKeyboard(Controller.InputCurrent);
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
                if (GetSystemTicksMs() >= Controller.Delay_CtrlUIOutput)
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
                    socketSend.Object = Controller.InputCurrent;
                    byte[] SerializedData = SerializeObjectToBytes(socketSend);

                    //Send socket data
                    TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vSocketServerIp, vArnoldVinkSockets.vSocketServerPort - 1, vArnoldVinkSockets.vSocketTimeout);
                    await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vSocketTimeout, false);

                    //Update delay time
                    Controller.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayNanoTicks;
                }
            }
            catch { }
        }
    }
}