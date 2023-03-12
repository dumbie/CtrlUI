using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVJsonFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Set keypad button
        void Btn_MapController_Mouse_Set(object sender, RoutedEventArgs args)
        {
            try
            {
                //Set button to map
                Button sendButton = sender as Button;
                string mapNameString = sendButton.ToolTip.ToString();
                Debug.WriteLine("Set button: " + mapNameString);
                vMappingControllerButton = sendButton;

                //Update mapping information
                textblock_SetController_Name.Text = mapNameString;
            }
            catch { }
        }

        //Cancel controller mapping
        void Btn_MapController_Mouse_Cancel(object sender, RoutedEventArgs args)
        {
            try
            {
                vMappingControllerStatus = MappingStatus.Cancel;
            }
            catch { }
        }

        //Unmap controller button
        void Btn_MapController_Mouse_Unmap(object sender, RoutedEventArgs args)
        {
            try
            {
                //Check if controller is connected
                ControllerStatus activeController = vActiveController();
                if (activeController == null)
                {
                    txt_ControllerMap_Status.Text = "Please connect a controller to unmap buttons.";
                    return;
                }

                string mapNameString = vMappingControllerButton.ToolTip.ToString();
                Debug.WriteLine("Unmapped button: " + mapNameString);
                txt_ControllerMap_Status.Text = "Unmapped '" + mapNameString + "' from the controller profile.";

                //Store new button mapping in Json controller
                if (vMappingControllerButton == btn_SetA) { activeController.Details.Profile.ButtonA = -1; }
                else if (vMappingControllerButton == btn_SetB) { activeController.Details.Profile.ButtonB = -1; }
                else if (vMappingControllerButton == btn_SetX) { activeController.Details.Profile.ButtonX = -1; }
                else if (vMappingControllerButton == btn_SetY) { activeController.Details.Profile.ButtonY = -1; }
                else if (vMappingControllerButton == btn_SetShoulderLeft) { activeController.Details.Profile.ButtonShoulderLeft = -1; }
                else if (vMappingControllerButton == btn_SetShoulderRight) { activeController.Details.Profile.ButtonShoulderRight = -1; }
                else if (vMappingControllerButton == btn_SetBack) { activeController.Details.Profile.ButtonBack = -1; }
                else if (vMappingControllerButton == btn_SetStart) { activeController.Details.Profile.ButtonStart = -1; }
                else if (vMappingControllerButton == btn_SetGuide) { activeController.Details.Profile.ButtonGuide = -1; }
                else if (vMappingControllerButton == btn_SetThumbLeft) { activeController.Details.Profile.ButtonThumbLeft = -1; }
                else if (vMappingControllerButton == btn_SetThumbRight) { activeController.Details.Profile.ButtonThumbRight = -1; }
                else if (vMappingControllerButton == btn_SetTriggerLeft) { activeController.Details.Profile.ButtonTriggerLeft = -1; }
                else if (vMappingControllerButton == btn_SetTriggerRight) { activeController.Details.Profile.ButtonTriggerRight = -1; }
                else if (vMappingControllerButton == btn_SetTouchpad) { activeController.Details.Profile.ButtonTouchpad = -1; }
                else if (vMappingControllerButton == btn_SetMedia) { activeController.Details.Profile.ButtonMedia = -1; }

                //Save changes to Json file
                JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
            }
            catch { }
        }

        //Map controller button
        async void Btn_MapController_Mouse_Map(object sender, RoutedEventArgs args)
        {
            try
            {
                //Check if controller is connected
                ControllerStatus activeController = vActiveController();
                if (activeController == null)
                {
                    txt_ControllerMap_Status.Text = "Please connect a controller to map buttons.";
                    return;
                }

                //Set button to map
                string mapNameString = vMappingControllerButton.ToolTip.ToString();
                vMappingControllerStatus = MappingStatus.Mapping;

                //Disable interface
                txt_ControllerMap_Status.Text = "Waiting for '" + mapNameString + "' press on the controller...";
                pb_ControllerMapProgress.IsIndeterminate = true;
                grid_ControllerPreview.IsEnabled = false;
                grid_ControllerPreview.Opacity = 0.50;
                button_SetController_Map.IsEnabled = false;
                button_SetController_Unmap.IsEnabled = false;
                button_SetController_Cancel.IsEnabled = true;

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
                            vMappingControllerStatus = MappingStatus.Cancel;
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
                button_SetController_Map.IsEnabled = true;
                button_SetController_Unmap.IsEnabled = true;
                button_SetController_Cancel.IsEnabled = false;
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
                        AVActions.DispatcherInvoke(delegate
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
                                else if (vMappingControllerButton == btn_SetTouchpad) { Controller.Details.Profile.ButtonTouchpad = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetMedia) { Controller.Details.Profile.ButtonMedia = buttonMapId; }
                            }
                            catch { }
                        });

                        //Save changes to Json file
                        JsonSaveObject(Controller.Details.Profile, GenerateJsonNameControllerProfile(Controller.Details.Profile));

                        //Reset controller button mapping
                        vMappingControllerStatus = MappingStatus.Done;
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}