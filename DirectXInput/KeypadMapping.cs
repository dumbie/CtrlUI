using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Set keypad button
        void Btn_MapKeypad_Mouse_Set(object sender, RoutedEventArgs args)
        {
            try
            {
                //Set button to map
                Button sendButton = sender as Button;
                string mapNameString = sendButton.Tag.ToString();
                Debug.WriteLine("Set button: " + mapNameString);
                vMappingKeypadButton = sendButton;

                //Get keypad mapping profile
                KeypadMapping keypadMappingProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                //Update mapping information
                textblock_SetPad_Name.Text = mapNameString;

                //Disable combobox event
                combobox_SetPad_Modifier.SelectionChanged -= ComboBox_MapKeypad_Save;
                combobox_SetPad_Keyboard.SelectionChanged -= ComboBox_MapKeypad_Save;

                //Select combobox index
                if (vMappingKeypadButton == btn_SetPadDPadLeft)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.DPadLeftMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.DPadLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadUp)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.DPadUpMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.DPadUp;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadRight)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.DPadRightMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.DPadRight;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadDown)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.DPadDownMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.DPadDown;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftLeft)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ThumbLeftLeftMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbLeftLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftUp)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ThumbLeftUpMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbLeftUp;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftRight)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ThumbLeftRightMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbLeftRight;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftDown)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ThumbLeftDownMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbLeftDown;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightLeft)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ThumbRightLeftMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbRightLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightUp)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ThumbRightUpMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbRightUp;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightRight)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ThumbRightRightMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbRightRight;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightDown)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ThumbRightDownMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbRightDown;
                }
                else if (vMappingKeypadButton == btn_SetPadA)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonAMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonA;
                }
                else if (vMappingKeypadButton == btn_SetPadB)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonBMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonB;
                }
                else if (vMappingKeypadButton == btn_SetPadX)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonXMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonX;
                }
                else if (vMappingKeypadButton == btn_SetPadY)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonYMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonY;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderLeft)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonShoulderLeftMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonShoulderLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderRight)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonShoulderRightMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonShoulderRight;
                }
                else if (vMappingKeypadButton == btn_SetPadBack)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonBackMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonBack;
                }
                else if (vMappingKeypadButton == btn_SetPadStart)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonStartMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonStart;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeft)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonThumbLeftMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonThumbLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRight)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonThumbRightMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonThumbRight;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerLeft)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonTriggerLeftMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonTriggerLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerRight)
                {
                    combobox_SetPad_Modifier.SelectedItem = keypadMappingProfile.ButtonTriggerRightMod;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonTriggerRight;
                }

                //Enable combobox event
                combobox_SetPad_Modifier.SelectionChanged += ComboBox_MapKeypad_Save;
                combobox_SetPad_Keyboard.SelectionChanged += ComboBox_MapKeypad_Save;
            }
            catch { }
        }

        //Unmap keypad button
        void Btn_MapKeypad_Mouse_Unmap(object sender, RoutedEventArgs args)
        {
            try
            {
                string mapNameString = vMappingKeypadButton.Tag.ToString();
                Debug.WriteLine("Unmapped button: " + mapNameString);
                txt_KeypadMap_Status.Text = "Unmapped button '" + mapNameString + "' from the keypad.";

                //Reset combobox selection
                vComboboxSaveEnabled = false;
                combobox_SetPad_Modifier.SelectedItem = KeyboardModifiers.None;
                combobox_SetPad_Keyboard.SelectedItem = KeyboardKeys.None;
                vComboboxSaveEnabled = true;

                //Get keypad mapping profile
                KeypadMapping keypadMappingProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                //Store new keypad mapping in Json
                if (vMappingKeypadButton == btn_SetPadDPadLeft)
                {
                    keypadMappingProfile.DPadLeftMod = KeyboardModifiers.None;
                    keypadMappingProfile.DPadLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadUp)
                {
                    keypadMappingProfile.DPadUpMod = KeyboardModifiers.None;
                    keypadMappingProfile.DPadUp = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadRight)
                {
                    keypadMappingProfile.DPadRightMod = KeyboardModifiers.None;
                    keypadMappingProfile.DPadRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadDown)
                {
                    keypadMappingProfile.DPadDownMod = KeyboardModifiers.None;
                    keypadMappingProfile.DPadDown = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftLeft)
                {
                    keypadMappingProfile.ThumbLeftLeftMod = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftUp)
                {
                    keypadMappingProfile.ThumbLeftUpMod = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftUp = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftRight)
                {
                    keypadMappingProfile.ThumbLeftRightMod = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftDown)
                {
                    keypadMappingProfile.ThumbLeftDownMod = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftDown = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightLeft)
                {
                    keypadMappingProfile.ThumbRightLeftMod = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightUp)
                {
                    keypadMappingProfile.ThumbRightUpMod = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightUp = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightRight)
                {
                    keypadMappingProfile.ThumbRightRightMod = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightDown)
                {
                    keypadMappingProfile.ThumbRightDownMod = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightDown = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadA)
                {
                    keypadMappingProfile.ButtonAMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonA = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadB)
                {
                    keypadMappingProfile.ButtonBMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonB = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadX)
                {
                    keypadMappingProfile.ButtonXMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonX = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadY)
                {
                    keypadMappingProfile.ButtonYMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonY = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderLeft)
                {
                    keypadMappingProfile.ButtonShoulderLeftMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonShoulderLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderRight)
                {
                    keypadMappingProfile.ButtonShoulderRightMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonShoulderRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadBack)
                {
                    keypadMappingProfile.ButtonBackMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonBack = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadStart)
                {
                    keypadMappingProfile.ButtonStartMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonStart = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeft)
                {
                    keypadMappingProfile.ButtonThumbLeftMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonThumbLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRight)
                {
                    keypadMappingProfile.ButtonThumbRightMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonThumbRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerLeft)
                {
                    keypadMappingProfile.ButtonTriggerLeftMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonTriggerLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerRight)
                {
                    keypadMappingProfile.ButtonTriggerRightMod = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonTriggerRight = KeyboardKeys.None;
                }

                //Save changes to Json file
                JsonSaveObject(vDirectKeypadMapping, @"User\DirectKeypadMapping3");

                //Update the keypad names
                App.vWindowKeypad.UpdateKeypadNames();

                //Update the keypad tooltips
                UpdateKeypadToolTips();
            }
            catch { }
        }

        //Save keypad button mapping
        void ComboBox_MapKeypad_Save(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Check if combobox saving is enabled
                if (!vComboboxSaveEnabled) { return; }

                //Get selected keypad buttons
                KeyboardModifiers usedModifierKey = (KeyboardModifiers)combobox_SetPad_Modifier.SelectedItem;
                KeyboardKeys usedVirtualKey = (KeyboardKeys)combobox_SetPad_Keyboard.SelectedItem;

                //Get keypad mapping profile
                KeypadMapping keypadMappingProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                //Update mapping information
                string mapNameString = vMappingKeypadButton.Tag.ToString();
                Debug.WriteLine("Mapped button: " + mapNameString);
                txt_KeypadMap_Status.Text = "Mapped button " + mapNameString + " to: " + usedModifierKey + " / " + usedVirtualKey;

                if (vMappingKeypadButton == btn_SetPadDPadLeft)
                {
                    keypadMappingProfile.DPadLeftMod = usedModifierKey;
                    keypadMappingProfile.DPadLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadUp)
                {
                    keypadMappingProfile.DPadUpMod = usedModifierKey;
                    keypadMappingProfile.DPadUp = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadRight)
                {
                    keypadMappingProfile.DPadRightMod = usedModifierKey;
                    keypadMappingProfile.DPadRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadDown)
                {
                    keypadMappingProfile.DPadDownMod = usedModifierKey;
                    keypadMappingProfile.DPadDown = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftLeft)
                {
                    keypadMappingProfile.ThumbLeftLeftMod = usedModifierKey;
                    keypadMappingProfile.ThumbLeftLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftUp)
                {
                    keypadMappingProfile.ThumbLeftUpMod = usedModifierKey;
                    keypadMappingProfile.ThumbLeftUp = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftRight)
                {
                    keypadMappingProfile.ThumbLeftRightMod = usedModifierKey;
                    keypadMappingProfile.ThumbLeftRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftDown)
                {
                    keypadMappingProfile.ThumbLeftDownMod = usedModifierKey;
                    keypadMappingProfile.ThumbLeftDown = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightLeft)
                {
                    keypadMappingProfile.ThumbRightLeftMod = usedModifierKey;
                    keypadMappingProfile.ThumbRightLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightUp)
                {
                    keypadMappingProfile.ThumbRightUpMod = usedModifierKey;
                    keypadMappingProfile.ThumbRightUp = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightRight)
                {
                    keypadMappingProfile.ThumbRightRightMod = usedModifierKey;
                    keypadMappingProfile.ThumbRightRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightDown)
                {
                    keypadMappingProfile.ThumbRightDownMod = usedModifierKey;
                    keypadMappingProfile.ThumbRightDown = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadA)
                {
                    keypadMappingProfile.ButtonAMod = usedModifierKey;
                    keypadMappingProfile.ButtonA = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadB)
                {
                    keypadMappingProfile.ButtonBMod = usedModifierKey;
                    keypadMappingProfile.ButtonB = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadX)
                {
                    keypadMappingProfile.ButtonXMod = usedModifierKey;
                    keypadMappingProfile.ButtonX = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadY)
                {
                    keypadMappingProfile.ButtonYMod = usedModifierKey;
                    keypadMappingProfile.ButtonY = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderLeft)
                {
                    keypadMappingProfile.ButtonShoulderLeftMod = usedModifierKey;
                    keypadMappingProfile.ButtonShoulderLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderRight)
                {
                    keypadMappingProfile.ButtonShoulderRightMod = usedModifierKey;
                    keypadMappingProfile.ButtonShoulderRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadBack)
                {
                    keypadMappingProfile.ButtonBackMod = usedModifierKey;
                    keypadMappingProfile.ButtonBack = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadStart)
                {
                    keypadMappingProfile.ButtonStartMod = usedModifierKey;
                    keypadMappingProfile.ButtonStart = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeft)
                {
                    keypadMappingProfile.ButtonThumbLeftMod = usedModifierKey;
                    keypadMappingProfile.ButtonThumbLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRight)
                {
                    keypadMappingProfile.ButtonThumbRightMod = usedModifierKey;
                    keypadMappingProfile.ButtonThumbRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerLeft)
                {
                    keypadMappingProfile.ButtonTriggerLeftMod = usedModifierKey;
                    keypadMappingProfile.ButtonTriggerLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerRight)
                {
                    keypadMappingProfile.ButtonTriggerRightMod = usedModifierKey;
                    keypadMappingProfile.ButtonTriggerRight = usedVirtualKey;
                }

                //Save changes to Json file
                JsonSaveObject(vDirectKeypadMapping, @"User\DirectKeypadMapping3");

                //Update the keypad names
                App.vWindowKeypad.UpdateKeypadNames();

                //Update the keypad tooltips
                UpdateKeypadToolTips();
            }
            catch { }
        }

        void ComboBox_MapKeypad_Load()
        {
            try
            {
                combobox_SetPad_Modifier.ItemsSource = Enum.GetValues(typeof(KeyboardModifiers));
                combobox_SetPad_Modifier.SelectedItem = KeyboardModifiers.None;

                combobox_SetPad_Keyboard.ItemsSource = Enum.GetValues(typeof(KeyboardKeys));
                combobox_SetPad_Keyboard.SelectedItem = KeyboardKeys.None;
            }
            catch { }
        }
    }
}