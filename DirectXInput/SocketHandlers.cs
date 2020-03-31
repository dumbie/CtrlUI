using ArnoldVinkCode;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
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
                async void TaskAction()
                {
                    try
                    {
                        await ReceivedSocketHandlerThread(tcpClient, receivedBytes);
                    }
                    catch { }
                }
                await AVActions.TaskStart(TaskAction, null);
            }
            catch { }
        }

        async Task ReceivedSocketHandlerThread(TcpClient tcpClient, byte[] receivedBytes)
        {
            try
            {
                //Deserialize the received bytes
                SocketSendContainer DeserializedBytes = DeserializeBytesToClass<SocketSendContainer>(receivedBytes);

                //Get the source server ip and port
                //Debug.WriteLine("Received socket from: " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort);

                //Check what kind of object was received
                if (DeserializedBytes.Object is string)
                {
                    string ReceivedString = (string)DeserializedBytes.Object;
                    //Debug.WriteLine("Received string: " + ReceivedString);

                    if (ReceivedString == "ControllerStatusSummaryList")
                    {
                        //List controller status
                        List<ControllerStatusSummary> controllerStatusSummaryList = new List<ControllerStatusSummary>();

                        //Gather controller status
                        ControllerStatusSummary controllerStatus0 = new ControllerStatusSummary(vController0.NumberId);
                        controllerStatus0.Manage = vController0.Manage;
                        controllerStatus0.Connected = vController0.Connected();
                        controllerStatus0.BatteryPercentageCurrent = vController0.BatteryPercentageCurrent;
                        controllerStatusSummaryList.Add(controllerStatus0);

                        ControllerStatusSummary controllerStatus1 = new ControllerStatusSummary(vController1.NumberId);
                        controllerStatus1.Manage = vController1.Manage;
                        controllerStatus1.Connected = vController1.Connected();
                        controllerStatus1.BatteryPercentageCurrent = vController1.BatteryPercentageCurrent;
                        controllerStatusSummaryList.Add(controllerStatus1);

                        ControllerStatusSummary controllerStatus2 = new ControllerStatusSummary(vController2.NumberId);
                        controllerStatus2.Manage = vController2.Manage;
                        controllerStatus2.Connected = vController2.Connected();
                        controllerStatus2.BatteryPercentageCurrent = vController2.BatteryPercentageCurrent;
                        controllerStatusSummaryList.Add(controllerStatus2);

                        ControllerStatusSummary controllerStatus3 = new ControllerStatusSummary(vController3.NumberId);
                        controllerStatus3.Manage = vController3.Manage;
                        controllerStatus3.Connected = vController3.Connected();
                        controllerStatus3.BatteryPercentageCurrent = vController3.BatteryPercentageCurrent;
                        controllerStatusSummaryList.Add(controllerStatus3);

                        //Prepare socket data
                        SocketSendContainer socketSend = new SocketSendContainer();
                        socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                        socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
                        socketSend.Object = controllerStatusSummaryList;
                        byte[] SendBytes = SerializeObjectToBytes(socketSend);

                        //Send socket data
                        TcpClient socketClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(DeserializedBytes.SourceIp, DeserializedBytes.SourcePort, vArnoldVinkSockets.vTcpClientTimeout);
                        await vArnoldVinkSockets.TcpClientSendBytes(socketClient, SendBytes, vArnoldVinkSockets.vTcpClientTimeout, false);
                    }
                }
            }
            catch { }
        }
    }
}