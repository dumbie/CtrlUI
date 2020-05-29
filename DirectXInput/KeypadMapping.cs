using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Map keypad button
        async void Btn_MapKeypad_MouseLeft(object sender, RoutedEventArgs args)
        {
            try
            {
                Button sendButton = sender as Button;
                string mapButton = sendButton.Tag.ToString();

                //Set button to map
                vMappingKeypadStatus = MappingStatus.Mapping;
                vMappingKeypadButton = mapButton;

                //Disable interface
                txt_KeypadMap_Status.Text = "Waiting for keyboard key press for '" + mapButton + "'...";
                pb_KeypadMapProgress.IsIndeterminate = true;
                grid_KeypadPreview.IsEnabled = false;
                grid_KeypadPreview.Opacity = 0.50;

                //Start mapping timer
                int countdownTimeout = 0;
                AVFunctions.TimerRenew(ref vMappingKeypadTimer);
                vMappingKeypadTimer.Interval = TimeSpan.FromSeconds(1);
                vMappingKeypadTimer.Tick += delegate
                {
                    try
                    {
                        if (countdownTimeout++ >= 10)
                        {
                            //Reset controller button mapping
                            vMappingKeypadStatus = MappingStatus.Cancel;
                            vMappingKeypadButton = "None";
                        }
                        else
                        {
                            txt_KeypadMap_Status.Text = "Waiting for keyboard key press for '" + mapButton + "'... " + (11 - countdownTimeout).ToString() + "sec.";
                        }
                    }
                    catch { }
                };
                vMappingKeypadTimer.Start();

                //Check if button is mapped
                while (vMappingKeypadStatus == MappingStatus.Mapping) { await Task.Delay(500); }
                vMappingKeypadTimer.Stop();

                if (vMappingKeypadStatus == MappingStatus.Done)
                {
                    txt_KeypadMap_Status.Text = "Changed '" + mapButton + "' to the pressed keyboard button.";
                }
                else
                {
                    Debug.WriteLine("Cancelled button mapping.");
                    txt_KeypadMap_Status.Text = "Cancelled button mapping, please select a key to change.";
                }

                //Enable interface
                pb_KeypadMapProgress.IsIndeterminate = false;
                grid_KeypadPreview.IsEnabled = true;
                grid_KeypadPreview.Opacity = 1.00;
            }
            catch { }
        }

        //Save keypad button mapping
        bool KeypadSaveMapping(KeysVirtual usedVirtualKey)
        {
            try
            {
                //Check if keypad mapping is enabled
                if (vMappingKeypadStatus == MappingStatus.Mapping)
                {
                    Debug.WriteLine("Mapped button " + vMappingKeypadButton + " to: " + usedVirtualKey);
                    if (vMappingKeypadButton == "Button A") { vDirectKeypadMapping.ButtonA = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button B") { vDirectKeypadMapping.ButtonB = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button X") { vDirectKeypadMapping.ButtonX = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button Y") { vDirectKeypadMapping.ButtonY = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button LB") { vDirectKeypadMapping.ButtonShoulderLeft = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button RB") { vDirectKeypadMapping.ButtonShoulderRight = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button Back") { vDirectKeypadMapping.ButtonBack = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button Start") { vDirectKeypadMapping.ButtonStart = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button Guide") { vDirectKeypadMapping.ButtonGuide = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button Thumb Left") { vDirectKeypadMapping.ButtonThumbLeft = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button Thumb Right") { vDirectKeypadMapping.ButtonThumbRight = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button Trigger Left") { vDirectKeypadMapping.ButtonTriggerLeft = usedVirtualKey; }
                    else if (vMappingKeypadButton == "Button Trigger Right") { vDirectKeypadMapping.ButtonTriggerRight = usedVirtualKey; }

                    //Reset controller button mapping
                    vMappingKeypadStatus = MappingStatus.Done;
                    vMappingKeypadButton = "None";

                    //Save changes to Json file
                    JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}