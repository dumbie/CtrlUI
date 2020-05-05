using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

                //Show the notification
                vNotificationVisible = true;
                UpdateNotificationPosition();
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "Assets/Icons/" + notificationDetails.Icon + ".png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    grid_Message_Status_Text.Text = notificationDetails.Text;
                    grid_Message_Status.Visibility = Visibility.Visible;
                });

                //Hide the notification in a few seconds
                async void TaskAction()
                {
                    try
                    {
                        await Notification_Wait_Hide();
                    }
                    catch { }
                }
                AVActions.TaskStart(TaskAction, null);
            }
            catch { }
        }

        //Hide the notification in a few seconds
        public async Task Notification_Wait_Hide()
        {
            try
            {
                //Wait for hiding time
                await Task.Delay(3000);

                //Hide the notification
                vNotificationVisible = false;
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Message_Status.Visibility = Visibility.Collapsed;
                });

                //Check notification queue
                if (vNotificationQueue.Any())
                {
                    NotificationDetails firstNotification = vNotificationQueue.FirstOrDefault();
                    Notification_Show_Status(firstNotification);
                    vNotificationQueue.Remove(firstNotification);
                }
            }
            catch { }
        }
    }
}