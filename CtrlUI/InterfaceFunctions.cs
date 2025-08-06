using ArnoldVinkCode;
using ArnoldVinkStyles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    public partial class WindowMain
    {
        //Register Interface Handlers
        void RegisterInterfaceHandlers()
        {
            try
            {
                //Main menu functions
                grid_Popup_MainMenu_button_Close.Click += Button_Popup_Close_Click;
                listView_MainMenu.PreviewKeyUp += ListView_MainMenu_KeyPressUp;
                listView_MainMenu.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_MainMenu_MousePressUp), true);

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
                button_Category_Menu_Gallery.Click += Button_Category_Menu_Click;
                button_Category_Menu_Search.Click += Button_Category_Menu_Click;

                //App list functions
                listView_Apps.PreviewKeyUp += ListView_Apps_KeyPressUp;
                listView_Apps.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Apps_MousePressUp), true);
                listView_Games.PreviewKeyUp += ListView_Apps_KeyPressUp;
                listView_Games.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Apps_MousePressUp), true);
                listView_Emulators.PreviewKeyUp += ListView_Apps_KeyPressUp;
                listView_Emulators.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Apps_MousePressUp), true);
                listView_Launchers.PreviewKeyUp += ListView_Apps_KeyPressUp;
                listView_Launchers.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Apps_MousePressUp), true);
                listView_Shortcuts.PreviewKeyUp += ListView_Apps_KeyPressUp;
                listView_Shortcuts.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Apps_MousePressUp), true);
                listView_Processes.PreviewKeyUp += ListView_Apps_KeyPressUp;
                listView_Processes.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Apps_MousePressUp), true);
                listView_Gallery.PreviewKeyUp += ListView_Apps_KeyPressUp;
                listView_Gallery.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Apps_MousePressUp), true);
                listView_Search.PreviewKeyUp += ListView_Apps_KeyPressUp;
                listView_Search.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Apps_MousePressUp), true);

                //Gallery functions
                ScrollViewer scrollViewer_Gallery = AVListView.GetListViewScrollViewer(listView_Gallery);
                if (scrollViewer_Gallery != null)
                {
                    scrollViewer_Gallery.ViewChanged += ListView_GalleryScrollViewer_ScrollChanged;
                }

                //MessageBox list functions
                listView_MessageBox.PreviewKeyUp += ListView_MessageBox_KeyPressUp;
                listView_MessageBox.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_MessageBox_MousePressUp), true);

                //Manage functions
                btn_Manage_ResetAppLogo.Click += Button_Manage_ResetAppLogo_Click;
                btn_Manage_AddAppLogo.Click += Button_AddAppLogo_Click;
                btn_AddAppPathExe.Click += Button_AddAppPathExe_Click;
                btn_AddAppPathLaunch.Click += Button_AddAppPathLaunch_Click;
                btn_AddAppPathRoms.Click += Button_AddAppPathRoms_Click;
                checkbox_AddLaunchSkipRom.Click += Checkbox_AddLaunchSkipRom_Click;
                grid_Popup_Manage_button_Close.Click += Grid_Popup_Manage_button_Close_Click;
                grid_Popup_Manage_button_Save.Click += Grid_Popup_Manage_button_Save_Click;
                listView_Manage_AddAppCategory.SelectionChanged += ListView_Manage_AddAppCategory_SelectionChanged;

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
                grid_Popup_ContentInformation_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_ContentInformation_button_Save.Click += Grid_Popup_ContentInformation_button_Save_Click;
                grid_Popup_HowLongToBeat_button_Close.Click += Button_Popup_Close_Click;

                //Sorting functions
                grid_Popup_Sorting_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Sorting_button_Direction.Click += Grid_Popup_Sorting_button_Direction_Click;
                listView_Sorting.PreviewKeyUp += ListView_Sorting_KeyPressUp;
                listView_Sorting.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Sorting_MousePressUp), true);

                //Search functions
                grid_Search_textbox.TextChangedDelay += grid_Search_textbox_TextChanged;
                grid_Search_button_Reset.Click += grid_Search_button_Reset_Click;

                //Text Input functions
                grid_Popup_TextInput_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_TextInput_textbox.PreviewKeyUp += Grid_Popup_TextInput_textbox_PreviewKeyUp;
                grid_Popup_TextInput_button_Reset.Click += Grid_Popup_TextInput_button_Reset_Click;
                grid_Popup_TextInput_button_Set.Click += Button_TextInputConfirmText_Click;
                grid_Popup_TextInput_button_ConfirmText.Click += Button_TextInputConfirmText_Click;

                //File Picker functions
                listView_FilePicker.PreviewKeyUp += ListView_FilePicker_KeyPressUp;
                listView_FilePicker.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_FilePicker_MousePressUp), true);
                grid_Popup_FilePicker_button_SelectFolder.Click += Grid_Popup_FilePicker_button_SelectFolder_Click;

                //Profile Manager functions
                grid_Popup_ProfileManager_button_ControllerRight.Click += Button_Popup_Close_Click;
                grid_Popup_ProfileManager_button_ChangeProfile.Click += Grid_Popup_ProfileManager_button_ChangeProfile_Click;
                grid_Popup_ProfileManager_button_ProfileAdd.Click += Grid_Popup_ProfileManager_button_ProfileAdd_Click;
                grid_Popup_ProfileManager_textbox_ProfileString1.KeyDown += grid_Popup_ProfileManager_textbox_ProfileString_KeyDown;
                grid_Popup_ProfileManager_textbox_ProfileString2.KeyDown += grid_Popup_ProfileManager_textbox_ProfileString_KeyDown;
                listView_ProfileManager.PreviewKeyUp += ListView_ProfileManager_KeyPressUp;
                listView_ProfileManager.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_ProfileManager_MousePressUp), true);

                //Color Picker functions
                grid_Popup_ColorPicker_button_ControllerRight.Click += Button_Popup_Close_Click;
                listView_ColorPicker.PreviewKeyUp += ListView_ColorPicker_KeyPressUp;
                listView_ColorPicker.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_ColorPicker_MousePressUp), true);

                //Welcome functions
                grid_Popup_Welcome_button_Start.Click += Grid_Popup_Welcome_button_Start_Click;
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
                btn_Settings_AppQuickLaunch.Click += Button_Settings_AppQuickLaunch;
                btn_Settings_LaunchDirectXInput.Click += Button_LaunchDirectXInput_Click;
                btn_Settings_LaunchScreenCaptureTool.Click += Button_LaunchScreenCaptureTool_Click;
                btn_Settings_LaunchFpsOverlayer.Click += Button_LaunchFpsOverlayer_Click;
                btn_Settings_CheckForUpdate.Click += Button_Settings_CheckForUpdate_Click;
                btn_Settings_ColorPickerAccent.Click += Button_Settings_ColorPickerAccent;
                btn_Settings_InterfaceSoundPackName.Click += Button_Settings_InterfaceSoundPackName;
                btn_Settings_InterfaceClockStyleName.Click += Button_Settings_InterfaceClockStyleName;
                btn_Settings_InterfaceFontStyleName.Click += Button_Settings_InterfaceFontStyleName;
                listView_SettingsMenu.PreviewKeyUp += ListView_Settings_KeyPressUp;
                listView_SettingsMenu.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_Settings_MousePressUp), true);
                listView_LauncherSetting.PreviewKeyUp += ListView_LauncherSetting_KeyPressUp;
                listView_LauncherSetting.AddHandler(PointerReleasedEvent, new PointerEventHandler(ListView_LauncherSetting_MousePressUp), true);

                //Monitor functions
                btn_Monitor_Switch_Primary.Click += Btn_Monitor_Switch_Primary_Click;
                btn_Monitor_Switch_Secondary.Click += Btn_Monitor_Switch_Secondary_Click;
                btn_Monitor_Switch_Duplicate.Click += Btn_Monitor_Switch_Duplicate_Click;
                btn_Monitor_Switch_Extend.Click += Btn_Monitor_Switch_Extend_Click;
                btn_Monitor_HDR_Enable.Click += Btn_Monitor_HDR_Enable_Click;
                btn_Monitor_HDR_Disable.Click += Btn_Monitor_HDR_Disable_Click;

                //Help functions
                btn_Help_ProjectWebsite.Click += Button_Help_ProjectWebsite_Click;
                btn_Help_OpenDonation.Click += Button_Help_OpenDonation_Click;

                //Global functions
                //this.AddHandler(PointerMovedEvent, new PointerEventHandler(AVAdjustCursor.PointerEvent_AdjustCursor), true);
                this.AddHandler(PointerPressedEvent, new PointerEventHandler(WindowMain_PreviewMouseDown), true);
                this.AddHandler(PreviewKeyUpEvent, new KeyEventHandler(WindowMain_KeyPressUp), true);

                Debug.WriteLine("Registered all the interface handlers.");
            }
            catch { }
        }

        //Adjust the application font family
        void AdjustApplicationFontStyle()
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
                    string fontPathUser = AVFunctions.ApplicationPathRoot() + "/Assets/User/Fonts/" + interfaceFontStyleName + ".ttf";
                    string fontPathDefault = AVFunctions.ApplicationPathRoot() + "/Assets/Default/Fonts/" + interfaceFontStyleName + ".ttf";
                    if (File.Exists(fontPathUser))
                    {
                        //FixStyle
                        //ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPathUser);
                        //this.FontFamily = fontFamilies.FirstOrDefault();
                    }
                    else if (File.Exists(fontPathDefault))
                    {
                        //FixStyle
                        //ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(fontPathDefault);
                        //this.FontFamily = fontFamilies.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed setting application font: " + ex.Message);
            }
        }

        //Update the user interface clock style
        async Task UpdateClockStyle()
        {
            try
            {
                string clockStyle = SettingLoad(vConfigurationCtrlUI, "InterfaceClockStyleName", typeof(string));
                string clockPath = "Assets/Default/Clocks/" + clockStyle;
                if (Directory.Exists("Assets/User/Clocks/" + clockStyle))
                {
                    clockPath = "Assets/User/Clocks/" + clockStyle;
                }

                img_Main_Time_Face.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = [clockPath + "/Face.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });

                img_Main_Time_Hour.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = [clockPath + "/Hour.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });

                img_Main_Time_Minute.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = [clockPath + "/Minute.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });

                img_Main_Time_Center.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = [clockPath + "/Center.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
            }
            catch { }
        }

        //Update the user interface clock time
        void UpdateClockTime()
        {
            try
            {
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    //Rotate clock images
                    int clockSecond = DateTime.Now.Second;
                    int clockMinute = DateTime.Now.Minute;
                    int clockHour = DateTime.Now.Hour;
                    RotateTransform rotateTransformMinute = new RotateTransform()
                    {
                        Angle = (clockMinute * 360 / 60)
                    };
                    img_Main_Time_Minute.RenderTransform = rotateTransformMinute;
                    img_Main_Time_Minute.RenderTransformOrigin = new Point(0.5, 0.5);
                    RotateTransform rotateTransformHour = new RotateTransform()
                    {
                        Angle = (clockHour * 360 / 12)
                    };
                    img_Main_Time_Hour.RenderTransform = rotateTransformHour;
                    img_Main_Time_Hour.RenderTransformOrigin = new Point(0.5, 0.5);

                    //Update time and date
                    txt_Main_Date.Text = DateTime.Now.ToString("d MMMM");
                    txt_Main_Time.Text = DateTime.Now.ToShortTimeString();

                    //Change visibility
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
                //FixStyle
                //foreach (ScrollViewerLoopHorizontal scrollViewer in AVFunctions.FindVisualChildren<ScrollViewerLoopHorizontal>(this))
                //{
                //    scrollViewer.ScrollPaused = pauseScroll;
                //}
                //foreach (ScrollViewerLoopVertical scrollViewer in AVFunctions.FindVisualChildren<ScrollViewerLoopVertical>(this))
                //{
                //    scrollViewer.ScrollPaused = pauseScroll;
                //}
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
                bool runningBattleNet = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "battle.net");
                bool runningDiscord = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "discord");
                bool runningDirectXInput = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "directxinput");
                bool runningScreenCaptureTool = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "screencapturetool");
                bool runningFpsOverlayer = processMultiList.Any(x => x.ExeNameNoExt.ToLower() == "fpsoverlayer");

                DispatcherInvoke(this.Dispatcher, delegate
                {
                    img_Menu_SteamStatus.Opacity = runningSteam ? 1.00 : 0.40;
                    img_Menu_EADesktopStatus.Opacity = runningEADesktop ? 1.00 : 0.40;
                    img_Menu_GoGStatus.Opacity = runningGog ? 1.00 : 0.40;
                    img_Menu_UbisoftStatus.Opacity = runningUbisoft ? 1.00 : 0.40;
                    img_Menu_EpicStatus.Opacity = runningEpic ? 1.00 : 0.40;
                    img_Menu_BattleNetStatus.Opacity = runningBattleNet ? 1.00 : 0.40;
                    img_Menu_DiscordStatus.Opacity = runningDiscord ? 1.00 : 0.40;
                    img_Menu_DirectXInputStatus.Opacity = runningDirectXInput ? 1.00 : 0.40;
                    img_Menu_ScreenCaptureToolStatus.Opacity = runningScreenCaptureTool ? 1.00 : 0.40;
                    img_Menu_FpsOverlayerStatus.Opacity = runningFpsOverlayer ? 1.00 : 0.40;
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
                double ApplicationImageMaxWidth = 80;

                Application.Current.Resources["ApplicationPanelSize"] = ApplicationPanelSize + targetSize;
                Application.Current.Resources["ApplicationImageHeight"] = ApplicationImageHeight + targetSize;
                Application.Current.Resources["ApplicationImageMaxWidth"] = ApplicationImageMaxWidth + targetSize;
            }
            catch { }
        }

        //Set content and resource images with Cache OnLoad
        async Task SetContentResourceXamlImages()
        {
            try
            {
                img_Menu_SteamStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Steam"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_UbisoftStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Ubisoft"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_EADesktopStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["EA Desktop"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_GoGStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["GoG"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_EpicStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Epic"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_BattleNetStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Battle.net"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_DiscordStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Discord"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_DirectXInputStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["DirectXInput"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_FpsOverlayerStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["FpsOverlayer"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                img_Menu_ScreenCaptureToolStatus.Source = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["ScreenCaptureTool"],
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });

                //Check if the first launch logo's need to be loaded
                if (SettingLoad(vConfigurationCtrlUI, "AppFirstLaunch", typeof(bool)))
                {
                    grid_Popup_Welcome_img_Edge.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Edge"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_Kodi.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Kodi"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_Spotify.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Spotify"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_PSRemote.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Remote Play"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_Discord.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Discord"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_Steam.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Steam"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_EADesktop.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["EA Desktop"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_Ubisoft.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Ubisoft"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_GoG.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["GoG"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_Battle.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Battle.net"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    grid_Popup_Welcome_img_Epic.Source = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Epic"],
                        SearchPaths = vImageSourceFoldersAppsCombined,
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SetContentResourceXamlImages failed: " + ex.Message);
            }
        }
    }
}