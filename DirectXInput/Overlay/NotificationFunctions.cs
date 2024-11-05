﻿using ArnoldVinkCode;
using System;
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
        public void Notification_Show_Status(string icon, string text)
        {
            try
            {
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = icon;
                notificationDetails.Text = text;
                Notification_Show_Status(notificationDetails);
            }
            catch { }
        }

        //Show the notification overlay
        public void Notification_Show_Status(NotificationDetails notificationDetails)
        {
            try
            {
                //Update the notification
                AVActions.DispatcherInvoke(delegate
                {
                    try
                    {
                        //Set notification text
                        grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/" + notificationDetails.Icon + ".png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
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
                        Show();
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
                        Hide();

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