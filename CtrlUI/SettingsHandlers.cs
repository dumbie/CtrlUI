using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle settings menu keyboard/controller tapped
        async void ListBox_Settings_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                Debug.WriteLine(e.Key);
                if (e.Key == Key.Space || e.Key == Key.Down)
                {
                    await Listbox_Settings_SingleTap();
                }
            }
            catch { }
        }

        //Handle settings menu mouse/touch tapped
        async void ListBox_Settings_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    vSingleTappedEvent = true;
                    await Task.Delay(500);
                    if (vSingleTappedEvent)
                    {
                        await Listbox_Settings_SingleTap();
                    }
                }
            }
            catch { }
        }

        //Handle main menu single tap
        async Task Listbox_Settings_SingleTap()
        {
            try
            {
                if (Listbox_SettingsMenu.SelectedIndex >= 0)
                {
                    //Hide all the setting tabs
                    settingsStackpanelLaunch.Visibility = Visibility.Collapsed;
                    settingsStackpanelApps.Visibility = Visibility.Collapsed;
                    settingsStackpanelInterface.Visibility = Visibility.Collapsed;
                    settingsStackpanelBackground.Visibility = Visibility.Collapsed;
                    settingsStackpanelSound.Visibility = Visibility.Collapsed;
                    settingsStackpanelBrowser.Visibility = Visibility.Collapsed;
                    settingsStackpanelNetwork.Visibility = Visibility.Collapsed;
                    settingsStackpanelOther.Visibility = Visibility.Collapsed;

                    //Show the requested setting tab
                    StackPanel SelStackPanel = (StackPanel)Listbox_SettingsMenu.SelectedItem;
                    if (SelStackPanel.Name == "settingsButtonLaunch")
                    {
                        settingsStackpanelLaunch.Visibility = Visibility.Visible;
                        await FocusOnElement(cb_SettingsLaunchFullscreen, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (SelStackPanel.Name == "settingsButtonApps")
                    {
                        settingsStackpanelApps.Visibility = Visibility.Visible;
                        await FocusOnElement(cb_SettingsShowOtherShortcuts, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (SelStackPanel.Name == "settingsButtonInterface")
                    {
                        settingsStackpanelInterface.Visibility = Visibility.Visible;
                        await FocusOnElement(cb_SettingsCloseMediaScreen, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (SelStackPanel.Name == "settingsButtonBackground")
                    {
                        settingsStackpanelBackground.Visibility = Visibility.Visible;
                        await FocusOnElement(cb_SettingsVideoBackground, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (SelStackPanel.Name == "settingsButtonSound")
                    {
                        settingsStackpanelSound.Visibility = Visibility.Visible;
                        await FocusOnElement(cb_SettingsShortcutVolume, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (SelStackPanel.Name == "settingsButtonBrowser")
                    {
                        settingsStackpanelBrowser.Visibility = Visibility.Visible;
                        await FocusOnElement(cb_SettingsShowHiddenFilesFolders, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (SelStackPanel.Name == "settingsButtonNetwork")
                    {
                        settingsStackpanelNetwork.Visibility = Visibility.Visible;
                        await FocusOnElement(txt_SettingsSocketClientPortStart, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (SelStackPanel.Name == "settingsButtonOther")
                    {
                        settingsStackpanelOther.Visibility = Visibility.Visible;
                        await FocusOnElement(btn_Settings_AppQuickLaunch, false, vProcessCurrent.MainWindowHandle);
                    }
                }
            }
            catch { }
        }

        //Open Windows Game controller settings
        async void Button_Settings_CheckControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Controller.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Manage controllers";
                Answers.Add(Answer1);

                DataBindString messageResult = await Popup_Show_MessageBox("This will open a window you can't controller", "", "You can always return back to CtrlUI using the 'Guide' button on your controller or you can use the Keyboard Controller's mouse function.", Answers);
                if (messageResult != null && messageResult == Answer1)
                {
                    Process.Start("joy.cpl");
                }
            }
            catch { }
        }

        //Change the quick launch app
        async void Button_Settings_AppQuickLaunch(object sender, RoutedEventArgs e)
        {
            try
            {
                //Add all apps to the string list
                List<DataBindString> Answers = new List<DataBindString>();
                foreach (DataBindApp dataBindApp in CombineAppLists(false, false))
                {
                    DataBindString stringApp = new DataBindString() { Name = dataBindApp.Name, ImageBitmap = dataBindApp.ImageBitmap };
                    Answers.Add(stringApp);
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Quick Launch Application", "", "Please select a new quick launch application:", Answers);
                if (messageResult != null)
                {
                    btn_Settings_AppQuickLaunch_TextBlock.Text = "Change quick launch app: " + messageResult.Name;

                    //Set previous quick launch application to false
                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.QuickLaunch))
                    {
                        dataBindApp.QuickLaunch = false;
                    }

                    //Set new quick launch application to true
                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.Name.ToLower() == messageResult.Name.ToLower()))
                    {
                        dataBindApp.QuickLaunch = true;
                    }

                    //Show changed message
                    Popup_Show_Status("Play", "Quick launch changed");

                    //Save changes to Json file
                    JsonSaveApplications();
                }
            }
            catch { }
        }

        //Change the interface background image
        async void Button_Settings_ChangeBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerFilterIn = new List<string> { "jpg", "png" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Background Image";
                vFilePickerDescription = "Please select a new background image:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, btn_Settings_ChangeBackgroundImage);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Unload the current background media
                UnloadBackgroundMedia();

                //Copy new background file
                File_Copy(vFilePickerResult.PathFile, "Assets/Background.png", true);

                //Disable video background
                cb_SettingsVideoBackground.IsChecked = false;
                SettingSave("VideoBackground", "False");

                //Disable desktop background
                cb_SettingsDesktopBackground.IsChecked = false;
                SettingSave("DesktopBackground", "False");

                //Update the background media
                UpdateBackgroundMedia();
            }
            catch { }
        }

        //Change the interface background video
        async void Button_Settings_ChangeBackgroundVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerFilterIn = new List<string> { "mp4" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Background Video";
                vFilePickerDescription = "Please select a new background video:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, btn_Settings_ChangeBackgroundVideo);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Unload the current background media
                UnloadBackgroundMedia();

                //Copy new background file
                File_Copy(vFilePickerResult.PathFile, "Assets/BackgroundLive.mp4", true);

                //Enable video background
                cb_SettingsVideoBackground.IsChecked = true;
                SettingSave("VideoBackground", "True");

                //Update the background media
                UpdateBackgroundMedia();
            }
            catch { }
        }

        //Change the interface font style
        async void Button_Settings_InterfaceFontStyleName(object sender, RoutedEventArgs e)
        {
            try
            {
                //Add font styles to string list
                List<DataBindString> Answers = new List<DataBindString>();
                BitmapImage imageFonts = FileToBitmapImage(new string[] { "Assets/Icons/Font.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);

                //Add default fonts
                DataBindString AnswerSegoe = new DataBindString();
                AnswerSegoe.ImageBitmap = imageFonts;
                AnswerSegoe.Name = "Segoe UI";
                Answers.Add(AnswerSegoe);

                DataBindString AnswerVerdana = new DataBindString();
                AnswerVerdana.ImageBitmap = imageFonts;
                AnswerVerdana.Name = "Verdana";
                Answers.Add(AnswerVerdana);

                DataBindString AnswerConsolas = new DataBindString();
                AnswerConsolas.ImageBitmap = imageFonts;
                AnswerConsolas.Name = "Consolas";
                Answers.Add(AnswerConsolas);

                //Add custom fonts
                DirectoryInfo directoryInfo = new DirectoryInfo("Assets/Fonts");
                FileInfo[] fontFiles = directoryInfo.GetFiles("*.ttf", SearchOption.TopDirectoryOnly);
                foreach (FileInfo fontFile in fontFiles)
                {
                    DataBindString AnswerCustom = new DataBindString();
                    AnswerCustom.ImageBitmap = imageFonts;
                    AnswerCustom.Name = Path.GetFileNameWithoutExtension(fontFile.Name);
                    Answers.Add(AnswerCustom);
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Interface Fonts", "", "Please select a font style to use:", Answers);
                if (messageResult != null)
                {
                    //Show changed message
                    Popup_Show_Status("Font", "Font style changed");

                    //Update the setting
                    SettingSave("InterfaceFontStyleName", messageResult.Name);

                    //Update the font style
                    UpdateAppFontStyle();
                }
            }
            catch { }
        }

        //Change the interface clock stlye
        async void Button_Settings_InterfaceClockStyleName(object sender, RoutedEventArgs e)
        {
            try
            {
                //Add clock styles to string list
                List<DataBindString> Answers = new List<DataBindString>();
                DirectoryInfo directoryInfo = new DirectoryInfo("Assets/Clocks");
                DirectoryInfo[] clockStyles = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);

                foreach (DirectoryInfo clockStyle in clockStyles)
                {
                    BitmapImage imageClocks = FileToBitmapImage(new string[] { "Assets/Clocks/" + clockStyle.Name + "/Preview.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    DataBindString AnswerCustom = new DataBindString();
                    AnswerCustom.ImageBitmap = imageClocks;
                    AnswerCustom.Name = clockStyle.Name;
                    Answers.Add(AnswerCustom);
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Interface Clocks", "", "Please select a clock style to use:", Answers);
                if (messageResult != null)
                {
                    //Show changed message
                    Popup_Show_Status("Clock", "Clock style changed");

                    //Update the setting
                    SettingSave("InterfaceClockStyleName", messageResult.Name);

                    //Update the clock style
                    UpdateClockStyle();
                }
            }
            catch { }
        }

        //Change the interface sound pack
        async void Button_Settings_InterfaceSoundPackName(object sender, RoutedEventArgs e)
        {
            try
            {
                //Add sound packs to string list
                List<DataBindString> Answers = new List<DataBindString>();
                DirectoryInfo directoryInfo = new DirectoryInfo("Assets/Sounds");
                DirectoryInfo[] soundPacks = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                BitmapImage imagePacks = FileToBitmapImage(new string[] { "Assets/Icons/VolumeUp.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);

                foreach (DirectoryInfo soundPack in soundPacks)
                {
                    DataBindString AnswerCustom = new DataBindString();
                    AnswerCustom.ImageBitmap = imagePacks;
                    AnswerCustom.Name = soundPack.Name;
                    Answers.Add(AnswerCustom);
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Interface Sounds", "", "Please select a sound pack to use:", Answers);
                if (messageResult != null)
                {
                    //Show changed message
                    Popup_Show_Status("VolumeUp", "Sound pack changed");

                    //Update the setting
                    SettingSave("InterfaceSoundPackName", messageResult.Name);
                }
            }
            catch { }
        }

        //Launch DirectXInput application
        async void Button_LaunchDirectXInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await LaunchDirectXInput();
            }
            catch { }
        }

        //Check for available application update
        async void Button_Settings_CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await CheckForAppUpdate(false);
            }
            catch { }
        }

        //Show the color picker popup
        async void Button_Settings_ColorPickerAccent(object sender, RoutedEventArgs args)
        {
            try
            {
                await Popup_ShowHide_ColorPicker(false);
            }
            catch { }
        }

        //Create geforce experience shortcut
        async void Button_Settings_AddGeforceExperience_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                //Set application shortcut paths
                string TargetFilePath = Assembly.GetEntryAssembly().CodeBase.Replace(".exe", "-Admin.exe");
                string TargetName = Assembly.GetEntryAssembly().GetName().Name;
                string TargetFileShortcut = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/" + TargetName + ".url";
                string TargetFileBoxArtFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/StreamingAssets/" + TargetName + "/box-art.png";
                string TargetFileBoxArtDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/StreamingAssets/" + TargetName;
                string TargetDirectoryStreamingAssets = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/StreamingAssets";

                //Check if the Streaming Assets folder exists
                Directory_Create(TargetDirectoryStreamingAssets, false);
                Directory_Create(TargetFileBoxArtDirectory, false);

                //Check if the shortcut already exists
                if (!File.Exists(TargetFileShortcut))
                {
                    Debug.WriteLine("Adding application to GeForce Experience");

                    using (StreamWriter StreamWriter = new StreamWriter(TargetFileShortcut))
                    {
                        StreamWriter.WriteLine("[InternetShortcut]");
                        StreamWriter.WriteLine("URL=" + TargetFilePath);
                        StreamWriter.WriteLine("IconFile=" + TargetFilePath.Replace("file:///", ""));
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }

                    //Copy art box to the Streaming Assets directory
                    File_Copy("Assets/BoxArt.png", TargetFileBoxArtFile, true);

                    btn_Settings_AddGeforceExperience_TextBlock.Text = "Remove CtrlUI from GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Check.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("CtrlUI has been added to GeForce Experience", "", "You can now remotely launch CtrlUI from your devices.", Answers);
                }
                else
                {
                    Debug.WriteLine("Removing application from GeForce Experience");

                    File_Delete(TargetFileShortcut);
                    Directory_Delete(TargetFileBoxArtDirectory);

                    btn_Settings_AddGeforceExperience_TextBlock.Text = "Add CtrlUI to GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Check.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("CtrlUI has been removed from GeForce Experience", "", "", Answers);
                }
            }
            catch (Exception ex)
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Check.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                Answer1.Name = "Alright";
                Answers.Add(Answer1);

                Debug.WriteLine("Failed add GeForce Experience: " + ex.Message);
                await Popup_Show_MessageBox("Failed to add CtrlUI to GeForce Experience", "", "Please make sure that GeForce experience is installed.", Answers);
            }
        }
    }
}