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
                    await CtrlUI_Show();
                }
            }
            catch { }
        }

        //Show CtrlUI
        private static async Task CtrlUI_Show()
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
                vWindowOverlay.Notification_Show_Status(notificationDetails);

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
                    await FpsOverlayer_ShowHide();
                }
            }
            catch { }
        }

        //Show or hide Fps Overlayer
        private static async Task FpsOverlayer_ShowHide()
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
                vWindowOverlay.Notification_Show_Status(notificationDetails);

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
                    vWindowOverlay.Notification_Show_Status(notificationFps);
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
                vWindowOverlay.Notification_Show_Status(notificationDetails);

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

        //Show or hide Fps Tools
        public static async Task FpsOverlayer_ShowHideTools()
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
                    vWindowOverlay.Notification_Show_Status(notificationFps);
                    Debug.WriteLine("Fps overlayer is not running.");
                    return;
                }

                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Show or hiding tools overlay");

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Tools";
                notificationDetails.Text = "Show or hiding tools overlay";
                vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.Object = "SwitchToolsOverlayVisibility";
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
                    vWindowOverlay.Notification_Show_Status(notificationFps);
                    Debug.WriteLine("Fps overlayer is not running.");
                    return;
                }

                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Show or hiding crosshair overlay");

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Crosshair";
                notificationDetails.Text = "Show or hiding crosshair overlay";
                vWindowOverlay.Notification_Show_Status(notificationDetails);

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

        //Screen Capture Tool capture image
        public static async Task ScreenCaptureToolCaptureImage()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Signal screen capture tool to capture image.");

                //Prepare socket data
                string socketSend = "CaptureImage";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                int socketServerPort = SettingLoad(vConfigurationCtrlUI, "ServerPort", typeof(int)) + 3;
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), socketServerPort);
                await vArnoldVinkSockets.UdpClientSendBytesBroadcast(true, socketServerPort, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Screen Capture Tool capture video
        public static async Task ScreenCaptureToolCaptureVideo()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                Debug.WriteLine("Signal screen capture tool to capture video.");

                //Prepare socket data
                string socketSend = "CaptureVideo";
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                int socketServerPort = SettingLoad(vConfigurationCtrlUI, "ServerPort", typeof(int)) + 3;
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), socketServerPort);
                await vArnoldVinkSockets.UdpClientSendBytesBroadcast(true, socketServerPort, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }
    }
}