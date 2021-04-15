using ArnoldVinkCode;
using AVForms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static ArnoldVinkCode.ProcessFunctions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Variables
        public static IntPtr vInteropWindowHandle = IntPtr.Zero;

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Register Hotkeys and Filtermessage
                ComponentDispatcher.ThreadFilterMessage += ReceivedFilterMessage;
            }
            catch { }
        }

        //Run application startup code
        public async Task Startup()
        {
            try
            {
                //Initialize Settings
                Settings_Check();
                Settings_Load();
                Settings_Save();

                //Change application accent color
                Settings_Load_AccentColor(vConfigurationCtrlUI);

                //Create the tray menu
                Application_CreateTrayMenu();

                //Check if application has launched as admin
                if (vAdministratorPermission)
                {
                    this.Title += " (Admin)";
                }

                //Check settings if window needs to be shown
                if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "AppFirstLaunch")))
                {
                    Debug.WriteLine("First launch showing the window.");
                    await Application_ShowHideWindow();
                }

                //Check if drivers are installed
                if (!CheckInstalledDrivers())
                {
                    if (!ShowInTaskbar) { await Application_ShowHideWindow(); }
                    await Message_InstallDrivers();
                    return;
                }

                //Open the virtual bus driver
                if (!await OpenVirtualBusDriver())
                {
                    if (!ShowInTaskbar) { await Application_ShowHideWindow(); }
                    await Message_InstallDrivers();
                    return;
                }

                //Open the hid hide device
                if (!OpenHidHideDevice())
                {
                    if (!ShowInTaskbar) { await Application_ShowHideWindow(); }
                    await Message_InstallDrivers();
                    return;
                }

                //Load the help text
                LoadHelp();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Load application close tools
                JsonLoadProfile(ref vDirectCloseTools, "DirectCloseTools");

                //Close running controller tools
                CloseControllerTools();

                //Load keyboard emoji and text list
                JsonLoadProfile(ref vDirectKeyboardEmojiListActivity, "DirectKeyboardEmojiListActivity");
                JsonLoadProfile(ref vDirectKeyboardEmojiListNature, "DirectKeyboardEmojiListNature");
                JsonLoadProfile(ref vDirectKeyboardEmojiListFood, "DirectKeyboardEmojiListFood");
                JsonLoadProfile(ref vDirectKeyboardEmojiListOther, "DirectKeyboardEmojiListOther");
                JsonLoadProfile(ref vDirectKeyboardEmojiListPeople, "DirectKeyboardEmojiListPeople");
                JsonLoadProfile(ref vDirectKeyboardEmojiListSmiley, "DirectKeyboardEmojiListSmiley");
                JsonLoadProfile(ref vDirectKeyboardEmojiListSymbol, "DirectKeyboardEmojiListSymbol");
                JsonLoadProfile(ref vDirectKeyboardEmojiListTravel, "DirectKeyboardEmojiListTravel");
                JsonLoadProfile(ref vDirectKeyboardTextList, "DirectKeyboardTextList");

                //Load controllers supported
                JsonLoadProfile(ref vDirectControllersSupported, "DirectControllersSupported");

                //Load controllers ignored
                JsonLoadProfile(ref vDirectControllersIgnoredUser, "DirectControllersIgnoredUser");
                JsonLoadProfile(ref vDirectControllersIgnoredDefault, "DirectControllersIgnoredDefault");

                //Load keypad mapping
                JsonLoadProfile(ref vDirectKeypadMapping, "DirectKeypadMapping");
                JsonLoadList_KeypadProfile();

                //Load controllers profile
                JsonLoadList_ControllerProfile();

                //Bind all the lists to ListBox
                ListBoxBindLists();

                //Reset HidHide to defaults
                vHidHideDevice.ListDeviceReset();
                vHidHideDevice.ListApplicationReset();

                //Allow DirectXInput in HidHide
                string appFilePath = Assembly.GetEntryAssembly().Location;
                vHidHideDevice.ListApplicationAdd(appFilePath);

                //Enable HidHide device
                vHidHideDevice.DeviceHideToggle(true);

                //Start the background tasks
                TasksBackgroundStart();

                //Set application first launch to false
                Setting_Save(vConfigurationDirectXInput, "AppFirstLaunch", "False");

                //Enable the socket server
                EnableSocketServer();
            }
            catch { }
        }

        //Bind the lists to the listbox elements
        void ListBoxBindLists()
        {
            try
            {
                combobox_KeyboardTextString.ItemsSource = vDirectKeyboardTextList;
                combobox_KeyboardTextString.DisplayMemberPath = "String1";
                combobox_KeyboardTextString.SelectedIndex = 0;

                combobox_KeypadProcessProfile.ItemsSource = vDirectKeypadMapping;
                combobox_KeypadProcessProfile.DisplayMemberPath = "Name";
                combobox_KeypadProcessProfile.SelectedIndex = 0;

                ListboxLoadIgnoredController();
            }
            catch { }
        }

        //Enable the socket server
        private void EnableSocketServer()
        {
            try
            {
                int SocketServerPort = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "ServerPort")) + 1;

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort, true, true);
                vArnoldVinkSockets.vSocketTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
            }
            catch { }
        }

        //Test the rumble button
        async void Btn_TestRumble_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    if (!vControllerRumbleTest)
                    {
                        vControllerRumbleTest = true;
                        Button SendButton = sender as Button;

                        UpdateControllerPreviewRumble(true);
                        if (SendButton.Name == "btn_RumbleTestLight")
                        {
                            ControllerOutput(activeController, true, false);
                        }
                        else
                        {
                            ControllerOutput(activeController, false, true);
                        }
                        await Task.Delay(1000);
                        UpdateControllerPreviewRumble(false);
                        ControllerOutput(activeController, false, false);

                        vControllerRumbleTest = false;
                    }
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Close other running controller tools
        void CloseControllerTools()
        {
            try
            {
                Debug.WriteLine("Closing other running controller tools.");
                foreach (ProfileShared closeTool in vDirectCloseTools)
                {
                    try
                    {
                        CloseProcessesByNameOrTitle(closeTool.String1, false);
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Make sure the correct window style is set
        async void CheckWindowStateAndStyle(object sender, EventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Minimized) { await Application_ShowHideWindow(); }
            }
            catch { }
        }

        //Application Close Handler
        protected async override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                await Application_Exit_Prompt();
            }
            catch { }
        }

        //Application close prompt
        public async Task Application_Exit_Prompt()
        {
            try
            {
                int messageResult = await AVMessageBox.MessageBoxPopup(this, "Do you really want to close DirectXInput?", "This will disconnect all your currently connected controllers.", "Close application", "Cancel", "", "");
                if (messageResult == 1)
                {
                    await Application_Exit();
                }
            }
            catch { }
        }

        //Close the application
        public async Task Application_Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Disable application window
                AppWindowDisable("Closing DirectXInput, please wait.");

                //Stop the background tasks
                await TasksBackgroundStop();

                //Disconnect all the controllers
                await StopAllControllers(true);

                //Reset HidHide to defaults
                vHidHideDevice.ListDeviceReset();
                vHidHideDevice.ListApplicationReset();

                //Disable HidHide device
                vHidHideDevice.DeviceHideToggle(false);

                //Close HidHide device
                vHidHideDevice.CloseDevice();

                //Disable the socket server
                if (vArnoldVinkSockets != null)
                {
                    await vArnoldVinkSockets.SocketServerDisable();
                }

                //Hide the visible tray icon
                TrayNotifyIcon.Visible = false;

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}