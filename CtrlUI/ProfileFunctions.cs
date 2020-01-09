using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle profile manager keyboard/controller tapped
        async void ListBox_ProfileManager_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await ProfileManager_DeleteProfile();
                }
            }
            catch { }
        }

        //Handle profile manager mouse/touch tapped
        async void ListBox_ProfileManager_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (vMousePressDownLeftClick)
                    {
                        await ProfileManager_DeleteProfile();
                    }
                }
            }
            catch { }
        }

        //Show profile manager popup
        async Task Popup_Show_ProfileManager()
        {
            try
            {
                //Show the manage popup
                await Popup_Show(grid_Popup_ProfileManager, grid_Popup_ProfileManager_button_ChangeProfile, true);

                //Load profiles
                await ProfileManager_LoadProfile();
            }
            catch { }
        }

        //Load profile in manager
        async Task ProfileManager_LoadProfile()
        {
            try
            {
                //Clear the current profile list
                List_ProfileManager.Clear();

                //Load the requested profile values
                if (vProfileManagerName == "AppsBlacklistProcess")
                {
                    grid_Popup_ProfileManager_button_ChangeProfile.Content = "Change edit profile (Blocked processes)";
                    vProfileManagerList = vAppsBlacklistProcess;
                    foreach (string profileString in vAppsBlacklistProcess)
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
                int selectedIndex = lb_ProfileManager.SelectedIndex;
                string selectedProfile = (string)lb_ProfileManager.SelectedItem;
                Debug.WriteLine("Removing profile value: " + selectedProfile);

                vProfileManagerList.Remove(selectedProfile);
                JsonSaveObject(vProfileManagerList, vProfileManagerName);

                //Remove the selected profile value
                await ListBoxRemoveItem(lb_ProfileManager, List_ProfileManager, selectedProfile);

                //Select the listbox index
                await ListBoxFocusOrSelectIndex(lb_ProfileManager, false, false, selectedIndex);
            }
            catch { }
        }

        //Add new profile value
        async void Grid_Popup_ProfileManager_button_ProfileAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string profileValue = grid_Popup_ProfileManager_textbox_ProfileValue.Text;
                Debug.WriteLine("Adding new profile value: " + profileValue);

                //Color brushes
                BrushConverter BrushConvert = new BrushConverter();
                Brush BrushInvalid = BrushConvert.ConvertFromString("#CD1A2B") as Brush;
                Brush BrushValid = BrushConvert.ConvertFromString("#1DB954") as Brush;

                //Check if the name is empty
                if (string.IsNullOrWhiteSpace(profileValue))
                {
                    grid_Popup_ProfileManager_textbox_ProfileValue.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter a profile value.");
                    return;
                }

                //Check if the name is place holder
                if (profileValue == "Profile value")
                {
                    grid_Popup_ProfileManager_textbox_ProfileValue.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter a profile value.");
                    return;
                }

                //Check if value already exists
                if (vProfileManagerList.Any(x => x.ToLower() == profileValue.ToLower()))
                {
                    grid_Popup_ProfileManager_textbox_ProfileValue.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Profile value already exists.");
                    return;
                }

                //Clear name from the textbox
                grid_Popup_ProfileManager_textbox_ProfileValue.Text = "Profile value";

                vProfileManagerList.Add(profileValue);
                JsonSaveObject(vProfileManagerList, vProfileManagerName);

                await ListBoxAddItem(lb_ProfileManager, List_ProfileManager, profileValue, false, false);

                grid_Popup_ProfileManager_textbox_ProfileValue.BorderBrush = BrushValid;
            }
            catch { }
        }

        //Change the edit profile
        async void Grid_Popup_ProfileManager_button_ChangeProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Add profile categories
                vFilePickerStrings.Clear();

                BitmapImage imageProfile = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Profile.png" }, IntPtr.Zero, -1);

                DataBindString stringShortcutLocations = new DataBindString() { Name = "Shortcut locations", PathFile = "ShortcutLocations", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringShortcutLocations);

                DataBindString stringFileLocations = new DataBindString() { Name = "File browser locations", PathFile = "FileLocations", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringFileLocations);

                DataBindString stringIgnoredProcesses = new DataBindString() { Name = "Blocked processes", PathFile = "AppsBlacklistProcess", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringIgnoredProcesses);

                DataBindString stringIgnoredShortcutsName = new DataBindString() { Name = "Blocked shortcuts names", PathFile = "AppsBlacklistShortcut", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringIgnoredShortcutsName);

                DataBindString stringIgnoredShortcutsUri = new DataBindString() { Name = "Blocked shortcut uri's", PathFile = "AppsBlacklistShortcutUri", ImageBitmap = imageProfile };
                vFilePickerStrings.Add(stringIgnoredShortcutsUri);

                //Show the category picker
                vFilePickerFilterIn = new string[] { };
                vFilePickerFilterOut = new string[] { };
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
            }
            catch { }
        }
    }
}