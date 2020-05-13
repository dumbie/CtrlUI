using ArnoldVinkCode;
using AVForms;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                    Application_ShowHideWindow();
                }

                //Check xbox bus driver status
                if (!await CheckXboxBusDriverStatus())
                {
                    if (!ShowInTaskbar) { Application_ShowHideWindow(); }
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

                //Load controllers profile
                JsonLoadList_ControllerProfile();

                //Reset HidGuardian to defaults
                HidGuardianResetDefaults();

                //Allow DirectXInput process in HidGuardian
                HidGuardianAllowProcess();

                //Start application tasks
                TasksBackgroundStart();

                //Set application first launch to false
                SettingSave("AppFirstLaunch", "False");

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

        //Unmap button
        void Btn_MapController_MouseRight(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus manageController = GetManageController();
                if (manageController != null)
                {
                    Button sendButton = sender as Button;
                    string mapButton = sendButton.Tag.ToString();

                    Debug.WriteLine("Unmapped button: " + mapButton);
                    txt_Application_Status.Text = "Unmapped '" + mapButton + "' from the controller profile.";

                    //Store new button mapping in Json controller
                    if (mapButton == "Button A") { manageController.Details.Profile.ButtonA = -1; }
                    if (mapButton == "Button B") { manageController.Details.Profile.ButtonB = -1; }
                    if (mapButton == "Button X") { manageController.Details.Profile.ButtonX = -1; }
                    if (mapButton == "Button Y") { manageController.Details.Profile.ButtonY = -1; }
                    if (mapButton == "Button LB") { manageController.Details.Profile.ButtonShoulderLeft = -1; }
                    if (mapButton == "Button RB") { manageController.Details.Profile.ButtonShoulderRight = -1; }
                    if (mapButton == "Button Back") { manageController.Details.Profile.ButtonBack = -1; }
                    if (mapButton == "Button Start") { manageController.Details.Profile.ButtonStart = -1; }
                    if (mapButton == "Button Guide") { manageController.Details.Profile.ButtonGuide = -1; }
                    if (mapButton == "Button Thumb Left") { manageController.Details.Profile.ButtonThumbLeft = -1; }
                    if (mapButton == "Button Thumb Right") { manageController.Details.Profile.ButtonThumbRight = -1; }
                    if (mapButton == "Button Trigger Left") { manageController.Details.Profile.ButtonTriggerLeft = -1; }
                    if (mapButton == "Button Trigger Right") { manageController.Details.Profile.ButtonTriggerRight = -1; }

                    //Save changes to Json file
                    JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                }
            }
            catch { }
        }

        //Map button
        async void Btn_MapController_MouseLeft(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus manageController = GetManageController();
                if (manageController != null)
                {
                    Button sendButton = sender as Button;
                    string mapButton = sendButton.Tag.ToString();

                    //Set button to map
                    manageController.Mapping[0] = "Map";
                    manageController.Mapping[1] = mapButton;

                    //Disable interface
                    txt_Application_Status.Text = "Waiting for '" + mapButton + "' press on the controller... ";
                    pb_UpdateProgress.IsIndeterminate = true;
                    grid_ControllerPreview.IsEnabled = false;
                    grid_ControllerPreview.Opacity = 0.50;

                    //Start mapping timer
                    int countdownTimeout = 0;
                    AVFunctions.TimerRenew(ref vDispatcherTimerMapping);
                    vDispatcherTimerMapping.Interval = TimeSpan.FromSeconds(1);
                    vDispatcherTimerMapping.Tick += delegate
                    {
                        try
                        {
                            if (countdownTimeout++ >= 10)
                            {
                                //Reset controller button mapping
                                manageController.Mapping[0] = "Cancel";
                                manageController.Mapping[1] = "None";
                            }
                            else
                            {
                                txt_Application_Status.Text = "Waiting for '" + mapButton + "' press on the controller... " + (11 - countdownTimeout).ToString() + "sec.";
                            }
                        }
                        catch { }
                    };
                    vDispatcherTimerMapping.Start();

                    //Check if button is mapped
                    while (manageController.Mapping[0] == "Map") { await Task.Delay(500); }
                    vDispatcherTimerMapping.Stop();

                    if (manageController.Mapping[0] == "Done")
                    {
                        txt_Application_Status.Text = "Changed '" + mapButton + "' to the pressed controller button.";
                    }
                    else
                    {
                        Debug.WriteLine("Cancelled button mapping.");
                        txt_Application_Status.Text = "Cancelled button mapping, please select a button to change.";
                    }

                    //Enable interface
                    pb_UpdateProgress.IsIndeterminate = false;
                    grid_ControllerPreview.IsEnabled = true;
                    grid_ControllerPreview.Opacity = 1.00;
                }
            }
            catch { }
        }

        //Test the rumble button
        async void Btn_TestRumble_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus ManageController = GetManageController();
                if (ManageController != null && !vControllerRumbleTest)
                {
                    vControllerRumbleTest = true;
                    Button SendButton = sender as Button;

                    if (SendButton.Name == "btn_RumbleTestLight") { SendXRumbleData(ManageController, true, true, false); } else { SendXRumbleData(ManageController, true, false, true); }
                    await Task.Delay(1000);
                    SendXRumbleData(ManageController, true, false, false);

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
                ControllerStatus ManageController = GetManageController();
                if (ManageController != null)
                {
                    await StopControllerAsync(ManageController, false);
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
                ControllerStatus ManageController = GetManageController();
                if (ManageController != null)
                {
                    int messageResult = await AVMessageBox.MessageBoxPopup(this, "Do you really want to remove this controller?", "This will reset the manage controller to it's defaults and disconnect it.", "Remove controller", "Cancel", "", "");
                    if (messageResult == 1)
                    {
                        Debug.WriteLine("Removed the controller: " + ManageController.Details.DisplayName);

                        NotificationDetails notificationDetails = new NotificationDetails();
                        notificationDetails.Icon = "Controller";
                        notificationDetails.Text = "Removed controller";
                        App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "Removed the controller: " + ManageController.Details.DisplayName;
                        });

                        vDirectControllersProfile.Remove(ManageController.Details.Profile);
                        await StopControllerAsync(ManageController, false);

                        //Save changes to Json file
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                }
            }
            catch { }
        }

        //Monitor window state changes
        void CheckWindowStateAndSize(object sender, EventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Minimized) { Application_ShowHideWindow(); }
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
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.Opacity = 0.80;
                    this.IsEnabled = false;
                });

                //Close the keyboard controller
                CloseProcessesByNameOrTitle("KeyboardController", false);

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