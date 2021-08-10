using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Display a certain grid page
        void ShowGridPage(FrameworkElement elementTarget)
        {
            try
            {
                if (elementTarget == grid_Debug)
                {
                    Debug.WriteLine("Enabling controller debug mode.");
                    vShowDebugInformation = true;
                }
                else
                {
                    Debug.WriteLine("Disabling controller debug mode.");
                    vShowDebugInformation = false;
                }

                grid_Connection.Visibility = Visibility.Collapsed;
                grid_Controller.Visibility = Visibility.Collapsed;
                grid_Battery.Visibility = Visibility.Collapsed;
                grid_Ignore.Visibility = Visibility.Collapsed;
                grid_Keyboard.Visibility = Visibility.Collapsed;
                grid_Keypad.Visibility = Visibility.Collapsed;
                grid_Settings.Visibility = Visibility.Collapsed;
                grid_Shortcuts.Visibility = Visibility.Collapsed;
                grid_Debug.Visibility = Visibility.Collapsed;
                grid_Help.Visibility = Visibility.Collapsed;
                elementTarget.Visibility = Visibility.Visible;
            }
            catch { }
        }

        //Register Interface Handlers
        void RegisterInterfaceHandlers()
        {
            try
            {
                //Make sure the correct window style is set
                StateChanged += CheckWindowStateAndStyle;

                //Main menu functions
                lb_Menu.PreviewKeyUp += lb_Menu_KeyPressUp;
                lb_Menu.PreviewMouseUp += lb_Menu_MousePressUp;

                //Connection functions
                button_Controller0.Click += Button_Controller0_Click;
                button_Controller1.Click += Button_Controller1_Click;
                button_Controller2.Click += Button_Controller2_Click;
                button_Controller3.Click += Button_Controller3_Click;
                btn_SearchNewControllers.Click += Btn_SearchNewControllers_Click;
                btn_IgnoreController.Click += btn_IgnoreController_Click;
                btn_DisconnectController.Click += Btn_DisconnectController_Click;
                btn_DisconnectControllerAll.Click += Btn_DisconnectControllerAll_Click;
                btn_RemoveController.Click += Btn_RemoveController_Click;
                btn_CopyDebugInformation.Click += Btn_CopyDebugInformation_Click;
                btn_CheckControllers.Click += btn_CheckControllers_Click;
                btn_CheckDeviceManager.Click += btn_CheckDeviceManager_Click;

                //Ignore functions
                listbox_ControllerIgnore.PreviewKeyUp += Listbox_ControllerIgnore_PreviewKeyUp;
                listbox_ControllerIgnore.PreviewMouseUp += Listbox_ControllerIgnore_PreviewMouseUp;

                //Controller functions
                btn_RumbleTestLight.Click += Btn_TestRumble_Click;
                btn_RumbleTestHeavy.Click += Btn_TestRumble_Click;

                //Save controller settings
                cb_ControllerFakeGuideButton.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.FakeGuideButton = cb_ControllerFakeGuideButton.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                //Controller Trigger
                cb_ControllerUseButtonTriggers.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.UseButtonTriggers = cb_ControllerUseButtonTriggers.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                slider_ControllerDeadzoneTriggerLeft.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerDeadzoneTriggerLeft.Text = textblock_ControllerDeadzoneTriggerLeft.Tag.ToString() + Convert.ToInt32(slider_ControllerDeadzoneTriggerLeft.Value) + "%";
                        activeController.Details.Profile.DeadzoneTriggerLeft = Convert.ToInt32(slider_ControllerDeadzoneTriggerLeft.Value);
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                slider_ControllerDeadzoneTriggerRight.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerDeadzoneTriggerRight.Text = textblock_ControllerDeadzoneTriggerRight.Tag.ToString() + Convert.ToInt32(slider_ControllerDeadzoneTriggerRight.Value) + "%";
                        activeController.Details.Profile.DeadzoneTriggerRight = Convert.ToInt32(slider_ControllerDeadzoneTriggerRight.Value);
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                slider_ControllerSensitivityTrigger.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerSensitivityTrigger.Text = textblock_ControllerSensitivityTrigger.Tag.ToString() + slider_ControllerSensitivityTrigger.Value.ToString("0.00");
                        activeController.Details.Profile.SensitivityTrigger = slider_ControllerSensitivityTrigger.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                //Controller D-Pad
                cb_ControllerDPadFourWayMovement.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.DPadFourWayMovement = cb_ControllerDPadFourWayMovement.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                //Controller Thumb Stick
                cb_ControllerThumbFlipMovement.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipMovement = cb_ControllerThumbFlipMovement.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                cb_ControllerThumbFlipAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipAxesLeft = cb_ControllerThumbFlipAxesLeft.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                cb_ControllerThumbFlipAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipAxesRight = cb_ControllerThumbFlipAxesRight.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                cb_ControllerThumbReverseAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbReverseAxesLeft = cb_ControllerThumbReverseAxesLeft.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                cb_ControllerThumbReverseAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbReverseAxesRight = cb_ControllerThumbReverseAxesRight.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                slider_ControllerDeadzoneThumbLeft.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerDeadzoneThumbLeft.Text = textblock_ControllerDeadzoneThumbLeft.Tag.ToString() + Convert.ToInt32(slider_ControllerDeadzoneThumbLeft.Value) + "%";
                        activeController.Details.Profile.DeadzoneThumbLeft = Convert.ToInt32(slider_ControllerDeadzoneThumbLeft.Value);
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                slider_ControllerDeadzoneThumbRight.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerDeadzoneThumbRight.Text = textblock_ControllerDeadzoneThumbRight.Tag.ToString() + Convert.ToInt32(slider_ControllerDeadzoneThumbRight.Value) + "%";
                        activeController.Details.Profile.DeadzoneThumbRight = Convert.ToInt32(slider_ControllerDeadzoneThumbRight.Value);
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                slider_ControllerSensitivityThumb.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerSensitivityThumb.Text = textblock_ControllerSensitivityThumb.Tag.ToString() + slider_ControllerSensitivityThumb.Value.ToString("0.00");
                        activeController.Details.Profile.SensitivityThumb = slider_ControllerSensitivityThumb.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                cb_ControllerRumbleEnabled.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ControllerRumbleEnabled = cb_ControllerRumbleEnabled.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                        if (activeController.Details.Profile.ControllerRumbleEnabled)
                        {
                            slider_ControllerRumbleStrength.IsEnabled = true;
                        }
                        else
                        {
                            slider_ControllerRumbleStrength.IsEnabled = false;
                        }
                    }
                };

                slider_ControllerRumbleStrength.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerRumbleStrength.Text = textblock_ControllerRumbleStrength.Tag.ToString() + Convert.ToInt32(slider_ControllerRumbleStrength.Value) + "%";
                        activeController.Details.Profile.ControllerRumbleStrength = Convert.ToInt32(slider_ControllerRumbleStrength.Value);
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                cb_TriggerRumbleEnabled.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.TriggerRumbleEnabled = cb_TriggerRumbleEnabled.IsChecked.Value;
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                        if (activeController.Details.Profile.TriggerRumbleEnabled)
                        {
                            slider_TriggerRumbleStrengthLeft.IsEnabled = true;
                            slider_TriggerRumbleStrengthRight.IsEnabled = true;
                        }
                        else
                        {
                            slider_TriggerRumbleStrengthLeft.IsEnabled = false;
                            slider_TriggerRumbleStrengthRight.IsEnabled = false;
                        }
                    }
                };

                slider_TriggerRumbleStrengthLeft.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_TriggerRumbleStrengthLeft.Text = textblock_TriggerRumbleStrengthLeft.Tag.ToString() + Convert.ToInt32(slider_TriggerRumbleStrengthLeft.Value) + "%";
                        activeController.Details.Profile.TriggerRumbleStrengthLeft = Convert.ToInt32(slider_TriggerRumbleStrengthLeft.Value);
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                slider_TriggerRumbleStrengthRight.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_TriggerRumbleStrengthRight.Text = textblock_TriggerRumbleStrengthRight.Tag.ToString() + Convert.ToInt32(slider_TriggerRumbleStrengthRight.Value) + "%";
                        activeController.Details.Profile.TriggerRumbleStrengthRight = Convert.ToInt32(slider_TriggerRumbleStrengthRight.Value);
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");
                    }
                };

                slider_ControllerLedBrightness.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerLedBrightness.Text = textblock_ControllerLedBrightness.Tag.ToString() + Convert.ToInt32(slider_ControllerLedBrightness.Value) + "%";
                        activeController.Details.Profile.LedBrightness = Convert.ToInt32(slider_ControllerLedBrightness.Value);
                        JsonSaveObject(vDirectControllersProfile, @"User\DirectControllersProfile");

                        //Controller update led color
                        ControllerLedColor(activeController);
                    }
                };

                //Controller button mapping functions
                btn_SetA.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetA.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetB.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetB.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetX.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetX.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetY.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetY.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetShoulderLeft.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetShoulderLeft.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetShoulderRight.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetShoulderRight.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetThumbLeft.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetThumbLeft.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetThumbRight.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetThumbRight.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetBack.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetBack.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetGuide.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetGuide.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetStart.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetStart.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetTriggerLeft.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetTriggerLeft.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;
                btn_SetTriggerRight.PreviewMouseLeftButtonUp += Btn_MapController_MouseLeft;
                btn_SetTriggerRight.PreviewMouseRightButtonUp += Btn_MapController_MouseRight;

                //Keypad button mapping functions
                btn_SetPadDPadLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadDPadLeft.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadDPadUp.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadDPadUp.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadDPadRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadDPadRight.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadDPadDown.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadDPadDown.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeftLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeftLeft.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeftUp.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeftUp.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeftRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeftRight.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeftDown.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeftDown.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbLeft.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRightLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRightLeft.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRightUp.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRightUp.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRightRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRightRight.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRightDown.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRightDown.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadThumbRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadThumbRight.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadA.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadA.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadB.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadB.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadX.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadX.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadY.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadY.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadShoulderLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadShoulderLeft.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadShoulderLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadShoulderLeft.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadShoulderRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadShoulderRight.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadTriggerRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadTriggerRight.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadBack.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadBack.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_SetPadStart.PreviewMouseLeftButtonUp += Btn_MapKeypad_MouseLeft;
                btn_SetPadStart.PreviewMouseRightButtonUp += Btn_MapKeypad_MouseRight;
                btn_Settings_KeyboardTextString_Add.Click += Btn_Settings_KeyboardTextString_Add_Click;
                btn_Settings_KeyboardTextString_Remove.Click += Btn_Settings_KeyboardTextString_Remove_Click;
                btn_Settings_KeypadProcessProfile_Add.Click += Btn_Settings_KeypadProcessProfile_Add_Click;
                btn_Settings_KeypadProcessProfile_Remove.Click += Btn_Settings_KeypadProcessProfile_Remove_Click;
                combobox_KeypadProcessProfile.SelectionChanged += Combobox_KeypadProcessProfile_SelectionChanged;

                //Settings functions
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
                vProcessFpsOverlayer = GetProcessByNameOrTitle("FpsOverlayer", false);
                vProcessForeground = GetProcessMultiFromWindowHandle(GetForegroundWindow());

                //Check if CtrlUI is currently activated
                if (vProcessCtrlUI != null && vProcessCtrlUI.Id == vProcessForeground.Identifier) { vProcessCtrlUIActivated = true; } else { vProcessCtrlUIActivated = false; }

                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        if (WindowState == WindowState.Maximized) { vAppMaximized = true; } else { vAppMaximized = false; }
                        if (WindowState == WindowState.Minimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vProcessCurrent.Id == vProcessForeground.Identifier)
                        {
                            AppWindowActivated();
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
        void AppWindowActivated()
        {
            try
            {
                if (!vAppActivated)
                {
                    vAppActivated = true;
                    Debug.WriteLine("Activated the application.");

                    //Enable application window
                    AppWindowEnable();
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

                    //Disable application window
                    AppWindowDisable("Application window is not activated.");
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

        //Install the required drivers message popup
        async Task Message_InstallDrivers()
        {
            try
            {
                List<string> messageAnswers = new List<string>();
                messageAnswers.Add("Install the drivers");
                messageAnswers.Add("Close application");

                string messageResult = await new AVMessageBox().Popup(this, "Install drivers", "Welcome to DirectXInput, it seems like you have not yet installed the required drivers to use this application, please make sure that you have installed the required drivers.\n\nDirectXInput will be closed during the installation of the required drivers.\n\nIf you just installed the drivers and this message shows up restart your PC.\n\nAfter some Windows updates you may need to reinstall the drivers to work.", messageAnswers);
                if (messageResult == "Install the drivers")
                {
                    if (!CheckRunningProcessByNameOrTitle("DriverInstaller", false))
                    {
                        await ProcessLauncherWin32Async("DriverInstaller.exe", "", "", false, false);
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

        //Update the required drivers message popup
        async Task Message_UpdateDrivers()
        {
            try
            {
                List<string> messageAnswers = new List<string>();
                messageAnswers.Add("Update the drivers");
                messageAnswers.Add("Cancel");

                string messageResult = await new AVMessageBox().Popup(this, "Update drivers", "DirectXInput will be closed during the installation of the required drivers.\n\nAfter some Windows updates you may need to reinstall the drivers to work.", messageAnswers);
                if (messageResult == "Update the drivers")
                {
                    if (!CheckRunningProcessByNameOrTitle("DriverInstaller", false))
                    {
                        await ProcessLauncherWin32Async("DriverInstaller.exe", "", "", false, false);
                        await Application_Exit();
                    }
                }
            }
            catch { }
        }
    }
}