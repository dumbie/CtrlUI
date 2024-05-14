using ArnoldVinkCode;
using System.Diagnostics;
using static ArnoldVinkCode.AVProcess;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

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
                    vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch application
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
                    vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Launch application
                    AVProcess.Launch_ShellExecute("FpsOverlayer-Launcher.exe", "", "", true);
                }
            }
            catch { }
        }

        //Launch Screen Capture Tool
        public static void LaunchScreenCaptureTool(bool forceLaunch, bool skipNotification)
        {
            try
            {
                if (forceLaunch || !Check_RunningProcessByName("ScreenCaptureTool", true))
                {
                    Debug.WriteLine("Launching Screen Capture Tool");

                    //Show notification
                    if (!skipNotification)
                    {
                        NotificationDetails notificationDetails = new NotificationDetails();
                        notificationDetails.Icon = "Screenshot";
                        notificationDetails.Text = "Launching Screen Capture Tool";
                        vWindowOverlay.Notification_Show_Status(notificationDetails);
                    }

                    //Launch application
                    AVProcess.Launch_ShellExecute("ScreenCaptureTool-Launcher.exe", "", "", true);
                }
            }
            catch { }
        }

        //Launch Xbox Game Bar
        public static void LaunchXboxGameBar()
        {
            try
            {
                //Play interface sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, true);

                //Launch application
                AVProcess.Launch_UwpApplication("Microsoft.XboxGamingOverlay_8wekyb3d8bbwe!App", string.Empty);
            }
            catch { }
        }
    }
}