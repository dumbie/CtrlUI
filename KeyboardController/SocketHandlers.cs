using ArnoldVinkCode;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static KeyboardController.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace KeyboardController
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
                //Debug.WriteLine("Received socket from (C): " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort);

                //Check what kind of object was received
                if (DeserializedBytes.Object is ControllerInput)
                {
                    if (!vControllerBusy)
                    {
                        vControllerBusy = true;

                        ControllerInput receivedControllerInput = (ControllerInput)DeserializedBytes.Object;
                        ControllerInteractionMouse(receivedControllerInput);
                        ControllerInteractionKeyboard(receivedControllerInput);

                        vControllerBusy = false;
                    }
                }
                else if (DeserializedBytes.Object is string)
                {
                    string receivedString = (string)DeserializedBytes.Object;
                    Debug.WriteLine("Received string: " + receivedString);
                    if (receivedString == "ApplicationExit")
                    {
                        await Application_Exit();
                    }
                    else if (receivedString == "SettingChangedColorAccentLight")
                    {
                        Settings_Load_CtrlUI(ref vConfigurationCtrlUI);
                        Settings_Load_AccentColor(vConfigurationCtrlUI);
                    }
                    else if (receivedString == "SettingChangedInterfaceSoundPackName")
                    {
                        Settings_Load_CtrlUI(ref vConfigurationCtrlUI);
                    }
                    else if (receivedString == "SettingChangedDisplayMonitor")
                    {
                        Settings_Load_CtrlUI(ref vConfigurationCtrlUI);
                        UpdateWindowPosition();
                    }
                }
            }
            catch { }
        }
    }
}