using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVClassConverters;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.ControllerTimings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if controller output needs to be forwarded
        async Task<bool> ControllerOutputApps(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Activated && !Controller.Disconnecting)
                {
                    //Check if a popup is visible
                    if (vWindowKeyboard.vWindowVisible)
                    {
                        vWindowKeyboard.ControllerInteractionMouse(Controller.InputCurrent);
                        await vWindowKeyboard.ControllerInteractionKeyboard(Controller.InputCurrent);
                        return true;
                    }
                    else if (vWindowKeypad.vWindowVisible)
                    {
                        vWindowKeypad.ControllerInteractionKeypadPreview(Controller.InputCurrent);
                        vWindowKeypad.ControllerInteractionMouse(Controller.InputCurrent);
                        vWindowKeypad.ControllerInteractionKeyboard(Controller.InputCurrent);
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
                    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort - 1);
                    await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);

                    //Update delay time
                    Controller.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks10;
                }
            }
            catch { }
        }
    }
}