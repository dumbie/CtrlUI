using ArnoldVinkCode;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
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
                    if (deserializedBytes.Object is ControllerInput)
                    {
                        if (!vControllerBusy)
                        {
                            vControllerBusy = true;

                            ControllerInput receivedControllerInput = (ControllerInput)deserializedBytes.Object;
                            await ControllerInteraction(receivedControllerInput);

                            vControllerBusy = false;
                        }
                    }
                    else if (deserializedBytes.Object is List<ControllerStatusDetails>)
                    {
                        List<ControllerStatusDetails> controllerStatusSummaryList = (List<ControllerStatusDetails>)deserializedBytes.Object;
                        UpdateControllerStatus(controllerStatusSummaryList);
                    }
                    else if (deserializedBytes.Object is string)
                    {
                        string receivedString = (string)deserializedBytes.Object;
                        Debug.WriteLine("Received socket string: " + receivedString);
                        if (receivedString == "SettingChangedShortcut")
                        {
                            vConfigurationDirectXInput = SettingLoadConfig("DirectXInput.exe.csettings");
                            UpdateControllerHelp();
                        }
                        else if (receivedString == "SettingChangedControllerColor")
                        {
                            vConfigurationDirectXInput = SettingLoadConfig("DirectXInput.exe.csettings");
                            UpdateControllerColor();
                        }
                        else if (receivedString == "AppWindowHideShow")
                        {
                            await AVActions.DispatcherInvoke(async delegate { await AppWindow_HideShow(); });
                        }
                    }
                }
            }
            catch { }
        }
    }
}