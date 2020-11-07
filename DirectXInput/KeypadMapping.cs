using ArnoldVinkCode;
using System;
using System.Diagnostics;
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
                //Set button to map
                Button sendButton = sender as Button;
                string mapNameString = sendButton.Tag.ToString();
                vMappingKeypadStatus = MappingStatus.Mapping;
                vMappingKeypadButton = sendButton;

                //Disable interface
                txt_KeypadMap_Status.Text = "Waiting for keyboard key press for '" + mapNameString + "'...";
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
                            vMappingKeypadButton = null;
                        }
                        else
                        {
                            txt_KeypadMap_Status.Text = "Waiting for keyboard key press for '" + mapNameString + "'... " + (11 - countdownTimeout).ToString() + "sec.";
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
                    txt_KeypadMap_Status.Text = "Changed '" + mapNameString + "' to the pressed keyboard button.";

                    //Update all keypad key tool tips
                    UpdateKeypadToolTips();

                    //Update all keypad key names
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

        //Unmap keypad button
        void Btn_MapKeypad_MouseRight(object sender, RoutedEventArgs args)
        {
            try
            {
                Button sendButton = sender as Button;
                string mapNameString = sendButton.Tag.ToString();
                Debug.WriteLine("Unmapped button: " + mapNameString);
                txt_KeypadMap_Status.Text = "Unmapped '" + mapNameString + "' from the keypad.";

                //Get keypad mapping profile
                KeypadMapping directKeypadMappingProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                //Store new keypad mapping in Json
                if (sendButton == btn_SetPadDPadLeft)
                {
                    directKeypadMappingProfile.DPadLeftMod = null;
                    directKeypadMappingProfile.DPadLeft = null;
                }
                else if (sendButton == btn_SetPadDPadUp)
                {
                    directKeypadMappingProfile.DPadUpMod = null;
                    directKeypadMappingProfile.DPadUp = null;
                }
                else if (sendButton == btn_SetPadDPadRight)
                {
                    directKeypadMappingProfile.DPadRightMod = null;
                    directKeypadMappingProfile.DPadRight = null;
                }
                else if (sendButton == btn_SetPadDPadDown)
                {
                    directKeypadMappingProfile.DPadDownMod = null;
                    directKeypadMappingProfile.DPadDown = null;
                }
                else if (sendButton == btn_SetPadThumbLeftLeft)
                {
                    directKeypadMappingProfile.ThumbLeftLeftMod = null;
                    directKeypadMappingProfile.ThumbLeftLeft = null;
                }
                else if (sendButton == btn_SetPadThumbLeftUp)
                {
                    directKeypadMappingProfile.ThumbLeftUpMod = null;
                    directKeypadMappingProfile.ThumbLeftUp = null;
                }
                else if (sendButton == btn_SetPadThumbLeftRight)
                {
                    directKeypadMappingProfile.ThumbLeftRightMod = null;
                    directKeypadMappingProfile.ThumbLeftRight = null;
                }
                else if (sendButton == btn_SetPadThumbLeftDown)
                {
                    directKeypadMappingProfile.ThumbLeftDownMod = null;
                    directKeypadMappingProfile.ThumbLeftDown = null;
                }
                else if (sendButton == btn_SetPadThumbRightLeft)
                {
                    directKeypadMappingProfile.ThumbRightLeftMod = null;
                    directKeypadMappingProfile.ThumbRightLeft = null;
                }
                else if (sendButton == btn_SetPadThumbRightUp)
                {
                    directKeypadMappingProfile.ThumbRightUpMod = null;
                    directKeypadMappingProfile.ThumbRightUp = null;
                }
                else if (sendButton == btn_SetPadThumbRightRight)
                {
                    directKeypadMappingProfile.ThumbRightRightMod = null;
                    directKeypadMappingProfile.ThumbRightRight = null;
                }
                else if (sendButton == btn_SetPadThumbRightDown)
                {
                    directKeypadMappingProfile.ThumbRightDownMod = null;
                    directKeypadMappingProfile.ThumbRightDown = null;
                }
                else if (sendButton == btn_SetPadA)
                {
                    directKeypadMappingProfile.ButtonAMod = null;
                    directKeypadMappingProfile.ButtonA = null;
                }
                else if (sendButton == btn_SetPadB)
                {
                    directKeypadMappingProfile.ButtonBMod = null;
                    directKeypadMappingProfile.ButtonB = null;
                }
                else if (sendButton == btn_SetPadX)
                {
                    directKeypadMappingProfile.ButtonXMod = null;
                    directKeypadMappingProfile.ButtonX = null;
                }
                else if (sendButton == btn_SetPadY)
                {
                    directKeypadMappingProfile.ButtonYMod = null;
                    directKeypadMappingProfile.ButtonY = null;
                }
                else if (sendButton == btn_SetPadShoulderLeft)
                {
                    directKeypadMappingProfile.ButtonShoulderLeftMod = null;
                    directKeypadMappingProfile.ButtonShoulderLeft = null;
                }
                else if (sendButton == btn_SetPadShoulderRight)
                {
                    directKeypadMappingProfile.ButtonShoulderRightMod = null;
                    directKeypadMappingProfile.ButtonShoulderRight = null;
                }
                else if (sendButton == btn_SetPadBack)
                {
                    directKeypadMappingProfile.ButtonBackMod = null;
                    directKeypadMappingProfile.ButtonBack = null;
                }
                else if (sendButton == btn_SetPadStart)
                {
                    directKeypadMappingProfile.ButtonStartMod = null;
                    directKeypadMappingProfile.ButtonStart = null;
                }
                else if (sendButton == btn_SetPadThumbLeft)
                {
                    directKeypadMappingProfile.ButtonThumbLeftMod = null;
                    directKeypadMappingProfile.ButtonThumbLeft = null;
                }
                else if (sendButton == btn_SetPadThumbRight)
                {
                    directKeypadMappingProfile.ButtonThumbRightMod = null;
                    directKeypadMappingProfile.ButtonThumbRight = null;
                }
                else if (sendButton == btn_SetPadTriggerLeft)
                {
                    directKeypadMappingProfile.ButtonTriggerLeftMod = null;
                    directKeypadMappingProfile.ButtonTriggerLeft = null;
                }
                else if (sendButton == btn_SetPadTriggerRight)
                {
                    directKeypadMappingProfile.ButtonTriggerRightMod = null;
                    directKeypadMappingProfile.ButtonTriggerRight = null;
                }

                //Save changes to Json file
                JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

                //Update the key names
                App.vWindowKeypad.UpdateKeypadNames();
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
                    KeypadMapping directKeypadMappingProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                    string mapNameString = vMappingKeypadButton.Tag.ToString();
                    Debug.WriteLine("Mapped button " + mapNameString + " to: " + usedModifierKey + " / " + usedVirtualKey);

                    if (vMappingKeypadButton == btn_SetPadDPadLeft)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.DPadLeftMod = usedModifierKey; } else { directKeypadMappingProfile.DPadLeftMod = null; }
                        directKeypadMappingProfile.DPadLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadDPadUp)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.DPadUpMod = usedModifierKey; } else { directKeypadMappingProfile.DPadUpMod = null; }
                        directKeypadMappingProfile.DPadUp = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadDPadRight)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.DPadRightMod = usedModifierKey; } else { directKeypadMappingProfile.DPadRightMod = null; }
                        directKeypadMappingProfile.DPadRight = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadDPadDown)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.DPadDownMod = usedModifierKey; } else { directKeypadMappingProfile.DPadDownMod = null; }
                        directKeypadMappingProfile.DPadDown = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbLeftLeft)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ThumbLeftLeftMod = usedModifierKey; } else { directKeypadMappingProfile.ThumbLeftLeftMod = null; }
                        directKeypadMappingProfile.ThumbLeftLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbLeftUp)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ThumbLeftUpMod = usedModifierKey; } else { directKeypadMappingProfile.ThumbLeftUpMod = null; }
                        directKeypadMappingProfile.ThumbLeftUp = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbLeftRight)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ThumbLeftRightMod = usedModifierKey; } else { directKeypadMappingProfile.ThumbLeftRightMod = null; }
                        directKeypadMappingProfile.ThumbLeftRight = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbLeftDown)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ThumbLeftDownMod = usedModifierKey; } else { directKeypadMappingProfile.ThumbLeftDownMod = null; }
                        directKeypadMappingProfile.ThumbLeftDown = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbRightLeft)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ThumbRightLeftMod = usedModifierKey; } else { directKeypadMappingProfile.ThumbRightLeftMod = null; }
                        directKeypadMappingProfile.ThumbRightLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbRightUp)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ThumbRightUpMod = usedModifierKey; } else { directKeypadMappingProfile.ThumbRightUpMod = null; }
                        directKeypadMappingProfile.ThumbRightUp = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbRightRight)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ThumbRightRightMod = usedModifierKey; } else { directKeypadMappingProfile.ThumbRightRightMod = null; }
                        directKeypadMappingProfile.ThumbRightRight = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbRightDown)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ThumbRightDownMod = usedModifierKey; } else { directKeypadMappingProfile.ThumbRightDownMod = null; }
                        directKeypadMappingProfile.ThumbRightDown = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadA)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonAMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonAMod = null; }
                        directKeypadMappingProfile.ButtonA = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadB)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonBMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonBMod = null; }
                        directKeypadMappingProfile.ButtonB = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadX)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonXMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonXMod = null; }
                        directKeypadMappingProfile.ButtonX = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadY)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonYMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonYMod = null; }
                        directKeypadMappingProfile.ButtonY = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadShoulderLeft)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonShoulderLeftMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonShoulderLeftMod = null; }
                        directKeypadMappingProfile.ButtonShoulderLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadShoulderRight)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonShoulderRightMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonShoulderRightMod = null; }
                        directKeypadMappingProfile.ButtonShoulderRight = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadBack)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonBackMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonBackMod = null; }
                        directKeypadMappingProfile.ButtonBack = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadStart)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonStartMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonStartMod = null; }
                        directKeypadMappingProfile.ButtonStart = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbLeft)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonThumbLeftMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonThumbLeftMod = null; }
                        directKeypadMappingProfile.ButtonThumbLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadThumbRight)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonThumbRightMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonThumbRightMod = null; }
                        directKeypadMappingProfile.ButtonThumbRight = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadTriggerLeft)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonTriggerLeftMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonTriggerLeftMod = null; }
                        directKeypadMappingProfile.ButtonTriggerLeft = usedVirtualKey;
                    }
                    else if (vMappingKeypadButton == btn_SetPadTriggerRight)
                    {
                        if (usedModifierKey != null) { directKeypadMappingProfile.ButtonTriggerRightMod = usedModifierKey; } else { directKeypadMappingProfile.ButtonTriggerRightMod = null; }
                        directKeypadMappingProfile.ButtonTriggerRight = usedVirtualKey;
                    }

                    //Reset controller button mapping
                    vMappingKeypadStatus = MappingStatus.Done;
                    vMappingKeypadButton = null;

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