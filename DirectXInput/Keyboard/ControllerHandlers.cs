using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInputOutputMouse;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Process controller input for mouse
        public async Task ControllerInteractionMouse(ControllerInput ControllerInput)
        {
            bool ControllerDelayMicro = false;
            bool ControllerDelayShort = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Mouse)
                {
                    //Get the mouse move amount
                    int moveSensitivity = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MouseMoveSensitivity"));
                    GetMouseMovementAmountFromThumb(moveSensitivity, ControllerInput.ThumbLeftX, ControllerInput.ThumbLeftY, true, out int moveHorizontalLeft, out int moveVerticalLeft);

                    //Move the mouse cursor
                    MouseMoveCursor(moveHorizontalLeft, moveVerticalLeft);

                    if (Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "KeyboardMode")) == 0)
                    {
                        //Get the mouse move amount
                        GetMouseMovementAmountFromThumb(moveSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, true, out int moveHorizontalRight, out int moveVerticalRight);

                        //Move the keyboard window
                        MoveKeyboardWindow(moveHorizontalRight, moveVerticalRight);
                    }
                    else
                    {
                        //Get the mouse scroll amount
                        int scrollSensitivity = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "MouseScrollSensitivity"));
                        GetMouseMovementAmountFromThumb(scrollSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, false, out int scrollHorizontalRight, out int scrollVerticalRight);

                        //Scroll the mouse wheel
                        MouseScrollWheel(scrollHorizontalRight, scrollVerticalRight);
                    }

                    //Emulate mouse click left
                    if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        if (!vMouseDownStatus)
                        {
                            vMouseDownStatus = true;
                            MouseToggle(false, true);

                            ControllerDelayMicro = true;
                        }
                    }
                    else if (vMouseDownStatus)
                    {
                        vMouseDownStatus = false;
                        MouseToggle(false, false);

                        ControllerDelayMicro = true;
                    }

                    //Emulate mouse click right
                    if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        await MousePress(true);

                        ControllerDelayShort = true;
                    }

                    if (ControllerDelayMicro)
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayMicroTicks;
                    }
                    else if (ControllerDelayShort)
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayShortTicks;
                    }
                    else
                    {
                        vControllerDelay_Mouse = GetSystemTicksMs() + vControllerDelayNanoTicks;
                    }
                }
            }
            catch { }
        }

        //Process controller input for keyboard
        public async Task ControllerInteractionKeyboard(ControllerInput ControllerInput)
        {
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Keyboard)
                {
                    //Send internal arrow left key
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Left, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Right, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Up, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle(KeysVirtual.Down, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }

                    //Send internal space key
                    else if (ControllerInput.ButtonA.PressedRaw)
                    {
                        await KeySendSingle(KeysVirtual.Space, vInteropWindowHandle);

                        ControllerDelayShort = true;
                    }
                    //Send external enter key
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingleAuto(KeysVirtual.Enter);

                        ControllerDelayShort = true;
                    }
                    //Send external space key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingleAuto(KeysVirtual.Space);

                        ControllerDelayShort = true;
                    }
                    //Send external backspace key
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingleAuto(KeysVirtual.BackSpace);

                        ControllerDelayShort = true;
                    }

                    //Send external arrow left key
                    else if (ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingleAuto(KeysVirtual.Left);

                        ControllerDelayShort = true;
                    }
                    //Send external arrow right key
                    else if (ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingleAuto(KeysVirtual.Right);

                        ControllerDelayShort = true;
                    }

                    //Switch caps lock
                    else if (ControllerInput.TriggerLeft > 0)
                    {
                        Debug.WriteLine("Button: TriggerLeft / Caps lock");

                        if (border_EmojiListPopup.Visibility == Visibility.Visible)
                        {
                            SwitchEmojiTypeListTrigger(true);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        }
                        else
                        {
                            await SwitchCapsLock();
                        }

                        ControllerDelayMedium = true;
                    }
                    //Send external tab
                    else if (ControllerInput.TriggerRight > 0)
                    {
                        Debug.WriteLine("Button: TriggerRight / Press Tab");

                        if (vCapsEnabled)
                        {
                            await KeyPressComboAuto(KeysVirtual.Shift, KeysVirtual.Tab);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        }
                        else if (border_EmojiListPopup.Visibility == Visibility.Visible)
                        {
                            SwitchEmojiTypeListTrigger(false);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        }
                        else
                        {
                            await KeyPressSingleAuto(KeysVirtual.Tab);
                            PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        }

                        ControllerDelayMedium = true;
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

                        ControllerDelayMedium = true;
                    }
                    //Switch scroll and move
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        Debug.WriteLine("Button: StartPressed / Scroll and Move");
                        SwitchKeyboardMode();

                        ControllerDelayMedium = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Keyboard = GetSystemTicksMs() + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayMedium)
                    {
                        vControllerDelay_Keyboard = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    }
                }
            }
            catch { }
        }
    }
}