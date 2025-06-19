using ArnoldVinkCode;
using System;
using System.Windows;
using static ArnoldVinkCode.AVImage;
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
                AVActions.DispatcherInvoke(delegate
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
                vDispatcherTimerOverlay.Interval = TimeSpan.FromMilliseconds(3000);
                vDispatcherTimerOverlay.Tick += delegate
                {
                    try
                    {
                        //Hide the notification
                        grid_Popup_Notification.Visibility = Visibility.Collapsed;

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