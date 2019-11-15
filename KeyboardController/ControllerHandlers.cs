using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using static KeyboardController.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.OutputKeyboard;
using static LibraryShared.OutputMouse;

namespace KeyboardController
{
    partial class WindowMain
    {
        //Process controller input for mouse
        public bool ControllerInteractionMouse(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayMicro = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            bool ControllerDelayLong = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Mouse)
                {
                    //Emulate mouse movement
                    MouseMovementThumb(ControllerInput.ThumbLeftX, ControllerInput.ThumbLeftY);

                    //Emulate mouse scrolling or move the keyboard window
                    if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardMode"]) == 0)
                    {
                        MoveKeyboardWindow(ControllerInput.ThumbRightX, ControllerInput.ThumbRightY);
                    }
                    else
                    {
                        MouseWheelScrollingThumb(ControllerInput.ThumbRightX, ControllerInput.ThumbRightY);
                    }

                    //Emulate mouse click left
                    if (ControllerInput.ButtonThumbLeft)
                    {
                        if (!vMouseHoldingLeft)
                        {
                            MouseToggle(false, true);

                            vMouseHoldingLeft = true;
                            ControllerUsed = true;
                            ControllerDelayMicro = true;
                        }
                    }
                    else
                    {
                        if (vMouseHoldingLeft)
                        {
                            MouseToggle(false, false);

                            vMouseHoldingLeft = false;
                            ControllerUsed = true;
                            ControllerDelayMicro = true;
                        }
                    }

                    //Emulate mouse click right
                    if (ControllerInput.ButtonThumbRight)
                    {
                        MousePressSingle(true);

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
                    else if (ControllerDelayMedium)
                    {
                        vControllerDelay_Mouse = Environment.TickCount + vControllerDelayMediumTicks;
                    }
                    else if (ControllerDelayLong)
                    {
                        vControllerDelay_Mouse = Environment.TickCount + vControllerDelayLongTicks;
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
            bool ControllerDelayMicro = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            bool ControllerDelayLong = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Keyboard)
                {
                    //Send internal arrow left key
                    if (ControllerInput.DPadLeft)
                    {
                        PlayInterfaceSound("KeyboardMove", false);
                        KeySendSingle((byte)KeysVirtual.Left, Process.GetCurrentProcess().MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send internal arrow right key
                    else if (ControllerInput.DPadRight)
                    {
                        PlayInterfaceSound("KeyboardMove", false);
                        KeySendSingle((byte)KeysVirtual.Right, Process.GetCurrentProcess().MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send internal arrow up key
                    else if (ControllerInput.DPadUp)
                    {
                        PlayInterfaceSound("KeyboardMove", false);
                        KeySendSingle((byte)KeysVirtual.Up, Process.GetCurrentProcess().MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send internal arrow down key
                    else if (ControllerInput.DPadDown)
                    {
                        PlayInterfaceSound("KeyboardMove", false);
                        KeySendSingle((byte)KeysVirtual.Down, Process.GetCurrentProcess().MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Send internal space key
                    else if (ControllerInput.ButtonA)
                    {
                        KeySendSingle((byte)KeysVirtual.Space, Process.GetCurrentProcess().MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external enter key
                    else if (ControllerInput.ButtonB)
                    {
                        PlayInterfaceSound("KeyboardPress", false);
                        KeyPressSingle((byte)KeysVirtual.Return, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external space key
                    else if (ControllerInput.ButtonY)
                    {
                        PlayInterfaceSound("KeyboardPress", false);
                        KeyPressSingle((byte)KeysVirtual.Space, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external backspace key
                    else if (ControllerInput.ButtonX)
                    {
                        PlayInterfaceSound("KeyboardPress", false);
                        KeyPressSingle((byte)KeysVirtual.Back, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Send external arrow left key
                    else if (ControllerInput.ButtonShoulderLeft)
                    {
                        PlayInterfaceSound("KeyboardPress", false);
                        KeyPressSingle((byte)KeysVirtual.Left, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external arrow right key
                    else if (ControllerInput.ButtonShoulderRight)
                    {
                        PlayInterfaceSound("KeyboardPress", false);
                        KeyPressSingle((byte)KeysVirtual.Right, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Send external shift+tab
                    else if (ControllerInput.TriggerLeft > 0)
                    {
                        PlayInterfaceSound("KeyboardPress", false);
                        KeyPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    //Send external tab
                    else if (ControllerInput.TriggerRight > 0)
                    {
                        PlayInterfaceSound("KeyboardPress", false);
                        KeyPressSingle((byte)KeysVirtual.Tab, false);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Switch scroll and move
                    else if (ControllerInput.ButtonBack)
                    {
                        Debug.WriteLine("Button: BackPressed / Caps lock");
                        SwitchCapsLock();

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    //Switch caps lock
                    else if (ControllerInput.ButtonStart)
                    {
                        Debug.WriteLine("Button: StartPressed / Scroll and Move");
                        SwitchKeyboardMode();

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    //Close keyboard
                    else if (ControllerInput.ButtonGuideShort || ControllerInput.ButtonGuideLong)
                    {
                        Debug.WriteLine("Button: GuideShort / Close");
                        await Application_Exit();

                        ControllerUsed = true;
                        ControllerDelayLong = true;
                    }

                    if (ControllerDelayMicro)
                    {
                        vControllerDelay_Keyboard = Environment.TickCount + vControllerDelayMicroTicks;
                    }
                    else if (ControllerDelayShort)
                    {
                        vControllerDelay_Keyboard = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayMedium)
                    {
                        vControllerDelay_Keyboard = Environment.TickCount + vControllerDelayMediumTicks;
                    }
                    else if (ControllerDelayLong)
                    {
                        vControllerDelay_Keyboard = Environment.TickCount + vControllerDelayLongTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }
    }
}