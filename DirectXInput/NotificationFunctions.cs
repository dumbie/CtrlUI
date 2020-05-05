using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static ArnoldVinkCode.AVImage;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowOverlay : Window
    {
        //Show the notification overlay
        public void Notification_Show_Status(NotificationDetails notificationDetails)
        {
            try
            {
                //Check if the notification is visible
                if (vNotificationVisible)
                {
                    Debug.WriteLine("Added notification to the queue: " + notificationDetails.Text);
                    vNotificationQueue.Add(notificationDetails);
                    return;
                }

                //Remove notification from the queue
                try
                {
                    //Debug.WriteLine("Removed notification from the queue: " + notificationDetails.Text);
                    vNotificationQueue.Remove(notificationDetails);
                }
                catch { }

                //Show the notification
                vNotificationVisible = true;
                UpdateNotificationPosition();
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "Assets/Icons/" + notificationDetails.Icon + ".png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    grid_Message_Status_Text.Text = notificationDetails.Text;
                    grid_Message_Status.Visibility = Visibility.Visible;
                });

                //Start notification timer
                AVFunctions.TimerRenew(ref vDispatcherTimerOverlay);
                vDispatcherTimerOverlay.Interval = TimeSpan.FromMilliseconds(3000);
                vDispatcherTimerOverlay.Tick += delegate
                {
                    try
                    {
                        //Hide the notification
                        vNotificationVisible = false;
                        grid_Message_Status.Visibility = Visibility.Collapsed;

                        //Check notification queue
                        if (vNotificationQueue.Any())
                        {
                            Notification_Show_Status(vNotificationQueue.FirstOrDefault());
                        }
                    }
                    catch { }
                };
                vDispatcherTimerOverlay.Start();
            }
            catch { }
        }
    }
}