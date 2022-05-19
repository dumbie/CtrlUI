using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;
using static LibraryShared.Settings;
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
                    double moveSensitivity = Convert.ToDouble(Setting_Load(vConfigurationDirectXInput, "KeyboardMouseMoveSensitivity"));
                    GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbLeftX, ControllerInput.ThumbLeftY, true, out int moveHorizontalLeft, out int moveVerticalLeft);

                    //Check the keyboard mode
                    if (Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMode")) == 0)
                    {
                        //Get the mouse move amount
                        GetMouseMovementAmountFromThumbDesktop(moveSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, true, out int moveHorizontalRight, out int moveVerticalRight);

                        //Move the keyboard window
                        MoveKeyboardWindow(moveHorizontalRight, moveVerticalRight);
                    }
                    else
                    {
                        //Get the mouse scroll amount
                        int scrollSensitivity = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMouseScrollSensitivity2"));
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
                    vFakerInputDevice.MouseRelative(moveHorizontalLeft, moveVerticalLeft, scrollHorizontalRight, scrollVerticalRight, buttonPress);

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
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Keyboard)
                {
                    //Send internal arrow left key
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                        KeySendSingle(KeysVirtual.Left, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                        KeySendSingle(KeysVirtual.Right, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                        KeySendSingle(KeysVirtual.Up, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false, false);
                        KeySendSingle(KeysVirtual.Down, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }

                    //Send internal space key
                    else if (ControllerInput.ButtonA.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.Space, vInteropWindowHandle);

                        ControllerDelay125 = true;
                    }
                    //Send external enter key
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.Enter, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelay125 = true;
                    }
                    //Send external space key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.Space, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelay125 = true;
                    }
                    //Send external backspace or delete key
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        if (vCapsEnabled)
                        {
                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.Delete, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                        }
                        else
                        {
                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.Backspace, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                        }

                        ControllerDelay125 = true;
                    }

                    //Send external arrow left key
                    else if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.ArrowLeft, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelay125 = true;
                    }
                    //Send external arrow right key
                    else if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.ArrowRight, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);

                        ControllerDelay125 = true;
                    }

                    //Switch caps lock
                    else if (ControllerInput.TriggerLeft > 0)
                    {
                        Debug.WriteLine("Button: TriggerLeft / Caps lock");

                        if (border_EmojiListPopup.Visibility == Visibility.Visible)
                        {
                            await SwitchEmojiTypeListTrigger(true);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }
                        else
                        {
                            SwitchCapsLock();
                        }

                        ControllerDelay250 = true;
                    }
                    //Send external tab
                    else if (ControllerInput.TriggerRight > 0)
                    {
                        Debug.WriteLine("Button: TriggerRight / Press Tab");

                        if (vCapsEnabled)
                        {
                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.ShiftLeft, KeyboardModifiers.None, KeyboardKeys.Tab, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }
                        else if (border_EmojiListPopup.Visibility == Visibility.Visible)
                        {
                            await SwitchEmojiTypeListTrigger(false);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }
                        else
                        {
                            vFakerInputDevice.KeyboardPressRelease(KeyboardModifiers.None, KeyboardModifiers.None, KeyboardKeys.Tab, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None, KeyboardKeys.None);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                        }

                        ControllerDelay250 = true;
                    }

                    //Show hide text emoji popup
                    else if (ControllerInput.ButtonBack.PressedRaw)
                    {
                        Debug.WriteLine("Button: BackPressed / Show hide text emoji popup");
                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                        {
                            if (vLastPopupListType == "Text")
                            {
                                await ShowHideTextListPopup();
                            }
                            else
                            {
                                await ShowHideEmojiListPopup();
                            }
                        });

                        ControllerDelay250 = true;
                    }
                    //Switch scroll and move
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        Debug.WriteLine("Button: StartPressed / Scroll and Move");
                        SwitchKeyboardMode();

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
                }
            }
            catch { }
        }
    }
}