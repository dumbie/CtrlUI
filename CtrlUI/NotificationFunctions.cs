using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show notification
        public async Task Notification_Show_Status(string icon, string text)
        {
            try
            {
                //Update the notification
                await DispatcherInvoke(this.Dispatcher, async delegate
                {
                    try
                    {
                        //Set notification text
                        image_Notification_Icon.Source = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/" + icon + ".png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        textblock_Notification_Status.Text = text;

                        //Show notification
                        grid_Popup_Notification.Visibility = Visibility.Visible;
                    }
                    catch { }
                });

                //Start notification timer
                vAVTimerOverlayNotification.Interval = 3000;
                vAVTimerOverlayNotification.Tick = delegate
                {
                    try
                    {
                        DispatcherInvoke(this.Dispatcher, delegate
                        {
                            //Stop notification timer
                            vAVTimerOverlayNotification.Stop();

                            //Hide notification
                            grid_Popup_Notification.Visibility = Visibility.Collapsed;
                        });
                    }
                    catch { }
                };
                vAVTimerOverlayNotification.Start();
            }
            catch { }
        }
    }
}