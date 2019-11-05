using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Register Interface Handlers
        void RegisterInterfaceHandlers()
        {
            try
            {
                //Monitor window state changes
                SizeChanged += CheckWindowStateAndSize;
                StateChanged += CheckWindowStateAndSize;

                //Main menu functions
                lb_Menu.PreviewKeyUp += lb_Menu_KeyPressUp;
                lb_Menu.PreviewMouseUp += lb_Menu_MousePressUp;

                //MessageBox functions
                grid_MessageBox_Btn1.Click += grid_MessageBox_Btn1_Click;
                grid_MessageBox_Btn2.Click += grid_MessageBox_Btn2_Click;
                grid_MessageBox_Btn3.Click += grid_MessageBox_Btn3_Click;
                grid_MessageBox_Btn4.Click += grid_MessageBox_Btn4_Click;

                //Controller functions
                button_Controller0.Click += Button_Controller0_Click;
                button_Controller1.Click += Button_Controller1_Click;
                button_Controller2.Click += Button_Controller2_Click;
                button_Controller3.Click += Button_Controller3_Click;
                btn_SearchNewControllers.Click += Btn_SearchNewControllers_Click;
                btn_DisconnectController.Click += Btn_DisconnectController_Click;
                btn_RemoveController.Click += Btn_RemoveController_Click;
                btn_RumbleTestLight.Click += Btn_TestRumble_Click;
                btn_RumbleTestHeavy.Click += Btn_TestRumble_Click;
                btn_DebugInformation.Click += Btn_CopyControllerDebugInfo_Click;

                //Save controller settings
                cb_ControllerFakeGuideButton.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Connected.Profile.FakeGuideButton = cb_ControllerFakeGuideButton.IsChecked.Value;
                        JsonSaveControllerProfile();
                    }
                };

                cb_ControllerUseButtonTriggers.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Connected.Profile.UseButtonTriggers = cb_ControllerUseButtonTriggers.IsChecked.Value;
                        JsonSaveControllerProfile();
                    }
                };

                cb_ControllerDPadFourWayMovement.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Connected.Profile.DPadFourWayMovement = cb_ControllerDPadFourWayMovement.IsChecked.Value;
                        JsonSaveControllerProfile();
                    }
                };

                cb_ControllerThumbFlipMovement.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Connected.Profile.ThumbFlipMovement = cb_ControllerThumbFlipMovement.IsChecked.Value;
                        JsonSaveControllerProfile();
                    }
                };

                cb_ControllerThumbFlipAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Connected.Profile.ThumbFlipAxesLeft = cb_ControllerThumbFlipAxesLeft.IsChecked.Value;
                        JsonSaveControllerProfile();
                    }
                };

                cb_ControllerThumbFlipAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Connected.Profile.ThumbFlipAxesRight = cb_ControllerThumbFlipAxesRight.IsChecked.Value;
                        JsonSaveControllerProfile();
                    }
                };

                cb_ControllerThumbReverseAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Connected.Profile.ThumbReverseAxesLeft = cb_ControllerThumbReverseAxesLeft.IsChecked.Value;
                        JsonSaveControllerProfile();
                    }
                };

                cb_ControllerThumbReverseAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Connected.Profile.ThumbReverseAxesRight = cb_ControllerThumbReverseAxesRight.IsChecked.Value;
                        JsonSaveControllerProfile();
                    }
                };

                slider_ControllerRumbleStrength.ValueChanged += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        textblock_ControllerRumbleStrength.Text = "Rumble strength: " + slider_ControllerRumbleStrength.Value.ToString("0") + "%";
                        ManageController.Connected.Profile.RumbleStrength = Convert.ToInt32(slider_ControllerRumbleStrength.Value);
                        JsonSaveControllerProfile();
                    }
                };

                slider_ControllerLedBrightness.ValueChanged += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        textblock_ControllerLedBrightness.Text = "Led brightness: " + slider_ControllerLedBrightness.Value.ToString("0") + "%";
                        ManageController.Connected.Profile.LedBrightness = Convert.ToInt32(slider_ControllerLedBrightness.Value);
                        JsonSaveControllerProfile();

                        //Update the controller led
                        SendXRumbleData(ManageController, true, false, false);
                    }
                };

                //Buttons functions
                btn_SetA.Click += Btn_MapController_Click;
                btn_SetB.Click += Btn_MapController_Click;
                btn_SetX.Click += Btn_MapController_Click;
                btn_SetY.Click += Btn_MapController_Click;
                btn_SetShoulderLeft.Click += Btn_MapController_Click;
                btn_SetShoulderRight.Click += Btn_MapController_Click;
                btn_SetThumbLeft.Click += Btn_MapController_Click;
                btn_SetThumbRight.Click += Btn_MapController_Click;
                btn_SetBack.Click += Btn_MapController_Click;
                btn_SetGuide.Click += Btn_MapController_Click;
                btn_SetStart.Click += Btn_MapController_Click;
                btn_SetTriggerLeft.Click += Btn_MapController_Click;
                btn_SetTriggerRight.Click += Btn_MapController_Click;

                //Settings functions
                btn_CheckControllers.Click += btn_CheckControllers_Click;
                btn_CheckDeviceManager.Click += btn_CheckDeviceManager_Click;
                btn_Settings_InstallDrivers.Click += btn_Settings_InstallDrivers_Click;

                //Help functions
                btn_Help_ProjectWebsite.Click += btn_Help_ProjectWebsite_Click;
                btn_Help_OpenDonation.Click += btn_Help_OpenDonation_Click;
            }
            catch { }
        }

        //Update the current window status
        void UpdateWindowStatus()
        {
            try
            {
                vProcessCtrlUI = GetProcessByName("CtrlUI", false);
                vProcessKeyboardController = GetProcessByName("KeyboardController", false);

                int FocusedAppId = GetFocusedProcess().Process.Id;
                if (vProcessCtrlUI != null && vProcessCtrlUI.Id == FocusedAppId) { vProcessCtrlUIActivated = true; } else { vProcessCtrlUIActivated = false; }

                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        if (WindowState == WindowState.Maximized) { vAppMaximized = true; } else { vAppMaximized = false; }
                        if (WindowState == WindowState.Minimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vCurrentProcessId == FocusedAppId)
                        {
                            vAppActivated = true;
                            grid_WindowActive.Opacity = 0;
                            grid_App.IsHitTestVisible = true;
                        }
                        else
                        {
                            vAppActivated = false;
                            grid_WindowActive.Opacity = 0.80;
                            grid_App.IsHitTestVisible = false;
                        }
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Install the required drivers message popup
        async Task Message_InstallDrivers()
        {
            try
            {
                int Result = await MessageBoxPopup("Welcome to DirectXInput", "It seems like you have not yet installed the required drivers to use this application, please make sure that you have installed the required drivers.\n\nDirectXInput will be closed during the installation of the required drivers.\n\nIf you just installed the drivers and this message shows up restart your PC.", "Install the drivers", "Close application", "", "");
                if (Result == 1)
                {
                    if (!CheckRunningProcessByName("DriverInstaller", false))
                    {
                        ProcessLauncherWin32("DriverInstaller.exe", "", "", false, false);
                        await Application_Exit(true);
                    }
                }
                else
                {
                    await Application_Exit(true);
                }
            }
            catch { }
        }

        //Display a certain grid page
        void ShowGridPage(FrameworkElement UIElement)
        {
            try
            {
                grid_Controller.Visibility = Visibility.Collapsed;
                grid_Buttons.Visibility = Visibility.Collapsed;
                grid_Settings.Visibility = Visibility.Collapsed;
                grid_Help.Visibility = Visibility.Collapsed;
                UIElement.Visibility = Visibility.Visible;
            }
            catch { }
        }
    }
}