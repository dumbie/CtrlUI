using ArnoldVinkCode;
using System.Diagnostics;
using static ArnoldVinkCode.AVProcess;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class ProcessLaunch
    {
        //Launch CtrlUI
        public static void LaunchCtrlUI(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !Check_RunningProcessByName("CtrlUI", true))
                {
                    Debug.WriteLine("Launching CtrlUI.");

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "Launching CtrlUI";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch CtrlUI
                    AVProcess.Launch_ShellExecute("CtrlUI-Launcher.exe", "", "", true);
                }
            }
            catch { }
        }

        //Launch Fps Overlayer
        public static void LaunchFpsOverlayer(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !Check_RunningProcessByName("FpsOverlayer", true))
                {
                    Debug.WriteLine("Launching Fps Overlayer");

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Fps";
                    notificationDetails.Text = "Launching Fps Overlayer";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch Fps Overlayer
                    AVProcess.Launch_ShellExecute("FpsOverlayer-Launcher.exe", "", "", true);
                }
            }
            catch { }
        }

        //Launch Screen Capture Tool
        public static void LaunchScreenCaptureTool(bool forceLaunch)
        {
            try
            {
                if (forceLaunch || !Check_RunningProcessByName("ScreenCaptureTool", true))
                {
                    Debug.WriteLine("Launching Screen Capture Tool");

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Screenshot";
                    notificationDetails.Text = "Launching Screen Capture Tool";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch Fps Overlayer
                    AVProcess.Launch_ShellExecute("ScreenCaptureTool.exe", "", "", true);
                }
            }
            catch { }
        }
    }
}