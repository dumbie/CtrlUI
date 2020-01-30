using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show profile manager popup
        async Task Popup_Show_ProfileManager()
        {
            try
            {
                //Show the manage popup
                await Popup_Show(grid_Popup_ProfileManager, grid_Popup_ProfileManager_button_ChangeProfile, true);

                //Load profile in manager
                await ProfileManager_LoadProfile();
            }
            catch { }
        }

        //Load profile in manager
        async Task ProfileManager_LoadProfile()
        {
            try
            {
                Debug.WriteLine("Changing edit profile to: " + vProfileManagerName);

                //Load the requested profile values
                if (vProfileManagerName == "CtrlIgnoreProcessName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Ignored process names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Process name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlIgnoreProcessName;
                    lb_ProfileManager.ItemsSource = vCtrlIgnoreProcessName;
                }
                else if (vProfileManagerName == "CtrlLocationsShortcut")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Shortcut locations";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Path";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlLocationsShortcut;
                    lb_ProfileManager.ItemsSource = vCtrlLocationsShortcut;
                }
                else if (vProfileManagerName == "CtrlLocationsFile")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "File browser locations";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Visible;
                    grid_Popup_ProfileManager_textblock_ProfileString2.Text = "Path";

                    vProfileManagerListShared = vCtrlLocationsFile;
                    lb_ProfileManager.ItemsSource = vCtrlLocationsFile;
                }
                else if (vProfileManagerName == "CtrlIgnoreShortcutName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Ignored shortcuts names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Shortcut name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlIgnoreShortcutName;
                    lb_ProfileManager.ItemsSource = vCtrlIgnoreShortcutName;
                }
                else if (vProfileManagerName == "CtrlIgnoreShortcutUri")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Ignored shortcut uri's";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Shortcut uri";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlIgnoreShortcutUri;
                    lb_ProfileManager.ItemsSource = vCtrlIgnoreShortcutUri;
                }
                else if (vProfileManagerName == "CtrlKeyboardProcessName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Keyboard open process names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Process name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlKeyboardProcessName;
                    lb_ProfileManager.ItemsSource = vCtrlKeyboardProcessName;
                }

                //Select the first listbox item
                await ListBoxFocusOrSelectIndex(lb_ProfileManager, true, false, 0);
            }
            catch { }
        }

        //Delete the edit profile
        async Task ProfileManager_DeleteProfile()
        {
            try
            {
                ProfileShared selectedProfile = (ProfileShared)lb_ProfileManager.SelectedItem;
                Debug.WriteLine("Removing profile value: " + selectedProfile);

                //Remove the selected profile value
                await ListBoxRemoveItem(lb_ProfileManager, vProfileManagerListShared, selectedProfile);

                //Save the updated json values
                JsonSaveObject(vProfileManagerListShared, vProfileManagerName);

                Popup_Show_Status("Profile", "Removed profile value");
            }
            catch { }
        }

        //Add new profile value
        private async Task AddSaveNewProfileValue()
        {
            try
            {
                string profileString1 = grid_Popup_ProfileManager_textbox_ProfileString1.Text;
                string profileString2 = grid_Popup_ProfileManager_textbox_ProfileString2.Text;
                Debug.WriteLine("Adding new profile value: " + profileString1 + " / " + profileString2);

                //Color brushes
                BrushConverter BrushConvert = new BrushConverter();
                Brush BrushInvalid = BrushConvert.ConvertFromString("#CD1A2B") as Brush;
                Brush BrushValid = BrushConvert.ConvertFromString("#1DB954") as Brush;

                //Check if the string1 is empty
                if (string.IsNullOrWhiteSpace(profileString1))
                {
                    grid_Popup_ProfileManager_textbox_ProfileString1.BorderBrush = BrushInvalid;
                    Popup_Show_Status("Profile", "Empty profile value");
                    Debug.WriteLine("Please enter a profile value.");
                    return;
                }

                //Check if the string2 is empty
                if (grid_Popup_ProfileManager_Value2.Visibility == Visibility.Visible && string.IsNullOrWhiteSpace(profileString2))
                {
                    grid_Popup_ProfileManager_textbox_ProfileString2.BorderBrush = BrushInvalid;
                    Popup_Show_Status("Profile", "Empty profile value");
                    Debug.WriteLine("Please enter a profile value.");
                    return;
                }

                //Create new profile shared
                ProfileShared profileShared = new ProfileShared();
                Func<ProfileShared, bool> profileFilter = null;

                //Check first string value
                if (!string.IsNullOrWhiteSpace(profileString1))
                {
                    profileShared.String1 = profileString1;
                    profileFilter = x => x.String1.ToLower() == profileString1.ToLower();
                }

                //Check second string value
                if (!string.IsNullOrWhiteSpace(profileString2))
                {
                    profileShared.String2 = profileString2;
                    profileFilter = x => x.String1.ToLower() == profileString1.ToLower() && x.String2.ToLower() == profileString2.ToLower();
                }

                //Check if values already exists
                if (vProfileManagerListShared.Any(profileFilter))
                {
                    grid_Popup_ProfileManager_textbox_ProfileString1.BorderBrush = BrushInvalid;
                    grid_Popup_ProfileManager_textbox_ProfileString2.BorderBrush = BrushInvalid;
                    Popup_Show_Status("Profile", "Profile already exists");
                    Debug.WriteLine("Profile value already exists.");
                    return;
                }

                //Clear added value from the textbox
                grid_Popup_ProfileManager_textbox_ProfileString1.Text = string.Empty;
                grid_Popup_ProfileManager_textbox_ProfileString2.Text = string.Empty;

                //Add the new profile value
                await ListBoxAddItem(lb_ProfileManager, vProfileManagerListShared, profileShared, false, false);

                //Save the updated json values
                JsonSaveObject(vProfileManagerListShared, vProfileManagerName);

                //Show profile added notification
                Popup_Show_Status("Profile", "New value added");
                grid_Popup_ProfileManager_textbox_ProfileString1.BorderBrush = BrushValid;
                grid_Popup_ProfileManager_textbox_ProfileString2.BorderBrush = BrushValid;
            }
            catch { }
        }

        //Change the edit profile category
        private async Task ChangeProfileCategory()
        {
            try
            {
                //Add profile categories
                vFilePickerStrings.Clear();

                BitmapImage imageProfile = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Profile.png" }, IntPtr.Zero, -1);

                DataBindString stringCtrlLocationsShortcut = new DataBindString() { Name = "Shortcut locations", NameDetail = "CtrlLocationsShortcut", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringCtrlLocationsShortcut);

                DataBindString stringCtrlLocationsFile = new DataBindString() { Name = "File browser locations", NameDetail = "CtrlLocationsFile", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringCtrlLocationsFile);

                DataBindString stringCtrlIgnoreProcessName = new DataBindString() { Name = "Ignored process names", NameDetail = "CtrlIgnoreProcessName", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringCtrlIgnoreProcessName);

                DataBindString stringCtrlIgnoreShortcutName = new DataBindString() { Name = "Ignored shortcuts names", NameDetail = "CtrlIgnoreShortcutName", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringCtrlIgnoreShortcutName);

                DataBindString stringCtrlIgnoreShortcutUri = new DataBindString() { Name = "Ignored shortcut uri's", NameDetail = "CtrlIgnoreShortcutUri", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringCtrlIgnoreShortcutUri);

                DataBindString stringCtrlKeyboardProcessName = new DataBindString() { Name = "Keyboard open process names", NameDetail = "CtrlKeyboardProcessName", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringCtrlKeyboardProcessName);

                //Show the category picker
                vFilePickerFilterIn = new List<string>();
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Profile Category";
                vFilePickerDescription = "Please select the profile to manage:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = false;
                vFilePickerShowDirectories = false;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("String", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Set the selected profile category
                vProfileManagerName = vFilePickerResult.PathFile;

                //Load profile in manager
                await ProfileManager_LoadProfile();
            }
            catch { }
        }
    }
}