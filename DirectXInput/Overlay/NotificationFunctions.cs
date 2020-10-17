using ArnoldVinkCode;
using System;
using System.Windows;
using static ArnoldVinkCode.AVImage;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput.OverlayCode
{
    public partial class WindowOverlay : Window
    {
        //Show the notification overlay
        public void Notification_Show_Status(NotificationDetails notificationDetails)
        {
            try
            {
                //Show the notification
                UpdateNotificationPosition();
                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/" + notificationDetails.Icon + ".png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        grid_Message_Status_Text.Text = notificationDetails.Text;
                        grid_Message_Status.Visibility = Visibility.Visible;
                    }
                    catch { }
                });

                //Start notification timer
                vDispatcherTimerOverlay.Interval = TimeSpan.FromMilliseconds(3000);
                vDispatcherTimerOverlay.Tick += delegate
                {
                    try
                    {
                        //Hide the notification
                        grid_Message_Status.Visibility = Visibility.Collapsed;
                    }
                    catch { }
                };
                AVFunctions.TimerReset(vDispatcherTimerOverlay);
            }
            catch { }
        }
    }
}