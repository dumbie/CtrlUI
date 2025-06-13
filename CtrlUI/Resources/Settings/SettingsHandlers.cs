using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSettings;
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
                    settingsStackpanelDisplay.Visibility = Visibility.Collapsed;
                    settingsStackpanelApps.Visibility = Visibility.Collapsed;
                    settingsStackpanelInterface.Visibility = Visibility.Collapsed;
                    settingsStackpanelSound.Visibility = Visibility.Collapsed;
                    settingsStackpanelBrowser.Visibility = Visibility.Collapsed;
                    settingsStackpanelOther.Visibility = Visibility.Collapsed;

                    //Show the requested setting tab
                    StackPanel SelStackPanel = (StackPanel)Listbox_SettingsMenu.SelectedItem;
                    if (SelStackPanel.Name == "settingsButtonLaunch")
                    {
                        settingsStackpanelLaunch.Visibility = Visibility.Visible;
                        await FocusElement(cb_SettingsWindowsStartup, vProcessCurrent.WindowHandleMain);
                    }
                    else if (SelStackPanel.Name == "settingsButtonDisplay")
                    {
                        settingsStackpanelDisplay.Visibility = Visibility.Visible;
                        await FocusElement(cb_SettingsLaunchMinimized, vProcessCurrent.WindowHandleMain);
                    }
                    else if (SelStackPanel.Name == "settingsButtonApps")
                    {
                        settingsStackpanelApps.Visibility = Visibility.Visible;
                        await FocusElement(cb_SettingsShowLibrarySteam, vProcessCurrent.WindowHandleMain);
                    }
                    else if (SelStackPanel.Name == "settingsButtonInterface")
                    {
                        settingsStackpanelInterface.Visibility = Visibility.Visible;
                        await FocusElement(cb_SettingsHideBatteryLevel, vProcessCurrent.WindowHandleMain);
                    }
                    else if (SelStackPanel.Name == "settingsButtonSound")
                    {
                        settingsStackpanelSound.Visibility = Visibility.Visible;
                        await FocusElement(cb_SettingsInterfaceSound, vProcessCurrent.WindowHandleMain);
                    }
                    else if (SelStackPanel.Name == "settingsButtonBrowser")
                    {
                        settingsStackpanelBrowser.Visibility = Visibility.Visible;
                        await FocusElement(cb_SettingsShowHiddenFilesFolders, vProcessCurrent.WindowHandleMain);
                    }
                    else if (SelStackPanel.Name == "settingsButtonOther")
                    {
                        settingsStackpanelOther.Visibility = Visibility.Visible;
                        await FocusElement(btn_Settings_AddGeforceExperience, vProcessCurrent.WindowHandleMain);
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
                DataBindString AnswerManage = new DataBindString();
                AnswerManage.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerManage.Name = "Manage controllers";
                Answers.Add(AnswerManage);

                DataBindString messageResult = await Popup_Show_MessageBox("This will open a window that can't be used with controller", "", "You can always return back to CtrlUI using the 'Guide' button on your controller or you can use the on screen keyboard mouse function.", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerManage)
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "joy.cpl";
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }
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
                foreach (DataBindApp dataBindApp in CombineAppLists(true, true, true, false, false, false, false))
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
                    foreach (DataBindApp dataBindApp in CombineAppLists(true, true, true, false, false, false, false).Where(x => x.QuickLaunch))
                    {
                        dataBindApp.QuickLaunch = false;
                    }

                    //Set new quick launch application to true
                    foreach (DataBindApp dataBindApp in CombineAppLists(true, true, true, false, false, false, false).Where(x => x.Name.ToLower() == messageResult.Name.ToLower()))
                    {
                        dataBindApp.QuickLaunch = true;
                    }

                    //Show changed message
                    await Notification_Send_Status("AppLaunch", "Quick launch app changed");

                    //Save changes to Json file
                    JsonSaveList_Applications();
                }
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
                BitmapImage imageFonts = FileToBitmapImage(new string[] { "Assets/Default/Icons/Font.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);

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

                DataBindString AnswerArial = new DataBindString();
                AnswerArial.ImageBitmap = imageFonts;
                AnswerArial.Name = "Arial";
                Answers.Add(AnswerArial);

                //Add custom fonts
                DirectoryInfo directoryInfoUser = new DirectoryInfo("Assets/User/Fonts");
                FileInfo[] fontFilesUser = directoryInfoUser.GetFiles("*.ttf", SearchOption.TopDirectoryOnly);
                DirectoryInfo directoryInfoDefault = new DirectoryInfo("Assets/Default/Fonts");
                FileInfo[] fontFilesDefault = directoryInfoDefault.GetFiles("*.ttf", SearchOption.TopDirectoryOnly);
                IEnumerable<FileInfo> fontFiles = fontFilesUser.Concat(fontFilesDefault);

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
                    await Notification_Send_Status("Font", "Font style changed");

                    //Update the setting
                    SettingSave(vConfigurationCtrlUI, "InterfaceFontStyleName", messageResult.Name);

                    //Adjust the application font family
                    AdjustApplicationFontStyle();
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

                DirectoryInfo directoryInfoUser = new DirectoryInfo("Assets/User/Clocks");
                DirectoryInfo[] clockStylesUser = directoryInfoUser.GetDirectories("*", SearchOption.TopDirectoryOnly);
                DirectoryInfo directoryInfoDefault = new DirectoryInfo("Assets/Default/Clocks");
                DirectoryInfo[] clockStylesDefault = directoryInfoDefault.GetDirectories("*", SearchOption.TopDirectoryOnly);
                IEnumerable<DirectoryInfo> clockStyles = clockStylesUser.Concat(clockStylesDefault);

                foreach (DirectoryInfo clockStyle in clockStyles)
                {
                    try
                    {
                        BitmapImage imageClocks = FileToBitmapImage(new string[] { clockStyle.FullName + "/Preview.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        DataBindString AnswerCustom = new DataBindString();
                        AnswerCustom.ImageBitmap = imageClocks;
                        AnswerCustom.Name = clockStyle.Name;
                        Answers.Add(AnswerCustom);
                    }
                    catch { }
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Interface Clocks", "", "Please select a clock style to use:", Answers);
                if (messageResult != null)
                {
                    //Show changed message
                    await Notification_Send_Status("Clock", "Clock style changed");

                    //Update the setting
                    SettingSave(vConfigurationCtrlUI, "InterfaceClockStyleName", messageResult.Name);

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
                DirectoryInfo directoryInfoUser = new DirectoryInfo("Assets/User/Sounds");
                DirectoryInfo[] soundPacksUser = directoryInfoUser.GetDirectories("*", SearchOption.TopDirectoryOnly);
                DirectoryInfo directoryInfoDefault = new DirectoryInfo("Assets/Default/Sounds");
                DirectoryInfo[] soundPacksDefault = directoryInfoDefault.GetDirectories("*", SearchOption.TopDirectoryOnly);
                IEnumerable<DirectoryInfo> soundPacks = soundPacksUser.Concat(soundPacksDefault);

                BitmapImage imagePacks = FileToBitmapImage(new string[] { "Assets/Default/Icons/VolumeUp.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);

                foreach (DirectoryInfo soundPack in soundPacks)
                {
                    try
                    {
                        DataBindString AnswerCustom = new DataBindString();
                        AnswerCustom.ImageBitmap = imagePacks;
                        AnswerCustom.Name = soundPack.Name;
                        Answers.Add(AnswerCustom);
                    }
                    catch { }
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Interface Sounds", "", "Please select a sound pack to use:", Answers);
                if (messageResult != null)
                {
                    //Show changed message
                    await Notification_Send_Status("VolumeUp", "Sound pack changed");

                    //Update the setting
                    SettingSave(vConfigurationCtrlUI, "InterfaceSoundPackName", messageResult.Name);
                }
            }
            catch { }
        }

        //Launch DirectXInput application
        async void Button_LaunchDirectXInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await LaunchDirectXInput(false);
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

        //Create CtrlUI GeForce experience shortcut
        async void Button_Settings_AddGeForceExperience_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                //Set application shortcut paths
                string TargetName = AVFunctions.ApplicationName();
                string TargetFilePath = AVFunctions.ApplicationPathExecutable().Replace(".exe", "-Launcher.exe");
                string TargetFileShortcut = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/" + TargetName + ".url";
                string TargetFileBoxArtFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/StreamingAssets/Default/" + TargetName + "/box-art.png";
                string TargetFileBoxArtDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/StreamingAssets/Default/" + TargetName;
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
                        StreamWriter.WriteLine("IconFile=" + TargetFilePath);
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }

                    //Copy art box to the Streaming Assets directory
                    File_Copy("Assets/BoxArt.png", TargetFileBoxArtFile, true);

                    btn_Settings_AddGeforceExperience_TextBlock.Text = "Remove CtrlUI from GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
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
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("CtrlUI has been removed from GeForce Experience", "", "", Answers);
                }
            }
            catch (Exception ex)
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                Answer1.Name = "Ok";
                Answers.Add(Answer1);

                Debug.WriteLine("Failed add GeForce Experience: " + ex.Message);
                await Popup_Show_MessageBox("Failed to add CtrlUI to GeForce Experience", "", "Please make sure that GeForce experience is installed.", Answers);
            }
        }

        //Create remote desktop geforce experience shortcut
        async void Button_Settings_AddRemoteDesktop_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                //Set application shortcut paths
                string TargetFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\mstsc.exe";
                string TargetName = "Remote Desktop";
                string TargetFileShortcut = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/" + TargetName + ".url";
                string TargetFileBoxArtFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/StreamingAssets/Default/" + TargetName + "/box-art.png";
                string TargetFileBoxArtDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NVIDIA Corporation/Shield Apps/StreamingAssets/Default/" + TargetName;
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
                        StreamWriter.WriteLine("IconFile=" + TargetFilePath);
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }

                    //Copy art box to the Streaming Assets directory
                    File_Copy("Assets/BoxArt-RemoteDesktop.png", TargetFileBoxArtFile, true);

                    btn_Settings_AddRemoteDesktop_TextBlock.Text = "Remove Remote Desktop from GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Remote Desktop has been added to GeForce Experience", "", "You can now remotely launch Remote Desktop from your devices.", Answers);
                }
                else
                {
                    Debug.WriteLine("Removing application from GeForce Experience");

                    File_Delete(TargetFileShortcut);
                    Directory_Delete(TargetFileBoxArtDirectory);

                    btn_Settings_AddRemoteDesktop_TextBlock.Text = "Add Remote Desktop to GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Remote Desktop has been removed from GeForce Experience", "", "", Answers);
                }
            }
            catch (Exception ex)
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                Answer1.Name = "Ok";
                Answers.Add(Answer1);

                Debug.WriteLine("Failed add GeForce Experience: " + ex.Message);
                await Popup_Show_MessageBox("Failed to add Remote Desktop to GeForce Experience", "", "Please make sure that GeForce experience is installed.", Answers);
            }
        }
    }
}