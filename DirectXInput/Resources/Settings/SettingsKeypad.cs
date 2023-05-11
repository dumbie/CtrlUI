using ArnoldVinkCode;
using ArnoldVinkCode.Styles;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVJsonFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Profile - Keypad add profile
        void Btn_Settings_KeypadProcessProfile_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string profileNameString = textbox_Settings_KeypadProcessProfile_Name.Text;
                string placeholderString = (string)textbox_Settings_KeypadProcessProfile_Name.GetValue(TextboxPlaceholder.PlaceholderProperty);

                //Check if profile name is set
                if (string.IsNullOrWhiteSpace(profileNameString) || profileNameString == placeholderString)
                {
                    NotificationDetails notificationDetailsNameSet = new NotificationDetails();
                    notificationDetailsNameSet.Icon = "Close";
                    notificationDetailsNameSet.Text = "No application name set";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetailsNameSet);
                    return;
                }

                //Check if profile name exists
                if (vDirectKeypadMapping.Any(x => x.Name.ToLower() == profileNameString.ToLower()))
                {
                    NotificationDetails notificationDetailsExists = new NotificationDetails();
                    notificationDetailsExists.Icon = "Close";
                    notificationDetailsExists.Text = "Keypad profile already exists";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetailsExists);
                    return;
                }

                //Add empty mapping to list
                KeypadMapping keypadMapping = new KeypadMapping();
                keypadMapping.Name = profileNameString;

                //Add profile to list
                vDirectKeypadMapping.Add(keypadMapping);

                //Save profile to Json file
                JsonSaveObject(keypadMapping, GenerateJsonNameKeypadMapping(keypadMapping));

                //Show notification
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Plus";
                notificationDetails.Text = "Added keypad profile";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                Debug.WriteLine("Added keypad profile.");
            }
            catch { }
        }

        //Profile - Keypad remove profile
        void Btn_Settings_KeypadProcessProfile_Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;
                if (selectedProfile.Name != "Default")
                {
                    //Remove mapping from list
                    vDirectKeypadMapping.Remove(selectedProfile);

                    //Remove Json file
                    AVFiles.File_Delete(GenerateJsonNameKeypadMapping(selectedProfile));

                    //Select the default profile
                    combobox_KeypadProcessProfile.SelectedIndex = 0;

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "RemoveCross";
                    notificationDetails.Text = "Removed keypad profile";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    Debug.WriteLine("Removed the keypad profile.");
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Close";
                    notificationDetails.Text = "Cannot remove default profile";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    Debug.WriteLine("Default profile cannot be removed.");
                }
            }
            catch { }
        }

        //Load and set keypad profile
        void Combobox_KeypadProcessProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UpdateKeypadInterface();
            }
            catch { }
        }

        //Update keypad interface
        void UpdateKeypadInterface()
        {
            try
            {
                //Get current selected profile
                KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                //Load keypad opacity
                textblock_KeypadOpacity.Text = textblock_KeypadOpacity.Tag + ": " + selectedProfile.KeypadOpacity.ToString("0.00") + "%";
                slider_KeypadOpacity.Value = selectedProfile.KeypadOpacity;

                //Load keypad display style
                combobox_KeypadDisplayStyle.SelectedIndex = selectedProfile.KeypadDisplayStyle;

                //Load keypad display size
                textblock_KeypadDisplaySize.Text = textblock_KeypadDisplaySize.Tag + ": " + selectedProfile.KeypadDisplaySize + "%";
                slider_KeypadDisplaySize.Value = selectedProfile.KeypadDisplaySize;

                //Load keypad mouse enabled
                cb_SettingsKeypadMouseMoveEnabled.IsChecked = selectedProfile.KeypadMouseMoveEnabled;

                //Load keypad mouse sensitivity
                textblock_SettingsKeypadMouseMoveSensitivity.Text = textblock_SettingsKeypadMouseMoveSensitivity.Tag + ": " + selectedProfile.KeypadMouseMoveSensitivity.ToString("0.00");
                slider_SettingsKeypadMouseMoveSensitivity.Value = selectedProfile.KeypadMouseMoveSensitivity;

                //Update all keypad tooltips
                UpdateKeypadToolTips();

                Debug.WriteLine("Updated keypad interface.");
            }
            catch { }
        }

        //Update all keypad key tool tips
        public void UpdateKeypadToolTips()
        {
            try
            {
                //Get current selected profile
                KeypadMapping selectedProfile = (KeypadMapping)combobox_KeypadProcessProfile.SelectedItem;

                //Update the keypad tool tips
                btn_SetPadDPadLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.DPadLeftMod0, selectedProfile.DPadLeftMod1, selectedProfile.DPadLeft, btn_SetPadDPadLeft.Tag.ToString()) };
                btn_SetPadDPadUp.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.DPadUpMod0, selectedProfile.DPadUpMod1, selectedProfile.DPadUp, btn_SetPadDPadUp.Tag.ToString()) };
                btn_SetPadDPadRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.DPadRightMod0, selectedProfile.DPadRightMod1, selectedProfile.DPadRight, btn_SetPadDPadRight.Tag.ToString()) };
                btn_SetPadDPadDown.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.DPadDownMod0, selectedProfile.DPadDownMod1, selectedProfile.DPadDown, btn_SetPadDPadDown.Tag.ToString()) };

                btn_SetPadThumbLeftLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbLeftLeftMod0, selectedProfile.ThumbLeftLeftMod1, selectedProfile.ThumbLeftLeft, btn_SetPadThumbLeftLeft.Tag.ToString()) };
                btn_SetPadThumbLeftUp.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbLeftUpMod0, selectedProfile.ThumbLeftUpMod1, selectedProfile.ThumbLeftUp, btn_SetPadThumbLeftUp.Tag.ToString()) };
                btn_SetPadThumbLeftRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbLeftRightMod0, selectedProfile.ThumbLeftRightMod1, selectedProfile.ThumbLeftRight, btn_SetPadThumbLeftRight.Tag.ToString()) };
                btn_SetPadThumbLeftDown.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbLeftDownMod0, selectedProfile.ThumbLeftDownMod1, selectedProfile.ThumbLeftDown, btn_SetPadThumbLeftDown.Tag.ToString()) };
                btn_SetPadThumbLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonThumbLeftMod0, selectedProfile.ButtonThumbLeftMod1, selectedProfile.ButtonThumbLeft, btn_SetPadThumbLeft.Tag.ToString()) };

                btn_SetPadThumbRightLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbRightLeftMod0, selectedProfile.ThumbRightLeftMod1, selectedProfile.ThumbRightLeft, btn_SetPadThumbRightLeft.Tag.ToString()) };
                btn_SetPadThumbRightUp.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbRightUpMod0, selectedProfile.ThumbRightUpMod1, selectedProfile.ThumbRightUp, btn_SetPadThumbRightUp.Tag.ToString()) };
                btn_SetPadThumbRightRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbRightRightMod0, selectedProfile.ThumbRightRightMod1, selectedProfile.ThumbRightRight, btn_SetPadThumbRightRight.Tag.ToString()) };
                btn_SetPadThumbRightDown.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbRightDownMod0, selectedProfile.ThumbRightDownMod1, selectedProfile.ThumbRightDown, btn_SetPadThumbRightDown.Tag.ToString()) };
                btn_SetPadThumbRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonThumbRightMod0, selectedProfile.ButtonThumbRightMod1, selectedProfile.ButtonThumbRight, btn_SetPadThumbRight.Tag.ToString()) };

                btn_SetPadBack.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonBackMod0, selectedProfile.ButtonBackMod1, selectedProfile.ButtonBack, btn_SetPadBack.Tag.ToString()) };
                btn_SetPadStart.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonStartMod0, selectedProfile.ButtonStartMod1, selectedProfile.ButtonStart, btn_SetPadStart.Tag.ToString()) };

                btn_SetPadX.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonXMod0, selectedProfile.ButtonXMod1, selectedProfile.ButtonX, btn_SetPadX.Tag.ToString()) };
                btn_SetPadY.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonYMod0, selectedProfile.ButtonYMod1, selectedProfile.ButtonY, btn_SetPadY.Tag.ToString()) };
                btn_SetPadA.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonAMod0, selectedProfile.ButtonAMod1, selectedProfile.ButtonA, btn_SetPadA.Tag.ToString()) };
                btn_SetPadB.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonBMod0, selectedProfile.ButtonBMod1, selectedProfile.ButtonB, btn_SetPadB.Tag.ToString()) };

                btn_SetPadShoulderLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonShoulderLeftMod0, selectedProfile.ButtonShoulderLeftMod1, selectedProfile.ButtonShoulderLeft, btn_SetPadShoulderLeft.Tag.ToString()) };
                btn_SetPadTriggerLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonTriggerLeftMod0, selectedProfile.ButtonTriggerLeftMod1, selectedProfile.ButtonTriggerLeft, btn_SetPadTriggerLeft.Tag.ToString()) };
                btn_SetPadShoulderRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonShoulderRightMod0, selectedProfile.ButtonShoulderRightMod1, selectedProfile.ButtonShoulderRight, btn_SetPadShoulderRight.Tag.ToString()) };
                btn_SetPadTriggerRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonTriggerRightMod0, selectedProfile.ButtonTriggerRightMod1, selectedProfile.ButtonTriggerRight, btn_SetPadTriggerRight.Tag.ToString()) };
            }
            catch { }
        }

        //Generate keypad tool tip
        string GenerateKeypadKeyToolTip(KeysModifierHid modifierKey0, KeysModifierHid modifierKey1, KeysHid virtualKey, string keyName)
        {
            try
            {
                if (modifierKey0 != KeysModifierHid.None && modifierKey1 != KeysModifierHid.None && virtualKey != KeysHid.None)
                {
                    return keyName + " is mapped to " + GetKeyboardModifiersName(modifierKey0, false) + " / " + GetKeyboardModifiersName(modifierKey1, false) + " / " + GetKeyboardKeysName(virtualKey, false);
                }
                else if (modifierKey0 != KeysModifierHid.None && modifierKey1 != KeysModifierHid.None)
                {
                    return keyName + " is mapped to " + GetKeyboardModifiersName(modifierKey0, false) + " / " + GetKeyboardModifiersName(modifierKey1, false);
                }
                else if (modifierKey0 != KeysModifierHid.None && virtualKey != KeysHid.None)
                {
                    return keyName + " is mapped to " + GetKeyboardModifiersName(modifierKey0, false) + " / " + GetKeyboardKeysName(virtualKey, false);
                }
                else if (modifierKey1 != KeysModifierHid.None && virtualKey != KeysHid.None)
                {
                    return keyName + " is mapped to " + GetKeyboardModifiersName(modifierKey1, false) + " / " + GetKeyboardKeysName(virtualKey, false);
                }
                else if (modifierKey0 != KeysModifierHid.None)
                {
                    return keyName + " is mapped to " + GetKeyboardModifiersName(modifierKey0, false);
                }
                else if (modifierKey1 != KeysModifierHid.None)
                {
                    return keyName + " is mapped to " + GetKeyboardModifiersName(modifierKey1, false);
                }
                else if (virtualKey != KeysHid.None)
                {
                    return keyName + " is mapped to " + GetKeyboardKeysName(virtualKey, false);
                }
            }
            catch { }
            return keyName + " is not mapped";
        }
    }
}