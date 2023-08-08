using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVProcess;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
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

                //Save button settings
                cb_ControllerFakeGuideButton.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.FakeGuideButton = cb_ControllerFakeGuideButton.IsChecked.Value;
                        if (cb_ControllerFakeGuideButton.IsChecked.Value)
                        {
                            cb_ControllerFakeMediaButton.IsChecked = false;
                            activeController.Details.Profile.FakeMediaButton = false;
                        }

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                cb_ControllerFakeMediaButton.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.FakeMediaButton = cb_ControllerFakeMediaButton.IsChecked.Value;
                        if (cb_ControllerFakeMediaButton.IsChecked.Value)
                        {
                            cb_ControllerFakeGuideButton.IsChecked = false;
                            activeController.Details.Profile.FakeGuideButton = false;
                        }

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                cb_ControllerFakeTouchpadButton.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.FakeTouchpadButton = cb_ControllerFakeTouchpadButton.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                //Controller Trigger
                cb_ControllerUseButtonTriggers.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.UseButtonTriggers = cb_ControllerUseButtonTriggers.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerDeadzoneTriggerLeft.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerDeadzoneTriggerLeft.Text = textblock_ControllerDeadzoneTriggerLeft.Tag.ToString() + Convert.ToInt32(slider_ControllerDeadzoneTriggerLeft.Value) + "%";
                        activeController.Details.Profile.DeadzoneTriggerLeft = Convert.ToInt32(slider_ControllerDeadzoneTriggerLeft.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerDeadzoneTriggerRight.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerDeadzoneTriggerRight.Text = textblock_ControllerDeadzoneTriggerRight.Tag.ToString() + Convert.ToInt32(slider_ControllerDeadzoneTriggerRight.Value) + "%";
                        activeController.Details.Profile.DeadzoneTriggerRight = Convert.ToInt32(slider_ControllerDeadzoneTriggerRight.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerSensitivityTriggerLeft.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerSensitivityTriggerLeft.Text = textblock_ControllerSensitivityTriggerLeft.Tag.ToString() + slider_ControllerSensitivityTriggerLeft.Value.ToString("0.00");
                        activeController.Details.Profile.SensitivityTriggerLeft = slider_ControllerSensitivityTriggerLeft.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerSensitivityTriggerRight.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerSensitivityTriggerRight.Text = textblock_ControllerSensitivityTriggerRight.Tag.ToString() + slider_ControllerSensitivityTriggerRight.Value.ToString("0.00");
                        activeController.Details.Profile.SensitivityTriggerRight = slider_ControllerSensitivityTriggerRight.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                //Controller DPad
                cb_ControllerDPadFourWayMovement.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.DPadFourWayMovement = cb_ControllerDPadFourWayMovement.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                //Controller Thumb Stick
                cb_ControllerThumbFlipMovement.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipMovement = cb_ControllerThumbFlipMovement.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                cb_ControllerThumbFlipAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipAxesLeft = cb_ControllerThumbFlipAxesLeft.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                cb_ControllerThumbFlipAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbFlipAxesRight = cb_ControllerThumbFlipAxesRight.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                cb_ControllerThumbReverseAxesLeft.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbReverseAxesLeft = cb_ControllerThumbReverseAxesLeft.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                cb_ControllerThumbReverseAxesRight.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ThumbReverseAxesRight = cb_ControllerThumbReverseAxesRight.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerDeadzoneThumbLeft.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerDeadzoneThumbLeft.Text = textblock_ControllerDeadzoneThumbLeft.Tag.ToString() + Convert.ToInt32(slider_ControllerDeadzoneThumbLeft.Value) + "%";
                        activeController.Details.Profile.DeadzoneThumbLeft = Convert.ToInt32(slider_ControllerDeadzoneThumbLeft.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerDeadzoneThumbRight.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerDeadzoneThumbRight.Text = textblock_ControllerDeadzoneThumbRight.Tag.ToString() + Convert.ToInt32(slider_ControllerDeadzoneThumbRight.Value) + "%";
                        activeController.Details.Profile.DeadzoneThumbRight = Convert.ToInt32(slider_ControllerDeadzoneThumbRight.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerSensitivityThumbLeft.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerSensitivityThumbLeft.Text = textblock_ControllerSensitivityThumbLeft.Tag.ToString() + slider_ControllerSensitivityThumbLeft.Value.ToString("0.00");
                        activeController.Details.Profile.SensitivityThumbLeft = slider_ControllerSensitivityThumbLeft.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerSensitivityThumbRight.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerSensitivityThumbRight.Text = textblock_ControllerSensitivityThumbRight.Tag.ToString() + slider_ControllerSensitivityThumbRight.Value.ToString("0.00");
                        activeController.Details.Profile.SensitivityThumbRight = slider_ControllerSensitivityThumbRight.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                cb_ControllerRumbleEnabled.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ControllerRumbleEnabled = cb_ControllerRumbleEnabled.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));

                        //Update settings interface
                        if (activeController.Details.Profile.ControllerRumbleEnabled)
                        {
                            combobox_ControllerRumbleMode.IsEnabled = true;
                            slider_ControllerRumbleStrength.IsEnabled = true;
                            slider_ControllerRumbleLimit.IsEnabled = true;
                        }
                        else
                        {
                            combobox_ControllerRumbleMode.IsEnabled = false;
                            slider_ControllerRumbleStrength.IsEnabled = false;
                            slider_ControllerRumbleLimit.IsEnabled = false;
                        }
                    }
                };

                combobox_ControllerRumbleMode.SelectionChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.ControllerRumbleMode = combobox_ControllerRumbleMode.SelectedIndex;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerRumbleLimit.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerRumbleLimit.Text = textblock_ControllerRumbleLimit.Tag.ToString() + Convert.ToInt32(slider_ControllerRumbleLimit.Value) + "%";
                        activeController.Details.Profile.ControllerRumbleLimit = Convert.ToInt32(slider_ControllerRumbleLimit.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerRumbleStrength.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerRumbleStrength.Text = textblock_ControllerRumbleStrength.Tag.ToString() + Convert.ToInt32(slider_ControllerRumbleStrength.Value) + "%";
                        activeController.Details.Profile.ControllerRumbleStrength = Convert.ToInt32(slider_ControllerRumbleStrength.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                cb_TriggerRumbleEnabled.Click += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        activeController.Details.Profile.TriggerRumbleEnabled = cb_TriggerRumbleEnabled.IsChecked.Value;

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));

                        //Update settings interface
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

                slider_TriggerRumbleLimit.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_TriggerRumbleLimit.Text = textblock_TriggerRumbleLimit.Tag.ToString() + Convert.ToInt32(slider_TriggerRumbleLimit.Value) + "%";
                        activeController.Details.Profile.TriggerRumbleLimit = Convert.ToInt32(slider_TriggerRumbleLimit.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_TriggerRumbleStrengthLeft.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_TriggerRumbleStrengthLeft.Text = textblock_TriggerRumbleStrengthLeft.Tag.ToString() + Convert.ToInt32(slider_TriggerRumbleStrengthLeft.Value) + "%";
                        activeController.Details.Profile.TriggerRumbleStrengthLeft = Convert.ToInt32(slider_TriggerRumbleStrengthLeft.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_TriggerRumbleStrengthRight.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_TriggerRumbleStrengthRight.Text = textblock_TriggerRumbleStrengthRight.Tag.ToString() + Convert.ToInt32(slider_TriggerRumbleStrengthRight.Value) + "%";
                        activeController.Details.Profile.TriggerRumbleStrengthRight = Convert.ToInt32(slider_TriggerRumbleStrengthRight.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));
                    }
                };

                slider_ControllerLedBrightness.ValueChanged += (sender, e) =>
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        textblock_ControllerLedBrightness.Text = textblock_ControllerLedBrightness.Tag.ToString() + Convert.ToInt32(slider_ControllerLedBrightness.Value) + "%";
                        activeController.Details.Profile.LedBrightness = Convert.ToInt32(slider_ControllerLedBrightness.Value);

                        //Save changes to Json file
                        JsonSaveObject(activeController.Details.Profile, GenerateJsonNameControllerProfile(activeController.Details.Profile));

                        //Controller update led color
                        ControllerLedColor(activeController);
                    }
                };

                //Controller button mapping functions
                btn_SetA.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetB.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetX.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetY.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetShoulderLeft.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetShoulderRight.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetThumbLeft.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetThumbRight.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetBack.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetGuide.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetStart.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetTriggerLeft.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetTriggerRight.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetTouchpad.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                btn_SetMedia.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Set;
                button_SetController_Map.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Map;
                button_SetController_Unmap.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Unmap;
                button_SetController_Cancel.PreviewMouseLeftButtonUp += Btn_MapController_Mouse_Cancel;

                //Keypad button mapping functions
                btn_SetPadDPadLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadDPadUp.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadDPadRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadDPadDown.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbLeftLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbLeftUp.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbLeftRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbLeftDown.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbRightLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbRightUp.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbRightRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbRightDown.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadThumbRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadA.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadB.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadX.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadY.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadShoulderLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadTriggerLeft.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadShoulderRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadTriggerRight.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadBack.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                btn_SetPadStart.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Set;
                button_SetPad_Unmap.PreviewMouseLeftButtonUp += Btn_MapKeypad_Mouse_Unmap;
                combobox_SetPad_Modifier0.SelectionChanged += ComboBox_MapKeypad_Save;
                combobox_SetPad_Modifier1.SelectionChanged += ComboBox_MapKeypad_Save;
                combobox_SetPad_Keyboard.SelectionChanged += ComboBox_MapKeypad_Save;
                btn_Settings_KeyboardTextString_Add.Click += Btn_Settings_KeyboardTextString_Add_Click;
                btn_Settings_KeyboardTextString_Remove.Click += Btn_Settings_KeyboardTextString_Remove_Click;
                btn_Settings_KeypadProcessProfile_Add.Click += Btn_Settings_KeypadProcessProfile_Add_Click;
                btn_Settings_KeypadProcessProfile_Remove.Click += Btn_Settings_KeypadProcessProfile_Remove_Click;
                combobox_KeypadProcessProfile.SelectionChanged += Combobox_KeypadProcessProfile_SelectionChanged;

                //Capture functions
                btn_Settings_OpenXboxGameBarOverlay.Click += Btn_Settings_OpenXboxGameBarOverlay_Click;
                btn_Settings_OpenXboxGameBarSettings.Click += Btn_Settings_OpenXboxGameBarSettings_Click;
                btn_Settings_OpenXboxCapture.Click += Btn_Settings_OpenXboxCapture_Click;
                btn_Settings_OpenCaptureFolder.Click += Btn_Settings_OpenCaptureFolder_Click;

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
                vProcessCtrlUI = Get_ProcessesMultiByName("CtrlUI", true).FirstOrDefault();
                vProcessFpsOverlayer = Get_ProcessesMultiByName("FpsOverlayer", true).FirstOrDefault();
                vProcessForeground = Get_ProcessMultiByWindowHandle(GetForegroundWindow());

                //Check if CtrlUI is currently activated
                vProcessCtrlUIActivated = vProcessCtrlUI != null && vProcessCtrlUI.Identifier == vProcessForeground.Identifier;

                AVActions.DispatcherInvoke(delegate
                {
                    try
                    {
                        if (WindowState == WindowState.Maximized) { vAppMaximized = true; } else { vAppMaximized = false; }
                        if (WindowState == WindowState.Minimized) { vAppMinimized = true; } else { vAppMinimized = false; }
                        if (vProcessCurrent.Identifier == vProcessForeground.Identifier)
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
                AVActions.DispatcherInvoke(delegate
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
                AVActions.DispatcherInvoke(delegate
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
                    if (!Check_RunningProcessByName("DriverInstaller", true))
                    {
                        AVProcess.Launch_ShellExecute("DriverInstaller.exe", "", "", true);
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

                string messageResult = await new AVMessageBox().Popup(this, "Update drivers", "There seem to be newer drivers available to install, DirectXInput will be closed during the installation of the required drivers.\n\nAfter some Windows updates you may need to reinstall the drivers to work.", messageAnswers);
                if (messageResult == "Update the drivers")
                {
                    if (!Check_RunningProcessByName("DriverInstaller", true))
                    {
                        AVProcess.Launch_ShellExecute("DriverInstaller.exe", "", "", true);
                        await Application_Exit();
                    }
                }
            }
            catch { }
        }
    }
}