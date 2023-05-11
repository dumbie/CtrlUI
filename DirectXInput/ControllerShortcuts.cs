using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVAudioDevice;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check if controller shortcut is pressed
        async Task<bool> ControllerShortcut(ControllerStatus Controller)
        {
            bool ControllerUsed = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay250 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= Controller.Delay_ControllerShortcut)
                {
                    //Activate the controller
                    if (Controller.InputCurrent.ButtonGuide.PressedShort)
                    {
                        Debug.WriteLine("Shortcut activate controller has been pressed.");
                        bool controllerActivated = ControllerActivate(Controller);

                        ControllerUsed = true;
                        ControllerDelay125 = true;

                        if (controllerActivated) { return ControllerUsed; }
                    }

                    //Hide popups with guide button
                    if (Controller.InputCurrent.ButtonGuide.PressedShort && (App.vWindowKeyboard.vWindowVisible || App.vWindowKeypad.vWindowVisible))
                    {
                        await HideOpenPopups();

                        ControllerUsed = true;
                        ControllerDelay750 = true;
                    }
                    //Show or launch CtrlUI application
                    else if (Controller.InputCurrent.ButtonGuide.PressedShort)
                    {
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutLaunchCtrlUI", typeof(bool)))
                        {
                            Debug.WriteLine("Shortcut show or hide CtrlUI has been pressed.");
                            await ToolFunctions.CtrlUI_LaunchShow();

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Show the keyboard or keypad
                    else if (Controller.InputCurrent.ButtonGuide.PressedLong)
                    {
                        if (!App.vWindowKeyboard.vWindowVisible && !App.vWindowKeypad.vWindowVisible)
                        {
                            if (vKeyboardKeypadLastActive == "Keyboard")
                            {
                                await KeyboardPopupHideShow(false, false);
                            }
                            else
                            {
                                await KeypadPopupHideShow(false);
                            }
                        }
                        else
                        {
                            await KeyboardKeypadPopupSwitch();
                        }

                        ControllerUsed = true;
                        ControllerDelay750 = true;
                    }
                    //Mute or unmute the input/microphone
                    else if (Controller.InputCurrent.ButtonMedia.PressedRaw)
                    {
                        int muteFunction = SettingLoad(vConfigurationDirectXInput, "ShortcutMuteFunction", typeof(int));
                        if (muteFunction == 0)
                        {
                            if (AudioMuteSwitch(true))
                            {
                                App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input volume muted");
                            }
                            else
                            {
                                App.vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input volume unmuted");
                            }
                        }
                        else
                        {
                            if (AudioMuteSwitch(false))
                            {
                                App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output volume muted");
                            }
                            else
                            {
                                App.vWindowOverlay.Notification_Show_Status("VolumeMute", "Output volume unmuted");
                            }
                        }

                        ControllerUsed = true;
                        ControllerDelay750 = true;
                    }
                    //Press Alt+Enter
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonShoulderRight.PressedRaw)
                    {
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutAltEnter", typeof(bool)))
                        {
                            Debug.WriteLine("Button Global - Alt+Enter");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Alt+Enter";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            KeysHidAction KeysHidAction = new KeysHidAction()
                            {
                                Modifiers = KeysModifierHid.AltLeft,
                                Key0 = KeysHid.Enter
                            };
                            vFakerInputDevice.KeyboardPressRelease(KeysHidAction);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Press Alt+Tab
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonShoulderLeft.PressedRaw)
                    {
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutAltTab", typeof(bool)))
                        {
                            Debug.WriteLine("Button Global - Press Alt+Tab");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Pressing Alt+Tab";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            //Press and hold Alt+Tab
                            KeysHidAction keyboardAltTab = new KeysHidAction()
                            {
                                Modifiers = KeysModifierHid.AltLeft,
                                Key0 = KeysHid.Tab
                            };
                            vFakerInputDevice.KeyboardPress(keyboardAltTab);

                            //Press and Hold Alt
                            KeysHidAction keyboardAlt = new KeysHidAction()
                            {
                                Modifiers = KeysModifierHid.AltLeft,
                            };
                            vFakerInputDevice.KeyboardPress(keyboardAlt);

                            vAltTabDownStatus = true;
                            ControllerUsed = true;
                            ControllerDelay250 = true;
                        }
                    }
                    //Release Alt+Tab
                    else if (vAltTabDownStatus && !Controller.InputCurrent.ButtonStart.PressedRaw)
                    {
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutAltTab", typeof(bool)))
                        {
                            Debug.WriteLine("Button Global - Release Alt+Tab");

                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "AppMiniMaxi";
                            notificationDetails.Text = "Releasing Alt+Tab";
                            App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                            //Release all key presses
                            vFakerInputDevice.KeyboardReset();

                            vAltTabDownStatus = false;
                            ControllerUsed = true;
                            ControllerDelay250 = true;
                        }
                    }
                    //Signal to capture image
                    else if (Controller.InputCurrent.ButtonTouchpad.PressedShort)
                    {
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutCaptureImage", typeof(bool)))
                        {
                            Debug.WriteLine("Button Global - Image capture");
                            XboxGameDVR.CaptureImage();

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Signal to capture video
                    else if (Controller.InputCurrent.ButtonTouchpad.PressedLong)
                    {
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutCaptureVideo", typeof(bool)))
                        {
                            Debug.WriteLine("Button Global - Video capture");
                            XboxGameDVR.CaptureVideo();

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Disconnect controller from Bluetooth
                    else if (Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonGuide.PressedRaw && Controller.Details.Wireless)
                    {
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutDisconnectBluetooth", typeof(bool)))
                        {
                            Debug.WriteLine("Shortcut disconnect Bluetooth has been pressed.");
                            await StopController(Controller, "manually", string.Empty);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }
                    //Press ctrl + alt + delete
                    else if (Controller.InputCurrent.ButtonBack.PressedRaw && Controller.InputCurrent.ButtonGuide.PressedRaw)
                    {
                        if (SettingLoad(vConfigurationDirectXInput, "ShortcutCtrlAltDelete", typeof(bool)))
                        {
                            Debug.WriteLine("Shortcut ctrl + alt + delete pressed.");

                            //Press ctrl + alt + delete
                            KeysHidAction KeysHidAction = new KeysHidAction()
                            {
                                Modifiers = KeysModifierHid.CtrlLeft | KeysModifierHid.AltLeft,
                                Key0 = KeysHid.Delete
                            };
                            vFakerInputDevice.KeyboardPressRelease(KeysHidAction);

                            //Show the keyboard
                            await KeyboardPopupHideShow(true, true);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                        }
                    }

                    if (ControllerDelay125)
                    {
                        Controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay250)
                    {
                        Controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayTicks250;
                    }
                    else if (ControllerDelay750)
                    {
                        Controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayTicks750;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Switch between keyboard and keypad
        async Task KeyboardKeypadPopupSwitch()
        {
            try
            {
                Debug.WriteLine("Switching between keyboard and keypad");
                await AVActions.DispatcherInvoke(async delegate
                {
                    if (App.vWindowKeyboard.vWindowVisible)
                    {
                        await App.vWindowKeyboard.Hide();
                        await App.vWindowKeypad.Show();
                    }
                    else
                    {
                        await App.vWindowKeypad.Hide();
                        await App.vWindowKeyboard.Show(false, false);
                    }
                });
            }
            catch { }
        }

        //Hide or show the keypad
        public async Task KeypadPopupHideShow(bool forceShow)
        {
            try
            {
                Debug.WriteLine("Shortcut keypad has been pressed.");
                await AVActions.DispatcherInvoke(async delegate
                {
                    if (!App.vWindowKeyboard.vWindowVisible && !App.vWindowKeypad.vWindowVisible)
                    {
                        if (forceShow || SettingLoad(vConfigurationDirectXInput, "ShortcutKeyboardPopup", typeof(bool)))
                        {
                            await App.vWindowKeypad.Show();
                        }
                    }
                    else if (!forceShow)
                    {
                        await App.vWindowKeyboard.Hide();
                        await App.vWindowKeypad.Hide();
                    }
                    else if (forceShow)
                    {
                        await App.vWindowKeyboard.Hide();
                        await App.vWindowKeypad.Show();
                    }
                });
            }
            catch { }
        }

        //Hide or show the keyboard
        public async Task KeyboardPopupHideShow(bool forceShow, bool forceKeyboardMode)
        {
            try
            {
                Debug.WriteLine("Shortcut keyboard has been pressed.");
                await AVActions.DispatcherInvoke(async delegate
                {
                    if (!App.vWindowKeyboard.vWindowVisible && !App.vWindowKeypad.vWindowVisible)
                    {
                        if (forceShow || SettingLoad(vConfigurationDirectXInput, "ShortcutKeyboardPopup", typeof(bool)))
                        {
                            await App.vWindowKeyboard.Show(forceKeyboardMode, forceKeyboardMode);
                        }
                    }
                    else if (!forceShow)
                    {
                        await App.vWindowKeyboard.Hide();
                        await App.vWindowKeypad.Hide();
                    }
                    else if (forceShow)
                    {
                        await App.vWindowKeypad.Hide();
                        await App.vWindowKeyboard.Show(forceKeyboardMode, forceKeyboardMode);
                    }
                    else if (forceKeyboardMode)
                    {
                        await App.vWindowKeyboard.SetModeKeyboard();
                    }
                });
            }
            catch { }
        }

        //Hide all opened popups
        async Task HideOpenPopups()
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    await App.vWindowKeyboard.Hide();
                    await App.vWindowKeypad.Hide();
                });
            }
            catch { }
        }
    }
}