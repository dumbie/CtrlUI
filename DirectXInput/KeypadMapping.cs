using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVJsonFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
using static LibraryShared.Classes;
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
                vComboboxSaveEnabled = false;

                //Select combobox index
                if (vMappingKeypadButton == btn_SetPadDPadLeft)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.DPadLeftMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.DPadLeftMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.DPadLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadUp)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.DPadUpMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.DPadUpMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.DPadUp;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadRight)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.DPadRightMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.DPadRightMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.DPadRight;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadDown)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.DPadDownMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.DPadDownMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.DPadDown;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftLeft)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ThumbLeftLeftMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ThumbLeftLeftMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbLeftLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftUp)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ThumbLeftUpMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ThumbLeftUpMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbLeftUp;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftRight)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ThumbLeftRightMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ThumbLeftRightMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbLeftRight;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftDown)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ThumbLeftDownMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ThumbLeftDownMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbLeftDown;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightLeft)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ThumbRightLeftMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ThumbRightLeftMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbRightLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightUp)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ThumbRightUpMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ThumbRightUpMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbRightUp;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightRight)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ThumbRightRightMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ThumbRightRightMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbRightRight;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightDown)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ThumbRightDownMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ThumbRightDownMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ThumbRightDown;
                }
                else if (vMappingKeypadButton == btn_SetPadA)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonAMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonAMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonA;
                }
                else if (vMappingKeypadButton == btn_SetPadB)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonBMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonBMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonB;
                }
                else if (vMappingKeypadButton == btn_SetPadX)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonXMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonXMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonX;
                }
                else if (vMappingKeypadButton == btn_SetPadY)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonYMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonYMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonY;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderLeft)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonShoulderLeftMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonShoulderLeftMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonShoulderLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderRight)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonShoulderRightMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonShoulderRightMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonShoulderRight;
                }
                else if (vMappingKeypadButton == btn_SetPadBack)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonBackMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonBackMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonBack;
                }
                else if (vMappingKeypadButton == btn_SetPadStart)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonStartMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonStartMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonStart;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeft)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonThumbLeftMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonThumbLeftMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonThumbLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRight)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonThumbRightMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonThumbRightMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonThumbRight;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerLeft)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonTriggerLeftMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonTriggerLeftMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonTriggerLeft;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerRight)
                {
                    combobox_SetPad_Modifier0.SelectedItem = keypadMappingProfile.ButtonTriggerRightMod0;
                    combobox_SetPad_Modifier1.SelectedItem = keypadMappingProfile.ButtonTriggerRightMod1;
                    combobox_SetPad_Keyboard.SelectedItem = keypadMappingProfile.ButtonTriggerRight;
                }

                //Enable combobox event
                vComboboxSaveEnabled = true;
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
                combobox_SetPad_Modifier0.SelectedItem = KeyboardModifiers.None;
                combobox_SetPad_Modifier1.SelectedItem = KeyboardModifiers.None;
                combobox_SetPad_Keyboard.SelectedItem = KeyboardKeys.None;
                vComboboxSaveEnabled = true;

                //Get keypad mapping profile
                KeypadMapping keypadMappingProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                //Store new keypad mapping in Json
                if (vMappingKeypadButton == btn_SetPadDPadLeft)
                {
                    keypadMappingProfile.DPadLeftMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.DPadLeftMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.DPadLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadUp)
                {
                    keypadMappingProfile.DPadUpMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.DPadUpMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.DPadUp = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadRight)
                {
                    keypadMappingProfile.DPadRightMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.DPadRightMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.DPadRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadDown)
                {
                    keypadMappingProfile.DPadDownMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.DPadDownMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.DPadDown = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftLeft)
                {
                    keypadMappingProfile.ThumbLeftLeftMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftLeftMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftUp)
                {
                    keypadMappingProfile.ThumbLeftUpMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftUpMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftUp = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftRight)
                {
                    keypadMappingProfile.ThumbLeftRightMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftRightMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftDown)
                {
                    keypadMappingProfile.ThumbLeftDownMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftDownMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbLeftDown = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightLeft)
                {
                    keypadMappingProfile.ThumbRightLeftMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightLeftMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightUp)
                {
                    keypadMappingProfile.ThumbRightUpMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightUpMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightUp = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightRight)
                {
                    keypadMappingProfile.ThumbRightRightMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightRightMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightDown)
                {
                    keypadMappingProfile.ThumbRightDownMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightDownMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ThumbRightDown = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadA)
                {
                    keypadMappingProfile.ButtonAMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonAMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonA = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadB)
                {
                    keypadMappingProfile.ButtonBMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonBMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonB = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadX)
                {
                    keypadMappingProfile.ButtonXMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonXMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonX = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadY)
                {
                    keypadMappingProfile.ButtonYMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonYMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonY = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderLeft)
                {
                    keypadMappingProfile.ButtonShoulderLeftMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonShoulderLeftMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonShoulderLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderRight)
                {
                    keypadMappingProfile.ButtonShoulderRightMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonShoulderRightMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonShoulderRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadBack)
                {
                    keypadMappingProfile.ButtonBackMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonBackMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonBack = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadStart)
                {
                    keypadMappingProfile.ButtonStartMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonStartMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonStart = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeft)
                {
                    keypadMappingProfile.ButtonThumbLeftMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonThumbLeftMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonThumbLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRight)
                {
                    keypadMappingProfile.ButtonThumbRightMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonThumbRightMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonThumbRight = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerLeft)
                {
                    keypadMappingProfile.ButtonTriggerLeftMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonTriggerLeftMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonTriggerLeft = KeyboardKeys.None;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerRight)
                {
                    keypadMappingProfile.ButtonTriggerRightMod0 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonTriggerRightMod1 = KeyboardModifiers.None;
                    keypadMappingProfile.ButtonTriggerRight = KeyboardKeys.None;
                }

                //Save changes to Json file
                JsonSaveObject(keypadMappingProfile, GenerateJsonNameKeypadMapping(keypadMappingProfile));

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
                KeyboardModifiers usedModifierKey0 = (KeyboardModifiers)combobox_SetPad_Modifier0.SelectedItem;
                KeyboardModifiers usedModifierKey1 = (KeyboardModifiers)combobox_SetPad_Modifier1.SelectedItem;
                KeyboardKeys usedVirtualKey = (KeyboardKeys)combobox_SetPad_Keyboard.SelectedItem;

                //Get keypad mapping profile
                KeypadMapping keypadMappingProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                //Update mapping information
                string mapNameString = vMappingKeypadButton.Tag.ToString();
                Debug.WriteLine("Mapped button: " + mapNameString);
                txt_KeypadMap_Status.Text = "Mapped button " + mapNameString + " to: " + usedModifierKey0 + " / " + usedModifierKey1 + " / " + usedVirtualKey;

                if (vMappingKeypadButton == btn_SetPadDPadLeft)
                {
                    keypadMappingProfile.DPadLeftMod0 = usedModifierKey0;
                    keypadMappingProfile.DPadLeftMod1 = usedModifierKey1;
                    keypadMappingProfile.DPadLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadUp)
                {
                    keypadMappingProfile.DPadUpMod0 = usedModifierKey0;
                    keypadMappingProfile.DPadUpMod1 = usedModifierKey1;
                    keypadMappingProfile.DPadUp = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadRight)
                {
                    keypadMappingProfile.DPadRightMod0 = usedModifierKey0;
                    keypadMappingProfile.DPadRightMod1 = usedModifierKey1;
                    keypadMappingProfile.DPadRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadDPadDown)
                {
                    keypadMappingProfile.DPadDownMod0 = usedModifierKey0;
                    keypadMappingProfile.DPadDownMod1 = usedModifierKey1;
                    keypadMappingProfile.DPadDown = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftLeft)
                {
                    keypadMappingProfile.ThumbLeftLeftMod0 = usedModifierKey0;
                    keypadMappingProfile.ThumbLeftLeftMod1 = usedModifierKey1;
                    keypadMappingProfile.ThumbLeftLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftUp)
                {
                    keypadMappingProfile.ThumbLeftUpMod0 = usedModifierKey0;
                    keypadMappingProfile.ThumbLeftUpMod1 = usedModifierKey1;
                    keypadMappingProfile.ThumbLeftUp = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftRight)
                {
                    keypadMappingProfile.ThumbLeftRightMod0 = usedModifierKey0;
                    keypadMappingProfile.ThumbLeftRightMod1 = usedModifierKey1;
                    keypadMappingProfile.ThumbLeftRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeftDown)
                {
                    keypadMappingProfile.ThumbLeftDownMod0 = usedModifierKey0;
                    keypadMappingProfile.ThumbLeftDownMod1 = usedModifierKey1;
                    keypadMappingProfile.ThumbLeftDown = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightLeft)
                {
                    keypadMappingProfile.ThumbRightLeftMod0 = usedModifierKey0;
                    keypadMappingProfile.ThumbRightLeftMod1 = usedModifierKey1;
                    keypadMappingProfile.ThumbRightLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightUp)
                {
                    keypadMappingProfile.ThumbRightUpMod0 = usedModifierKey0;
                    keypadMappingProfile.ThumbRightUpMod1 = usedModifierKey1;
                    keypadMappingProfile.ThumbRightUp = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightRight)
                {
                    keypadMappingProfile.ThumbRightRightMod0 = usedModifierKey0;
                    keypadMappingProfile.ThumbRightRightMod1 = usedModifierKey1;
                    keypadMappingProfile.ThumbRightRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRightDown)
                {
                    keypadMappingProfile.ThumbRightDownMod0 = usedModifierKey0;
                    keypadMappingProfile.ThumbRightDownMod1 = usedModifierKey1;
                    keypadMappingProfile.ThumbRightDown = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadA)
                {
                    keypadMappingProfile.ButtonAMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonAMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonA = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadB)
                {
                    keypadMappingProfile.ButtonBMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonBMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonB = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadX)
                {
                    keypadMappingProfile.ButtonXMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonXMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonX = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadY)
                {
                    keypadMappingProfile.ButtonYMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonYMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonY = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderLeft)
                {
                    keypadMappingProfile.ButtonShoulderLeftMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonShoulderLeftMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonShoulderLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadShoulderRight)
                {
                    keypadMappingProfile.ButtonShoulderRightMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonShoulderRightMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonShoulderRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadBack)
                {
                    keypadMappingProfile.ButtonBackMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonBackMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonBack = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadStart)
                {
                    keypadMappingProfile.ButtonStartMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonStartMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonStart = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbLeft)
                {
                    keypadMappingProfile.ButtonThumbLeftMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonThumbLeftMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonThumbLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadThumbRight)
                {
                    keypadMappingProfile.ButtonThumbRightMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonThumbRightMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonThumbRight = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerLeft)
                {
                    keypadMappingProfile.ButtonTriggerLeftMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonTriggerLeftMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonTriggerLeft = usedVirtualKey;
                }
                else if (vMappingKeypadButton == btn_SetPadTriggerRight)
                {
                    keypadMappingProfile.ButtonTriggerRightMod0 = usedModifierKey0;
                    keypadMappingProfile.ButtonTriggerRightMod1 = usedModifierKey1;
                    keypadMappingProfile.ButtonTriggerRight = usedVirtualKey;
                }

                //Save changes to Json file
                JsonSaveObject(keypadMappingProfile, GenerateJsonNameKeypadMapping(keypadMappingProfile));

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
                combobox_SetPad_Modifier0.ItemsSource = Enum.GetValues(typeof(KeyboardModifiers));
                combobox_SetPad_Modifier0.SelectedItem = KeyboardModifiers.None;

                combobox_SetPad_Modifier1.ItemsSource = Enum.GetValues(typeof(KeyboardModifiers));
                combobox_SetPad_Modifier1.SelectedItem = KeyboardModifiers.None;

                combobox_SetPad_Keyboard.ItemsSource = Enum.GetValues(typeof(KeyboardKeys));
                combobox_SetPad_Keyboard.SelectedItem = KeyboardKeys.None;
            }
            catch { }
        }
    }
}