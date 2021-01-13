using ArnoldVinkCode;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle received socket data
        public async Task ReceivedSocketHandler(TcpClient tcpClient, UdpEndPointDetails endPoint, byte[] receivedBytes)
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
                await AVActions.TaskStart(TaskAction);
            }
            catch { }
        }

        async Task ReceivedSocketHandlerThread(TcpClient tcpClient, byte[] receivedBytes)
        {
            try
            {
                //Deserialize the received bytes
                if (!DeserializeBytesToObject(receivedBytes, out SocketSendContainer deserializedBytes)) { return; }

                //Get the source server ip and port
                //Debug.WriteLine("Received socket from (C): " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort + "/" + DeserializedBytes.Object);

                //Check what kind of object was received
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
                    await UpdateControllerStatus(controllerStatusSummaryList);
                }
                else if (deserializedBytes.Object is string)
                {
                    string receivedString = (string)deserializedBytes.Object;
                    //Debug.WriteLine("Received string: " + receivedString);
                    if (receivedString == "SettingChangedShortcut")
                    {
                        vConfigurationDirectXInput = Settings_Load_DirectXInput();
                        UpdateControllerHelp();
                    }
                    else if (receivedString == "SettingChangedControllerColor")
                    {
                        vConfigurationDirectXInput = Settings_Load_DirectXInput();
                        UpdateControllerColor();
                    }
                    else if (receivedString == "AppWindowHideShow")
                    {
                        await AVActions.ActionDispatcherInvokeAsync(async delegate { await AppWindow_HideShow(); });
                    }
                }
            }
            catch { }
        }
    }
}