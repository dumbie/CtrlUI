using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class ProcessFunctions
    {
        //Launch or show CtrlUI
        public static async Task LaunchShowCtrlUI()
        {
            try
            {
                if (!CheckRunningProcessByNameOrTitle("CtrlUI", false, true))
                {
                    await LaunchCtrlUI(true);
                }
                else
                {
                    await ShowCtrlUI();
                }
            }
            catch { }
        }

        private static async Task LaunchCtrlUI(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !CheckRunningProcessByNameOrTitle("CtrlUI", false, true))
                {
                    Debug.WriteLine("Launching CtrlUI.");

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "AppLaunch";
                    notificationDetails.Text = "Launching CtrlUI";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch CtrlUI
                    await ProcessLauncherWin32Async("CtrlUI-Launcher.exe", "", "", true, false);
                }
            }
            catch { }
        }

        private static async Task ShowCtrlUI()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Showing CtrlUI.");

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "AppLaunch";
                notificationDetails.Text = "Showing or hiding CtrlUI";
                await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "AppWindowHideShow";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vSocketServerIp, vArnoldVinkSockets.vSocketServerPort - 1, vArnoldVinkSockets.vSocketTimeout);
                await vArnoldVinkSockets.TcpClientSendBytesServer(tcpClient, SerializedData, vArnoldVinkSockets.vSocketTimeout, false);
            }
            catch { }
        }

        //Launch or close the Fps Overlayer
        public static async Task LaunchCloseFpsOverlayer()
        {
            try
            {
                if (CheckRunningProcessByNameOrTitle("FpsOverlayer", false, true))
                {
                    await CloseFpsOverlayer();
                }
                else
                {
                    await LaunchFpsOverlayer(true);
                }
            }
            catch { }
        }

        //Close the Fps Overlayer
        private static async Task CloseFpsOverlayer()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Hiding Fps Overlayer");

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Fps";
                notificationDetails.Text = "Hiding Fps Overlayer";
                await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "ApplicationExit";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vSocketServerIp, vArnoldVinkSockets.vSocketServerPort + 1, vArnoldVinkSockets.vSocketTimeout);
                await vArnoldVinkSockets.TcpClientSendBytesServer(tcpClient, SerializedData, vArnoldVinkSockets.vSocketTimeout, false);
            }
            catch { }
        }

        //Launch the Fps Overlayer
        public static async Task LaunchFpsOverlayer(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !CheckRunningProcessByNameOrTitle("FpsOverlayer", false, true))
                {
                    Debug.WriteLine("Showing Fps Overlayer");

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Fps";
                    notificationDetails.Text = "Showing Fps Overlayer";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch Fps Overlayer
                    await ProcessLauncherWin32Async("FpsOverlayer-Launcher.exe", "", "", true, false);
                }
            }
            catch { }
        }
    }
}