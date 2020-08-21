using ArnoldVinkCode;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace FpsOverlayer
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
                //Debug.WriteLine("Received socket from: " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort);

                //Check what kind of object was received
                if (DeserializedBytes.Object is string)
                {
                    string receivedString = (string)DeserializedBytes.Object;
                    Debug.WriteLine("Received string: " + receivedString);
                    if (receivedString == "ApplicationExit")
                    {
                        await Application_Exit();
                    }
                    else if (receivedString == "SettingChangedDisplayMonitor")
                    {
                        vConfigurationCtrlUI = Settings_Load_CtrlUI();
                        UpdateWindowPosition();
                    }
                }
                else if (DeserializedBytes.Object is KeypadSize)
                {
                    KeypadSize receivedKeypadSize = (KeypadSize)DeserializedBytes.Object;

                    //Set the window keypad margin
                    vKeypadBottomMargin = receivedKeypadSize.Height;

                    //Update the fps overlay position
                    UpdateFpsOverlayPosition(vTargetProcess.Name);
                }
            }
            catch { }
        }
    }
}