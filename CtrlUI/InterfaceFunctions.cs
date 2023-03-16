using ArnoldVinkCode;
using ArnoldVinkCode.Styles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Register Interface Handlers
        void RegisterInterfaceHandlers()
        {
            try
            {
                //Main menu functions
                grid_Popup_MainMenu_button_Close.Click += Button_Popup_Close_Click;
                listbox_MainMenu.PreviewKeyUp += ListBox_Menu_KeyPressUp;
                listbox_MainMenu.PreviewMouseUp += ListBox_Menu_MousePressUp;

                //Header menu functions
                button_MenuHamburger.Click += Button_MenuHamburger_Click;
                button_MenuSorting.Click += Button_MenuSorting_Click;
                button_MenuMinimize.Click += Button_MenuMinimize_Click;
                button_MenuClose.Click += Button_MenuClose_Click;

                //Category menu functions
                button_Category_Menu_Games.Click += Button_Category_Menu_Click;
                button_Category_Menu_Apps.Click += Button_Category_Menu_Click;
                button_Category_Menu_Emulators.Click += Button_Category_Menu_Click;
                button_Category_Menu_Launchers.Click += Button_Category_Menu_Click;
                button_Category_Menu_Shortcuts.Click += Button_Category_Menu_Click;
                button_Category_Menu_Processes.Click += Button_Category_Menu_Click;
                button_Category_Menu_Search.Click += Button_Category_Menu_Click;

                //App list functions
                lb_Games.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Games.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Apps.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Apps.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Emulators.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Emulators.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Launchers.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Launchers.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Shortcuts.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Shortcuts.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Processes.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Processes.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Search.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Search.PreviewMouseUp += ListBox_Apps_MousePressUp;

                //MessageBox list functions
                lb_MessageBox.PreviewKeyUp += ListBox_MessageBox_KeyPressUp;
                lb_MessageBox.PreviewMouseUp += ListBox_MessageBox_MousePressUp;

                //Manage functions
                btn_Manage_ResetAppLogo.Click += Button_Manage_ResetAppLogo_Click;
                btn_Manage_SaveEditApp.Click += Button_Manage_SaveEditApp_Click;
                lb_Manage_AddAppCategory.SelectionChanged += Lb_Manage_AddAppCategory_SelectionChanged;
                btn_AddAppLogo.Click += Button_AddAppLogo_Click;
                btn_AddAppPathExe.Click += Button_AddAppPathExe_Click;
                btn_AddAppPathLaunch.Click += Button_AddAppPathLaunch_Click;
                btn_AddAppPathRoms.Click += Button_AddAppPathRoms_Click;
                checkbox_AddLaunchSkipRom.Click += Checkbox_AddLaunchSkipRom_Click;
                grid_Popup_Manage_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Manage_button_Save.Click += Grid_Popup_Manage_button_Save_Click;

                //Move app functions
                btn_MoveAppLeft.Click += Btn_MoveAppLeft_Click;
                btn_MoveAppRight.Click += Btn_MoveAppRight_Click;

                //Popup functions
                grid_Popup_FilePicker_button_ControllerRight.Click += Button_FilePicker_button_ControllerRight_Click;
                grid_Popup_FilePicker_button_ControllerLeft.Click += Button_FilePicker_button_ControllerLeft_Click;
                grid_Popup_FilePicker_button_ControllerUp.Click += Button_FilePicker_button_ControllerUp_Click;
                grid_Popup_FilePicker_button_ControllerBack.Click += Grid_Popup_FilePicker_button_ControllerBack_Click;
                grid_Popup_FilePicker_button_ControllerStart.Click += Button_FilePicker_button_ControllerStart_Click;
                grid_Popup_Monitor_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Help_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_MoveApplication_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_MessageBox_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_HowLongToBeat_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Welcome_button_Close.Click += Button_Popup_Close_Click;

                //Search functions
                grid_Search_textbox.TextChanged += grid_Search_textbox_TextChanged;
                grid_Search_button_Reset.Click += grid_Search_button_Reset_Click;

                //Text Input functions
                grid_Popup_TextInput_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_TextInput_textbox.PreviewKeyUp += Grid_Popup_TextInput_textbox_PreviewKeyUp;
                grid_Popup_TextInput_button_Reset.Click += Grid_Popup_TextInput_button_Reset_Click;
                grid_Popup_TextInput_button_Set.Click += Button_TextInputConfirmText_Click;
                grid_Popup_TextInput_button_ConfirmText.Click += Button_TextInputConfirmText_Click;

                //File Picker functions
                lb_FilePicker.PreviewKeyUp += ListBox_FilePicker_KeyPressUp;
                lb_FilePicker.PreviewMouseUp += ListBox_FilePicker_MousePressUp;
                lb_FilePicker.SelectionChanged += Lb_FilePicker_SelectionChanged;
                grid_Popup_FilePicker_button_SelectFolder.Click += Grid_Popup_FilePicker_button_SelectFolder_Click;

                //Profile Manager functions
                grid_Popup_ProfileManager_button_ControllerRight.Click += Button_Popup_Close_Click;
                grid_Popup_ProfileManager_button_ChangeProfile.Click += Grid_Popup_ProfileManager_button_ChangeProfile_Click;
                grid_Popup_ProfileManager_button_ProfileAdd.Click += Grid_Popup_ProfileManager_button_ProfileAdd_Click;
                grid_Popup_ProfileManager_textbox_ProfileString1.KeyDown += grid_Popup_ProfileManager_textbox_ProfileString_KeyDown;
                grid_Popup_ProfileManager_textbox_ProfileString2.KeyDown += grid_Popup_ProfileManager_textbox_ProfileString_KeyDown;
                lb_ProfileManager.PreviewKeyUp += ListBox_ProfileManager_KeyPressUp;
                lb_ProfileManager.PreviewMouseUp += ListBox_ProfileManager_MousePressUp;

                //Color Picker functions
                grid_Popup_ColorPicker_button_ControllerRight.Click += Button_Popup_Close_Click;
                lb_ColorPicker.PreviewKeyUp += ListBox_ColorPicker_KeyPressUp;
                lb_ColorPicker.PreviewMouseUp += ListBox_ColorPicker_MousePressUp;

                //Welcome functions
                grid_Popup_Welcome_button_Start.Click += Button_Popup_Close_Click;
                grid_Popup_Welcome_button_Edge.Click += Grid_Popup_Welcome_button_Edge_Click;
                grid_Popup_Welcome_button_Kodi.Click += Grid_Popup_Welcome_button_Kodi_Click;
                grid_Popup_Welcome_button_Spotify.Click += Grid_Popup_Welcome_button_Spotify_Click;
                grid_Popup_Welcome_button_Steam.Click += Grid_Popup_Welcome_button_Steam_Click;
                grid_Popup_Welcome_button_EADesktop.Click += Grid_Popup_Welcome_button_EADesktop_Click;
                grid_Popup_Welcome_button_Ubisoft.Click += Grid_Popup_Welcome_button_Ubisoft_Click;
                grid_Popup_Welcome_button_GoG.Click += Grid_Popup_Welcome_button_GoG_Click;
                grid_Popup_Welcome_button_Epic.Click += Grid_Popup_Welcome_button_Epic_Click;
                grid_Popup_Welcome_button_Battle.Click += Grid_Popup_Welcome_button_Battle_Click;
                grid_Popup_Welcome_button_PSRemote.Click += Grid_Popup_Welcome_button_PSRemote_Click;
                grid_Popup_Welcome_button_Discord.Click += Grid_Popup_Welcome_button_Discord_Click;

                //Settings functions
                grid_Popup_Settings_button_Close.Click += Button_Popup_Close_Click;
                Listbox_SettingsMenu.PreviewKeyDown += ListBox_Settings_KeyPressUp;
                Listbox_SettingsMenu.PreviewMouseUp += ListBox_Settings_MousePressUp;
                btn_Settings_AppQuickLaunch.Click += Button_Settings_AppQuickLaunch;
                btn_Settings_LaunchDirectXInput.Click += Button_LaunchDirectXInput_Click;
                btn_Settings_CheckControllers.Click += Button_Settings_CheckControllers_Click;
                btn_Settings_CheckForUpdate.Click += Button_Settings_CheckForUpdate_Click;
                btn_Settings_AddGeforceExperience.Click += Button_Settings_AddGeforceExperience_Click;
                btn_Settings_AddRemoteDesktop.Click += Button_Settings_AddRemoteDesktop_Click;
                btn_Settings_ColorPickerAccent.Click += Button_Settings_ColorPickerAccent;
                btn_Settings_InterfaceSoundPackName.Click += Button_Settings_InterfaceSoundPackName;
                btn_Settings_InterfaceClockStyleName.Click += Button_Settings_InterfaceClockStyleName;
                btn_Settings_InterfaceFontStyleName.Click += Button_Settings_InterfaceFontStyleName;

                //Monitor functions
                btn_Monitor_Switch_Primary.Click += Btn_Monitor_Switch_Primary_Click;
                btn_Monitor_Switch_Secondary.Click += Btn_Monitor_Switch_Secondary_Click;
                btn_Monitor_Switch_Duplicate.Click += Btn_Monitor_Switch_Duplicate_Click;
                btn_Monitor_Switch_Extend.Click += Btn_Monitor_Switch_Extend_Click;
                btn_Monitor_Move_App.Click += Btn_Monitor_Move_App_Click;
                btn_Monitor_HDR_Enable.Click += Btn_Monitor_HDR_Enable_Click;
                btn_Monitor_HDR_Disable.Click += Btn_Monitor_HDR_Disable_Click;

                //Help functions
                btn_Help_ProjectWebsite.Click += Button_Help_ProjectWebsite_Click;
                btn_Help_OpenDonation.Click += Button_Help_OpenDonation_Click;

                //Global functions
                this.PreviewMouseDown += WindowMain_PreviewMouseDown;
                this.PreviewKeyUp += WindowMain_KeyPressUp;

                Debug.WriteLine("Registered all the interface handlers.");
            }
            catch { }
        }

        //Adjust the application font family
        void UpdateAppFontStyle()
        {
            try
            {
                string interfaceFontStyleName = SettingLoad(vConfigurationCtrlUI, "InterfaceFontStyleName", typeof(string));
                if (interfaceFontStyleName == "Segoe UI" || interfaceFontStyleName == "Verdana" || interfaceFontStyleName == "Consolas" || interfaceFontStyleName == "Arial")
                {
                    this.FontFamily = new FontFamily(interfaceFontStyleName);
                }
                else
                {
                    string fontPathUser = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Assets/User/Fonts/" + interfaceFontStyleName + ".ttf";
                    string fontPathDefault = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Assets/Default/Fonts/" + interfaceFontStyleName + ".ttf";
                    if (File.Exists(fontPathUser))
                    {
                        ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPathUser);
                        this.FontFamily = fontFamilies.FirstOrDefault();
                    }
                    else if (File.Exists(fontPathDefault))
                    {
                        ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPathDefault);
                        this.FontFamily = fontFamilies.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed setting application font: " + ex.Message);
            }
        }

        //Update the user interface clock style
        void UpdateClockStyle()
        {
            try
            {
                string clockStyle = SettingLoad(vConfigurationCtrlUI, "InterfaceClockStyleName", typeof(string));
                string clockPath = "Assets/Default/Clocks/" + clockStyle;
                if (Directory.Exists("Assets/User/Clocks/" + clockStyle))
                {
                    clockPath = "Assets/User/Clocks/" + clockStyle;
                }

                img_Main_Time_Face.Source = FileToBitmapImage(new string[] { clockPath + "/Face.png" }, null, vImageBackupSource, IntPtr.Zero, 40, 0);
                img_Main_Time_Hour.Source = FileToBitmapImage(new string[] { clockPath + "/Hour.png" }, null, vImageBackupSource, IntPtr.Zero, 40, 0);
                img_Main_Time_Minute.Source = FileToBitmapImage(new string[] { clockPath + "/Minute.png" }, null, vImageBackupSource, IntPtr.Zero, 40, 0);
                img_Main_Time_Center.Source = FileToBitmapImage(new string[] { clockPath + "/Center.png" }, null, vImageBackupSource, IntPtr.Zero, 40, 0);
            }
            catch { }
        }

        //Update the user interface clock time
        void UpdateClockTime()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Rotate the clock images
                    int clockSecond = DateTime.Now.Second;
                    int clockMinute = DateTime.Now.Minute;
                    int clockHour = DateTime.Now.Hour;
                    img_Main_Time_Minute.LayoutTransform = new RotateTransform((clockMinute * 360 / 60) + (clockSecond / 60 * 6));
                    img_Main_Time_Hour.LayoutTransform = new RotateTransform((clockHour * 360 / 12) + (clockMinute / 2));

                    //Update the time and date
                    txt_Main_Date.Text = DateTime.Now.ToString("d MMMM");
                    txt_Main_Time.Text = DateTime.Now.ToShortTimeString();

                    //Change the visibility
                    if (vMainMenuOpen)
                    {
                        txt_Main_Date.Visibility = Visibility.Visible;
                        txt_Main_Time.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txt_Main_Date.Visibility = Visibility.Collapsed;
                        txt_Main_Time.Visibility = Visibility.Visible;
                    }
                });
            }
            catch { }
        }

        //Pause or resume all ScrollViewerLoops
        void PauseResumeScrollviewerLoops(bool pauseScroll)
        {
            try
            {
                foreach (ScrollViewerLoopHorizontal scrollViewer in AVFunctions.FindVisualChildren<ScrollViewerLoopHorizontal>(this))
                {
                    scrollViewer.ScrollPaused = pauseScroll;
                }
                foreach (ScrollViewerLoopVertical scrollViewer in AVFunctions.FindVisualChildren<ScrollViewerLoopVertical>(this))
                {
                    scrollViewer.ScrollPaused = pauseScroll;
                }
            }
            catch { }
        }

        //Check the applications running status
        void CheckAppRunningStatus(List<ProcessMulti> processMultiList)
        {
            try
            {
                //Get all running processes
                if (processMultiList == null)
                {
                    processMultiList = AVProcess.Get_AllProcessesMulti();
                }

                //Update main menu launchers status
                bool runningSteam = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "steam");
                bool runningEADesktop = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "eadesktop" || x.ExeNameNoExt.ToLower() == "origin");
                bool runningGog = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "galaxyclient");
                bool runningUbisoft = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "ubisoftconnect" || x.ExeNameNoExt.ToLower() == "upc");
                bool runningEpic = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "epicgameslauncher");
                bool runningBlizzard = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "battle.net");
                bool runningRockstar = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "rockstarservice");
                bool runningDiscord = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "discord");
                bool runningDirectXInput = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "directxinput");
                bool runningFpsOverlayer = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "fpsoverlayer");

                AVActions.DispatcherInvoke(delegate
                {
                    if (runningSteam)
                    {
                        img_Menu_SteamStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_SteamStatus.Opacity = 0.40;
                    }

                    if (runningEADesktop)
                    {
                        img_Menu_EADesktopStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_EADesktopStatus.Opacity = 0.40;
                    }

                    if (runningGog)
                    {
                        img_Menu_GoGStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_GoGStatus.Opacity = 0.40;
                    }

                    if (runningUbisoft)
                    {
                        img_Menu_UbisoftStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_UbisoftStatus.Opacity = 0.40;
                    }

                    if (runningEpic)
                    {
                        img_Menu_EpicStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_EpicStatus.Opacity = 0.40;
                    }

                    if (runningBlizzard)
                    {
                        img_Menu_BattleNetStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_BattleNetStatus.Opacity = 0.40;
                    }

                    if (runningRockstar)
                    {
                        img_Menu_RockstarStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_RockstarStatus.Opacity = 0.40;
                    }

                    if (runningDiscord)
                    {
                        img_Menu_DiscordStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_DiscordStatus.Opacity = 0.40;
                    }

                    if (runningDirectXInput)
                    {
                        img_Menu_DirectXInputStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_DirectXInputStatus.Opacity = 0.40;
                    }

                    if (runningFpsOverlayer)
                    {
                        img_Menu_FpsOverlayerStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_FpsOverlayerStatus.Opacity = 0.40;
                    }
                });
            }
            catch { }
        }

        //Adjust the application font size
        void AdjustApplicationFontSize()
        {
            try
            {
                int targetFontSize = SettingLoad(vConfigurationCtrlUI, "AppFontSize", typeof(int));
                Debug.WriteLine("Adjusting the font size to: " + targetFontSize);

                double TextSizeTiny = 10;
                double TextSizeInterface = 16;
                double TextSizeSmall = 18;
                double TextSizeMedium = 20;
                double TextSizeLarge = 24;
                double TextSizeHuge = 28;
                double TextSizePreTitle = 50;
                double TextSizeSubTitle = 60;
                double TextSizeTitle = 75;

                Application.Current.Resources["TextSizeTiny"] = TextSizeTiny + targetFontSize;
                Application.Current.Resources["TextSizeInterface"] = TextSizeInterface + targetFontSize;
                Application.Current.Resources["TextSizeSmall"] = TextSizeSmall + targetFontSize;
                Application.Current.Resources["TextSizeMedium"] = TextSizeMedium + targetFontSize;
                Application.Current.Resources["TextSizeLarge"] = TextSizeLarge + targetFontSize;
                Application.Current.Resources["TextSizeHuge"] = TextSizeHuge + targetFontSize;
                Application.Current.Resources["TextSizePreTitle"] = TextSizePreTitle + targetFontSize;
                Application.Current.Resources["TextSizeSubTitle"] = TextSizeSubTitle + targetFontSize;
                Application.Current.Resources["TextSizeTitle"] = TextSizeTitle + targetFontSize;
            }
            catch { }
        }

        //Adjust the application image size
        void AdjustApplicationImageSize()
        {
            try
            {
                int targetSize = SettingLoad(vConfigurationCtrlUI, "AppImageSize", typeof(int));
                Debug.WriteLine("Adjusting the image size to: " + targetSize);

                double ApplicationPanelSize = 110;
                double ApplicationImageHeight = 100;
                double ApplicationImageMaxWidth = 90;

                Application.Current.Resources["ApplicationPanelSize"] = ApplicationPanelSize + targetSize;
                Application.Current.Resources["ApplicationImageHeight"] = ApplicationImageHeight + targetSize;
                Application.Current.Resources["ApplicationImageMaxWidth"] = ApplicationImageMaxWidth + targetSize;
            }
            catch { }
        }

        //Set content and resource images with Cache OnLoad
        void SetContentResourceXamlImages()
        {
            try
            {
                img_Menu_SteamStatus.Source = vImagePreloadSteam;
                img_Menu_UbisoftStatus.Source = vImagePreloadUbisoft;
                img_Menu_EADesktopStatus.Source = vImagePreloadEADesktop;
                img_Menu_GoGStatus.Source = vImagePreloadGoG;
                img_Menu_EpicStatus.Source = vImagePreloadEpic;
                img_Menu_BattleNetStatus.Source = vImagePreloadBattleNet;
                img_Menu_RockstarStatus.Source = vImagePreloadRockstar;
                img_Menu_DiscordStatus.Source = vImagePreloadDiscord;
                img_Menu_DirectXInputStatus.Source = FileToBitmapImage(new string[] { "DirectXInput" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                img_Menu_FpsOverlayerStatus.Source = FileToBitmapImage(new string[] { "FpsOverlayer" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);

                //Check if the first launch logo's need to be loaded
                if (SettingLoad(vConfigurationCtrlUI, "AppFirstLaunch", typeof(bool)))
                {
                    grid_Popup_Welcome_img_Edge.Source = FileToBitmapImage(new string[] { "Edge" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                    grid_Popup_Welcome_img_Kodi.Source = FileToBitmapImage(new string[] { "Kodi" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                    grid_Popup_Welcome_img_Spotify.Source = FileToBitmapImage(new string[] { "Spotify" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                    grid_Popup_Welcome_img_PSRemote.Source = FileToBitmapImage(new string[] { "Remote Play" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                    grid_Popup_Welcome_img_Discord.Source = vImagePreloadDiscord;
                    grid_Popup_Welcome_img_Steam.Source = vImagePreloadSteam;
                    grid_Popup_Welcome_img_EADesktop.Source = vImagePreloadEADesktop;
                    grid_Popup_Welcome_img_Ubisoft.Source = vImagePreloadUbisoft;
                    grid_Popup_Welcome_img_GoG.Source = vImagePreloadGoG;
                    grid_Popup_Welcome_img_Battle.Source = vImagePreloadBattleNet;
                    grid_Popup_Welcome_img_Epic.Source = vImagePreloadEpic;
                }
            }
            catch { }
        }
    }
}