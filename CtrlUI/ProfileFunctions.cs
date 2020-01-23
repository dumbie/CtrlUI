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

                //Clear the current profile list
                List_ProfileManager.Clear();

                //Load the requested profile values
                if (vProfileManagerName == "CtrlIgnoreProcessName")
                {
                    grid_Popup_ProfileManager_button_ChangeProfile.Content = "Change edit profile (Blocked process names)";
                    vProfileManagerList = vCtrlIgnoreProcessName;
                    foreach (string profileString in vCtrlIgnoreProcessName)
                    {
                        await ListBoxAddItem(lb_ProfileManager, List_ProfileManager, profileString, false, false);
                    }
                }
                else if (vProfileManagerName == "CtrlLocationsShortcut")
                {
                    grid_Popup_ProfileManager_button_ChangeProfile.Content = "Change edit profile (Shortcut locations)";
                    vProfileManagerList = vCtrlLocationsShortcut;
                    foreach (string profileString in vCtrlLocationsShortcut)
                    {
                        await ListBoxAddItem(lb_ProfileManager, List_ProfileManager, profileString, false, false);
                    }
                }
                else if (vProfileManagerName == "CtrlLocationsFile")
                {
                    //grid_Popup_ProfileManager_button_ChangeProfile.Content = "Change edit profile (File browser locations)";
                    //vProfileManagerList = vCtrlLocationsFile;
                    //foreach (string profileString in vCtrlLocationsFile)
                    //{
                    //    await ListBoxAddItem(lb_ProfileManager, List_ProfileManager, profileString, false, false);
                    //}
                }
                else if (vProfileManagerName == "CtrlIgnoreShortcutName")
                {
                    grid_Popup_ProfileManager_button_ChangeProfile.Content = "Change edit profile (Blocked shortcuts names)";
                    vProfileManagerList = vCtrlIgnoreShortcutName;
                    foreach (string profileString in vCtrlIgnoreShortcutName)
                    {
                        await ListBoxAddItem(lb_ProfileManager, List_ProfileManager, profileString, false, false);
                    }
                }
                else if (vProfileManagerName == "CtrlIgnoreShortcutUri")
                {
                    grid_Popup_ProfileManager_button_ChangeProfile.Content = "Change edit profile (Blocked shortcut uri's)";
                    vProfileManagerList = vCtrlIgnoreShortcutUri;
                    foreach (string profileString in vCtrlIgnoreShortcutUri)
                    {
                        await ListBoxAddItem(lb_ProfileManager, List_ProfileManager, profileString, false, false);
                    }
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
                string selectedProfile = (string)lb_ProfileManager.SelectedItem;
                Debug.WriteLine("Removing profile value: " + selectedProfile);

                //Remove the selected profile value from json
                vProfileManagerList.Remove(selectedProfile);
                JsonSaveObject(vProfileManagerList, vProfileManagerName);

                //Remove the selected profile value from listbox
                await ListBoxRemoveItem(lb_ProfileManager, List_ProfileManager, selectedProfile);
            }
            catch { }
        }

        //Add new profile value
        private async Task AddSaveNewProfileValue()
        {
            try
            {
                string profileValue = grid_Popup_ProfileManager_textbox_ProfileValue1.Text;
                Debug.WriteLine("Adding new profile value: " + profileValue);

                //Color brushes
                BrushConverter BrushConvert = new BrushConverter();
                Brush BrushInvalid = BrushConvert.ConvertFromString("#CD1A2B") as Brush;
                Brush BrushValid = BrushConvert.ConvertFromString("#1DB954") as Brush;

                //Check if the name is empty
                if (string.IsNullOrWhiteSpace(profileValue))
                {
                    grid_Popup_ProfileManager_textbox_ProfileValue1.BorderBrush = BrushInvalid;
                    Popup_Show_Status("Profile", "Empty profile value");
                    Debug.WriteLine("Please enter a profile value.");
                    return;
                }

                //Check if the name is place holder
                if (profileValue == "Profile value")
                {
                    grid_Popup_ProfileManager_textbox_ProfileValue1.BorderBrush = BrushInvalid;
                    Popup_Show_Status("Profile", "Invalid profile value");
                    Debug.WriteLine("Please enter a valid value.");
                    return;
                }

                //Check if value already exists
                if (vProfileManagerList.Any(x => x.ToLower() == profileValue.ToLower()))
                {
                    grid_Popup_ProfileManager_textbox_ProfileValue1.BorderBrush = BrushInvalid;
                    Popup_Show_Status("Profile", "Profile already exists");
                    Debug.WriteLine("Profile value already exists.");
                    return;
                }

                //Clear name from the textbox
                grid_Popup_ProfileManager_textbox_ProfileValue1.Text = "Profile value";

                //Add new profile to json
                vProfileManagerList.Add(profileValue);
                JsonSaveObject(vProfileManagerList, vProfileManagerName);

                //Add new profile to the listbox
                await ListBoxAddItem(lb_ProfileManager, List_ProfileManager, profileValue, false, false);

                //Show profile added notification
                Popup_Show_Status("Profile", "New value added");
                grid_Popup_ProfileManager_textbox_ProfileValue1.BorderBrush = BrushValid;
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

                DataBindString stringCtrlLocationsShortcut = new DataBindString() { Name = "Shortcut locations", PathFile = "CtrlLocationsShortcut", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringCtrlLocationsShortcut);

                DataBindString stringCtrlLocationsFile = new DataBindString() { Name = "File browser locations", PathFile = "CtrlLocationsFile", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringCtrlLocationsFile);

                DataBindString stringIgnoredProcesses = new DataBindString() { Name = "Blocked process names", PathFile = "CtrlIgnoreProcessName", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringIgnoredProcesses);

                DataBindString stringIgnoredShortcutsName = new DataBindString() { Name = "Blocked shortcuts names", PathFile = "CtrlIgnoreShortcutName", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringIgnoredShortcutsName);

                DataBindString stringIgnoredShortcutsUri = new DataBindString() { Name = "Blocked shortcut uri's", PathFile = "CtrlIgnoreShortcutUri", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringIgnoredShortcutsUri);

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