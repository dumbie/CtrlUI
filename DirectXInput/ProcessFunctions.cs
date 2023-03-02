using ArnoldVinkCode;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVProcess;
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
                if (!Check_RunningProcessByName("CtrlUI", true))
                {
                    LaunchCtrlUI(true);
                }
                else
                {
                    await ShowCtrlUI();
                }
            }
            catch { }
        }

        private static void LaunchCtrlUI(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !Check_RunningProcessByName("CtrlUI", true))
                {
                    Debug.WriteLine("Launching CtrlUI.");

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "AppLaunch";
                    notificationDetails.Text = "Launching CtrlUI";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch CtrlUI
                    AVProcessTool.Launch_Exe("CtrlUI-Launcher.exe", "", "", false, true, false);
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
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "AppWindowHideShow";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort - 1);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Launch or close the Fps Overlayer
        public static async Task LaunchCloseFpsOverlayer()
        {
            try
            {
                if (Check_RunningProcessByName("FpsOverlayer", true))
                {
                    await CloseFpsOverlayer();
                }
                else
                {
                    LaunchFpsOverlayer(true);
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
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "ApplicationExit";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort + 1);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Launch the Fps Overlayer
        public static void LaunchFpsOverlayer(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !Check_RunningProcessByName("FpsOverlayer", true))
                {
                    Debug.WriteLine("Showing Fps Overlayer");

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Fps";
                    notificationDetails.Text = "Showing Fps Overlayer";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch Fps Overlayer
                    AVProcessTool.Launch_Exe("FpsOverlayer-Launcher.exe", "", "", false, true, false);
                }
            }
            catch { }
        }
    }
}