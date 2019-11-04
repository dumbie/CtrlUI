using ArnoldVinkCode;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSocketClass;
using static ArnoldVinkCode.AVClassConverters;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Handle received socket data
        public async Task ReceivedSocketHandler(TcpClient tcpClient, byte[] receivedBytes)
        {
            try
            {
                void TaskAction()
                {
                    try
                    {
                        ReceivedSocketHandlerThread(tcpClient, receivedBytes);
                    }
                    catch { }
                }
                await AVActions.TaskStart(TaskAction, null);
            }
            catch { }
        }

        void ReceivedSocketHandlerThread(TcpClient tcpClient, byte[] receivedBytes)
        {
            try
            {
                //Deserialize the received bytes
                SocketSendContainer DeserializedBytes = DeserializeBytesToClass<SocketSendContainer>(receivedBytes);

                //Get the source server ip and port
                //Debug.WriteLine("Received socket from: " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort);

                //Check what kind of object was received
                if (DeserializedBytes.Object is string[])
                {
                    string[] ReceivedStringArray = (string[])DeserializedBytes.Object;
                    //Debug.WriteLine("Received string: " + ReceivedString);

                    if (ReceivedStringArray[0] == "ControllerInfo")
                    {
                        string RequestedControllerId = ReceivedStringArray[1];

                        ControllerStatus ControllerTarget = null;
                        if (RequestedControllerId == "0")
                        {
                            ControllerTarget = vController0;
                        }
                        else if (RequestedControllerId == "1")
                        {
                            ControllerTarget = vController1;
                        }
                        else if (RequestedControllerId == "2")
                        {
                            ControllerTarget = vController2;
                        }
                        else if (RequestedControllerId == "3")
                        {
                            ControllerTarget = vController3;
                        }

                        //Gather controller status
                        ControllerStatusSend sendInfo = new ControllerStatusSend();
                        sendInfo.NumberId = Convert.ToInt32(RequestedControllerId);
                        sendInfo.Manage = ControllerTarget.Manage;
                        sendInfo.Connected = ControllerTarget.Connected != null;
                        sendInfo.BatteryPercentageCurrent = ControllerTarget.BatteryPercentageCurrent;

                        //Prepare socket data
                        SocketSendContainer socketSend = new SocketSendContainer();
                        socketSend.SourceIp = vSocketServer.vTcpListenerIp;
                        socketSend.SourcePort = vSocketServer.vTcpListenerPort;
                        socketSend.Object = sendInfo;
                        byte[] SendBytes = SerializeObjectToBytes(socketSend);

                        //Send socket data
                        TcpClient socketClient = vSocketClient.SocketClientCheck(DeserializedBytes.SourceIp, DeserializedBytes.SourcePort, vSocketServer.vTcpListenerTimeout);
                        vSocketClient.SocketClientSendBytes(socketClient, SendBytes, vSocketServer.vTcpListenerTimeout, false);
                    }
                }
            }
            catch { }
        }
    }
}