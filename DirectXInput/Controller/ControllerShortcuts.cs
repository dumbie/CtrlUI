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
                //Check shortcut buttons
                var sortedTriggers = keysHotkey.Trigger.Where(x => x != ControllerButtons.None);
                int countHotkeys = sortedTriggers.Count();
                if (countHotkeys == 0) { return false; }

                List<bool> shortcutPressed = new List<bool>();
                foreach (ControllerButtons button in sortedTriggers)
                {
                    try
                    {
                        //Fix shortcuts with two holding buttons
                        bool shortcutPress = false;
                        if (countHotkeys == 1)
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
                            if (keysHotkey.Hold)
                            {
                                shortcutPress = keysPressed[(byte)button].PressedLong;
                            }
                            else
                            {
                                shortcutPress = keysPressed[(byte)button].PressedRaw;
                            }
                        }
                        shortcutPressed.Add(shortcutPress);
                    }
                    catch { }
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
                        if (ControllerActivate(controller))
                        {
                            Debug.WriteLine("Shortcut activate controller has been pressed.");
                            ControllerUsed = true;
                            ControllerDelay125 = true;
                            return ControllerUsed;
                        }
                    }

                    foreach (ShortcutTriggerController shortcutTrigger in vShortcutsController.OrderByDescending(x => x.Trigger.Count(x => x != ControllerButtons.None)).ThenByDescending(x => x.Hold))
                    {
                        //Launch CtrlUI application or switch keyboard mode
                        if (shortcutTrigger.Name == "LaunchCtrlUI")
                        {
                            if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                            {
                                Debug.WriteLine("Shortcut show or hide CtrlUI has been pressed.");
                                if (vWindowKeyboard.vWindowVisible || vWindowKeypad.vWindowVisible)
                                {
                                    await HideOpenPopups();
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
                        if (shortcutTrigger.Name == "KeyboardPopup")
                        {
                            if (CheckShortcutButton(controller.InputCurrent.Buttons, shortcutTrigger, false))
                            {
                                Debug.WriteLine("Shortcut show or hide keyboard has been pressed.");
                                if (vWindowKeyboard.vWindowVisible || vWindowKeypad.vWindowVisible)
                                {
                                    await KeyboardKeypadPopupSwitch();
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
                        if (shortcutTrigger.Name == "MuteOutput")
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
                        if (shortcutTrigger.Name == "MuteInput")
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
                        if (shortcutTrigger.Name == "AltEnter")
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
                        if (shortcutTrigger.Name == "CtrlAltDelete")
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
                        if (shortcutTrigger.Name == "AltTab")
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
                        if (shortcutTrigger.Name == "CaptureImage")
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
                        if (shortcutTrigger.Name == "CaptureVideo")
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
                        if (shortcutTrigger.Name == "DisconnectController")
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