using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVProcess;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace DirectXInput
{
    partial class XboxGameDVR
    {
        //Show Xbox Game Bar
        public static void ShowXboxGameBar()
        {
            try
            {
                //Play interface sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, true);

                //Launch Xbox Game Bar app
                Launch_UwpApplication("Microsoft.XboxGamingOverlay_8wekyb3d8bbwe!App", string.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Xbox failed to show Xbox Game Bar: " + ex.Message);
            }
        }

        //Capture screenshot
        public static void CaptureImage()
        {
            try
            {
                //Show notification (GetStatus workaround)
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Screenshot";
                notificationDetails.Text = "Taking screenshot";
                vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Check if capture is available
                if (!CaptureIsAvailable(false))
                {
                    //Play interface sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "CaptureFailed", false, true);
                    return;
                }

                //Play interface sound
                PlayInterfaceSound(vConfigurationCtrlUI, "CaptureScreenshot", false, true);

                //Capture keyboard shortcut
                //Fix find way to directly signal Xbox Game DVR
                vFakerInputDevice.KeyboardPressRelease(GetKeysHidAction_TakeScreenshot());
            }
            catch { }
        }

        //Capture video
        public static void CaptureVideo()
        {
            try
            {
                //Show notification (GetStatus workaround)
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Screenshot";
                notificationDetails.Text = "Toggling video capture";
                vWindowOverlay.Notification_Show_Status(notificationDetails);

                //Check if capture is available
                if (!CaptureIsAvailable(true))
                {
                    //Play interface sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "CaptureFailed", false, true);
                    return;
                }

                //Play interface sound
                if (CaptureIsRecording())
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "CaptureVideoStop", false, true);
                }
                else
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "CaptureVideoStart", false, true);
                }

                //Capture keyboard shortcut
                //Fix find way to directly signal Xbox Game DVR
                vFakerInputDevice.KeyboardPressRelease(GetKeysHidAction_ToggleRecording());
            }
            catch { }
        }
    }
}