using ArnoldVinkCode;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

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
                SocketSendContainer deserializedBytes = DeserializeBytesToClass<SocketSendContainer>(receivedBytes);

                //Get the source server ip and port
                //Debug.WriteLine("Received socket from: " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort);

                //Check what kind of object was received
                if (deserializedBytes.Object is NotificationDetails)
                {
                    NotificationDetails receivedNotificationDetails = (NotificationDetails)deserializedBytes.Object;
                    App.vWindowOverlay.Notification_Show_Status(receivedNotificationDetails.Icon, receivedNotificationDetails.Text);
                }
                else if (deserializedBytes.Object is string)
                {
                    string receivedString = (string)deserializedBytes.Object;
                    //Debug.WriteLine("Received string: " + receivedString);
                    if (receivedString == "SettingChangedAccentColor")
                    {
                        Settings_Load_CtrlUI(ref vConfigurationCtrlUI);
                        Settings_Load_AccentColor(vConfigurationCtrlUI);
                    }
                    else if (receivedString == "SettingChangedDisplayMonitor")
                    {
                        Settings_Load_CtrlUI(ref vConfigurationCtrlUI);
                        App.vWindowOverlay.UpdateWindowPosition();
                    }
                    else if (receivedString == "SettingChangedTextPosition")
                    {
                        Settings_Load_FpsOverlayer(ref vConfigurationFpsOverlayer);
                        App.vWindowOverlay.UpdateNotificationPosition();
                        App.vWindowOverlay.UpdateBatteryPosition();
                    }
                    else if (receivedString == "ControllerStatusSummaryList")
                    {
                        await SendControllerStatusDetailsList(deserializedBytes);
                    }
                }
            }
            catch { }
        }

        async Task SendControllerStatusDetailsList(SocketSendContainer deserializedBytes)
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                //List controller status
                List<ControllerStatusDetails> controllerStatusDetailsList = new List<ControllerStatusDetails>();

                //Gather controller status
                ControllerStatusDetails controllerStatus0 = new ControllerStatusDetails(vController0.NumberId);
                controllerStatus0.Manage = vController0.Manage;
                controllerStatus0.Connected = vController0.Connected();
                controllerStatus0.BatteryPercentageCurrent = vController0.BatteryPercentageCurrent;
                controllerStatusDetailsList.Add(controllerStatus0);

                ControllerStatusDetails controllerStatus1 = new ControllerStatusDetails(vController1.NumberId);
                controllerStatus1.Manage = vController1.Manage;
                controllerStatus1.Connected = vController1.Connected();
                controllerStatus1.BatteryPercentageCurrent = vController1.BatteryPercentageCurrent;
                controllerStatusDetailsList.Add(controllerStatus1);

                ControllerStatusDetails controllerStatus2 = new ControllerStatusDetails(vController2.NumberId);
                controllerStatus2.Manage = vController2.Manage;
                controllerStatus2.Connected = vController2.Connected();
                controllerStatus2.BatteryPercentageCurrent = vController2.BatteryPercentageCurrent;
                controllerStatusDetailsList.Add(controllerStatus2);

                ControllerStatusDetails controllerStatus3 = new ControllerStatusDetails(vController3.NumberId);
                controllerStatus3.Manage = vController3.Manage;
                controllerStatus3.Connected = vController3.Connected();
                controllerStatus3.BatteryPercentageCurrent = vController3.BatteryPercentageCurrent;
                controllerStatusDetailsList.Add(controllerStatus3);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
                socketSend.Object = controllerStatusDetailsList;
                byte[] SendBytes = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient socketClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(deserializedBytes.SourceIp, deserializedBytes.SourcePort, vArnoldVinkSockets.vTcpClientTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(socketClient, SendBytes, vArnoldVinkSockets.vTcpClientTimeout, false);
            }
            catch { }
        }
    }
}