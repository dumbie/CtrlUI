using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInputOutputMouse;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.Keyboard
{
    partial class WindowKeyboard
    {
        //Process controller input for mouse
        public async Task<bool> ControllerInteractionMouse(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayMicro = false;
            bool ControllerDelayShort = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Mouse)
                {
                    //Get the mouse move amount
                    int moveSensitivity = Convert.ToInt32(ConfigurationManager.AppSettings["MouseMoveSensitivity"]);
                    GetMouseMovementAmountFromThumb(moveSensitivity, ControllerInput.ThumbLeftX, ControllerInput.ThumbLeftY, true, out int moveHorizontalLeft, out int moveVerticalLeft);

                    //Move the mouse cursor
                    MouseMoveCursor(moveHorizontalLeft, moveVerticalLeft);

                    if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardMode"]) == 0)
                    {
                        //Get the mouse move amount
                        GetMouseMovementAmountFromThumb(moveSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, true, out int moveHorizontalRight, out int moveVerticalRight);

                        //Move the keyboard window
                        MoveKeyboardWindow(moveHorizontalRight, moveVerticalRight);
                    }
                    else
                    {
                        //Get the mouse scroll amount
                        int scrollSensitivity = Convert.ToInt32(ConfigurationManager.AppSettings["MouseScrollSensitivity"]);
                        GetMouseMovementAmountFromThumb(scrollSensitivity, ControllerInput.ThumbRightX, ControllerInput.ThumbRightY, false, out int scrollHorizontalRight, out int scrollVerticalRight);

                        //Scroll the mouse wheel
                        MouseScrollWheel(scrollHorizontalRight, scrollVerticalRight);
                    }

                    //Emulate mouse click left
                    if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        if (!vMouseStatus.Contains(MouseVirtual.MOUSEEVENTF_LEFTDOWN))
                        {
                            vMouseStatus.Add(MouseVirtual.MOUSEEVENTF_LEFTDOWN);
                            MouseToggle(false, true);

                            ControllerUsed = true;
                            ControllerDelayMicro = true;
                        }
                    }
                    else
                    {
                        if (vMouseStatus.Contains(MouseVirtual.MOUSEEVENTF_LEFTDOWN))
                        {
                            vMouseStatus.RemoveAll(x => x == MouseVirtual.MOUSEEVENTF_LEFTDOWN);
                            MouseToggle(false, false);

                            ControllerUsed = true;
                            ControllerDelayMicro = true;
                        }
                    }

                    //Emulate mouse click right
                    if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        await MousePress(true);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    if (ControllerDelayMicro)
                    {
                        vControllerDelay_Mouse = Environment.TickCount + vControllerDelayMicroTicks;
                    }
                    else if (ControllerDelayShort)
                    {
                        vControllerDelay_Mouse = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else
                    {
                        vControllerDelay_Mouse = Environment.TickCount + vControllerDelayNanoTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process controller input for keyboard
        public async Task<bool> ControllerInteractionKeyboard(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Keyboard)
                {
                    //Send internal arrow left key
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle((byte)KeysVirtual.Left, vInteropWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle((byte)KeysVirtual.Right, vInteropWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle((byte)KeysVirtual.Up, vInteropWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Move", false);
                        await KeySendSingle((byte)KeysVirtual.Down, vInteropWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Send internal space key
                    else if (ControllerInput.ButtonA.PressedRaw)
                    {
                        await KeySendSingle((byte)KeysVirtual.Space, vInteropWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external enter key
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingle((byte)KeysVirtual.Enter, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external space key
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingle((byte)KeysVirtual.Space, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external backspace key
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingle((byte)KeysVirtual.BackSpace, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Send external arrow left key
                    else if (ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingle((byte)KeysVirtual.Left, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external arrow right key
                    else if (ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingle((byte)KeysVirtual.Right, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Send external shift+tab
                    else if (ControllerInput.TriggerLeft > 0)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external tab
                    else if (ControllerInput.TriggerRight > 0)
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);
                        await KeyPressSingle((byte)KeysVirtual.Tab, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Switch scroll and move
                    else if (ControllerInput.ButtonBack.PressedRaw)
                    {
                        Debug.WriteLine("Button: BackPressed / Caps lock");
                        await SwitchCapsLock();

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    //Switch caps lock
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        Debug.WriteLine("Button: StartPressed / Scroll and Move");
                        SwitchKeyboardMode();

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Keyboard = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayMedium)
                    {
                        vControllerDelay_Keyboard = Environment.TickCount + vControllerDelayMediumTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }
    }
}