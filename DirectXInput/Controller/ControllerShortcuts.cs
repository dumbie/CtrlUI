using ArnoldVinkCode;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVAudioDevice;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.ControllerTimings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check controller shortcut button press
        public static bool CheckShortcutButton(ControllerButtonDetails[] keysPressed, ShortcutTriggerController keysHotkey, bool checkAny)
        {
            try
            {
                List<bool> shortcutPressed = new List<bool>();
                foreach (ControllerButtons button in keysHotkey.Trigger)
                {
                    if (button != ControllerButtons.None)
                    {
                        bool shortcutPress = false;
                        if (keysHotkey.Trigger.Count() == 1)
                        {
                            if (keysHotkey.Hold)
                            {
                                shortcutPress = keysPressed[(byte)button].PressedLong;
                            }
                            else
                            {
                                shortcutPress = keysPressed[(byte)button].PressedShort;
                            }
                        }
                        else
                        {
                            //Fix holding two buttons
                            shortcutPress = keysPressed[(byte)button].PressedRaw;
                        }
                        shortcutPressed.Add(shortcutPress);
                    }
                }

                if (checkAny)
                {
                    return shortcutPressed.Any(x => x);
                }
                else
                {
                    return shortcutPressed.All(x => x);
                }
            }
            catch
            {
                return false;
            }
        }

        //Check if controller shortcut is pressed
        async Task<bool> ControllerShortcut(ControllerStatus controller)
        {
            bool ControllerUsed = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay250 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= controller.Delay_ControllerShortcut)
                {
                    //Activate controller
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Guide].PressedShort)
                    {
                        Debug.WriteLine("Shortcut activate controller has been pressed.");
                        if (ControllerActivate(controller))
                        {
                            ControllerUsed = true;
                            ControllerDelay125 = true;
                            return ControllerUsed;
                        }
                    }

                    //Fix sort shortcuts by used buttons, execute 2 buttons first.

                    //Launch CtrlUI application or switch keyboard mode
                    ShortcutTriggerController shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "LaunchCtrlUI");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            Debug.WriteLine("Shortcut show or hide CtrlUI has been pressed.");
                            if (vWindowKeyboard.vWindowVisible || vWindowKeypad.vWindowVisible)
                            {
                                await KeyboardKeypadPopupSwitch();
                            }
                            else
                            {
                                await ToolFunctions.CtrlUI_LaunchShow();

                                ControllerUsed = true;
                                ControllerDelay750 = true;
                                return ControllerUsed;
                            }
                        }
                    }

                    //Show or hide keyboard or keypad
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "KeyboardPopup");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            Debug.WriteLine("Shortcut show or hide keyboard has been pressed.");
                            if (vWindowKeyboard.vWindowVisible || vWindowKeypad.vWindowVisible)
                            {
                                await HideOpenPopups();
                            }
                            else
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

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                            return ControllerUsed;
                        }
                    }

                    //Mute or unmute audio output
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "MuteOutput");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            if (AudioMuteSwitch(false))
                            {
                                vWindowOverlay.Notification_Show_Status("VolumeMute", "Output volume muted");
                            }
                            else
                            {
                                vWindowOverlay.Notification_Show_Status("VolumeMute", "Output volume unmuted");
                            }

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                            return ControllerUsed;
                        }
                    }

                    //Mute or unmute audio input
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "MuteInput");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            if (AudioMuteSwitch(true))
                            {
                                vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input volume muted");
                            }
                            else
                            {
                                vWindowOverlay.Notification_Show_Status("MicrophoneMute", "Input volume unmuted");
                            }

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                            return ControllerUsed;
                        }
                    }

                    //Press Alt+Enter
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "AltEnter");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            Debug.WriteLine("Button Global - Press Alt+Enter");
                            vWindowOverlay.Notification_Show_Status("AppMiniMaxi", "Pressing Alt+Enter");

                            KeysHidAction KeysHidAction = new KeysHidAction()
                            {
                                Modifiers = KeysModifierHid.AltLeft,
                                Key0 = KeysHid.Enter
                            };
                            vFakerInputDevice.KeyboardPressRelease(KeysHidAction);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                            return ControllerUsed;
                        }
                    }

                    //Press Ctrl+Alt+Delete
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "CtrlAltDelete");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            Debug.WriteLine("Button Global - Press Ctrl+Alt+Delete");
                            vWindowOverlay.Notification_Show_Status("Applications", "Pressing Ctrl+Alt+Delete");

                            KeysHidAction KeysHidAction = new KeysHidAction()
                            {
                                Modifiers = KeysModifierHid.CtrlLeft | KeysModifierHid.AltLeft,
                                Key0 = KeysHid.Delete
                            };
                            vFakerInputDevice.KeyboardPressRelease(KeysHidAction);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                            return ControllerUsed;
                        }
                    }

                    //Press Alt+Tab
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "AltTab");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            Debug.WriteLine("Button Global - Press Alt+Tab");
                            vWindowOverlay.Notification_Show_Status("AppMiniMaxi", "Pressing Alt+Tab");

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
                            return ControllerUsed;
                        }
                        else if (vAltTabDownStatus && !CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, true))
                        {
                            Debug.WriteLine("Button Global - Release Alt+Tab");
                            vWindowOverlay.Notification_Show_Status("AppMiniMaxi", "Releasing Alt+Tab");

                            //Release all key presses
                            vFakerInputDevice.KeyboardReset();

                            vAltTabDownStatus = false;
                            ControllerUsed = true;
                            ControllerDelay250 = true;
                            return ControllerUsed;
                        }
                    }

                    //Signal to capture screen image
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "CaptureImage");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            Debug.WriteLine("Button Global - Capture screen image");
                            await ToolFunctions.ScreenCaptureToolCaptureImage();

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                            return ControllerUsed;
                        }
                    }

                    //Signal to capture screen video
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "CaptureVideo");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            Debug.WriteLine("Button Global - Capture screen video");
                            await ToolFunctions.ScreenCaptureToolCaptureVideo();

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                            return ControllerUsed;
                        }
                    }

                    //Disconnect controller from Bluetooth
                    shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "DisconnectController");
                    if (shortcutTrigger != null)
                    {
                        if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                        {
                            Debug.WriteLine("Shortcut disconnect Bluetooth has been pressed.");
                            await StopController(controller, "manually", string.Empty);

                            ControllerUsed = true;
                            ControllerDelay750 = true;
                            return ControllerUsed;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (ControllerDelay125)
                {
                    controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayTicks125;
                }
                else if (ControllerDelay250)
                {
                    controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayTicks250;
                }
                else if (ControllerDelay750)
                {
                    controller.Delay_ControllerShortcut = GetSystemTicksMs() + vControllerDelayTicks750;
                }
            }
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
                    if (vWindowKeyboard.vWindowVisible)
                    {
                        await vWindowKeyboard.Hide();
                        await vWindowKeypad.Show();
                    }
                    else
                    {
                        await vWindowKeypad.Hide();
                        await vWindowKeyboard.Show(false, false);
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
                    if (!vWindowKeyboard.vWindowVisible && !vWindowKeypad.vWindowVisible)
                    {
                        if (forceShow || SettingLoad(vConfigurationDirectXInput, "ShortcutKeyboardPopup", typeof(bool)))
                        {
                            await vWindowKeypad.Show();
                        }
                    }
                    else if (!forceShow)
                    {
                        await vWindowKeyboard.Hide();
                        await vWindowKeypad.Hide();
                    }
                    else if (forceShow)
                    {
                        await vWindowKeyboard.Hide();
                        await vWindowKeypad.Show();
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
                    if (!vWindowKeyboard.vWindowVisible && !vWindowKeypad.vWindowVisible)
                    {
                        if (forceShow || SettingLoad(vConfigurationDirectXInput, "ShortcutKeyboardPopup", typeof(bool)))
                        {
                            await vWindowKeyboard.Show(forceKeyboardMode, forceKeyboardMode);
                        }
                    }
                    else if (!forceShow)
                    {
                        await vWindowKeyboard.Hide();
                        await vWindowKeypad.Hide();
                    }
                    else if (forceShow)
                    {
                        await vWindowKeypad.Hide();
                        await vWindowKeyboard.Show(forceKeyboardMode, forceKeyboardMode);
                    }
                    else if (forceKeyboardMode)
                    {
                        await vWindowKeyboard.SetModeKeyboard();
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
                    await vWindowKeyboard.Hide();
                    await vWindowKeypad.Hide();
                });
            }
            catch { }
        }
    }
}