using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.JsonFunctions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Unmap controller button
        void Btn_MapController_MouseRight(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    Button sendButton = sender as Button;
                    string mapNameString = sendButton.ToolTip.ToString();
                    Debug.WriteLine("Unmapped button: " + mapNameString);
                    txt_ControllerMap_Status.Text = "Unmapped '" + mapNameString + "' from the controller profile.";

                    //Store new button mapping in Json controller
                    if (sendButton == btn_SetA) { activeController.Details.Profile.ButtonA = -1; }
                    else if (sendButton == btn_SetB) { activeController.Details.Profile.ButtonB = -1; }
                    else if (sendButton == btn_SetX) { activeController.Details.Profile.ButtonX = -1; }
                    else if (sendButton == btn_SetY) { activeController.Details.Profile.ButtonY = -1; }
                    else if (sendButton == btn_SetShoulderLeft) { activeController.Details.Profile.ButtonShoulderLeft = -1; }
                    else if (sendButton == btn_SetShoulderRight) { activeController.Details.Profile.ButtonShoulderRight = -1; }
                    else if (sendButton == btn_SetBack) { activeController.Details.Profile.ButtonBack = -1; }
                    else if (sendButton == btn_SetStart) { activeController.Details.Profile.ButtonStart = -1; }
                    else if (sendButton == btn_SetGuide) { activeController.Details.Profile.ButtonGuide = -1; }
                    else if (sendButton == btn_SetThumbLeft) { activeController.Details.Profile.ButtonThumbLeft = -1; }
                    else if (sendButton == btn_SetThumbRight) { activeController.Details.Profile.ButtonThumbRight = -1; }
                    else if (sendButton == btn_SetTriggerLeft) { activeController.Details.Profile.ButtonTriggerLeft = -1; }
                    else if (sendButton == btn_SetTriggerRight) { activeController.Details.Profile.ButtonTriggerRight = -1; }

                    //Save changes to Json file
                    JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                }
            }
            catch { }
        }

        //Map controller button
        async void Btn_MapController_MouseLeft(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    //Set button to map
                    Button sendButton = sender as Button;
                    string mapNameString = sendButton.ToolTip.ToString();
                    vMappingControllerStatus = MappingStatus.Mapping;
                    vMappingControllerButton = sendButton;

                    //Disable interface
                    txt_ControllerMap_Status.Text = "Waiting for '" + mapNameString + "' press on the controller...";
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
                                vMappingControllerButton = null;
                            }
                            else
                            {
                                txt_ControllerMap_Status.Text = "Waiting for '" + mapNameString + "' press on the controller... " + (11 - countdownTimeout).ToString() + "sec.";
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
                        txt_ControllerMap_Status.Text = "Changed '" + mapNameString + "' to the pressed controller button.";
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
                    int buttonMapId = Array.FindIndex(Controller.InputCurrent.ButtonPressStatus, ButtonPressed => ButtonPressed);
                    if (buttonMapId != -1)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            try
                            {
                                string mapNameString = vMappingControllerButton.ToolTip.ToString();
                                Debug.WriteLine("Mapped button " + mapNameString + " to: " + buttonMapId);
                                if (vMappingControllerButton == btn_SetA) { Controller.Details.Profile.ButtonA = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetB) { Controller.Details.Profile.ButtonB = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetX) { Controller.Details.Profile.ButtonX = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetY) { Controller.Details.Profile.ButtonY = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetShoulderLeft) { Controller.Details.Profile.ButtonShoulderLeft = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetShoulderRight) { Controller.Details.Profile.ButtonShoulderRight = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetBack) { Controller.Details.Profile.ButtonBack = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetStart) { Controller.Details.Profile.ButtonStart = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetGuide) { Controller.Details.Profile.ButtonGuide = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetThumbLeft) { Controller.Details.Profile.ButtonThumbLeft = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetThumbRight) { Controller.Details.Profile.ButtonThumbRight = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetTriggerLeft) { Controller.Details.Profile.ButtonTriggerLeft = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetTriggerRight) { Controller.Details.Profile.ButtonTriggerRight = buttonMapId; }
                            }
                            catch { }
                        });

                        //Reset controller button mapping
                        vMappingControllerStatus = MappingStatus.Done;
                        vMappingControllerButton = null;

                        //Save changes to Json file
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}