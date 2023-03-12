using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async void Button_AddAppLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check the selected categories
                AppCategory selectedAppCategory = (AppCategory)lb_Manage_AddAppCategory.SelectedIndex;
                EmulatorCategory selectedEmulatorCategory = (EmulatorCategory)lb_Manage_AddEmulatorCategory.SelectedIndex;

                //Check if there is an application name set
                if (string.IsNullOrWhiteSpace(tb_AddAppName.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    if (selectedAppCategory == AppCategory.Emulator)
                    {
                        await Popup_Show_MessageBox("Please enter a platform name first", "", "", Answers);
                    }
                    else
                    {
                        await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                    }

                    return;
                }

                //Check if there is an application exe set
                if (tb_AddAppName.Text == "Select application executable file first")
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
                Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Update the new application image
                if (vEditAppDataBind != null)
                {
                    //Check invalid file name
                    string saveFileName = FilterNameFile(vEditAppDataBind.Name);

                    //Copy the new application image
                    File_Copy(vFilePickerResult.PathFile, "Assets/User/Apps/" + saveFileName + ".png", true);

                    //Load the new application image
                    BitmapImage applicationImage = Image_Application_Load(vEditAppDataBind, vImageLoadSize);

                    //Set the new application image
                    img_AddAppLogo.Source = applicationImage;
                    vEditAppDataBind.ImageBitmap = applicationImage;
                }
                else
                {
                    //Check invalid file name
                    string saveFileName = FilterNameFile(tb_AddAppName.Text);

                    //Copy the new application image
                    File_Copy(vFilePickerResult.PathFile, "Assets/User/Apps/" + saveFileName + ".png", true);

                    //Load the new application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { vFilePickerResult.PathFile }, null, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);

                    //Set the new application image
                    img_AddAppLogo.Source = applicationImage;
                }
            }
            catch { }
        }

        async void Button_AddAppPathExe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check the selected categories
                AppCategory selectedAppCategory = (AppCategory)lb_Manage_AddAppCategory.SelectedIndex;
                EmulatorCategory selectedEmulatorCategory = (EmulatorCategory)lb_Manage_AddEmulatorCategory.SelectedIndex;

                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterIn = new List<string> { "exe" };
                vFilePickerSettings.Title = "Application Executable";
                vFilePickerSettings.Description = "Please select an application executable:";
                Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Set fullpath to exe path textbox
                tb_AddAppPathExe.Text = vFilePickerResult.PathFile;
                tb_AddAppPathExe.IsEnabled = true;

                //Set application launch path to textbox
                tb_AddAppPathLaunch.IsEnabled = true;
                btn_AddAppPathLaunch.IsEnabled = true;

                //Set application name to textbox
                if (selectedAppCategory == AppCategory.Emulator)
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
                Popup_Show_FilePicker("PC", -1, false, null);

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
                Popup_Show_FilePicker("PC", -1, false, null);

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
    }
}