using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Unmap controller button
        void Btn_MapController_MouseRight(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = GetActiveController();
                if (activeController != null)
                {
                    Button sendButton = sender as Button;
                    string mapButton = sendButton.Tag.ToString();

                    Debug.WriteLine("Unmapped button: " + mapButton);
                    txt_ControllerMap_Status.Text = "Unmapped '" + mapButton + "' from the controller profile.";

                    //Store new button mapping in Json controller
                    if (mapButton == "Button A") { activeController.Details.Profile.ButtonA = -1; }
                    else if (mapButton == "Button B") { activeController.Details.Profile.ButtonB = -1; }
                    else if (mapButton == "Button X") { activeController.Details.Profile.ButtonX = -1; }
                    else if (mapButton == "Button Y") { activeController.Details.Profile.ButtonY = -1; }
                    else if (mapButton == "Button LB") { activeController.Details.Profile.ButtonShoulderLeft = -1; }
                    else if (mapButton == "Button RB") { activeController.Details.Profile.ButtonShoulderRight = -1; }
                    else if (mapButton == "Button Back") { activeController.Details.Profile.ButtonBack = -1; }
                    else if (mapButton == "Button Start") { activeController.Details.Profile.ButtonStart = -1; }
                    else if (mapButton == "Button Guide") { activeController.Details.Profile.ButtonGuide = -1; }
                    else if (mapButton == "Button Thumb Left") { activeController.Details.Profile.ButtonThumbLeft = -1; }
                    else if (mapButton == "Button Thumb Right") { activeController.Details.Profile.ButtonThumbRight = -1; }
                    else if (mapButton == "Button Trigger Left") { activeController.Details.Profile.ButtonTriggerLeft = -1; }
                    else if (mapButton == "Button Trigger Right") { activeController.Details.Profile.ButtonTriggerRight = -1; }

                    //Save changes to Json file
                    JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                }
            }
            catch { }
        }

        //Map controller button
        async void Btn_MapController_MouseLeft(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = GetActiveController();
                if (activeController != null)
                {
                    Button sendButton = sender as Button;
                    string mapButton = sendButton.Tag.ToString();

                    //Set button to map
                    vMappingControllerStatus = MappingStatus.Mapping;
                    vMappingControllerButton = mapButton;

                    //Disable interface
                    txt_ControllerMap_Status.Text = "Waiting for '" + mapButton + "' press on the controller...";
                    pb_ControllerMapProgress.IsIndeterminate = true;
                    grid_ControllerPreview.IsEnabled = false;
                    grid_ControllerPreview.Opacity = 0.50;

                    //Start mapping timer
                    int countdownTimeout = 0;
                    AVFunctions.TimerRenew(ref vMappingControllerTimer);
                    vMappingControllerTimer.Interval = TimeSpan.FromSeconds(1);
                    vMappingControllerTimer.Tick += delegate
                    {
                        try
                        {
                            if (countdownTimeout++ >= 10)
                            {
                                //Reset controller button mapping
                                vMappingControllerStatus = MappingStatus.Cancel;
                                vMappingControllerButton = "None";
                            }
                            else
                            {
                                txt_ControllerMap_Status.Text = "Waiting for '" + mapButton + "' press on the controller... " + (11 - countdownTimeout).ToString() + "sec.";
                            }
                        }
                        catch { }
                    };
                    vMappingControllerTimer.Start();

                    //Check if button is mapped
                    while (vMappingControllerStatus == MappingStatus.Mapping) { await Task.Delay(500); }
                    vMappingControllerTimer.Stop();

                    if (vMappingControllerStatus == MappingStatus.Done)
                    {
                        txt_ControllerMap_Status.Text = "Changed '" + mapButton + "' to the pressed controller button.";
                    }
                    else
                    {
                        Debug.WriteLine("Cancelled button mapping.");
                        txt_ControllerMap_Status.Text = "Cancelled button mapping, please select a button to change.";
                    }

                    //Enable interface
                    pb_ControllerMapProgress.IsIndeterminate = false;
                    grid_ControllerPreview.IsEnabled = true;
                    grid_ControllerPreview.Opacity = 1.00;
                }
            }
            catch { }
        }

        //Save controller button mapping
        bool ControllerSaveMapping(ControllerStatus Controller)
        {
            try
            {
                //Check if controller mapping is enabled
                if (vMappingControllerStatus == MappingStatus.Mapping)
                {
                    //Store new button mapping in Json controller
                    int ButtonMapId = Array.FindIndex(Controller.InputCurrent.RawBytes, ButtonPressed => ButtonPressed);
                    if (ButtonMapId != -1)
                    {
                        Debug.WriteLine("Mapped button " + vMappingControllerButton + " to: " + ButtonMapId);
                        if (vMappingControllerButton == "Button A") { Controller.Details.Profile.ButtonA = ButtonMapId; }
                        else if (vMappingControllerButton == "Button B") { Controller.Details.Profile.ButtonB = ButtonMapId; }
                        else if (vMappingControllerButton == "Button X") { Controller.Details.Profile.ButtonX = ButtonMapId; }
                        else if (vMappingControllerButton == "Button Y") { Controller.Details.Profile.ButtonY = ButtonMapId; }
                        else if (vMappingControllerButton == "Button LB") { Controller.Details.Profile.ButtonShoulderLeft = ButtonMapId; }
                        else if (vMappingControllerButton == "Button RB") { Controller.Details.Profile.ButtonShoulderRight = ButtonMapId; }
                        else if (vMappingControllerButton == "Button Back") { Controller.Details.Profile.ButtonBack = ButtonMapId; }
                        else if (vMappingControllerButton == "Button Start") { Controller.Details.Profile.ButtonStart = ButtonMapId; }
                        else if (vMappingControllerButton == "Button Guide") { Controller.Details.Profile.ButtonGuide = ButtonMapId; }
                        else if (vMappingControllerButton == "Button Thumb Left") { Controller.Details.Profile.ButtonThumbLeft = ButtonMapId; }
                        else if (vMappingControllerButton == "Button Thumb Right") { Controller.Details.Profile.ButtonThumbRight = ButtonMapId; }
                        else if (vMappingControllerButton == "Button Trigger Left") { Controller.Details.Profile.ButtonTriggerLeft = ButtonMapId; }
                        else if (vMappingControllerButton == "Button Trigger Right") { Controller.Details.Profile.ButtonTriggerRight = ButtonMapId; }

                        //Reset controller button mapping
                        vMappingControllerStatus = MappingStatus.Done;
                        vMappingControllerButton = "None";

                        //Save changes to Json file
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}