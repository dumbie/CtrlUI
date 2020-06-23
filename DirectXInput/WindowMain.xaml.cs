using ArnoldVinkCode;
using AVForms;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static ArnoldVinkCode.ProcessFunctions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
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
                Settings_Load_CtrlUI(ref vConfigurationCtrlUI);
                Settings_Load_AccentColor(vConfigurationCtrlUI);
                Settings_Load_FpsOverlayer(ref vConfigurationFpsOverlayer);
                Settings_Load();
                Settings_Save();

                //Create the tray menu
                Application_CreateTrayMenu();

                //Check if application has launched as admin
                if (vAdministratorPermission)
                {
                    this.Title += " (Admin)";
                }

                //Check settings if window needs to be shown
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["AppFirstLaunch"]))
                {
                    Debug.WriteLine("First launch showing the window.");
                    await Application_ShowHideWindow();
                }

                //Check xbox bus driver status
                if (!await CheckXboxBusDriverStatus())
                {
                    if (!ShowInTaskbar) { await Application_ShowHideWindow(); }
                    await Message_InstallDrivers();
                    return;
                }

                //Load the help text
                LoadHelp();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Load Json profiles
                JsonLoadProfile(ref vDirectCloseTools, "DirectCloseTools");

                //Close running controller tools
                CloseControllerTools();

                //Load controllers supported
                JsonLoadProfile(ref vDirectControllersSupported, "DirectControllersSupported");

                //Load controllers ignored
                JsonLoadProfile(ref vDirectControllersIgnored, "DirectControllersIgnored");

                //Load keypad mapping
                JsonLoadProfile(ref vDirectKeypadMapping, "DirectKeypadMapping");
                Load_Keypad_Profile();

                //Load controllers profile
                JsonLoadList_ControllerProfile();

                //Reset HidGuardian to defaults
                HidGuardianResetDefaults();

                //Allow DirectXInput process in HidGuardian
                HidGuardianAllowProcess();

                //Start the background tasks
                TasksBackgroundStart();

                //Set application first launch to false
                SettingSave(vConfigurationApplication, "AppFirstLaunch", "False");

                //Enable the socket server
                EnableSocketServer();
            }
            catch { }
        }

        //Enable the socket server
        private void EnableSocketServer()
        {
            try
            {
                int SocketServerPort = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["ServerPort"].Value) + 1;

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort);
                vArnoldVinkSockets.vTcpClientTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
            }
            catch { }
        }

        //Test the rumble button
        async void Btn_TestRumble_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = GetActiveController();
                if (activeController != null && !vControllerRumbleTest)
                {
                    vControllerRumbleTest = true;
                    Button SendButton = sender as Button;

                    if (SendButton.Name == "btn_RumbleTestLight") { SendXRumbleData(activeController, true, true, false); } else { SendXRumbleData(activeController, true, false, true); }
                    await Task.Delay(1000);
                    SendXRumbleData(activeController, true, false, false);

                    vControllerRumbleTest = false;
                }
            }
            catch { }
        }

        //Disconnect and stop the controller
        async void Btn_DisconnectController_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = GetActiveController();
                if (activeController != null)
                {
                    await StopControllerAsync(activeController, false, string.Empty);
                }
            }
            catch { }
        }

        //Disconnect and stop all controllers
        async void Btn_DisconnectControllerAll_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                await StopAllControllers();
            }
            catch { }
        }

        //Remove the controller from the list
        async void Btn_RemoveController_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = GetActiveController();
                if (activeController != null)
                {
                    int messageResult = await AVMessageBox.MessageBoxPopup(this, "Do you really want to remove this controller?", "This will reset the active controller to it's defaults and disconnect it.", "Remove controller", "Cancel", "", "");
                    if (messageResult == 1)
                    {
                        Debug.WriteLine("Removed the controller: " + activeController.Details.DisplayName);

                        NotificationDetails notificationDetails = new NotificationDetails();
                        notificationDetails.Icon = "Controller";
                        notificationDetails.Text = "Removed controller";
                        App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "Removed the controller: " + activeController.Details.DisplayName;
                        });

                        vDirectControllersProfile.Remove(activeController.Details.Profile);
                        await StopControllerAsync(activeController, false, "removed");

                        //Save changes to Json file
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
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
                await StopAllControllers();

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