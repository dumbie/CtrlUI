using ArnoldVinkCode.Styles;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            }
            catch { }
        }
    }
}