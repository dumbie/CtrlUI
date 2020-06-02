using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
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

                    //Update the key names
                    App.vWindowKeypad.UpdateKeypadNames();
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
        bool KeypadSaveMapping(KeysVirtual usedVirtualKey, KeysVirtual? usedModifierKey)
        {
            try
            {
                //Check if keypad mapping is enabled
                if (vMappingKeypadStatus == MappingStatus.Mapping)
                {
                    //Get keypad mapping profile
                    KeypadMapping directKeypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name == "Default").FirstOrDefault();

                    Debug.WriteLine("Mapped button " + vMappingKeypadButton + " to: " + usedModifierKey + " / " + usedVirtualKey);
                    if (vMappingKeypadButton == "Button A")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonAMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonAMod = null; }
                        directKeypadMappingProfile.ButtonA = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button B")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonBMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonBMod = null; }
                        directKeypadMappingProfile.ButtonB = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button X")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonXMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonXMod = null; }
                        directKeypadMappingProfile.ButtonX = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button Y")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonYMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonYMod = null; }
                        directKeypadMappingProfile.ButtonY = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button LB")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonShoulderLeftMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonShoulderLeftMod = null; }
                        directKeypadMappingProfile.ButtonShoulderLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button RB")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonShoulderRightMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonShoulderRightMod = null; }
                        directKeypadMappingProfile.ButtonShoulderRight = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button Back")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonBackMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonBackMod = null; }
                        directKeypadMappingProfile.ButtonBack = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button Start")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonStartMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonStartMod = null; }
                        directKeypadMappingProfile.ButtonStart = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button Guide")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonGuideMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonGuideMod = null; }
                        directKeypadMappingProfile.ButtonGuide = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button Thumb Left")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonThumbLeftMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonThumbLeftMod = null; }
                        directKeypadMappingProfile.ButtonThumbLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button Thumb Right")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonThumbRightMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonThumbRightMod = null; }
                        directKeypadMappingProfile.ButtonThumbRight = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button Trigger Left")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonTriggerLeftMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonTriggerLeftMod = null; }
                        directKeypadMappingProfile.ButtonTriggerLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == "Button Trigger Right")
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonTriggerRightMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonTriggerRightMod = null; }
                        directKeypadMappingProfile.ButtonTriggerRight = usedVirtualKey;
                    }

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