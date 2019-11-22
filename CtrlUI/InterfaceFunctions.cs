using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInterface;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update an element visibility
        void UpdateElementVisibility(FrameworkElement Target, bool Visible)
        {
            try
            {
                if (Visible)
                {
                    AVActions.ActionDispatcherInvoke(delegate { Target.Visibility = Visibility.Visible; });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate { Target.Visibility = Visibility.Collapsed; });
                }
            }
            catch { }
        }

        //Check the top or bottom listbox
        ListBox TopVisibleListBoxWithItems()
        {
            try
            {
                if (sp_Games.Visibility == Visibility.Visible && lb_Games.Items.Count > 0) { return lb_Games; }
                else if (sp_Apps.Visibility == Visibility.Visible && lb_Apps.Items.Count > 0) { return lb_Apps; }
                else if (sp_Emulators.Visibility == Visibility.Visible && lb_Emulators.Items.Count > 0) { return lb_Emulators; }
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
                //Monitor window state changes
                SizeChanged += CheckWindowStateAndSize;
                StateChanged += CheckWindowStateAndSize;

                //Main menu functions
                grid_Popup_MainMenu_button_Close.Click += Button_Popup_Close_Click;
                listbox_MainMenu.PreviewKeyUp += ListBox_Menu_KeyPressUp;
                listbox_MainMenu.PreviewMouseUp += ListBox_Menu_MousePressUp;
                button_MenuHamburger.Click += Button_MenuHamburger_Click;
                button_MenuSearch.Click += Button_MenuSearch_Click;
                button_MenuSorting.Click += Button_MenuSorting_Click;
                button_MenuRefresh.Click += Button_MenuRefresh_Click;

                //App list functions
                lb_Search.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Search.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Games.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Games.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Apps.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Apps.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Emulators.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Emulators.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Shortcuts.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Shortcuts.PreviewMouseUp += ListBox_Apps_MousePressUp;
                lb_Processes.PreviewKeyUp += ListBox_Apps_KeyPressUp;
                lb_Processes.PreviewMouseUp += ListBox_Apps_MousePressUp;

                //MessageBox list functions
                lb_MessageBox.PreviewKeyUp += ListBox_MessageBox_KeyPressUp;
                lb_MessageBox.PreviewMouseUp += ListBox_MessageBox_MousePressUp;

                //Manage functions
                btn_Manage_SaveEditApp.Click += Button_Manage_SaveEditApp_Click;
                btn_Manage_AddUwpAdd.Click += Button_Manage_AddUwpAdd_Click;
                btn_Manage_AddAppCategory.Click += Button_ShowStringPicker;
                btn_Manage_MoveAppLeft.Click += Btn_Manage_MoveAppLeft_Click;
                btn_Manage_MoveAppRight.Click += Btn_Manage_MoveAppRight_Click;
                btn_Manage_Cancel.Click += Btn_Manage_Cancel_Click;

                //Media functions
                grid_Popup_Media_Previous.Click += Button_Media_PreviousItem;
                grid_Popup_Media_PlayPause.Click += Button_Media_PlayPause;
                grid_Popup_Media_Next.Click += Button_Media_NextItem;
                grid_Popup_Media_VolumeMute.Click += Button_Media_VolumeMute;
                grid_Popup_Media_VolumeDown.Click += Button_Media_VolumeDown;
                grid_Popup_Media_VolumeUp.Click += Button_Media_VolumeUp;

                //Popup functions
                grid_Popup_FilePicker_button_ControllerRight.Click += Button_FilePicker_button_ControllerRight_Click;
                grid_Popup_FilePicker_button_ControllerLeft.Click += Button_FilePicker_button_ControllerLeft_Click;
                grid_Popup_FilePicker_button_ControllerUp.Click += Button_FilePicker_button_ControllerUp_Click;
                grid_Popup_Media_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Manage_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Settings_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Help_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_MessageBox_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Welcome_button_Close.Click += Button_Popup_Close_Click;

                //Search functions
                lb_Search.PreviewKeyDown += ListBox_Search_KeyPressDown;
                grid_Popup_Search_textbox_Search.TextChanged += Grid_Popup_Search_textbox_Search_TextChanged;
                grid_Popup_Search_button_Close.Click += Button_Popup_Close_Click;
                grid_Popup_Search_button_KeyboardControllerIcon.Click += Button_SearchKeyboardController_Click;
                grid_Popup_Search_button_KeyboardControllerButton.Click += Button_SearchKeyboardController_Click;
                grid_Popup_Search_button_ResetSearch.Click += Grid_Popup_Search_button_ResetSearch_Click;

                //File Picker functions
                lb_FilePicker.PreviewKeyUp += ListBox_FilePicker_KeyPressUp;
                lb_FilePicker.PreviewKeyDown += ListBox_FilePicker_KeyPressDown;
                lb_FilePicker.PreviewMouseUp += ListBox_FilePicker_MousePressUp;
                lb_FilePicker.SelectionChanged += Lb_FilePicker_SelectionChanged;
                grid_Popup_FilePicker_button_SelectFolder.Click += Grid_Popup_FilePicker_button_SelectFolder_Click;
                btn_AddAppLogo.Click += Button_ShowFilePicker;
                btn_AddAppExePath.Click += Button_ShowFilePicker;
                btn_AddAppPathLaunch.Click += Button_ShowFilePicker;
                btn_AddAppPathRoms.Click += Button_ShowFilePicker;
                btn_Settings_ChangeBackground.Click += Button_ShowFilePicker;

                //Color Picker functions
                grid_Popup_ColorPicker_button_ControllerRight.Click += Button_Popup_Close_Click;
                lb_ColorPicker.PreviewKeyUp += ListBox_ColorPicker_KeyPressUp;
                lb_ColorPicker.PreviewMouseUp += ListBox_ColorPicker_MousePressUp;

                //Welcome functions
                grid_Popup_Welcome_button_LaunchDirectXInput.Click += Button_LaunchDirectXInput_Click;
                grid_Popup_Welcome_button_Start.Click += Button_Popup_Close_Click;
                grid_Popup_Welcome_button_Kodi.Click += Button_ShowFilePicker;
                grid_Popup_Welcome_button_Spotify.Click += Button_ShowFilePicker;
                grid_Popup_Welcome_button_Steam.Click += Button_ShowFilePicker;
                grid_Popup_Welcome_button_Origin.Click += Button_ShowFilePicker;
                grid_Popup_Welcome_button_Uplay.Click += Button_ShowFilePicker;
                grid_Popup_Welcome_button_GoG.Click += Button_ShowFilePicker;
                grid_Popup_Welcome_button_Battle.Click += Button_ShowFilePicker;
                grid_Popup_Welcome_button_PS4Remote.Click += Button_ShowFilePicker;

                //Settings functions
                btn_Settings_AppQuickLaunch.Click += Button_ShowStringPicker;
                btn_Settings_LaunchDirectXInput.Click += Button_LaunchDirectXInput_Click;
                btn_Settings_CheckControllers.Click += Button_Settings_CheckControllers_Click;
                btn_Settings_CheckForUpdate.Click += Button_Settings_CheckForUpdate_Click;
                btn_Settings_AddGeforceExperience.Click += Button_Settings_AddGeforceExperience_Click;
                btn_Settings_ColorPickerAccent.Click += Button_Settings_ColorPickerAccent;

                //Help functions
                btn_Help_ProjectWebsite.Click += Button_Help_ProjectWebsite_Click;
                btn_Help_OpenDonation.Click += Button_Help_OpenDonation_Click;

                //Global functions
                this.PreviewMouseMove += WindowMain_MouseMove;
                this.PreviewMouseDown += WindowMain_PreviewMouseDown;
                this.PreviewKeyUp += WindowMain_KeyPressUp;

                //Allow files to be dropped into window
                this.PreviewDrop += Application_DragDropFile;
                this.AllowDrop = true;

                Debug.WriteLine("Registered all the interface handlers.");
            }
            catch { }
        }

        //Update the user interface clock
        void UpdateClock()
        {
            try
            {
                //Check the clock image
                string ClockNumber = DateTime.Now.ToString("hmm");

                //Update the time and image
                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        img_Main_Time.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Clock/" + ClockNumber + ".png" }, IntPtr.Zero, -1);
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
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Update the current window status
        async Task UpdateWindowStatus()
        {
            try
            {
                vProcessDirectXInput = GetProcessByNameOrTitle("DirectXInput", false);
                vProcessKeyboardController = GetProcessByNameOrTitle("KeyboardController", false);
                int focusedAppId = GetFocusedProcess().Process.Id;
                bool appActivated = false;

                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        if (WindowState == WindowState.Maximized) { vAppMaximized = true; } else { vAppMaximized = false; }
                        if (WindowState == WindowState.Minimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vProcessCurrent.Id == focusedAppId)
                        {
                            grid_WindowActive.Opacity = 0;
                            grid_App.IsHitTestVisible = true;
                            if (!vAppActivated) { appActivated = true; }
                            vAppActivated = true;
                        }
                        else
                        {
                            grid_WindowActive.Opacity = 0.80;
                            grid_App.IsHitTestVisible = false;
                            vAppActivated = false;
                        }
                    }
                    catch { }
                });

                //Check if application window activated
                if (appActivated)
                {
                    await AppWindowActivated();
                }
            }
            catch { }
        }

        //Application window activated event
        async Task AppWindowActivated()
        {
            try
            {
                Debug.WriteLine("Welcome back to the application.");

                //Hide the mouse cursor
                await MouseCursorHide();
            }
            catch { }
        }

        //Update the applications running icons
        void UpdateAppRunningIcon(FrameworkElement Target, bool Enabled)
        {
            try
            {
                if (Enabled)
                {
                    AVActions.ActionDispatcherInvoke(delegate { Target.Opacity = 1.00; });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate { Target.Opacity = 0.30; });
                }
            }
            catch { }
        }

        //Check the applications running status
        async Task CheckAppRunningStatus(IEnumerable<Process> ProcessesList)
        {
            try
            {
                //Check if processes list is provided
                if (ProcessesList == null) { ProcessesList = Process.GetProcesses(); }

                //Update main menu launchers status
                UpdateAppRunningIcon(img_Menu_SteamStatus, ProcessesList.Any(x => x.ProcessName.ToLower() == "steam"));
                UpdateAppRunningIcon(img_Menu_OriginStatus, ProcessesList.Any(x => x.ProcessName.ToLower() == "origin"));
                UpdateAppRunningIcon(img_Menu_GoGStatus, ProcessesList.Any(x => x.ProcessName.ToLower() == "galaxyclient"));
                UpdateAppRunningIcon(img_Menu_UplayStatus, ProcessesList.Any(x => x.ProcessName.ToLower() == "upc"));
                UpdateAppRunningIcon(img_Menu_DirectXInput, ProcessesList.Any(x => x.ProcessName.ToLower() == "directxinput"));
                UpdateAppRunningIcon(img_Menu_BethesdaStatus, ProcessesList.Any(x => x.ProcessName.ToLower() == "bethesdanetlauncher"));
                UpdateAppRunningIcon(img_Menu_EpicStatus, ProcessesList.Any(x => x.ProcessName.ToLower() == "epicgameslauncher"));
                UpdateAppRunningIcon(img_Menu_BlizzardStatus, ProcessesList.Any(x => x.ProcessName.ToLower() == "battle.net"));

                //Update all the apps running status
                foreach (DataBindApp dataBindApp in CombineAppLists(true, false).Where(x => x.StatusLauncher == Visibility.Collapsed))
                {
                    try
                    {
                        //Debug.WriteLine("Checking application status: " + ListApp.Type + "/" + ListApp.Category + "/" + ListApp.Name + "/" + ListApp.PathExe);

                        //Get the process multi from DataBindApp
                        ProcessMulti multiProcess = await GetProcessMultiFromDataBindApp(dataBindApp, false);

                        //Check if the process multi is running
                        if (multiProcess == null)
                        {
                            dataBindApp.ProcessId = -1;
                            dataBindApp.WindowHandle = IntPtr.Zero;
                            dataBindApp.StatusRunning = Visibility.Collapsed;
                            dataBindApp.StatusSuspended = Visibility.Collapsed;
                            dataBindApp.RunningTimeLastUpdate = 0;
                            dataBindApp.ProcessRunningCount = string.Empty;
                            continue;
                        }

                        //Check process multi application status
                        if (multiProcess.ProcessId > 0) { dataBindApp.ProcessId = multiProcess.ProcessId; }
                        if (multiProcess.WindowHandle != IntPtr.Zero) { dataBindApp.WindowHandle = multiProcess.WindowHandle; }
                        dataBindApp.RunningTimeLastUpdate = Environment.TickCount;

                        //Update the application status icons
                        bool processSuspended = CheckProcessSuspended(multiProcess.ProcessThreads);
                        if (processSuspended)
                        {
                            dataBindApp.StatusRunning = Visibility.Collapsed;
                            dataBindApp.StatusSuspended = Visibility.Visible;
                        }
                        else
                        {
                            dataBindApp.StatusRunning = Visibility.Visible;
                            dataBindApp.StatusSuspended = Visibility.Collapsed;
                        }

                        if (multiProcess.ProcessCount > 1)
                        {
                            dataBindApp.ProcessRunningCount = Convert.ToString(multiProcess.ProcessCount);
                        }
                        else
                        {
                            dataBindApp.ProcessRunningCount = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed checking application status: " + dataBindApp.Name + "/" + ex.Message);
                    }
                }
            }
            catch { }
        }

        //Force focus on a listbox
        async Task FocusOnListbox(ListBox FocusListBox, bool FirstIndex, bool LastIndex, int IndexNumber)
        {
            try
            {
                //Select first available listbox
                if (FocusListBox != null && FocusListBox.IsEnabled && FocusListBox.Visibility == Visibility.Visible && FocusListBox.Items.Count > 0)
                {
                    //Update the listbox layout
                    FocusListBox.UpdateLayout();

                    //Select a listbox item index
                    ListBoxSelectIndex(FocusListBox, FirstIndex, LastIndex, IndexNumber);

                    //Focus on the listbox and item
                    int SelectedIndex = FocusListBox.SelectedIndex;

                    //Scroll to the listbox item
                    object ScrollListBoxItem = FocusListBox.Items[SelectedIndex];
                    FocusListBox.ScrollIntoView(ScrollListBoxItem);

                    //Force focus on an element
                    ListBoxItem FocusListBoxItem = (ListBoxItem)FocusListBox.ItemContainerGenerator.ContainerFromInd‌​ex(SelectedIndex);
                    await FocusOnElement(FocusListBoxItem, false, vProcessCurrent.MainWindowHandle);

                    Debug.WriteLine("Focusing on listbox index: " + SelectedIndex);
                }
                else
                {
                    Debug.WriteLine("Listbox cannot be focused on, pressing tab key.");
                    KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                }
            }
            catch
            {
                Debug.WriteLine("Failed focusing on the listbox, pressing tab key.");
                KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
            }
        }

        //Select a listbox item index
        void ListBoxSelectIndex(ListBox FocusListBox, bool FirstIndex, bool LastIndex, int IndexNumber)
        {
            try
            {
                if (FirstIndex)
                {
                    FocusListBox.SelectedIndex = 0;
                }
                else if (LastIndex)
                {
                    FocusListBox.SelectedIndex = FocusListBox.Items.Count - 1;
                }
                else if (IndexNumber != -1)
                {
                    if (IndexNumber >= FocusListBox.Items.Count)
                    {
                        FocusListBox.SelectedIndex = FocusListBox.Items.Count - 1;
                    }
                    else
                    {
                        FocusListBox.SelectedIndex = IndexNumber;
                    }
                }

                //Check the list index
                if (FocusListBox.SelectedIndex == -1)
                {
                    FocusListBox.SelectedIndex = 0;
                }
            }
            catch
            {
                Debug.WriteLine("Failed selecting the listbox index.");
            }
        }

        //Remove all matching items from a listbox
        async Task ListBoxRemoveAll<T>(ListBox ListBox, Collection<T> Collection, Func<T, bool> RemoveCondition)
        {
            try
            {
                int ListCount = ListBox.Items.Count;
                int ListSelectedIndex = ListBox.SelectedIndex;
                Collection.RemoveAll(RemoveCondition);
                if (ListCount != ListBox.Items.Count)
                {
                    if (Keyboard.FocusedElement == ListBox)
                    {
                        Debug.WriteLine(ListBox.Name + " " + (ListCount - ListBox.Items.Count) + " items have been removed, selecting the listbox.");
                        await FocusOnListbox(ListBox, false, false, ListSelectedIndex);
                    }
                    else
                    {
                        Debug.WriteLine(ListBox.Name + " " + (ListCount - ListBox.Items.Count) + " items have been removed, selecting the index.");
                        ListBoxSelectIndex(ListBox, false, false, ListSelectedIndex);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed removing all from listbox.");
            }
        }

        //Show the mouse cursor
        void MouseCursorShow()
        {
            try
            {
                //Update the last mouse interaction time
                vMouseLastInteraction = Environment.TickCount;

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
                vMouseLastInteraction = Environment.TickCount;

                //Check if the mouse hide setting is enabled
                if (ConfigurationManager.AppSettings["HideMouseCursor"] == "False")
                {
                    return;
                }

                //Check if the application is active and any controller is connected
                if (vAppActivated && vControllerAnyConnected() && vProcessKeyboardController == null)
                {
                    //Move the mouse cursor
                    Point LocationFromScreen = new Point();
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        LocationFromScreen = this.PointToScreen(new Point(255, 44));
                    });

                    int TargetX = Convert.ToInt32(LocationFromScreen.X);
                    int TargetY = Convert.ToInt32(LocationFromScreen.Y);
                    SetCursorPos(TargetX, TargetY);
                    await Task.Delay(10);

                    //Hide the mouse cursor
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        Mouse.SetCursor(Cursors.None);
                    });

                    //Debug.WriteLine("Hiding the mouse cursor.");
                }
            }
            catch { }
        }

        //Check if the mouse cursor has moved
        async Task MouseCursorCheckMovement()
        {
            try
            {
                //Get the current mouse position
                GetCursorPos(out PointWin MouseCurrentPosition);

                //Check if the mouse has moved since the last time
                bool LastInteraction = Environment.TickCount - vMouseLastInteraction > 5000;
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
                if (vAppMinimized || !vAppActivated)
                {
                    Debug.WriteLine("Showing the CtrlUI window.");
                    PlayInterfaceSound("PopupOpen", false);

                    //Get previous focused application
                    ProcessFocus ForegroundProcess = GetFocusedProcess();
                    if (ForegroundProcess != null)
                    {
                        try
                        {
                            //Check if application title is blacklisted
                            bool titleBlacklisted = vAppsBlacklistProcess.Any(x => x.ToLower() == ForegroundProcess.Title.ToLower());

                            //Check if application process is blacklisted
                            bool processBlacklisted = vAppsBlacklistProcess.Any(x => x.ToLower() == ForegroundProcess.Process.ProcessName.ToLower());

                            //Save previous focused application
                            if (!titleBlacklisted && !processBlacklisted)
                            {
                                vPrevFocusedProcess = ForegroundProcess;
                            }

                            //Disable top most window from the process
                            Debug.WriteLine("Disabling top most from process: " + ForegroundProcess.Process.ProcessName);
                            SetWindowPos(ForegroundProcess.WindowHandle, (IntPtr)WindowPosition.NoTopMost, 0, 0, 0, 0, (int)WindowSWP.NOMOVE | (int)WindowSWP.NOSIZE);
                        }
                        catch { }
                    }

                    //Recover the CtrlUI window state (Border workaround)
                    if (vAppMinimized)
                    {
                        if (vAppPrevWindowState == WindowState.Maximized)
                        {
                            await AppSwitchScreenMode(true, false);
                        }
                        else
                        {
                            await AppSwitchScreenMode(false, true);
                        }
                    }

                    //Force focus on CtrlUI
                    FocusProcessWindowPrepare("CtrlUI", vProcessCurrent.Id, vProcessCurrent.MainWindowHandle, 0, false, true, false);
                }
                else
                {
                    Debug.WriteLine("Hiding the CtrlUI window.");
                    PlayInterfaceSound("PopupClose", false);

                    //Check if a previous process is available
                    if (vPrevFocusedProcess == null)
                    {
                        Popup_Show_Status("Close", "No app to show found");
                        Debug.WriteLine("Previous process not found.");
                        return;
                    }

                    //Check if application title is blacklisted
                    if (vAppsBlacklistProcess.Any(x => x.ToLower() == vPrevFocusedProcess.Title.ToLower()))
                    {
                        Popup_Show_Status("Close", "App is blacklisted");
                        Debug.WriteLine("Previous process title is blacklisted.");
                        return;
                    }

                    //Check if application process is blacklisted
                    if (vAppsBlacklistProcess.Any(x => x.ToLower() == vPrevFocusedProcess.Process.ProcessName.ToLower()))
                    {
                        Popup_Show_Status("Close", "App is blacklisted");
                        Debug.WriteLine("Previous process name is blacklisted.");
                        return;
                    }

                    //Check if application process is still running
                    if (!CheckRunningProcessByNameOrTitle(vPrevFocusedProcess.Process.ProcessName, false))
                    {
                        Popup_Show_Status("Close", "App no longer running");
                        Debug.WriteLine("Previous process is no longer running.");
                        return;
                    }

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Switch.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Return to application";
                    Answers.Add(Answer1);

                    DataBindString Answer2 = new DataBindString();
                    Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                    Answer2.Name = "Close the application";
                    Answers.Add(Answer2);

                    DataBindString Answer3 = new DataBindString();
                    Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Minimize.png" }, IntPtr.Zero, -1);
                    Answer3.Name = "Minimize CtrlUI";
                    Answers.Add(Answer3);

                    DataBindString cancelString = new DataBindString();
                    cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                    cancelString.Name = "Cancel";
                    Answers.Add(cancelString);

                    DataBindString Result = await Popup_Show_MessageBox("Return to previous application or minimize?", "", "You can always return to " + vPrevFocusedProcess.Title + " later on.", Answers);
                    if (Result != null)
                    {
                        if (Result == Answer1)
                        {
                            //Minimize the CtrlUI window
                            if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                            //Force focus on the app
                            FocusProcessWindowPrepare(vPrevFocusedProcess.Title, vPrevFocusedProcess.Process.Id, vPrevFocusedProcess.WindowHandle, 0, false, false, false);
                        }
                        else if (Result == Answer2)
                        {
                            Popup_Show_Status("Closing", "Closing " + vPrevFocusedProcess.Title);
                            Debug.WriteLine("Closing process: " + vPrevFocusedProcess.Title + " / " + vPrevFocusedProcess.Process.Id + " / " + vPrevFocusedProcess.WindowHandle);

                            //Check if the application is UWP or Win32
                            if (CheckProcessIsUwp(vPrevFocusedProcess.WindowHandle))
                            {
                                bool ClosedProcess = await CloseProcessUwpByWindowHandleOrProcessId(vPrevFocusedProcess.Title, vPrevFocusedProcess.Process.Id, vPrevFocusedProcess.WindowHandle);
                                if (ClosedProcess)
                                {
                                    vPrevFocusedProcess = null;
                                }
                            }
                            else
                            {
                                bool ClosedProcess = CloseProcessById(vPrevFocusedProcess.Process.Id);
                                if (ClosedProcess)
                                {
                                    vPrevFocusedProcess = null;
                                }
                            }
                        }
                        else if (Result == Answer3)
                        {
                            //Minimize the CtrlUI window
                            await AppMinimize(false);
                        }
                    }
                }
            }
            catch
            {
                Popup_Show_Status("Close", "Failed to minimize or show app");
                Debug.WriteLine("Failed to minimize or show application.");
            }
        }

        //Minimize the application and save previous state
        async Task AppMinimize(bool Delay)
        {
            try
            {
                Debug.WriteLine("Minimizing the CtrlUI window.");

                //Save the CtrlUI window state
                vAppPrevWindowState = WindowState;
                vAppActivated = false;
                vAppMinimized = true;

                //Disable the CtrlUI window
                grid_WindowActive.Opacity = 0.80;
                grid_App.IsHitTestVisible = false;

                //Minimize the CtrlUI application
                WindowState = WindowState.Minimized;

                //Wait for application to minimize
                if (Delay)
                {
                    await Task.Delay(1000);
                }
            }
            catch { }
        }

        //Switch application between fullscreen and windowed
        async Task AppSwitchScreenMode(bool ForceMaximized, bool ForceNormal)
        {
            try
            {
                if (!ForceNormal && (ForceMaximized || WindowState != WindowState.Maximized))
                {
                    Debug.WriteLine("Maximizing CtrlUI window.");

                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;

                    ////Hide the Windows taskbar
                    //IntPtr hWnd = FindWindow("Shell_TrayWnd", string.Empty);
                    //ShowWindow(hWnd, (int)WindowShowCmd.Hide);

                    vAppMaximized = true;
                }
                else
                {
                    Debug.WriteLine("Restoring CtrlUI window.");

                    WindowStyle = WindowStyle.SingleBorderWindow;
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

        //Update the application background image
        void UpdateBackgroundImage()
        {
            try
            {
                string cacheWorkaround = new string(' ', new Random().Next(1, 20));
                string defaultWallpaper = "Assets\\Background.png" + cacheWorkaround;
                if (ConfigurationManager.AppSettings["DesktopBackground"] == "False")
                {
                    grid_Main_img_Background.ImageSource = FileToBitmapImage(new string[] { defaultWallpaper }, IntPtr.Zero, -1);
                }
                else
                {
                    string desktopWallpaper = Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WallPaper", defaultWallpaper).ToString();
                    if (File.Exists(desktopWallpaper))
                    {
                        grid_Main_img_Background.ImageSource = FileToBitmapImage(new string[] { desktopWallpaper }, IntPtr.Zero, -1);
                    }
                    else
                    {
                        grid_Main_img_Background.ImageSource = FileToBitmapImage(new string[] { defaultWallpaper }, IntPtr.Zero, -1);
                    }
                }
            }
            catch { }
        }

        //Adjust the application font size
        void AdjustApplicationFontSize()
        {
            try
            {
                int FontSize = Convert.ToInt32(ConfigurationManager.AppSettings["AppFontSize"]);
                Debug.WriteLine("Adjusting the font size to: " + FontSize);

                double DefaultTextSizeTiny = 10;
                double TextSizeInterface = 16;
                double TextSizeSmall = 18;
                double TextSizeMedium = 20;
                double TextSizeLarge = 24;
                double TextSizeHuge = 28;
                double TextSizeTitle = 80;

                Application.Current.Resources["TextSizeTiny"] = DefaultTextSizeTiny + FontSize;
                Application.Current.Resources["TextSizeInterface"] = TextSizeInterface + FontSize;
                Application.Current.Resources["TextSizeSmall"] = TextSizeSmall + FontSize;
                Application.Current.Resources["TextSizeMedium"] = TextSizeMedium + FontSize;
                Application.Current.Resources["TextSizeLarge"] = TextSizeLarge + FontSize;
                Application.Current.Resources["TextSizeHuge"] = TextSizeHuge + FontSize;
                Application.Current.Resources["TextSizeTitle"] = TextSizeTitle + FontSize;
            }
            catch { }
        }

        //Set content and resource images with Cache OnLoad
        void SetContentResourceXamlImages()
        {
            try
            {
                img_Menu_SteamStatus.Source = FileToBitmapImage(new string[] { "Steam" }, IntPtr.Zero, 30);
                img_Menu_UplayStatus.Source = FileToBitmapImage(new string[] { "Uplay" }, IntPtr.Zero, 30);
                img_Menu_OriginStatus.Source = FileToBitmapImage(new string[] { "Origin" }, IntPtr.Zero, 30);
                img_Menu_GoGStatus.Source = FileToBitmapImage(new string[] { "GoG" }, IntPtr.Zero, 30);
                img_Menu_DirectXInput.Source = FileToBitmapImage(new string[] { "DirectXInput" }, IntPtr.Zero, 30);
                img_Menu_BethesdaStatus.Source = FileToBitmapImage(new string[] { "Bethesda" }, IntPtr.Zero, 30);
                img_Menu_EpicStatus.Source = FileToBitmapImage(new string[] { "Epic" }, IntPtr.Zero, 30);
                img_Menu_BlizzardStatus.Source = FileToBitmapImage(new string[] { "Battle.net" }, IntPtr.Zero, 30);

                //Check if the first launch logo's need to be loaded
                if (ConfigurationManager.AppSettings["AppFirstLaunch"] == "True")
                {
                    grid_Popup_Welcome_img_Kodi.Source = FileToBitmapImage(new string[] { "Kodi" }, IntPtr.Zero, 75);
                    grid_Popup_Welcome_img_Spotify.Source = FileToBitmapImage(new string[] { "Spotify" }, IntPtr.Zero, 75);
                    grid_Popup_Welcome_img_Steam.Source = FileToBitmapImage(new string[] { "Steam" }, IntPtr.Zero, 75);
                    grid_Popup_Welcome_img_Origin.Source = FileToBitmapImage(new string[] { "Origin" }, IntPtr.Zero, 75);
                    grid_Popup_Welcome_img_Uplay.Source = FileToBitmapImage(new string[] { "Uplay" }, IntPtr.Zero, 75);
                    grid_Popup_Welcome_img_GoG.Source = FileToBitmapImage(new string[] { "GoG" }, IntPtr.Zero, 75);
                    grid_Popup_Welcome_img_Battle.Source = FileToBitmapImage(new string[] { "Battle.net" }, IntPtr.Zero, 75);
                    grid_Popup_Welcome_img_PS4Remote.Source = FileToBitmapImage(new string[] { "Remote Play" }, IntPtr.Zero, 75);
                }
            }
            catch { }
        }
    }
}