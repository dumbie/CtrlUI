using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVJsonFunctions;
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
                //Show the manage popup
                await Popup_Show(grid_Popup_ProfileManager, grid_Popup_ProfileManager_button_ChangeProfile);

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
                if (vProfileManagerName == "CtrlLocationsShortcut")
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
                else if (vProfileManagerName == "CtrlIgnoreLauncherName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Ignored launcher names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Launcher name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlIgnoreLauncherName;
                    lb_ProfileManager.ItemsSource = vCtrlIgnoreLauncherName;
                }
                else if (vProfileManagerName == "CtrlIgnoreShortcutName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Ignored shortcuts names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Shortcut name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlIgnoreShortcutName;
                    lb_ProfileManager.ItemsSource = vCtrlIgnoreShortcutName;
                }
                else if (vProfileManagerName == "CtrlKeyboardExtensionName")
                {
                    grid_Popup_ProfileManager_txt_Description.Text = "Keyboard open extension names";
                    grid_Popup_ProfileManager_textblock_ProfileString1.Text = "Extension name";
                    grid_Popup_ProfileManager_Value2.Visibility = Visibility.Collapsed;

                    vProfileManagerListShared = vCtrlKeyboardExtensionName;
                    lb_ProfileManager.ItemsSource = vCtrlKeyboardExtensionName;
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
                await ListBoxFocusOrSelectIndex(lb_ProfileManager, false, 0, this, vProcessCurrent.WindowHandleMain);
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
                await ListBoxRemoveItem(lb_ProfileManager, vProfileManagerListShared, selectedProfile, true);

                //Save the updated json values
                JsonSaveObject(vProfileManagerListShared, @"Profiles\User\" + vProfileManagerName + ".json");

                await Notification_Send_Status("Profile", "Removed profile value");
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
                    await Notification_Send_Status("Profile", "Empty profile value");
                    Debug.WriteLine("Please enter a profile value.");
                    return;
                }

                //Check if the string2 is empty
                if (grid_Popup_ProfileManager_Value2.Visibility == Visibility.Visible && string.IsNullOrWhiteSpace(profileString2))
                {
                    grid_Popup_ProfileManager_textbox_ProfileString2.BorderBrush = BrushInvalid;
                    await Notification_Send_Status("Profile", "Empty profile value");
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
                    await Notification_Send_Status("Profile", "Profile already exists");
                    Debug.WriteLine("Profile value already exists.");
                    return;
                }

                //Clear added value from the textbox
                grid_Popup_ProfileManager_textbox_ProfileString1.Text = string.Empty;
                grid_Popup_ProfileManager_textbox_ProfileString2.Text = string.Empty;

                //Add the new profile value
                await ListBoxAddItem(lb_ProfileManager, vProfileManagerListShared, profileShared, false, false);

                //Save the updated json values
                JsonSaveObject(vProfileManagerListShared, @"Profiles\User\" + vProfileManagerName + ".json");

                //Show profile added notification
                await Notification_Send_Status("Profile", "New value added");
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
                List<DataBindString> Answers = new List<DataBindString>();

                BitmapImage imageProfile = FileToBitmapImage(new string[] { "Assets/Default/Icons/Profile.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);

                DataBindString stringCtrlLocationsShortcut = new DataBindString() { Name = "Shortcut locations", Data1 = "CtrlLocationsShortcut", ImageBitmap = imageProfile };
                Answers.Add(stringCtrlLocationsShortcut);

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

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Profile Category", "", "Please select the profile to manage:", Answers);
                if (messageResult != null)
                {
                    if (messageResult.Data1 != null)
                    {
                        //Set the selected profile category
                        vProfileManagerName = messageResult.Data1.ToString();

                        //Load profile in manager
                        await ProfileManager_LoadProfile();
                    }
                }
            }
            catch { }
        }

        //Backup Json profiles
        void ProfileMakeBackup()
        {
            try
            {
                Debug.WriteLine("Creating Json profiles backup.");

                //Create backup directory
                AVFiles.Directory_Create("Backups", false);

                //Cleanup profile backups
                FileInfo[] fileInfo = new DirectoryInfo("Backups").GetFiles("*.zip");
                foreach (FileInfo backupFile in fileInfo)
                {
                    try
                    {
                        TimeSpan backupSpan = DateTime.Now - backupFile.CreationTime;
                        if (backupSpan.TotalDays > 5)
                        {
                            backupFile.Delete();
                        }
                    }
                    catch { }
                }

                //Create profile backup
                string backupTime = DateTime.Now.ToString("yyyyMMddHHmmss") + "-Profiles.zip";
                ZipFile.CreateFromDirectory("Profiles\\User", "Backups\\" + backupTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed making profiles backup: " + ex.Message);
            }
        }
    }
}