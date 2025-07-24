using ArnoldVinkCode;
using ArnoldVinkStyles;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show notification
        public void Notification_Show_Status(string icon, string text)
        {
            try
            {
                //Update the notification
                DispatcherInvoke(delegate
                {
                    try
                    {
                        //Set notification text
                        image_Notification_Icon.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/" + icon + ".png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        textblock_Notification_Status.Text = text;

                        //Show the notification
                        grid_Popup_Notification.Visibility = Visibility.Visible;
                    }
                    catch { }
                });

                //Start notification timer
                vAVTimerOverlay.Interval = 3000;
                vAVTimerOverlay.Tick = delegate
                {
                    try
                    {
                        DispatcherInvoke(delegate
                        {
                            //Stop notification timer
                            vAVTimerOverlay.Stop();

                            //Hide notification
                            grid_Popup_Notification.Visibility = Visibility.Collapsed;
                        });
                    }
                    catch { }
                };
                vAVTimerOverlay.Start();
            }
            catch { }
        }
    }
}