using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class ToolFunctions
    {
        //Launch or show CtrlUI
        public static async Task CtrlUI_LaunchShow()
        {
            try
            {
                if (!Check_RunningProcessByName("CtrlUI", true))
                {
                    ProcessLaunch.LaunchCtrlUI(true);
                }
                else
                {
                    await ShowCtrlUI();
                }
            }
            catch { }
        }

        //Show CtrlUI
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

        //Launch or show or hide Fps Overlayer
        public static async Task FpsOverlayer_LaunchShowHide()
        {
            try
            {
                if (!Check_RunningProcessByName("FpsOverlayer", true))
                {
                    ProcessLaunch.LaunchFpsOverlayer(true);
                }
                else
                {
                    await ShowHideFpsOverlayer();
                }
            }
            catch { }
        }

        //Show or hide Fps Overlayer
        private static async Task ShowHideFpsOverlayer()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Show or hiding Fps Overlayer");

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Fps";
                notificationDetails.Text = "Show or hiding Fps Overlayer";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "SwitchFpsOverlayVisibility";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort + 1);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Move Fps Overlayer position
        public static async Task FpsOverlayer_ChangePosition()
        {
            try
            {
                //Check if fps overlayer is running
                if (!Check_RunningProcessByName("FpsOverlayer", true))
                {
                    //Show notification
                    NotificationDetails notificationFps = new NotificationDetails();
                    notificationFps.Icon = "Fps";
                    notificationFps.Text = "Fps Overlayer is not running";
                    App.vWindowOverlay.Notification_Show_Status(notificationFps);
                    Debug.WriteLine("Fps overlayer is not running.");
                    return;
                }

                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Changing Fps Overlayer position");

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Fps";
                notificationDetails.Text = "Changing Fps Overlayer position";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "ChangeFpsOverlayPosition";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort + 1);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Show or hide Fps Browser
        public static async Task FpsOverlayer_ShowHideBrowser()
        {
            try
            {
                //Check if fps overlayer is running
                if (!Check_RunningProcessByName("FpsOverlayer", true))
                {
                    //Show notification
                    NotificationDetails notificationFps = new NotificationDetails();
                    notificationFps.Icon = "Fps";
                    notificationFps.Text = "Fps Overlayer is not running";
                    App.vWindowOverlay.Notification_Show_Status(notificationFps);
                    Debug.WriteLine("Fps overlayer is not running.");
                    return;
                }

                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Show or hiding browser overlay");

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Browser";
                notificationDetails.Text = "Show or hiding browser overlay";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "SwitchBrowserOverlayVisibility";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort + 1);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Show or hide Fps Crosshair
        public static async Task FpsOverlayer_ShowHideCrosshair()
        {
            try
            {
                //Check if fps overlayer is running
                if (!Check_RunningProcessByName("FpsOverlayer", true))
                {
                    //Show notification
                    NotificationDetails notificationFps = new NotificationDetails();
                    notificationFps.Icon = "Fps";
                    notificationFps.Text = "Fps Overlayer is not running";
                    App.vWindowOverlay.Notification_Show_Status(notificationFps);
                    Debug.WriteLine("Fps overlayer is not running.");
                    return;
                }

                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Show or hiding browser overlay");

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Crosshair";
                notificationDetails.Text = "Show or hiding crosshair overlay";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "SwitchCrosshairOverlayVisibility";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), vArnoldVinkSockets.vSocketServerPort + 1);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Screen Capture Tool take screenshot
        public static async Task ScreenCaptureToolTakeScreenshot()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Signal screen capture tool to take screenshot.");

                //Prepare socket data
                string socketSend = "TakeScreenshot";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                int SocketServerPort = SettingLoad(vConfigurationDirectXInput, "ServerPortScreenCaptureTool", typeof(int));
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), SocketServerPort);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }
    }
}