using System;
using System.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.Media.AppRecording;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class XboxGameDVR
    {
        //Check if capture is available
        public static bool CaptureIsAvailable()
        {
            try
            {
                //Check if app recording is present
                bool appRecordingPresent = ApiInformation.IsTypePresent("Windows.Media.AppRecording.AppRecordingManager");
                if (!appRecordingPresent)
                {
                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Screenshot";
                    notificationDetails.Text = "Xbox capture not available";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    Debug.WriteLine("Xbox capture not available.");
                    return false;
                }

                //Get manager and status
                AppRecordingManager recordingManager = AppRecordingManager.GetDefault();
                AppRecordingStatus recordingStatus = null;
                try
                {
                    //Only works when app window is visible (0x80070490)
                    recordingStatus = recordingManager.GetStatus();
                }
                catch
                {
                    Debug.WriteLine("Xbox capture failed to get status.");
                    return true;
                }

                //Check if capture is disabled
                if (recordingStatus.Details.IsDisabledByUser || recordingStatus.Details.IsDisabledBySystem)
                {
                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Screenshot";
                    notificationDetails.Text = "Xbox capture is disabled";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    Debug.WriteLine("Xbox capture is disabled.");
                    return false;
                }

                //Check if GPU is supported
                if (recordingStatus.Details.IsGpuConstrained)
                {
                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Screenshot";
                    notificationDetails.Text = "Xbox capture unsupported GPU";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    Debug.WriteLine("Xbox capture unsupported GPU.");
                    return false;
                }

                //Return result
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Xbox capture status available failed: " + ex.Message);
                return false;
            }
        }

        //Check if capture is recording
        public static bool CaptureIsRecording()
        {
            try
            {
                //Get manager and status
                AppRecordingManager recordingManager = AppRecordingManager.GetDefault();
                AppRecordingStatus recordingStatus = recordingManager.GetStatus();

                //Return result
                return !recordingStatus.CanRecord && recordingStatus.Details.IsCaptureResourceUnavailable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Xbox capture status recording failed: " + ex.Message);
                return false;
            }
        }

        //Xbox capture status debug
        public static void CaptureStatusDebug()
        {
            try
            {
                //Check if app recording is present
                bool appRecordingPresent = ApiInformation.IsTypePresent("Windows.Media.AppRecording.AppRecordingManager");

                //Get manager and status
                AppRecordingManager recordingManager = AppRecordingManager.GetDefault();
                AppRecordingStatus recordingStatus = recordingManager.GetStatus();

                //Write debug information
                Debug.WriteLine("Xbox recording present: " + appRecordingPresent);
                Debug.WriteLine("Xbox CanRecord: " + recordingStatus.CanRecord);
                Debug.WriteLine("Xbox CanRecordTimeSpan: " + recordingStatus.CanRecordTimeSpan);
                Debug.WriteLine("Xbox IsAnyAppBroadcasting: " + recordingStatus.Details.IsAnyAppBroadcasting);
                Debug.WriteLine("Xbox IsAppInactive: " + recordingStatus.Details.IsAppInactive);
                Debug.WriteLine("Xbox IsBlockedForApp: " + recordingStatus.Details.IsBlockedForApp);
                Debug.WriteLine("Xbox IsCaptureResourceUnavailable: " + recordingStatus.Details.IsCaptureResourceUnavailable);
                Debug.WriteLine("Xbox IsDisabledBySystem: " + recordingStatus.Details.IsDisabledBySystem);
                Debug.WriteLine("Xbox IsDisabledByUser: " + recordingStatus.Details.IsDisabledByUser);
                Debug.WriteLine("Xbox IsGameStreamInProgress: " + recordingStatus.Details.IsGameStreamInProgress);
                Debug.WriteLine("Xbox IsGpuConstrained: " + recordingStatus.Details.IsGpuConstrained);
                Debug.WriteLine("Xbox IsTimeSpanRecordingDisabled: " + recordingStatus.Details.IsTimeSpanRecordingDisabled);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Xbox capture status failed: " + ex.Message);
            }
        }
    }
}