using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Process controller input for mouse
        public void ControllerInteractionMouse(ControllerInput ControllerInput)
        {
            bool ControllerDelay125 = false;
            bool ControllerDelay500 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Mouse)
                {
                    int scrollHorizontalRight = 0;
                    int scrollVerticalRight = 0;
                    MouseButtons buttonPress = MouseButtons.None;

                    //Get the mouse move amount
                    double moveSensitivity = SettingLoad(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity", typeof(double));
                    GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbLeftX, ControllerInput.ThumbLeftY, true, out int moveHorizontalLeft, out int moveVerticalLeft);

                    //Check the keyboard mode
                    KeyboardMode keyboardMode = (KeyboardMode)SettingLoad(vConfigurationDirectXInput, "KeyboardMode", typeof(int));
                    if (keyboardMode == KeyboardMode.Media || keyboardMode == KeyboardMode.Tool)
                    {
                        //Get the mouse move amount
                        GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, true, out int moveHorizontalRight, out int moveVerticalRight);

                        //Move the keyboard window
                        MoveKeyboardWindow(moveHorizontalRight, moveVerticalRight);
                    }
                    else if (keyboardMode == KeyboardMode.Keyboard)
                    {
                        //Get the mouse scroll amount
                        int scrollSensitivity = SettingLoad(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity2", typeof(int));
                        GetMouseMovementAmountFromThumbScroll(scrollSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, false, out scrollHorizontalRight, out scrollVerticalRight);
                        if (scrollHorizontalRight != 0 || scrollVerticalRight != 0)
                        {
                            ControllerDelay125 = true;
                        }
                    }

                    //Emulate mouse button press
                    if (ControllerInput.ButtonShoulderLeft.PressedRaw && ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        buttonPress = MouseButtons.MiddleButton;
                        ControllerDelay500 = true;
                    }
                    else if (ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        buttonPress = MouseButtons.LeftButton;
                    }
                    else if (ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        buttonPress = MouseButtons.RightButton;
                    }
                    else
                    {
                        buttonPress = MouseButtons.None;
                    }

                    //Update current mouse input
                    MouseAction mouseAction = new MouseAction()
                    {
                        MoveHorizontal = moveHorizontalLeft,
                        MoveVertical = moveVerticalLeft,
                        ScrollHorizontal = scrollHorizontalRight,
                        ScrollVertical = scrollVerticalRight,
                        Button = buttonPress
                    };
                    vFakerInputDevice.MouseRelative(mouseAction);

                    //Delay input to prevent repeat
                    if (ControllerDelay125)
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay500)
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayTicks500;
                    }
                    else
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayTicks10;
                    }
                }
            }
            catch { }
        }

        //Process controller input for keyboard
        public async Task ControllerInteractionKeyboard(ControllerInput ControllerInput)
        {
            bool ControllerDelay125 = false;
            bool ControllerDelay250 = false;
            bool ControllerDelay500 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Keyboard)
                {
                    //Check the keyboard mode
                    KeyboardMode keyboardMode = (KeyboardMode)SettingLoad(vConfigurationDirectXInput, "KeyboardMode", typeof(int));

                    //Send internal arrow left key
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        if (keyboardMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.ArrowLeft
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                        }
                        else
                        {
                            //Check the foreground window
                            if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                            {
                                //Update the window style
                                WindowUpdateStyleVisible(vInteropWindowHandle, true, true, false);

                                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                                KeySendSingle(KeysVirtual.Left, vInteropWindowHandle);
                            }
                        }
                        ControllerDelay125 = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        if (keyboardMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.ArrowRight
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                        }
                        else
                        {
                            //Check the foreground window
                            if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                            {
                                //Update the window style
                                WindowUpdateStyleVisible(vInteropWindowHandle, true, true, false);

                                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                                KeySendSingle(KeysVirtual.Right, vInteropWindowHandle);
                            }
                        }
                        ControllerDelay125 = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        if (keyboardMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.ArrowUp
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                        }
                        else
                        {
                            //Check the foreground window
                            if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                            {
                                //Update the window style
                                WindowUpdateStyleVisible(vInteropWindowHandle, true, true, false);

                                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                                KeySendSingle(KeysVirtual.Up, vInteropWindowHandle);
                            }
                        }
                        ControllerDelay125 = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        if (keyboardMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.ArrowDown
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                        }
                        else
                        {
                            //Check the foreground window
                            if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                            {
                                //Update the window style
                                WindowUpdateStyleVisible(vInteropWindowHandle, true, true, false);

                                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                                KeySendSingle(KeysVirtual.Down, vInteropWindowHandle);
                            }
                        }
                        ControllerDelay125 = true;
                    }

                    //Send internal space key
                    else if (ControllerInput.ButtonA.PressedRaw)
                    {
                        //Check the foreground window
                        if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                        {
                            //Update the window style
                            WindowUpdateStyleVisible(vInteropWindowHandle, true, true, false);

                            KeySendSingle(KeysVirtual.Space, vInteropWindowHandle);
                        }

                        if (CheckTextPopupsOpen())
                        {
                            ControllerDelay250 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    //Send external enter key
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (CheckTextPopupsOpen())
                        {
                            await HideTextPopups();
                            ControllerDelay250 = true;
                        }
                        else if (keyboardMode == KeyboardMode.Media)
                        {
                            MediaNext();
                            ControllerDelay250 = true;
                        }
                        else
                        {
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.Enter
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }
                    //Send external space key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (keyboardMode == KeyboardMode.Media)
                        {
                            MediaPlayPause();
                            ControllerDelay250 = true;
                        }
                        else
                        {
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.Space
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }
                    //Send external backspace or delete key
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (keyboardMode == KeyboardMode.Media)
                        {
                            MediaPrevious();
                            ControllerDelay250 = true;
                        }
                        else if (vCapsEnabled)
                        {
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.Delete
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                        else
                        {
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.Backspace
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }

                    //Send external arrow left key
                    else if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (keyboardMode == KeyboardMode.Media)
                        {
                            VolumeOutputMute();
                            ControllerDelay500 = true;
                        }
                        else
                        {
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.ArrowLeft
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }
                    //Send external arrow right key
                    else if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (keyboardMode == KeyboardMode.Media)
                        {
                            VolumeInputMute();
                            ControllerDelay500 = true;
                        }
                        else
                        {
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.ArrowRight
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }

                    //Mute volume
                    else if (ControllerInput.TriggerLeft > 0 && ControllerInput.TriggerRight > 0)
                    {
                        Debug.WriteLine("Button: TriggerLeft and TriggerRight / Mute");

                        if (keyboardMode == KeyboardMode.Media)
                        {
                            VolumeOutputMute();
                        }

                        ControllerDelay500 = true;
                    }
                    //Switch caps lock
                    else if (ControllerInput.TriggerLeft > 80)
                    {
                        Debug.WriteLine("Button: TriggerLeft / Caps lock");

                        if (border_EmojiListPopup.Visibility == Visibility.Visible)
                        {
                            await SwitchEmojiTypeListTrigger(true);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }
                        else if (keyboardMode == KeyboardMode.Media)
                        {
                            VolumeDown();
                        }
                        else
                        {
                            SwitchCapsLock();
                        }

                        ControllerDelay250 = true;
                    }
                    //Send external tab
                    else if (ControllerInput.TriggerRight > 80)
                    {
                        Debug.WriteLine("Button: TriggerRight / Press Tab");

                        if (border_EmojiListPopup.Visibility == Visibility.Visible)
                        {
                            await SwitchEmojiTypeListTrigger(false);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }
                        else if (keyboardMode == KeyboardMode.Media)
                        {
                            VolumeUp();
                        }
                        else if (vCapsEnabled)
                        {
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Modifiers = KeyboardModifiers.ShiftLeft,
                                Key0 = KeyboardKeys.Tab
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }
                        else
                        {
                            KeyboardAction keyboardAction = new KeyboardAction()
                            {
                                Key0 = KeyboardKeys.Tab
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }

                        ControllerDelay250 = true;
                    }

                    //Show hide text emoji popup
                    else if (ControllerInput.ButtonBack.PressedRaw)
                    {
                        Debug.WriteLine("Button: BackPressed / Show hide text emoji popup");

                        if (keyboardMode == KeyboardMode.Media)
                        {
                            MediaFullscreen();
                        }
                        else if (keyboardMode == KeyboardMode.Keyboard)
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                if (vLastPopupListType == "Text")
                                {
                                    await ShowHideTextListPopup();
                                }
                                else if (vLastPopupListType == "Shortcut")
                                {
                                    await ShowHideShortcutListPopup();
                                }
                                else
                                {
                                    await ShowHideEmojiListPopup();
                                }
                            });
                        }

                        ControllerDelay250 = true;
                    }
                    //Switch keyboard mode
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        Debug.WriteLine("Button: StartPressed / Switch keyboard mode");
                        await SwitchKeyboardMode();

                        ControllerDelay250 = true;
                    }

                    //Delay input to prevent repeat
                    if (ControllerDelay125)
                    {
                        vControllerDelay_Keyboard = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay250)
                    {
                        vControllerDelay_Keyboard = GetSystemTicksMs() + vControllerDelayTicks250;
                    }
                    else if (ControllerDelay500)
                    {
                        vControllerDelay_Keyboard = GetSystemTicksMs() + vControllerDelayTicks500;
                    }
                }
            }
            catch { }
        }
    }
}