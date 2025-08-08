using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
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
                //Select manage profile
                await SelectProfileCategory();

                //Check manage profile
                if (string.IsNullOrWhiteSpace(vProfileManagerName))
                {
                    Debug.WriteLine("No manage profile is set.");
                    return;
                }

                //Show manage popup
                await Popup_Show(grid_Popup_ProfileManager, grid_Popup_ProfileManager_button_ProfileAdd);

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

                //Load requested profile values
                if (vProfileManagerName == "CtrlLocationsShortcut")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Shortcut load locations";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Path";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlLocationsShortcut;
                    listView_ProfileManager.ItemsSource = vCtrlLocationsShortcut;
                }
                else if (vProfileManagerName == "CtrlLocationsGallery")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Gallery load locations";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Path";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlLocationsGallery;
                    listView_ProfileManager.ItemsSource = vCtrlLocationsGallery;
                }
                else if (vProfileManagerName == "CtrlLocationsFile")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "File browser locations";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Visible;
                    grid_Popup_ProfileManager_textblock_ProfileString2.Text = "Path";

                    vProfileManagerListShared = vCtrlLocationsFile;
                    listView_ProfileManager.ItemsSource = vCtrlLocationsFile;
                }
                else if (vProfileManagerName == "CtrlIgnoreLauncherName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Ignored launcher names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Launcher name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlIgnoreLauncherName;
                    listView_ProfileManager.ItemsSource = vCtrlIgnoreLauncherName;
                }
                else if (vProfileManagerName == "CtrlIgnoreShortcutName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Ignored shortcuts names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Shortcut name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlIgnoreShortcutName;
                    listView_ProfileManager.ItemsSource = vCtrlIgnoreShortcutName;
                }
                else if (vProfileManagerName == "CtrlKeyboardExtensionName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Keyboard open extension names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Extension name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlKeyboardExtensionName;
                    listView_ProfileManager.ItemsSource = vCtrlKeyboardExtensionName;
                }
                else if (vProfileManagerName == "CtrlKeyboardProcessName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Keyboard open process names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Process name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlKeyboardProcessName;
                    listView_ProfileManager.ItemsSource = vCtrlKeyboardProcessName;
                }

                //Select the first listbox item
                await ListViewFocusIndex(listView_ProfileManager, false, 0, vProcessCurrent.WindowHandleMain);
            }
            catch { }
        }

        //Delete the edit profile
        async Task ProfileManager_DeleteProfile(ProfileShared profileShared)
        {
            try
            {
                //Check clicked object
                if (profileShared == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                Debug.WriteLine("Removing profile value: " + profileShared);

                //Remove the selected profile value
                await ListViewRemoveItem(listView_ProfileManager, vProfileManagerListShared, profileShared, true);

                //Save the updated json values
                JsonSaveObject(vProfileManagerListShared, @"Profiles\User\" + vProfileManagerName + ".json");

                await Notification_Show_Status("Profile", "Removed profile value");
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
                SolidColorBrush BrushInvalid = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                SolidColorBrush BrushValid = (SolidColorBrush)Application.Current.Resources["ApplicationValidBrush"];

                //Check if the string1 is empty
                if (string.IsNullOrWhiteSpace(profileString1))
                {
                    grid_Popup_ProfileManager_textbox_ProfileString1.BorderBrush = BrushInvalid;
                    await Notification_Show_Status("Profile", "Empty profile value");
                    Debug.WriteLine("Please enter a profile value.");
                    return;
                }

                //Check if the string2 is empty
                if (grid_Popup_ProfileManager_Value2.Visibility == Visibility.Visible && string.IsNullOrWhiteSpace(profileString2))
                {
                    grid_Popup_ProfileManager_textbox_ProfileString2.BorderBrush = BrushInvalid;
                    await Notification_Show_Status("Profile", "Empty profile value");
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
                    await Notification_Show_Status("Profile", "Profile already exists");
                    Debug.WriteLine("Profile value already exists.");
                    return;
                }

                //Clear added value from the textbox
                grid_Popup_ProfileManager_textbox_ProfileString1.Text = string.Empty;
                grid_Popup_ProfileManager_textbox_ProfileString2.Text = string.Empty;

                //Add the new profile value
                await ListViewAddItem(listView_ProfileManager, vProfileManagerListShared, profileShared, false, false);

                //Save the updated json values
                JsonSaveObject(vProfileManagerListShared, @"Profiles\User\" + vProfileManagerName + ".json");

                //Show profile added notification
                await Notification_Show_Status("Profile", "New value added");
                grid_Popup_ProfileManager_textbox_ProfileString1.BorderBrush = BrushValid;
                grid_Popup_ProfileManager_textbox_ProfileString2.BorderBrush = BrushValid;
            }
            catch { }
        }

        //Select edit profile category
        private async Task SelectProfileCategory()
        {
            try
            {
                //Add profile categories
                List<DataBindString> Answers = new List<DataBindString>();

                BitmapImage imageProfile = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Profile.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });

                DataBindString stringCtrlLocationsShortcut = new DataBindString() { Name = "Shortcut load locations", Data1 = "CtrlLocationsShortcut", ImageBitmap = imageProfile };
                Answers.Add(stringCtrlLocationsShortcut);

                DataBindString stringCtrlLocationsGallery = new DataBindString() { Name = "Gallery load locations", Data1 = "CtrlLocationsGallery", ImageBitmap = imageProfile };
                Answers.Add(stringCtrlLocationsGallery);

                DataBindString stringCtrlLocationsFile = new DataBindString() { Name = "File browser locations", Data1 = "CtrlLocationsFile", ImageBitmap = imageProfile };
                Answers.Add(stringCtrlLocationsFile);

                DataBindString stringCtrlIgnoreLauncherName = new DataBindString() { Name = "Ignored launcher names", Data1 = "CtrlIgnoreLauncherName", ImageBitmap = imageProfile };
                Answers.Add(stringCtrlIgnoreLauncherName);

                DataBindString stringCtrlIgnoreShortcutName = new DataBindString() { Name = "Ignored shortcuts names", Data1 = "CtrlIgnoreShortcutName", ImageBitmap = imageProfile };
                Answers.Add(stringCtrlIgnoreShortcutName);

                DataBindString stringCtrlKeyboardExtensionName = new DataBindString() { Name = "Keyboard open extension names", Data1 = "CtrlKeyboardExtensionName", ImageBitmap = imageProfile };
                Answers.Add(stringCtrlKeyboardExtensionName);

                DataBindString stringCtrlKeyboardProcessName = new DataBindString() { Name = "Keyboard open process names", Data1 = "CtrlKeyboardProcessName", ImageBitmap = imageProfile };
                Answers.Add(stringCtrlKeyboardProcessName);

                //Show messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Profile Category", string.Empty, "Please select profile category to manage:", Answers);
                if (messageResult != null)
                {
                    if (messageResult.Data1 != null)
                    {
                        //Set selected profile category
                        vProfileManagerName = messageResult.Data1.ToString();
                        return;
                    }
                }

                //Reset selected profile category
                vProfileManagerName = string.Empty;
            }
            catch { }
        }
    }
}