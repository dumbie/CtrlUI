using ArnoldVinkStyles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle settings menu keyboard/controller tapped
        async void ListView_Settings_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //Check which key is pressed
                if (e.Key == VirtualKey.Space || e.Key == VirtualKey.Down)
                {
                    //Get clicked item
                    DataBindString clickedObject = AVListView.GetRoutedListViewItemObject<DataBindString>(e);

                    await Listbox_Settings_Click(clickedObject);
                }
            }
            catch { }
        }

        //Handle settings menu mouse/touch tapped
        async void ListView_Settings_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    //Get clicked item
                    DataBindString clickedObject = AVListView.GetRoutedListViewItemObject<DataBindString>(e);

                    await Listbox_Settings_Click(clickedObject);
                }
            }
            catch { }
        }

        //Handle settings menu click
        async Task Listbox_Settings_Click(DataBindString dataBindString)
        {
            try
            {
                //Check clicked object
                if (dataBindString == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                //Hide all the setting tabs
                settingsStackpanelLaunch.Visibility = Visibility.Collapsed;
                settingsStackpanelDisplay.Visibility = Visibility.Collapsed;
                settingsStackpanelApps.Visibility = Visibility.Collapsed;
                settingsStackpanelInterface.Visibility = Visibility.Collapsed;
                settingsStackpanelSound.Visibility = Visibility.Collapsed;
                settingsStackpanelBrowser.Visibility = Visibility.Collapsed;
                settingsStackpanelOther.Visibility = Visibility.Collapsed;

                //Show the requested setting tab
                if (dataBindString.Name == "Startup")
                {
                    settingsStackpanelLaunch.Visibility = Visibility.Visible;
                    await FocusFrameworkElement(cb_SettingsWindowsStartup);
                }
                else if (dataBindString.Name == "Display")
                {
                    settingsStackpanelDisplay.Visibility = Visibility.Visible;
                    await FocusFrameworkElement(cb_SettingsMonitorPreventSleep);
                }
                else if (dataBindString.Name == "Launchers")
                {
                    settingsStackpanelApps.Visibility = Visibility.Visible;
                    int selectedIndex = listView_LauncherSetting.SelectedIndex;
                    await ListViewFocusIndex(listView_LauncherSetting, false, selectedIndex);
                }
                else if (dataBindString.Name == "Interface")
                {
                    settingsStackpanelInterface.Visibility = Visibility.Visible;
                    await FocusFrameworkElement(cb_SettingsHideBatteryLevel);
                }
                else if (dataBindString.Name == "Sound")
                {
                    settingsStackpanelSound.Visibility = Visibility.Visible;
                    await FocusFrameworkElement(cb_SettingsInterfaceSound);
                }
                else if (dataBindString.Name == "File Browser")
                {
                    settingsStackpanelBrowser.Visibility = Visibility.Visible;
                    await FocusFrameworkElement(cb_SettingsShowHiddenFilesFolders);
                }
                else if (dataBindString.Name == "Other")
                {
                    settingsStackpanelOther.Visibility = Visibility.Visible;
                    await FocusFrameworkElement(slider_SettingsGalleryLoadDays);
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
                    await Notification_Show_Status("AppLaunch", "Quick launch app changed");

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
                BitmapImage imageFonts = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Font.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });

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
                    await Notification_Show_Status("Font", "Font style changed");

                    //Update the setting
                    vSettings.Set("InterfaceFontStyleName", messageResult.Name);

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
                        BitmapImage imageClocks = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = [clockStyle.FullName + "/Preview.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });

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
                    await Notification_Show_Status("Clock", "Clock style changed");

                    //Update the setting
                    vSettings.Set("InterfaceClockStyleName", messageResult.Name);

                    //Update the clock style
                    await UpdateClockStyle();
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

                BitmapImage imagePacks = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/VolumeUp.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });

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
                    await Notification_Show_Status("VolumeUp", "Sound pack changed");

                    //Update the setting
                    vSettings.Set("InterfaceSoundPackName", messageResult.Name);
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

        //Launch FpsOverlayer application
        void Button_LaunchFpsOverlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LaunchFpsOverlayer(false);
            }
            catch { }
        }

        //Launch ScreenCapy application
        void Button_LaunchScreenCapy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LaunchScreenCapy(false);
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