using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.JsonFunctions;

namespace CtrlUI
{
    partial class WindowMain
    {
        async void Button_AddAppLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if there is an application name set
                if (string.IsNullOrWhiteSpace(tb_AddAppName.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                    return;
                }

                //Check if there is an application name set
                if (tb_AddAppName.Text == "Select application executable file first" || tb_AddAppEmulatorName.Text == "Select application executable file first")
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please select an application executable file first", "", "", Answers);
                    return;
                }

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "jpg", "png" };
                vFilePickerSettings.Title = "Application Image";
                vFilePickerSettings.Description = "Please select a new application image:";
                vFilePickerSettings.ShowEmulatorImages = true;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Load the new application image
                BitmapImage applicationImage = FileToBitmapImage(new string[] { vFilePickerResult.PathFile }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);

                //Update the new application image
                img_AddAppLogo.Source = applicationImage;
                if (vEditAppDataBind != null)
                {
                    vEditAppDataBind.ImageBitmap = applicationImage;
                }

                //Copy the new application image
                File_Copy(vFilePickerResult.PathFile, "Assets/User/Apps/" + tb_AddAppName.Text + ".png", true);
            }
            catch { }
        }

        async void Button_AddAppExePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "exe" };
                vFilePickerSettings.Title = "Application Executable";
                vFilePickerSettings.Description = "Please select an application executable:";
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Set fullpath to exe path textbox
                tb_AddAppExePath.Text = vFilePickerResult.PathFile;
                tb_AddAppExePath.IsEnabled = true;

                //Set application launch path to textbox
                tb_AddAppPathLaunch.Text = Path.GetDirectoryName(vFilePickerResult.PathFile);
                tb_AddAppPathLaunch.IsEnabled = true;
                btn_AddAppPathLaunch.IsEnabled = true;

                //Set application name to textbox
                if ((AppCategory)lb_Manage_AddAppCategory.SelectedIndex == AppCategory.Emulator)
                {
                    tb_AddAppName.Text = string.Empty;
                    tb_AddAppEmulatorName.Text = vFilePickerResult.Name.Replace(".exe", "");
                }
                else
                {
                    tb_AddAppName.Text = vFilePickerResult.Name.Replace(".exe", "");
                    tb_AddAppEmulatorName.Text = string.Empty;
                }

                tb_AddAppName.IsEnabled = true;
                tb_AddAppEmulatorName.IsEnabled = true;

                //Set application image to image preview
                img_AddAppLogo.Source = FileToBitmapImage(new string[] { tb_AddAppName.Text, vFilePickerResult.PathFile }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                btn_AddAppLogo.IsEnabled = true;
                btn_Manage_ResetAppLogo.IsEnabled = true;
            }
            catch { }
        }

        async void Button_AddAppPathLaunch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.Title = "Launch Folder";
                vFilePickerSettings.Description = "Please select the launch folder:";
                vFilePickerSettings.ShowFiles = false;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                tb_AddAppPathLaunch.Text = vFilePickerResult.PathFile;
                tb_AddAppPathLaunch.IsEnabled = true;
                btn_AddAppPathLaunch.IsEnabled = true;
            }
            catch { }
        }

        async void Button_AddAppPathRoms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.Title = "Rom Folder";
                vFilePickerSettings.Description = "Please select the rom folder:";
                vFilePickerSettings.ShowFiles = false;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                tb_AddAppPathRoms.Text = vFilePickerResult.PathFile;
                tb_AddAppPathRoms.IsEnabled = true;
            }
            catch { }
        }

        //Update manage interface according to category
        void Lb_Manage_AddAppCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ManageInterface_UpdateCategory((AppCategory)lb_Manage_AddAppCategory.SelectedIndex, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to switch app category: " + ex.Message);
            }
        }

        //Reset the application image
        async void Button_Manage_ResetAppLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Image_Application_Reset(vEditAppDataBind);
            }
            catch { }
        }

        //Add or edit application to list apps and Json file
        async void Grid_Popup_Manage_button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SaveEditManageApplication();
            }
            catch { }
        }

        //Add or edit application to list apps and Json file
        async void Button_Manage_SaveEditApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SaveEditManageApplication();
            }
            catch { }
        }

        private void Checkbox_AddLaunchSkipRom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox senderSource = (CheckBox)sender;
                if ((bool)senderSource.IsChecked)
                {
                    tb_AddAppPathRoms.IsEnabled = false;
                    btn_AddAppPathRoms.IsEnabled = false;
                }
                else
                {
                    tb_AddAppPathRoms.IsEnabled = true;
                    btn_AddAppPathRoms.IsEnabled = true;
                }
            }
            catch { }
        }

        private void Checkbox_AddLaunchEnableHDR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Enable monitor HDR
                string executableName = string.Empty;
                string executableNameRaw = string.Empty;
                if (string.IsNullOrWhiteSpace(tb_AddAppNameExe.Text))
                {
                    executableName = Path.GetFileNameWithoutExtension(tb_AddAppExePath.Text).ToLower();
                    executableNameRaw = tb_AddAppExePath.Text.ToLower();
                }
                else
                {
                    executableName = Path.GetFileNameWithoutExtension(tb_AddAppNameExe.Text).ToLower();
                    executableNameRaw = tb_AddAppNameExe.Text.ToLower();
                }
                List<ProfileShared> enabledHDR = vCtrlHDRProcessName.Where(x => x.String1.ToLower() == executableName || x.String1.ToLower() == executableNameRaw).ToList();

                if ((bool)checkbox_AddLaunchEnableHDR.IsChecked)
                {
                    if (!enabledHDR.Any())
                    {
                        ProfileShared newProfile = new ProfileShared();
                        newProfile.String1 = executableNameRaw;
                        vCtrlHDRProcessName.Add(newProfile);
                        JsonSaveObject(vCtrlHDRProcessName, @"User\CtrlHDRProcessName");
                    }
                    Debug.WriteLine("Enabled HDR profile for: " + executableNameRaw);
                }
                else
                {
                    if (enabledHDR.Any())
                    {
                        foreach (ProfileShared removeProfile in enabledHDR)
                        {
                            vCtrlHDRProcessName.Remove(removeProfile);
                        }
                        JsonSaveObject(vCtrlHDRProcessName, @"User\CtrlHDRProcessName");
                    }
                    Debug.WriteLine("Disabled HDR profile for: " + executableNameRaw);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to change enable HDR profile: " + ex.Message);
            }
        }
    }
}