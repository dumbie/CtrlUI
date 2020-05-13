using ArnoldVinkCode;
using System;
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
                await AVActions.TaskStart(TaskAction);
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
                //Debug.WriteLine("Received socket from (C): " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort + "/" + DeserializedBytes.Object);

                //Check what kind of object was received
                if (DeserializedBytes.Object is ControllerInput)
                {
                    if (!vControllerBusy)
                    {
                        vControllerBusy = true;

                        ControllerInput receivedControllerInput = (ControllerInput)DeserializedBytes.Object;
                        await ControllerInteraction(receivedControllerInput);

                        vControllerBusy = false;
                    }
                }
                else if (DeserializedBytes.Object is List<ControllerStatusDetails>)
                {
                    List<ControllerStatusDetails> controllerStatusSummaryList = (List<ControllerStatusDetails>)DeserializedBytes.Object;
                    await UpdateControllerStatus(controllerStatusSummaryList);
                }
                else if (DeserializedBytes.Object is string)
                {
                    string receivedString = (string)DeserializedBytes.Object;
                    //Debug.WriteLine("Received string: " + receivedString);
                    if (receivedString == "SettingChangedShortcut")
                    {
                        Settings_Load_DirectXInput(ref vConfigurationDirectXInput);
                        UpdateControllerHelp();
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