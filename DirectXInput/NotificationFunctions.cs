using ArnoldVinkCode;
using System;
using System.Windows;
using static ArnoldVinkCode.AVImage;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowOverlay : Window
    {
        //Show the notification overlay
        public void Notification_Show_Status(string targetIcon, string targetText)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    UpdateNotificationPosition();
                    grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "Assets/Icons/" + targetIcon + ".png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    grid_Message_Status_Text.Text = targetText;
                    grid_Message_Status.Visibility = Visibility.Visible;
                });

                vDispatcherTimerOverlay.Stop();
                vDispatcherTimerOverlay.Interval = TimeSpan.FromMilliseconds(4000);
                vDispatcherTimerOverlay.Tick += delegate
                {
                    grid_Message_Status.Visibility = Visibility.Collapsed;
                    vDispatcherTimerOverlay.Stop();
                };
                vDispatcherTimerOverlay.Start();
            }
            catch { }
        }
    }
}