using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

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
                    MouseHidButtons buttonPress = MouseHidButtons.None;

                    //Get the mouse move amount
                    double moveSensitivity = SettingLoad(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity", typeof(double));
                    GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbLeftX, ControllerInput.ThumbLeftY, true, out int moveHorizontalLeft, out int moveVerticalLeft);

                    //Check the keyboard mode
                    if (vKeyboardCurrentMode == KeyboardMode.Media)
                    {
                        //Get the mouse move amount
                        GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, true, out int moveHorizontalRight, out int moveVerticalRight);

                        //Move the keyboard window
                        MoveKeyboardWindow(moveHorizontalRight, moveVerticalRight);
                    }
                    else if (vKeyboardCurrentMode == KeyboardMode.Keyboard)
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
                        buttonPress = MouseHidButtons.MiddleButton;
                        ControllerDelay500 = true;
                    }
                    else if (ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        buttonPress = MouseHidButtons.LeftButton;
                    }
                    else if (ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        buttonPress = MouseHidButtons.RightButton;
                    }
                    else
                    {
                        buttonPress = MouseHidButtons.None;
                    }

                    //Update current mouse input
                    MouseHidAction mouseAction = new MouseHidAction()
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
                    //Send internal arrow left key
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowLeft
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                        }
                        else
                        {
                            //Check the foreground window
                            if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                            {
                                //Play interface sound
                                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);

                                //Update window style
                                WindowUpdateStyle(vInteropWindowHandle, true, true, false);

                                //Check keyboard focus
                                FocusCheckKeyboard(this, vInteropWindowHandle);

                                //Send arrow left to window
                                KeySendSingle(KeysVirtual.ArrowLeft, vInteropWindowHandle);
                            }
                        }
                        ControllerDelay125 = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowRight
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                        }
                        else
                        {
                            //Check the foreground window
                            if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                            {
                                //Play interface sound
                                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);

                                //Update window style
                                WindowUpdateStyle(vInteropWindowHandle, true, true, false);

                                //Check keyboard focus
                                FocusCheckKeyboard(this, vInteropWindowHandle);

                                //Send arrow right to window
                                KeySendSingle(KeysVirtual.ArrowRight, vInteropWindowHandle);
                            }
                        }
                        ControllerDelay125 = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowUp
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                        }
                        else
                        {
                            //Check the foreground window
                            if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                            {
                                //Play interface sound
                                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);

                                //Update window style
                                WindowUpdateStyle(vInteropWindowHandle, true, true, false);

                                //Check keyboard focus
                                FocusCheckKeyboard(this, vInteropWindowHandle);

                                //Send arrow up to window
                                KeySendSingle(KeysVirtual.ArrowUp, vInteropWindowHandle);
                            }
                        }
                        ControllerDelay125 = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowDown
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                        }
                        else
                        {
                            //Check the foreground window
                            if (vInteropWindowHandle != vProcessForeground.WindowHandleMain)
                            {
                                //Play interface sound
                                PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);

                                //Update window style
                                WindowUpdateStyle(vInteropWindowHandle, true, true, false);

                                //Check keyboard focus
                                FocusCheckKeyboard(this, vInteropWindowHandle);

                                //Send arrow down to window
                                KeySendSingle(KeysVirtual.ArrowDown, vInteropWindowHandle);
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
                            //Play interface sound
                            PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);

                            //Update window style
                            WindowUpdateStyle(vInteropWindowHandle, true, true, false);

                            //Check keyboard focus
                            FocusCheckKeyboard(this, vInteropWindowHandle);

                            //Send space key to window
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
                        else if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            MediaNext();
                            ControllerDelay250 = true;
                        }
                        else
                        {
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.Enter
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }
                    //Send external space key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            MediaPlayPause();
                            ControllerDelay250 = true;
                        }
                        else
                        {
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.Space
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }
                    //Send external backspace or delete key
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            MediaPrevious();
                            ControllerDelay250 = true;
                        }
                        else if (vCapsEnabled)
                        {
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.Delete
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                        else
                        {
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.BackSpace
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }

                    //Send external arrow left key
                    else if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            VolumeOutputMute();
                            ControllerDelay500 = true;
                        }
                        else
                        {
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowLeft
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }
                    //Send external arrow right key
                    else if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            VolumeInputMute();
                            ControllerDelay500 = true;
                        }
                        else
                        {
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowRight
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            ControllerDelay125 = true;
                        }
                    }

                    //Mute volume
                    else if (ControllerInput.TriggerLeft > 0 && ControllerInput.TriggerRight > 0)
                    {
                        Debug.WriteLine("Button: TriggerLeft and TriggerRight / Mute");

                        if (vKeyboardCurrentMode == KeyboardMode.Media)
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
                        else if (vKeyboardCurrentMode == KeyboardMode.Media)
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
                        else if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            VolumeUp();
                        }
                        else if (vCapsEnabled)
                        {
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Modifiers = KeysModifierHid.ShiftLeft,
                                Key0 = KeysHid.Tab
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }
                        else
                        {
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.Tab
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

                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            MediaFullscreen();
                        }
                        else if (vKeyboardCurrentMode == KeyboardMode.Keyboard)
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