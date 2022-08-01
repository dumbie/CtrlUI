using ArnoldVinkCode;
using ArnoldVinkCode.Styles;
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
using System.Windows.Media;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputInterop;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check the top or bottom listbox
        ListBox TopVisibleListBoxWithItems()
        {
            try
            {
                if (sp_Games.Visibility == Visibility.Visible && lb_Games.Items.Count > 0) { return lb_Games; }
                else if (sp_Apps.Visibility == Visibility.Visible && lb_Apps.Items.Count > 0) { return lb_Apps; }
                else if (sp_Emulators.Visibility == Visibility.Visible && lb_Emulators.Items.Count > 0) { return lb_Emulators; }
                else if (sp_Launchers.Visibility == Visibility.Visible && lb_Launchers.Items.Count > 0) { return lb_Launchers; }
                else if (sp_Shortcuts.Visibility == Visibility.Visible && lb_Shortcuts.Items.Count > 0) { return lb_Shortcuts; }
                else if (sp_Processes.Visibility == Visibility.Visible && lb_Processes.Items.Count > 0) { return lb_Processes; }
            }
            catch { }
            return null;
        }
        ListBox BottomVisibleListBox()
        {
            try
            {
                if (sp_Processes.Visibility == Visibility.Visible) { return lb_Processes; }
                else if (sp_Shortcuts.Visibility == Visibility.Visible) { return lb_Shortcuts; }
                else if (sp_Launchers.Visibility == Visibility.Visible) { return lb_Launchers; }
                else if (sp_Emulators.Visibility == Visibility.Visible) { return lb_Emulators; }
                else if (sp_Apps.Visibility == Visibility.Visible) { return lb_Apps; }
                else if (sp_Games.Visibility == Visibility.Visible) { return lb_Games; }
            }
            catch { }
            return null;
        }

        //Register Interface Handlers
        void RegisterInterfaceHandlers()
        {
            try
            {
                //Make sure the correct window style is set
                StateChanged += CheckWindowStateAndStyle;

                //Main menu functions
                grid_Popup_MainMenu_button_Close.Click += Button_Popup_Close_Click;
                listbox_MainMenu.PreviewKeyUp += ListBox_Menu_KeyPressUp;
                listbox_MainMenu.PreviewMouseUp += ListBox_Menu_MousePressUp;
                button_MenuHamburger.Click += Button_MenuHamburger_Click;
                button_MenuSearch.Click += Button_MenuSearch_Click;
                button_MenuSorting.Click += Button_MenuSorting_Click;

                //App list functions
                lb_Search.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Search.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Games.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Games.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Games.GotFocus += ListBox_Apps_GotFocus;
                lb_Apps.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Apps.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Apps.GotFocus += ListBox_Apps_GotFocus;
                lb_Emulators.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Emulators.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Emulators.GotFocus += ListBox_Apps_GotFocus;
                lb_Launchers.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Launchers.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Launchers.GotFocus += ListBox_Apps_GotFocus;
                lb_Shortcuts.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Shortcuts.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Shortcuts.GotFocus += ListBox_Apps_GotFocus;
                lb_Processes.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Processes.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Processes.GotFocus += ListBox_Apps_GotFocus;

                //MessageBox list functions
                lb_MessageBox.PreviewKeyUp += ListBox_MessageBox_KeyPressUp;
                lb_MessageBox.PreviewMouseUp += ListBox_MessageBox_MousePressUp;

                //Manage functions
                btn_Manage_ResetAppLogo.Click += Button_Manage_ResetAppLogo_Click;
                btn_Manage_SaveEditApp.Click += Button_Manage_SaveEditApp_Click;
                lb_Manage_AddAppCategory.SelectionChanged += Lb_Manage_AddAppCategory_SelectionChanged;
                btn_AddAppLogo.Click += Button_AddAppLogo_Click;
                btn_AddAppExePath.Click += Button_AddAppExePath_Click;
                btn_AddAppPathLaunch.Click += Button_AddAppPathLaunch_Click;
                btn_AddAppPathRoms.Click += Button_AddAppPathRoms_Click;
                checkbox_AddLaunchEnableHDR.Click += Checkbox_AddLaunchEnableHDR_Click;
                checkbox_AddLaunchSkipRom.Click += Checkbox_AddLaunchSkipRom_Click;

                //Move app functions
                btn_MoveAppLeft.Click += Btn_MoveAppLeft_Click;
                btn_MoveAppRight.Click += Btn_MoveAppRight_Click;

                //Popup functions
                grid_Popup_FilePicker_button_ControllerRight.Click += Button_FilePicker_button_ControllerRight_Click;
                grid_Popup_FilePicker_button_ControllerLeft.Click += Button_FilePicker_button_ControllerLeft_Click;
                grid_Popup_FilePicker_button_ControllerUp.Click += Button_FilePicker_button_ControllerUp_Click;
                grid_Popup_FilePicker_button_ControllerStart.Click += Button_FilePicker_button_ControllerStart_Click;
                grid_Popup_Manage_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Monitor_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Help_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_MoveApplication_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_MessageBox_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Welcome_button_Close.Click += Button_Popup_Close_Click;

                //Search functions
                grid_Popup_Search_textbox.TextChanged += Grid_Popup_Search_textbox_TextChanged;
                grid_Popup_Search_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Search_button_InteractItem.Click += Button_SearchInteractItem_Click;
                grid_Popup_Search_button_KeyboardControllerIcon.Click += Button_SearchKeyboardController_Click;
                grid_Popup_Search_button_Reset.Click += Grid_Popup_Search_button_Reset_Click;

                //Text Input functions
                grid_Popup_TextInput_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_TextInput_textbox.PreviewKeyUp += Grid_Popup_TextInput_textbox_PreviewKeyUp;
                grid_Popup_TextInput_button_KeyboardControllerIcon.Click += Button_TextInputKeyboardController_Click;
                grid_Popup_TextInput_button_Reset.Click += Grid_Popup_TextInput_button_Reset_Click;
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
                btn_Settings_ChangeBackgroundImage.Click += Button_Settings_ChangeBackgroundImage_Click;
                btn_Settings_ChangeBackgroundVideo.Click += Button_Settings_ChangeBackgroundVideo_Click;
                btn_Settings_ResetBackground.Click += Button_Settings_ResetBackground_Click;
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

                //MediaElement functions
                grid_Video_Background.MediaEnded += Grid_Video_Background_MediaEnded;
                //grid_Video_Background.MediaFailed += Grid_Video_Background_MediaFailed;

                //Global functions
                this.PreviewMouseMove += WindowMain_MouseMove;
                this.PreviewMouseDown += WindowMain_PreviewMouseDown;
                this.PreviewKeyUp += WindowMain_KeyPressUp;

                Debug.WriteLine("Registered all the interface handlers.");
            }
            catch { }
        }

        //Scroll to apps listbox header
        void ListBox_Apps_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get listbox header position
                Point listboxPoint = scrollviewer_AppLists.TranslatePoint(new Point(), (ListBox)sender);
                double headerOffset = scrollviewer_AppLists.VerticalOffset - listboxPoint.Y - 50;

                //Check if listbox header is in view
                if (headerOffset < scrollviewer_AppLists.VerticalOffset)
                {
                    scrollviewer_AppLists.ScrollToVerticalOffset(headerOffset);
                    //Debug.WriteLine("Scrolled to application listbox header.");
                }
            }
            catch { }
        }

        //Adjust the application font family
        void UpdateAppFontStyle()
        {
            try
            {
                string interfaceFontStyleName = Setting_Load(vConfigurationCtrlUI, "InterfaceFontStyleName").ToString();
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
                string clockStyle = Setting_Load(vConfigurationCtrlUI, "InterfaceClockStyleName").ToString();
                string clockPath = "Assets/Default/Clocks/" + clockStyle;
                if (Directory.Exists("Assets/User/Clocks/" + clockStyle))
                {
                    clockPath = "Assets/User/Clocks/" + clockStyle;
                }

                img_Main_Time_Face.Source = FileToBitmapImage(new string[] { clockPath + "/Face.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 40, 0);
                img_Main_Time_Hour.Source = FileToBitmapImage(new string[] { clockPath + "/Hour.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 40, 0);
                img_Main_Time_Minute.Source = FileToBitmapImage(new string[] { clockPath + "/Minute.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 40, 0);
                img_Main_Time_Center.Source = FileToBitmapImage(new string[] { clockPath + "/Center.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 40, 0);
            }
            catch { }
        }

        //Update the user interface clock time
        void UpdateClockTime()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Rotate the clock images
                    int clockSecond = DateTime.Now.Second;
                    int clockMinute = DateTime.Now.Minute;
                    int clockHour = DateTime.Now.Hour;
                    img_Main_Time_Minute.LayoutTransform = new RotateTransform((clockMinute * 360 / 60) + (clockSecond / 60 * 6));
                    img_Main_Time_Hour.LayoutTransform = new RotateTransform((clockHour * 360 / 12) + (clockMinute / 2));

                    //Change the time format
                    if (vMainMenuOpen)
                    {
                        txt_Main_Date.Text = DateTime.Now.ToString("d MMMM");
                        txt_Main_Time.Text = DateTime.Now.ToShortTimeString();
                    }
                    else
                    {
                        txt_Main_Date.Text = string.Empty;
                        txt_Main_Time.Text = DateTime.Now.ToShortTimeString();
                    }
                });
            }
            catch { }
        }

        //Update the current window status
        async Task UpdateWindowStatus()
        {
            try
            {
                vProcessDirectXInput = GetProcessByNameOrTitle("DirectXInput", false, true);
                int focusedAppId = GetProcessMultiFromWindowHandle(GetForegroundWindow()).Identifier;

                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    try
                    {
                        if (WindowState == WindowState.Maximized) { vAppMaximized = true; } else { vAppMaximized = false; }
                        if (WindowState == WindowState.Minimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vProcessCurrent.Id == focusedAppId)
                        {
                            await AppWindowActivated();
                        }
                        else
                        {
                            AppWindowDeactivated();
                        }
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Application window activated event
        async Task AppWindowActivated()
        {
            try
            {
                if (!vAppActivated)
                {
                    vAppActivated = true;
                    Debug.WriteLine("Activated the application.");

                    //Play background media
                    grid_Video_Background.Play();

                    //Enable application window
                    AppWindowEnable();

                    //Hide the mouse cursor
                    await MouseCursorHide();

                    //Resume ScrollViewerLoops
                    PauseResumeScrollviewerLoops(false);

                    //Prevent monitor sleep
                    UpdateMonitorSleepAuto();
                }
            }
            catch { }
        }

        //Application window deactivated event
        void AppWindowDeactivated()
        {
            try
            {
                if (vAppActivated)
                {
                    vAppActivated = false;
                    Debug.WriteLine("Deactivated the application.");

                    //Pause background media
                    grid_Video_Background.Pause();

                    //Disable application window
                    AppWindowDisable("Application window is not activated.");

                    //Pause ScrollViewerLoops
                    PauseResumeScrollviewerLoops(true);

                    //Allow monitor sleep
                    UpdateMonitorSleepAllow();
                }
            }
            catch { }
        }

        //Enable application window
        void AppWindowEnable()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Enable the application window
                    grid_WindowActive.Visibility = Visibility.Collapsed;
                });
            }
            catch { }
        }

        //Disable application window
        void AppWindowDisable(string windowText)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Update window status message
                    grid_WindowActiveText.Text = windowText;

                    //Disable the application window
                    grid_WindowActive.Visibility = Visibility.Visible;
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
        void CheckAppRunningStatus(Process[] processesList)
        {
            try
            {
                //Check if processes list is provided
                if (processesList == null)
                {
                    processesList = Process.GetProcesses();
                }

                //Update main menu launchers status
                bool runningSteam = processesList.Any(x => x.ProcessName.ToLower() == "steam");
                bool runningEADesktop = processesList.Any(x => x.ProcessName.ToLower() == "eadesktop" || x.ProcessName.ToLower() == "origin");
                bool runningGog = processesList.Any(x => x.ProcessName.ToLower() == "galaxyclient");
                bool runningUbisoft = processesList.Any(x => x.ProcessName.ToLower() == "ubisoftconnect" || x.ProcessName.ToLower() == "upc");
                bool runningBethesda = processesList.Any(x => x.ProcessName.ToLower() == "bethesdanetlauncher");
                bool runningEpic = processesList.Any(x => x.ProcessName.ToLower() == "epicgameslauncher");
                bool runningBlizzard = processesList.Any(x => x.ProcessName.ToLower() == "battle.net");
                bool runningRockstar = processesList.Any(x => x.ProcessName.ToLower() == "rockstarservice");
                bool runningDiscord = processesList.Any(x => x.ProcessName.ToLower() == "discord");
                bool runningDirectXInput = processesList.Any(x => x.ProcessName.ToLower() == "directxinput");
                bool runningFpsOverlayer = processesList.Any(x => x.ProcessName.ToLower() == "fpsoverlayer");

                AVActions.ActionDispatcherInvoke(delegate
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

                    if (runningBethesda)
                    {
                        img_Menu_BethesdaStatus.Opacity = 1.00;
                    }
                    else
                    {
                        img_Menu_BethesdaStatus.Opacity = 0.40;
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

        //Show the mouse cursor
        void MouseCursorShow()
        {
            try
            {
                //Update the last mouse interaction time
                vMouseLastInteraction = GetSystemTicksMs();

                //Set the mouse cursor when not visible
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (this.Cursor == Cursors.None)
                    {
                        Mouse.SetCursor(Cursors.Arrow);
                    }
                });
            }
            catch { }
        }

        //Hide the mouse cursor
        async Task MouseCursorHide()
        {
            try
            {
                //Update the last mouse interaction time
                vMouseLastInteraction = GetSystemTicksMs();

                //Check if mouse hide setting is enabled
                if (!Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideMouseCursor")))
                {
                    return;
                }

                //Check if keyboard is visible, application is active and any controller is connected
                IntPtr keyboardWindowHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "DirectXInput Keyboard (Visible)");
                if (!vAppActivated || !vControllerAnyConnected() || keyboardWindowHandle != IntPtr.Zero)
                {
                    return;
                }

                //Move the mouse cursor
                Point LocationFromScreen = new Point();
                AVActions.ActionDispatcherInvoke(delegate
                {
                    LocationFromScreen = this.PointToScreen(new Point(255, 44));
                });

                int targetX = Convert.ToInt32(LocationFromScreen.X);
                int targetY = Convert.ToInt32(LocationFromScreen.Y);
                SetCursorPos(targetX, targetY);
                await Task.Delay(10);

                //Hide the mouse cursor
                AVActions.ActionDispatcherInvoke(delegate
                {
                    Mouse.SetCursor(Cursors.None);
                });

                //Debug.WriteLine("Hiding the mouse cursor.");
            }
            catch { }
        }

        //Check if the mouse cursor has moved
        async Task MouseCursorCheckMovement()
        {
            try
            {
                //Get the current mouse position
                GetCursorPos(out WindowPoint MouseCurrentPosition);

                //Check if the mouse has moved since the last time
                bool LastInteraction = (GetSystemTicksMs() - vMouseLastInteraction) >= 5000;
                bool LastMovement = MouseCurrentPosition.X == vMousePreviousPosition.X && MouseCurrentPosition.Y == vMousePreviousPosition.Y;
                if (LastInteraction && LastMovement)
                {
                    await MouseCursorHide();
                }

                //Update the previous mouse position
                vMousePreviousPosition = MouseCurrentPosition;
            }
            catch { }
        }

        //Hide or recover the CtrlUI application
        async Task AppWindow_HideShow()
        {
            try
            {
                Debug.WriteLine("Show or hide the CtrlUI window.");

                //Get the current focused application
                ProcessMulti foregroundProcess = GetProcessMultiFromWindowHandle(GetForegroundWindow());

                if (vAppMinimized || !vAppActivated)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                    //Check previous focused application
                    try
                    {
                        //Check if application title or process is blacklisted
                        bool titleBlacklisted = vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == foregroundProcess.Title.ToLower());
                        bool processBlacklisted = vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == foregroundProcess.Name.ToLower());
                        if (!titleBlacklisted && !processBlacklisted)
                        {
                            //Save the previous focused application
                            vPrevFocusedProcess = foregroundProcess;
                        }
                    }
                    catch { }

                    //Disable top most window from foreground process
                    try
                    {
                        Debug.WriteLine("Disabling top most from process: " + foregroundProcess.Name);
                        SetWindowPos(foregroundProcess.WindowHandle, (IntPtr)WindowPosition.NoTopMost, 0, 0, 0, 0, (int)WindowSWP.NOMOVE | (int)WindowSWP.NOSIZE);
                    }
                    catch { }

                    //Force focus on CtrlUI
                    await PrepareFocusProcessWindow("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, true, false, false);
                }
                else
                {
                    //Disable top most window from foreground process
                    try
                    {
                        Debug.WriteLine("Disabling top most from process: " + foregroundProcess.Name);
                        SetWindowPos(foregroundProcess.WindowHandle, (IntPtr)WindowPosition.NoTopMost, 0, 0, 0, 0, (int)WindowSWP.NOMOVE | (int)WindowSWP.NOSIZE);
                    }
                    catch { }

                    //Force focus on CtrlUI
                    await PrepareFocusProcessWindow("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, true, false, false);

                    //Check if a previous process is available
                    if (vPrevFocusedProcess == null)
                    {
                        Debug.WriteLine("Previous application process not found.");
                        await ApplicationPopupMinimize("No application to show found");
                        return;
                    }

                    //Check if application name is blacklisted
                    if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == vPrevFocusedProcess.Name.ToLower()))
                    {
                        Debug.WriteLine("Previous process name is blacklisted: " + vPrevFocusedProcess.Name);
                        await ApplicationPopupMinimize("Previous app name is blacklisted");
                        return;
                    }

                    //Check if application title is blacklisted
                    if (vCtrlIgnoreProcessName.Any(x => x.String1.ToLower() == vPrevFocusedProcess.Title.ToLower()))
                    {
                        Debug.WriteLine("Previous process title is blacklisted: " + vPrevFocusedProcess.Title);
                        await ApplicationPopupMinimize("Previous app title is blacklisted");
                        return;
                    }

                    //Check if application process is still running
                    if (!CheckRunningProcessByNameOrTitle(vPrevFocusedProcess.Name, false, true))
                    {
                        Debug.WriteLine("Previous process is no longer running.");
                        await ApplicationPopupMinimize("Previous app is no longer running");
                        return;
                    }

                    //Show application switch popup
                    await ApplicationPopupSwitch();
                }
            }
            catch
            {
                await Notification_Send_Status("Close", "Failed to minimize or show app");
                Debug.WriteLine("Failed to minimize or show application.");
            }
        }

        //Show application switch popup
        async Task ApplicationPopupSwitch()
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerSwitch = new DataBindString();
                AnswerSwitch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerSwitch.Name = "Return to application";
                Answers.Add(AnswerSwitch);

                DataBindString AnswerClose = new DataBindString();
                AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerClose.Name = "Close the application";
                Answers.Add(AnswerClose);

                DataBindString AnswerMinimize = new DataBindString();
                AnswerMinimize.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMinimize.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerMinimize.Name = "Minimize CtrlUI";
                Answers.Add(AnswerMinimize);

                DataBindString messageResult = await Popup_Show_MessageBox("Return to previous application?", "", "You can always return to " + vPrevFocusedProcess.Title + " later on.", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerSwitch)
                    {
                        //Minimize the CtrlUI window
                        if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "MinimizeAppOnShow")))
                        {
                            await AppMinimize(true);
                        }

                        //Check keyboard controller launch
                        string fileNameNoExtension = Path.GetFileNameWithoutExtension(vPrevFocusedProcess.Name);
                        bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == vPrevFocusedProcess.Path.ToLower());
                        bool keyboardLaunch = keyboardProcess && vControllerAnyConnected();

                        //Force focus on the app
                        await PrepareFocusProcessWindow(vPrevFocusedProcess.Title, vPrevFocusedProcess.Identifier, vPrevFocusedProcess.WindowHandle, 0, false, false, false, keyboardLaunch);
                    }
                    else if (messageResult == AnswerClose)
                    {
                        await Notification_Send_Status("AppClose", "Closing " + vPrevFocusedProcess.Title);
                        Debug.WriteLine("Closing process: " + vPrevFocusedProcess.Title + " / " + vPrevFocusedProcess.Identifier + " / " + vPrevFocusedProcess.WindowHandle);

                        //Check if the application is UWP or Win32
                        if (CheckProcessIsUwp(vPrevFocusedProcess.WindowHandle))
                        {
                            bool ClosedProcess = await CloseProcessUwpByWindowHandleOrProcessId(vPrevFocusedProcess.Title, vPrevFocusedProcess.Identifier, vPrevFocusedProcess.WindowHandle);
                            if (ClosedProcess)
                            {
                                await Notification_Send_Status("AppClose", "Closed " + vPrevFocusedProcess.Title);
                                Debug.WriteLine("Closed process: " + vPrevFocusedProcess.Title + " / " + vPrevFocusedProcess.Identifier + " / " + vPrevFocusedProcess.WindowHandle);
                                vPrevFocusedProcess = null;
                            }
                        }
                        else
                        {
                            bool ClosedProcess = CloseProcessById(vPrevFocusedProcess.Identifier);
                            if (ClosedProcess)
                            {
                                await Notification_Send_Status("AppClose", "Closed " + vPrevFocusedProcess.Title);
                                Debug.WriteLine("Closed process: " + vPrevFocusedProcess.Title + " / " + vPrevFocusedProcess.Identifier + " / " + vPrevFocusedProcess.WindowHandle);
                                vPrevFocusedProcess = null;
                            }
                        }
                    }
                    else if (messageResult == AnswerMinimize)
                    {
                        //Minimize the CtrlUI window
                        await AppMinimize(false);
                    }
                }
            }
            catch { }
        }

        //Show application minimize popup
        async Task ApplicationPopupMinimize(string headerString)
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerMinimize = new DataBindString();
                AnswerMinimize.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMinimize.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerMinimize.Name = "Minimize CtrlUI";
                Answers.Add(AnswerMinimize);

                DataBindString messageResult = await Popup_Show_MessageBox(headerString, "", "Would you like to minimize CtrlUI?", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerMinimize)
                    {
                        //Minimize the CtrlUI window
                        await AppMinimize(false);
                    }
                }
            }
            catch { }
        }

        //Minimize the application and save previous state
        async Task AppMinimize(bool minimizeDelay)
        {
            try
            {
                Debug.WriteLine("Minimizing the CtrlUI window.");

                //Save the CtrlUI window state
                vAppActivated = false;
                vAppMinimized = true;

                //Disable the CtrlUI window
                grid_WindowActive.Opacity = 0.80;
                grid_App.IsHitTestVisible = false;

                //Minimize the CtrlUI application
                WindowState = WindowState.Minimized;

                //Wait for application to minimize
                if (minimizeDelay)
                {
                    await Task.Delay(1000);
                }
            }
            catch { }
        }

        //Switch application between fullscreen and windowed
        async Task AppSwitchScreenMode(bool forceMaximized, bool forceNormal)
        {
            try
            {
                if (!forceNormal && (forceMaximized || WindowState != WindowState.Maximized))
                {
                    Debug.WriteLine("Maximizing CtrlUI window.");

                    WindowState = WindowState.Maximized;

                    ////Hide the Windows taskbar
                    //IntPtr hWnd = FindWindow("Shell_TrayWnd", string.Empty);
                    //ShowWindow(hWnd, (int)WindowShowCmd.Hide);

                    vAppMaximized = true;
                }
                else
                {
                    Debug.WriteLine("Restoring CtrlUI window.");

                    WindowState = WindowState.Normal;

                    ////Show the Windows taskbar
                    //IntPtr hWnd = FindWindow("Shell_TrayWnd", string.Empty);
                    //ShowWindow(hWnd, (int)WindowShowCmd.Normal);

                    vAppMaximized = false;
                }

                //Hide the mouse cursor
                await MouseCursorHide();
            }
            catch { }
        }

        //Adjust the application font size
        void AdjustApplicationFontSize()
        {
            try
            {
                int targetFontSize = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "AppFontSize"));
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

        //Set content and resource images with Cache OnLoad
        void SetContentResourceXamlImages()
        {
            try
            {
                img_Menu_SteamStatus.Source = vImagePreloadSteam;
                img_Menu_UbisoftStatus.Source = vImagePreloadUbisoft;
                img_Menu_EADesktopStatus.Source = vImagePreloadEADesktop;
                img_Menu_GoGStatus.Source = vImagePreloadGoG;
                img_Menu_BethesdaStatus.Source = vImagePreloadBethesda;
                img_Menu_EpicStatus.Source = vImagePreloadEpic;
                img_Menu_BattleNetStatus.Source = vImagePreloadBattleNet;
                img_Menu_RockstarStatus.Source = vImagePreloadRockstar;
                img_Menu_DiscordStatus.Source = vImagePreloadDiscord;

                img_Menu_DirectXInputStatus.Source = FileToBitmapImage(new string[] { "DirectXInput" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);
                img_Menu_FpsOverlayerStatus.Source = FileToBitmapImage(new string[] { "FpsOverlayer" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 100, 0);

                //Check if the first launch logo's need to be loaded
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "AppFirstLaunch")))
                {
                    grid_Popup_Welcome_img_Edge.Source = FileToBitmapImage(new string[] { "Edge" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 75, 0);
                    grid_Popup_Welcome_img_Kodi.Source = FileToBitmapImage(new string[] { "Kodi" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 75, 0);
                    grid_Popup_Welcome_img_Spotify.Source = FileToBitmapImage(new string[] { "Spotify" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 75, 0);
                    grid_Popup_Welcome_img_PSRemote.Source = FileToBitmapImage(new string[] { "Remote Play" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 75, 0);
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