using ArnoldVinkCode;
using System;
using System.Windows;
using static KeyboardController.AppVariables;

namespace KeyboardController
{
    partial class WindowMain
    {
        //Show the status popup
        void Popup_Show_Status(string Message)
        {
            try
            {
                vDispatcherTimer.Stop();

                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Message_Status_Text.Text = Message;
                    grid_Message_Status.Visibility = Visibility.Visible;
                });

                vDispatcherTimer.Interval = TimeSpan.FromSeconds(3);
                vDispatcherTimer.Tick += delegate
                {
                    grid_Message_Status.Visibility = Visibility.Collapsed;
                    vDispatcherTimer.Stop();
                };
                vDispatcherTimer.Start();
            }
            catch { }
        }
    }
}