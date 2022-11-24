using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVImage;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput.OverlayCode
{
    public partial class WindowOverlay : Window
    {
        //Show the notification overlay
        public async Task Notification_Show_Status(string icon, string text)
        {
            try
            {
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = icon;
                notificationDetails.Text = text;
                await Notification_Show_Status(notificationDetails);
            }
            catch { }
        }

        //Show the notification overlay
        public async Task Notification_Show_Status(NotificationDetails notificationDetails)
        {
            try
            {
                //Update the notification
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    try
                    {
                        //Set notification text
                        grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/" + notificationDetails.Icon + ".png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        grid_Message_Status_Text.Text = notificationDetails.Text;
                        if (notificationDetails.Color != null)
                        {
                            grid_Message_Status_Border.Background = new SolidColorBrush((Color)notificationDetails.Color);
                        }
                        else
                        {
                            grid_Message_Status_Border.Background = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];
                        }

                        //Show the notification
                        await this.Show();
                    }
                    catch { }
                });

                //Start notification timer
                vDispatcherTimerOverlay.Interval = TimeSpan.FromMilliseconds(3000);
                vDispatcherTimerOverlay.Tick += async delegate
                {
                    try
                    {
                        //Hide the notification
                        await this.Hide();

                        //Renew the timer
                        AVFunctions.TimerRenew(ref vDispatcherTimerOverlay);
                    }
                    catch { }
                };
                AVFunctions.TimerReset(vDispatcherTimerOverlay);
            }
            catch { }
        }
    }
}