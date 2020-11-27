using ArnoldVinkCode;
using AVForms;
using System.Diagnostics;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Reset temp blocked controller path list
        void Btn_SearchNewControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reset temp blocked controller path list
                vControllerTempBlockPaths.Clear();

                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Controller";
                notificationDetails.Text = "Searching for controllers";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                Debug.WriteLine("Reset temp blocked controller path list.");
            }
            catch { }
        }

        //Ignore and disconnect controller
        async void Btn_IgnoreController_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    int messageResult = await AVMessageBox.MessageBoxPopup(this, "Do you really want to ignore this controller?", "This will prevent the controller from been converted to XInput.", "Ignore the controller", "Cancel", "", "");
                    if (messageResult == 1)
                    {
                        //Update json profile
                        activeController.Details.Profile.Ignore = true;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");

                        //Release controller from hid guardian
                        HidGuardianReleaseController(activeController.Details);

                        //Disconnect the controller
                        await StopControllerAsync(activeController, false, "ignored");

                        Debug.Write("Ignored active controller.");
                    }
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Allow all the ignored controllers
        void Btn_AllowIgnoredControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Allow all the ignored controllers
                foreach (ControllerProfile profile in vDirectControllersProfile)
                {
                    profile.Ignore = false;
                }

                //Save changes to Json file
                JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");

                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Controller";
                notificationDetails.Text = "Allowed all controllers";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                Debug.WriteLine("Showing all the ignored controllers.");
            }
            catch { }
        }

        //Generate controller debug information
        string GenerateControllerDebugInformation()
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    string RawPackets = "(Out" + activeController.OutputReport.Length + "/In" + activeController.InputReport.Length + ")";
                    RawPackets += "(OffHd" + activeController.InputHeaderOffsetByte + ")";
                    RawPackets += "(OffBt" + activeController.InputButtonOffsetByte + ")";
                    RawPackets += "(ProductId" + activeController.Details.Profile.ProductID + "/VendorId" + activeController.Details.Profile.VendorID + ")";
                    for (int Packet = 0; Packet < activeController.InputReport.Length; Packet++) { RawPackets = RawPackets + " " + activeController.InputReport[Packet]; }
                    return RawPackets;
                }
                else
                {
                    return "No controller connected to debug";
                }
            }
            catch { }
            return "Failed to generate debug information";
        }

        //Copy controller debug information
        void Btn_CopyDebugInformation_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    Clipboard.SetText(GenerateControllerDebugInformation());

                    Debug.WriteLine("Controller debug information copied to clipboard.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Paste";
                    notificationDetails.Text = "Debug information copied";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
                else
                {
                    Debug.WriteLine("Controller debug information is not available.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Disconnect and stop the controller
        async void Btn_DisconnectController_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    await StopControllerAsync(activeController, false, "manually");
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
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
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);
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
                    int messageResult = await AVMessageBox.MessageBoxPopup(this, "Do you really want to remove this controller?", "This will reset the active controller to it's defaults and disconnect it.", "Remove controller profile", "Cancel", "", "");
                    if (messageResult == 1)
                    {
                        Debug.WriteLine("Removed the controller: " + activeController.Details.DisplayName);

                        NotificationDetails notificationDetails = new NotificationDetails();
                        notificationDetails.Icon = "Controller";
                        notificationDetails.Text = "Removed controller";
                        App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "Removed the controller: " + activeController.Details.DisplayName;
                        });

                        vDirectControllersProfile.Remove(activeController.Details.Profile);
                        await StopControllerAsync(activeController, false, "removed");

                        //Save changes to Json file
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
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