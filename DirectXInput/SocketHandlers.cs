using ArnoldVinkCode;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.MainColors;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Handle received socket data
        public void ReceivedSocketHandler(TcpClient tcpClient, UdpEndPointDetails endPoint, byte[] receivedBytes)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        if (tcpClient != null)
                        {
                            //await ReceivedTcpSocketHandlerThread(tcpClient, receivedBytes);
                        }
                        else
                        {
                            await ReceivedUdpSocketHandlerThread(endPoint, receivedBytes);
                        }
                    }
                    catch { }
                }
                AVActions.TaskStartBackground(TaskAction);
            }
            catch { }
        }

        async Task ReceivedUdpSocketHandlerThread(UdpEndPointDetails endPoint, byte[] receivedBytes)
        {
            try
            {
                //Get the source server ip and port
                //Debug.WriteLine("Received udp socket from: " + endPoint.IPEndPoint.Address.ToString() + ":" + endPoint.IPEndPoint.Port);

                //Deserialize the received bytes
                if (DeserializeBytesToObject(receivedBytes, out SocketSendContainer deserializedBytes))
                {
                    //Check what kind of object was received
                    if (deserializedBytes.Object is NotificationDetails)
                    {
                        NotificationDetails receivedNotificationDetails = (NotificationDetails)deserializedBytes.Object;
                        App.vWindowOverlay.Notification_Show_Status(receivedNotificationDetails);
                    }
                    else if (deserializedBytes.Object is string)
                    {
                        string receivedString = (string)deserializedBytes.Object;
                        Debug.WriteLine("Received socket string: " + receivedString);
                        if (receivedString == "SettingChangedColorAccentLight")
                        {
                            vConfigurationCtrlUI = SettingLoadConfig("CtrlUI.exe.csettings");

                            //Change application accent color
                            string colorLightHex = SettingLoad(vConfigurationCtrlUI, "ColorAccentLight", typeof(string));
                            ChangeApplicationAccentColor(colorLightHex);
                            vApplicationAccentLightBrush = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];
                        }
                        else if (receivedString == "SettingChangedInterfaceSoundPackName")
                        {
                            vConfigurationCtrlUI = SettingLoadConfig("CtrlUI.exe.csettings");
                        }
                        else if (receivedString == "SettingChangedInterfaceClockStyleName")
                        {
                            vConfigurationCtrlUI = SettingLoadConfig("CtrlUI.exe.csettings");
                            App.vWindowKeyboard.UpdateClockStyle();
                        }
                        else if (receivedString == "SettingChangedDisplayMonitor")
                        {
                            vConfigurationCtrlUI = SettingLoadConfig("CtrlUI.exe.csettings");
                            App.vWindowOverlay.UpdateWindowPosition();
                            App.vWindowKeyboard.UpdateWindowPosition();
                            App.vWindowKeypad.UpdateWindowPosition();
                        }
                        else if (receivedString == "ControllerStatusSummaryList")
                        {
                            await SendControllerStatusDetailsList(deserializedBytes);
                        }
                        else if (receivedString == "KeyboardHideShow")
                        {
                            await KeyboardPopupHideShow(false, false);
                        }
                        else if (receivedString == "KeyboardShow")
                        {
                            await KeyboardPopupHideShow(true, true);
                        }
                    }
                }
                else
                {
                    //Check incoming gyro dsu bytes
                    await GyroDsuClientHandler(endPoint, receivedBytes);
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
                controllerStatus0.Activated = vController0.Activated;
                controllerStatus0.Connected = vController0.Connected();
                controllerStatus0.BatteryCurrent = vController0.BatteryCurrent;
                controllerStatusDetailsList.Add(controllerStatus0);

                ControllerStatusDetails controllerStatus1 = new ControllerStatusDetails(vController1.NumberId);
                controllerStatus1.Activated = vController1.Activated;
                controllerStatus1.Connected = vController1.Connected();
                controllerStatus1.BatteryCurrent = vController1.BatteryCurrent;
                controllerStatusDetailsList.Add(controllerStatus1);

                ControllerStatusDetails controllerStatus2 = new ControllerStatusDetails(vController2.NumberId);
                controllerStatus2.Activated = vController2.Activated;
                controllerStatus2.Connected = vController2.Connected();
                controllerStatus2.BatteryCurrent = vController2.BatteryCurrent;
                controllerStatusDetailsList.Add(controllerStatus2);

                ControllerStatusDetails controllerStatus3 = new ControllerStatusDetails(vController3.NumberId);
                controllerStatus3.Activated = vController3.Activated;
                controllerStatus3.Connected = vController3.Connected();
                controllerStatus3.BatteryCurrent = vController3.BatteryCurrent;
                controllerStatusDetailsList.Add(controllerStatus3);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = controllerStatusDetailsList;
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(deserializedBytes.SourceIp), deserializedBytes.SourcePort);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }
    }
}