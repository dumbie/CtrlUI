﻿using ArnoldVinkStyles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVImage;
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
                if (!AVInterface.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

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
                    if (SelStackPanel.Name == "settingsButtonStartup")
                    {
                        settingsStackpanelLaunch.Visibility = Visibility.Visible;
                        await FocusElement(cb_SettingsWindowsStartup, vProcessCurrent.WindowHandleMain);
                    }
                    else if (SelStackPanel.Name == "settingsButtonDisplay")
                    {
                        settingsStackpanelDisplay.Visibility = Visibility.Visible;
                        await FocusElement(cb_SettingsLaunchMinimized, vProcessCurrent.WindowHandleMain);
                    }
                    else if (SelStackPanel.Name == "settingsButtonLaunchers")
                    {
                        settingsStackpanelApps.Visibility = Visibility.Visible;
                        int selectedIndex = listbox_LauncherSetting.SelectedIndex;
                        await ListBoxFocusIndex(listbox_LauncherSetting, false, selectedIndex, vProcessCurrent.WindowHandleMain);
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
                        await FocusElement(slider_SettingsGalleryLoadDays, vProcessCurrent.WindowHandleMain);
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
                    Notification_Show_Status("AppLaunch", "Quick launch app changed");

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
                    Notification_Show_Status("Font", "Font style changed");

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
                    Notification_Show_Status("Clock", "Clock style changed");

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
                    Notification_Show_Status("VolumeUp", "Sound pack changed");

                    //Update the setting
                    SettingSave(vConfigurationCtrlUI, "InterfaceSoundPackName", messageResult.Name);
                }
            }
            catch { }
        }

        //Launch DirectXInput application
        void Button_LaunchDirectXInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LaunchDirectXInput(false);
            }
            catch { }
        }

        //Launch Fps Overlayer application
        void Button_LaunchFpsOverlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LaunchFpsOverlayer(false);
            }
            catch { }
        }

        //Launch Screen Capture Tool application
        void Button_LaunchScreenCaptureTool_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LaunchScreenCaptureTool(false);
            }
            catch { }
        }

        //Check for available application update
        async void Button_Settings_CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await UpdateCheck(false);
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
    }
}