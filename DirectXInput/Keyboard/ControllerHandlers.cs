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
using static LibraryShared.ControllerTimings;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Process controller input for mouse
        public void ControllerInteractionMouse(ControllerInput ControllerInput)
        {
            try
            {
                //Set mouse movement
                long moveMouseDelay = 0;
                int moveVerticalLeft = 0;
                int moveVerticalRight = 0;
                int moveHorizontalLeft = 0;
                int moveHorizontalRight = 0;
                if (GetSystemTicksMs() >= vControllerDelay_MouseMove)
                {
                    //Set default mouse delay
                    moveMouseDelay = vControllerDelayTicks10;

                    //Get mouse move sensitivity
                    double moveSensitivity = SettingLoad(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity", typeof(double));

                    //Get mouse move amount
                    GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbLeftX, ControllerInput.ThumbLeftY, true, out moveHorizontalLeft, out moveVerticalLeft);

                    //Check keyboard mode
                    if (vKeyboardCurrentMode == KeyboardMode.Media)
                    {
                        //Get window move amount
                        GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, true, out moveHorizontalRight, out moveVerticalRight);

                        //Move keyboard window
                        MoveKeyboardWindow(moveHorizontalRight, moveVerticalRight);
                    }

                    //Delay input to prevent repeat
                    vControllerDelay_MouseMove = GetSystemTicksMs() + moveMouseDelay;
                }

                //Set mouse button
                long buttonMouseDelay = 0;
                MouseHidButtons buttonPressed = MouseHidButtons.None;
                if (GetSystemTicksMs() >= vControllerDelay_MouseButton)
                {
                    //Set default mouse delay
                    buttonMouseDelay = vControllerDelayTicks10;

                    //Check mouse button press
                    if (ControllerInput.Buttons[(byte)ControllerButtons.ShoulderLeft].PressedRaw && ControllerInput.Buttons[(byte)ControllerButtons.ShoulderRight].PressedRaw)
                    {
                        buttonPressed = MouseHidButtons.MiddleButton;
                        buttonMouseDelay = vControllerDelayTicks500;
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ShoulderLeft].PressedRaw)
                    {
                        buttonPressed = MouseHidButtons.LeftButton;
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ShoulderRight].PressedRaw)
                    {
                        buttonPressed = MouseHidButtons.RightButton;
                    }

                    //Delay input to prevent repeat
                    vControllerDelay_MouseButton = GetSystemTicksMs() + buttonMouseDelay;
                }

                //Set mouse scroll
                long scrollMouseDelay = 0;
                int scrollVerticalRight = 0;
                int scrollHorizontalRight = 0;
                if (GetSystemTicksMs() >= vControllerDelay_MouseScroll)
                {
                    //Check keyboard mode
                    if (vKeyboardCurrentMode == KeyboardMode.Keyboard)
                    {
                        //Get the mouse scroll amount
                        int scrollSensitivity = SettingLoad(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity2", typeof(int));
                        GetMouseMovementAmountFromThumbScroll(scrollSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, false, out scrollHorizontalRight, out scrollVerticalRight);
                        if (scrollHorizontalRight != 0 || scrollVerticalRight != 0)
                        {
                            scrollMouseDelay = 125;
                        }

                        //Delay input to prevent repeat
                        vControllerDelay_MouseScroll = GetSystemTicksMs() + scrollMouseDelay;
                    }
                }

                //Update current mouse input
                if (moveMouseDelay != 0 || buttonMouseDelay != 0 || scrollMouseDelay != 0)
                {
                    MouseHidAction mouseAction = new MouseHidAction()
                    {
                        MoveHorizontal = moveHorizontalLeft,
                        MoveVertical = moveVerticalLeft,
                        ScrollHorizontal = scrollHorizontalRight,
                        ScrollVertical = scrollVerticalRight,
                        Button = buttonPressed
                    };
                    vFakerInputDevice.MouseRelative(mouseAction);
                }
            }
            catch { }
        }

        //Process controller input for keyboard
        public async Task ControllerInteractionKeyboard(ControllerInput ControllerInput)
        {
            bool ControllerDelay30 = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay250 = false;
            bool ControllerDelay500 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Keyboard)
                {
                    //Send internal arrow left key
                    if (ControllerInput.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw)
                    {
                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowLeft
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);

                            ControllerDelay125 = true;
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

                                //Check navigation delay
                                if (ControllerInput.Buttons[(byte)ControllerButtons.DPadLeft].PressTimeCurrent > vControllerButtonPressTap)
                                {
                                    ControllerDelay30 = true;
                                }
                                else
                                {
                                    ControllerDelay125 = true;
                                }
                            }
                        }
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw)
                    {
                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowRight
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);

                            ControllerDelay125 = true;
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

                                //Check navigation delay
                                if (ControllerInput.Buttons[(byte)ControllerButtons.DPadRight].PressTimeCurrent > vControllerButtonPressTap)
                                {
                                    ControllerDelay30 = true;
                                }
                                else
                                {
                                    ControllerDelay125 = true;
                                }
                            }
                        }
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw)
                    {
                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowUp
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);

                            ControllerDelay125 = true;
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

                                //Check navigation delay
                                if (ControllerInput.Buttons[(byte)ControllerButtons.DPadUp].PressTimeCurrent > vControllerButtonPressTap)
                                {
                                    ControllerDelay30 = true;
                                }
                                else
                                {
                                    ControllerDelay125 = true;
                                }
                            }
                        }
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw)
                    {
                        if (vKeyboardCurrentMode == KeyboardMode.Media)
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                            KeysHidAction keyboardAction = new KeysHidAction()
                            {
                                Key0 = KeysHid.ArrowDown
                            };
                            vFakerInputDevice.KeyboardPressRelease(keyboardAction);

                            ControllerDelay125 = true;
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

                                //Check navigation delay
                                if (ControllerInput.Buttons[(byte)ControllerButtons.DPadDown].PressTimeCurrent > vControllerButtonPressTap)
                                {
                                    ControllerDelay30 = true;
                                }
                                else
                                {
                                    ControllerDelay125 = true;
                                }
                            }
                        }
                    }

                    //Send internal space key
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.A].PressedRaw)
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
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.B].PressedRaw)
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
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.Y].PressedRaw)
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
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.X].PressedRaw)
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
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeft].PressedRaw)
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
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbRight].PressedRaw)
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
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.Back].PressedRaw)
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
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.Start].PressedRaw)
                    {
                        Debug.WriteLine("Button: StartPressed / Switch keyboard mode");
                        await SwitchKeyboardMode();

                        ControllerDelay250 = true;
                    }

                    //Delay input to prevent repeat
                    if (ControllerDelay30)
                    {
                        vControllerDelay_Keyboard = GetSystemTicksMs() + vControllerDelayTicks30;
                    }
                    else if (ControllerDelay125)
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