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
                //Debug.WriteLine("Received socket from: " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort);

                //Check what kind of object was received
                if (deserializedBytes.Object is string)
                {
                    string receivedString = (string)deserializedBytes.Object;
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
                else if (deserializedBytes.Object is KeypadSize)
                {
                    KeypadSize receivedKeypadSize = (KeypadSize)deserializedBytes.Object;

                    //Set the window keypad margin
                    vKeypadBottomMargin = receivedKeypadSize.Height;

                    //Update the fps overlay position
                    await UpdateFpsOverlayPosition(vTargetProcess.Name);
                }
            }
            catch { }
        }
    }
}