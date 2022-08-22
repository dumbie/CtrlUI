using ArnoldVinkCode;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Disconnect and stop the controller
        async void Btn_DisconnectController_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    await StopController(activeController, "manually", string.Empty);
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Disconnect and stop all controllers
        async void Btn_DisconnectControllerAll_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                await StopAllControllers(false);

                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Controller";
                notificationDetails.Text = "Disconnected all controllers";
                await App.vWindowOverlay.Notification_Show_Status(notificationDetails);
            }
            catch { }
        }

        //Remove the controller from the list
        async void Btn_RemoveController_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    List<string> messageAnswers = new List<string>();
                    messageAnswers.Add("Remove controller profile");
                    messageAnswers.Add("Cancel");

                    string messageResult = await new AVMessageBox().Popup(this, "Do you really want to remove this controller?", "This will reset the active controller to it's defaults and disconnect it.", messageAnswers);
                    if (messageResult == "Remove controller profile")
                    {
                        Debug.WriteLine("Removed the controller: " + activeController.Details.DisplayName);

                        NotificationDetails notificationDetails = new NotificationDetails();
                        notificationDetails.Icon = "Controller";
                        notificationDetails.Text = "Removed controller";
                        await App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                        //Stop the controller task
                        await StopController(activeController, "removed", "Controller " + activeController.Details.DisplayName + " removed and disconnected.");

                        //Remove Json in list
                        vDirectControllersProfile.Remove(activeController.Details.Profile);

                        //Remove Json file
                        JsonRemoveFile(GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Open Windows Game controller settings
        void btn_CheckControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("joy.cpl");
            }
            catch { }
        }

        //Open Windows device manager
        void btn_CheckDeviceManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("devmgmt.msc");
            }
            catch { }
        }
    }
}