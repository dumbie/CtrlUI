using ArnoldVinkCode;
using AVForms;
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
        //Display a certain grid page
        void ShowGridPage(FrameworkElement elementTarget)
        {
            try
            {
                grid_Controller.Visibility = Visibility.Collapsed;
                grid_Buttons.Visibility = Visibility.Collapsed;
                grid_Settings.Visibility = Visibility.Collapsed;
                grid_Help.Visibility = Visibility.Collapsed;
                elementTarget.Visibility = Visibility.Visible;
            }
            catch { }
        }

        //Update element visibility
        void UpdateElementVisibility(FrameworkElement elementTarget, bool Visible)
        {
            try
            {
                if (Visible)
                {
                    AVActions.ActionDispatcherInvoke(delegate { elementTarget.Visibility = Visibility.Visible; });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate { elementTarget.Visibility = Visibility.Collapsed; });
                }
            }
            catch { }
        }

        //Update element enabled
        void UpdateElementEnabled(FrameworkElement elementTarget, bool Enabled)
        {
            try
            {
                if (Enabled)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        elementTarget.IsEnabled = true;
                        elementTarget.IsHitTestVisible = true;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        elementTarget.IsEnabled = false;
                        elementTarget.IsHitTestVisible = false;
                    });
                }
            }
            catch { }
        }

        //Update element opacity
        void UpdateElementOpacity(FrameworkElement elementTarget, double opacityTarget)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate { elementTarget.Opacity = opacityTarget; });
            }
            catch { }
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
                lb_Menu.PreviewKeyUp += lb_Menu_KeyPressUp;
                lb_Menu.PreviewMouseUp += lb_Menu_MousePressUp;

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
                        ManageController.Details.Profile.FakeGuideButton = cb_ControllerFakeGuideButton.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerUseButtonTriggers.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Details.Profile.UseButtonTriggers = cb_ControllerUseButtonTriggers.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerDPadFourWayMovement.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Details.Profile.DPadFourWayMovement = cb_ControllerDPadFourWayMovement.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbFlipMovement.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Details.Profile.ThumbFlipMovement = cb_ControllerThumbFlipMovement.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbFlipAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Details.Profile.ThumbFlipAxesLeft = cb_ControllerThumbFlipAxesLeft.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbFlipAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Details.Profile.ThumbFlipAxesRight = cb_ControllerThumbFlipAxesRight.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbReverseAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Details.Profile.ThumbReverseAxesLeft = cb_ControllerThumbReverseAxesLeft.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                cb_ControllerThumbReverseAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        ManageController.Details.Profile.ThumbReverseAxesRight = cb_ControllerThumbReverseAxesRight.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                slider_ControllerRumbleStrength.ValueChanged += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        textblock_ControllerRumbleStrength.Text = "Rumble strength: " + slider_ControllerRumbleStrength.Value.ToString("0") + "%";
                        ManageController.Details.Profile.RumbleStrength = Convert.ToInt32(slider_ControllerRumbleStrength.Value);
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                    }
                };

                slider_ControllerLedBrightness.ValueChanged += (sender, e) =>
                {
                    ControllerStatus ManageController = GetManageController();
                    if (ManageController != null)
                    {
                        textblock_ControllerLedBrightness.Text = "Led brightness: " + slider_ControllerLedBrightness.Value.ToString("0") + "%";
                        ManageController.Details.Profile.LedBrightness = Convert.ToInt32(slider_ControllerLedBrightness.Value);
                        JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");

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
                vProcessCtrlUI = GetProcessByNameOrTitle("CtrlUI", false);
                vProcessKeyboardController = GetProcessByNameOrTitle("KeyboardController", false);
                int FocusedAppId = GetFocusedProcess().Identifier;

                //Check if CtrlUI is currently activated
                if (vProcessCtrlUI != null && vProcessCtrlUI.Id == FocusedAppId) { vProcessCtrlUIActivated = true; } else { vProcessCtrlUIActivated = false; }

                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        if (WindowState == WindowState.Maximized) { vAppMaximized = true; } else { vAppMaximized = false; }
                        if (WindowState == WindowState.Minimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vProcessCurrent.Id == FocusedAppId)
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
                int messageResult = await AVMessageBox.MessageBoxPopup(this, "Drivers not installed", "It seems like you have not yet installed the required drivers to use this application, please make sure that you have installed the required drivers.\n\nDirectXInput will be closed during the installation of the required drivers.\n\nIf you just installed the drivers and this message shows up restart your PC.", "Install the drivers", "Close application", "", "");
                if (messageResult == 1)
                {
                    if (!CheckRunningProcessByNameOrTitle("DriverInstaller", false))
                    {
                        ProcessLauncherWin32("DriverInstaller.exe", "", "", false, false);
                        await Application_Exit();
                    }
                }
                else
                {
                    await Application_Exit();
                }
            }
            catch { }
        }
    }
}