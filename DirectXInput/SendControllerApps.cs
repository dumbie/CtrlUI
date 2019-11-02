using System;
using System.Net.Sockets;
using static ArnoldVinkCode.ArnoldVinkSocketClass;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.ClassConverters;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Send controller output to CtrlUI
        void OutputAppCtrlUI(ControllerStatus Controller)
        {
            try
            {
                if (Environment.TickCount >= Controller.Delay_CtrlUIOutput || Controller.InputCurrent.ButtonGuideShort || Controller.InputCurrent.ButtonGuideLong)
                {
                    //Prepare socket data
                    SocketSendContainer socketSend = new SocketSendContainer();
                    socketSend.SourceIp = vSocketServer.vTcpListenerIp;
                    socketSend.SourcePort = vSocketServer.vTcpListenerPort;
                    socketSend.Object = Controller.InputCurrent;
                    byte[] SerializedData = SerializeObjectToBytes(socketSend);

                    //Send socket data
                    TcpClient socketClient = vSocketClient.SocketClientCheck(vSocketServer.vTcpListenerIp, vSocketServer.vTcpListenerPort - 1, vSocketServer.vTcpListenerTimeout);
                    vSocketClient.SocketClientSendBytes(socketClient, SerializedData, vSocketServer.vTcpListenerTimeout, false);

                    //Update delay time
                    Controller.Delay_CtrlUIOutput = Environment.TickCount + vControllerDelayPollingTicks;
                }
            }
            catch { }
        }

        //Send controller output to Keyboard Controller
        void OutputAppKeyboardController(ControllerStatus Controller)
        {
            try
            {
                if (Environment.TickCount >= Controller.Delay_KeyboardControllerShortcut || Controller.InputCurrent.ButtonGuideShort || Controller.InputCurrent.ButtonGuideLong)
                {
                    //Prepare socket data
                    SocketSendContainer socketSend = new SocketSendContainer();
                    socketSend.SourceIp = vSocketServer.vTcpListenerIp;
                    socketSend.SourcePort = vSocketServer.vTcpListenerPort;
                    socketSend.Object = Controller.InputCurrent;
                    byte[] SerializedData = SerializeObjectToBytes(socketSend);

                    //Send socket data
                    TcpClient socketClient = vSocketClient.SocketClientCheck(vSocketServer.vTcpListenerIp, vSocketServer.vTcpListenerPort + 1, vSocketServer.vTcpListenerTimeout);
                    vSocketClient.SocketClientSendBytes(socketClient, SerializedData, vSocketServer.vTcpListenerTimeout, false);

                    //Update delay time
                    Controller.Delay_KeyboardControllerShortcut = Environment.TickCount + vControllerDelayPollingTicks;
                }
            }
            catch { }
        }
    }
}