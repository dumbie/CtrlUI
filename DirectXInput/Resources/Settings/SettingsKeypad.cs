using ArnoldVinkCode.Styles;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
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
                    notificationDetailsExists.Text = "Application already exists";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetailsExists);
                    return;
                }

                //Add empty mapping to list
                KeypadMapping newProfile = new KeypadMapping();
                newProfile.Name = profileNameString;
                vDirectKeypadMapping.Add(newProfile);

                //Save changes to Json file
                JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

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

                    //Save changes to Json file
                    JsonSaveObject(vDirectKeypadMapping, "DirectKeypadMapping");

                    //Select the default profile
                    combobox_KeypadProcessProfile.SelectedIndex = 0;

                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "RemoveCross";
                    notificationDetails.Text = "Application removed";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    Debug.WriteLine("Removed the keypad profile.");
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Close";
                    notificationDetails.Text = "Cannot remove default";
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
                Load_Keypad_Profile();
            }
            catch { }
        }

        //Load keypad profile
        void Load_Keypad_Profile()
        {
            try
            {
                Debug.WriteLine("Loading keypad profile.");

                //Select the default profile
                if (combobox_KeypadProcessProfile.SelectedIndex == -1)
                {
                    combobox_KeypadProcessProfile.ItemsSource = vDirectKeypadMapping;
                    combobox_KeypadProcessProfile.DisplayMemberPath = "Name";
                    combobox_KeypadProcessProfile.SelectedIndex = 0;
                    return;
                }

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

                //Load keypad repeat interval
                textblock_KeypadRepeatIntervalMs.Text = textblock_KeypadRepeatIntervalMs.Tag + ": " + selectedProfile.ButtonRepeatIntervalMs + "ms";
                slider_KeypadRepeatIntervalMs.Value = selectedProfile.ButtonRepeatIntervalMs;

                //Update all keypad key tool tips
                UpdateKeypadToolTips();
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
                btn_SetPadDPadLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.DPadLeftMod, selectedProfile.DPadLeft, btn_SetPadDPadLeft.Tag.ToString()) };
                btn_SetPadDPadUp.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.DPadUpMod, selectedProfile.DPadUp, btn_SetPadDPadUp.Tag.ToString()) };
                btn_SetPadDPadRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.DPadRightMod, selectedProfile.DPadRight, btn_SetPadDPadRight.Tag.ToString()) };
                btn_SetPadDPadDown.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.DPadDownMod, selectedProfile.DPadDown, btn_SetPadDPadDown.Tag.ToString()) };

                btn_SetPadThumbLeftLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbLeftLeftMod, selectedProfile.ThumbLeftLeft, btn_SetPadThumbLeftLeft.Tag.ToString()) };
                btn_SetPadThumbLeftUp.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbLeftUpMod, selectedProfile.ThumbLeftUp, btn_SetPadThumbLeftUp.Tag.ToString()) };
                btn_SetPadThumbLeftRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbLeftRightMod, selectedProfile.ThumbLeftRight, btn_SetPadThumbLeftRight.Tag.ToString()) };
                btn_SetPadThumbLeftDown.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbLeftDownMod, selectedProfile.ThumbLeftDown, btn_SetPadThumbLeftDown.Tag.ToString()) };
                btn_SetPadThumbLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonThumbLeftMod, selectedProfile.ButtonThumbLeft, btn_SetPadThumbLeft.Tag.ToString()) };

                btn_SetPadThumbRightLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbRightLeftMod, selectedProfile.ThumbRightLeft, btn_SetPadThumbRightLeft.Tag.ToString()) };
                btn_SetPadThumbRightUp.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbRightUpMod, selectedProfile.ThumbRightUp, btn_SetPadThumbRightUp.Tag.ToString()) };
                btn_SetPadThumbRightRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbRightRightMod, selectedProfile.ThumbRightRight, btn_SetPadThumbRightRight.Tag.ToString()) };
                btn_SetPadThumbRightDown.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ThumbRightDownMod, selectedProfile.ThumbRightDown, btn_SetPadThumbRightDown.Tag.ToString()) };
                btn_SetPadThumbRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonThumbRightMod, selectedProfile.ButtonThumbRight, btn_SetPadThumbRight.Tag.ToString()) };

                btn_SetPadBack.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonBackMod, selectedProfile.ButtonBack, btn_SetPadBack.Tag.ToString()) };
                btn_SetPadStart.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonStartMod, selectedProfile.ButtonStart, btn_SetPadStart.Tag.ToString()) };

                btn_SetPadX.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonXMod, selectedProfile.ButtonX, btn_SetPadX.Tag.ToString()) };
                btn_SetPadY.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonYMod, selectedProfile.ButtonY, btn_SetPadY.Tag.ToString()) };
                btn_SetPadA.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonAMod, selectedProfile.ButtonA, btn_SetPadA.Tag.ToString()) };
                btn_SetPadB.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonBMod, selectedProfile.ButtonB, btn_SetPadB.Tag.ToString()) };

                btn_SetPadShoulderLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonShoulderLeftMod, selectedProfile.ButtonShoulderLeft, btn_SetPadShoulderLeft.Tag.ToString()) };
                btn_SetPadTriggerLeft.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonTriggerLeftMod, selectedProfile.ButtonTriggerLeft, btn_SetPadTriggerLeft.Tag.ToString()) };
                btn_SetPadShoulderRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonShoulderRightMod, selectedProfile.ButtonShoulderRight, btn_SetPadShoulderRight.Tag.ToString()) };
                btn_SetPadTriggerRight.ToolTip = new ToolTip() { Content = GenerateKeypadKeyToolTip(selectedProfile.ButtonTriggerRightMod, selectedProfile.ButtonTriggerRight, btn_SetPadTriggerRight.Tag.ToString()) };
            }
            catch { }
        }

        //Generate keypad tool tip
        string GenerateKeypadKeyToolTip(KeysVirtual? modifierKey, KeysVirtual? virtualKey, string keyName)
        {
            try
            {
                if (modifierKey != null)
                {
                    return keyName + " is mapped to " + GetVirtualKeyName((KeysVirtual)modifierKey, false) + " / " + GetVirtualKeyName((KeysVirtual)virtualKey, false);
                }
                else if (virtualKey != null)
                {
                    return keyName + " is mapped to " + GetVirtualKeyName((KeysVirtual)virtualKey, false);
                }
            }
            catch { }
            return keyName + " is not mapped";
        }
    }
}