using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVInputOutputClass;
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
                txt_ControllerMap_Status.Text = "Unmapped '" + mapNameString + "' from controller profile.";

                //Store new button mapping in Json controller
                if (vMappingControllerButton == btn_SetA) { activeController.Details.Profile.ButtonA = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetB) { activeController.Details.Profile.ButtonB = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetX) { activeController.Details.Profile.ButtonX = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetY) { activeController.Details.Profile.ButtonY = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetShoulderLeft) { activeController.Details.Profile.ButtonShoulderLeft = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetShoulderRight) { activeController.Details.Profile.ButtonShoulderRight = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetBack) { activeController.Details.Profile.ButtonBack = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetStart) { activeController.Details.Profile.ButtonStart = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetGuide) { activeController.Details.Profile.ButtonGuide = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetThumbLeft) { activeController.Details.Profile.ButtonThumbLeft = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetThumbRight) { activeController.Details.Profile.ButtonThumbRight = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetTriggerLeft) { activeController.Details.Profile.ButtonTriggerLeft = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetTriggerRight) { activeController.Details.Profile.ButtonTriggerRight = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetOne) { activeController.Details.Profile.ButtonOne = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetTwo) { activeController.Details.Profile.ButtonTwo = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetThree) { activeController.Details.Profile.ButtonThree = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetFour) { activeController.Details.Profile.ButtonFour = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetFive) { activeController.Details.Profile.ButtonFive = ControllerButtons.None; }
                else if (vMappingControllerButton == btn_SetSix) { activeController.Details.Profile.ButtonSix = ControllerButtons.None; }

                //Save changes to Json file
                JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
            }
            catch { }
        }

        //Default controller button
        void Btn_MapController_Mouse_Default(object sender, RoutedEventArgs args)
        {
            try
            {
                //Check if controller is connected
                ControllerStatus activeController = vActiveController();
                if (activeController == null)
                {
                    txt_ControllerMap_Status.Text = "Please connect a controller to default buttons.";
                    return;
                }

                string mapNameString = vMappingControllerButton.ToolTip.ToString();
                Debug.WriteLine("Default button: " + mapNameString);
                txt_ControllerMap_Status.Text = "Default '" + mapNameString + "' restored for controller profile.";

                //Store new button mapping in Json controller
                if (vMappingControllerButton == btn_SetA) { activeController.Details.Profile.ButtonA = null; }
                else if (vMappingControllerButton == btn_SetB) { activeController.Details.Profile.ButtonB = null; }
                else if (vMappingControllerButton == btn_SetX) { activeController.Details.Profile.ButtonX = null; }
                else if (vMappingControllerButton == btn_SetY) { activeController.Details.Profile.ButtonY = null; }
                else if (vMappingControllerButton == btn_SetShoulderLeft) { activeController.Details.Profile.ButtonShoulderLeft = null; }
                else if (vMappingControllerButton == btn_SetShoulderRight) { activeController.Details.Profile.ButtonShoulderRight = null; }
                else if (vMappingControllerButton == btn_SetBack) { activeController.Details.Profile.ButtonBack = null; }
                else if (vMappingControllerButton == btn_SetStart) { activeController.Details.Profile.ButtonStart = null; }
                else if (vMappingControllerButton == btn_SetGuide) { activeController.Details.Profile.ButtonGuide = null; }
                else if (vMappingControllerButton == btn_SetThumbLeft) { activeController.Details.Profile.ButtonThumbLeft = null; }
                else if (vMappingControllerButton == btn_SetThumbRight) { activeController.Details.Profile.ButtonThumbRight = null; }
                else if (vMappingControllerButton == btn_SetTriggerLeft) { activeController.Details.Profile.ButtonTriggerLeft = null; }
                else if (vMappingControllerButton == btn_SetTriggerRight) { activeController.Details.Profile.ButtonTriggerRight = null; }
                else if (vMappingControllerButton == btn_SetOne) { activeController.Details.Profile.ButtonOne = null; }
                else if (vMappingControllerButton == btn_SetTwo) { activeController.Details.Profile.ButtonTwo = null; }
                else if (vMappingControllerButton == btn_SetThree) { activeController.Details.Profile.ButtonThree = null; }
                else if (vMappingControllerButton == btn_SetFour) { activeController.Details.Profile.ButtonFour = null; }
                else if (vMappingControllerButton == btn_SetFive) { activeController.Details.Profile.ButtonFive = null; }
                else if (vMappingControllerButton == btn_SetSix) { activeController.Details.Profile.ButtonSix = null; }

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
                button_SetController_Default.IsEnabled = false;
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
                button_SetController_Default.IsEnabled = true;
                button_SetController_Cancel.IsEnabled = false;
            }
            catch { }
        }
    }
}