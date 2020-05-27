using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput.Keypad
{
    partial class WindowKeypad
    {
        //Process controller input for keyboard
        public async Task<bool> ControllerInteractionKeyboard(ControllerInput controllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayMicro = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            bool ControllerDelayLong = false;
            try
            {
                //Update interface controller preview
                if (Environment.TickCount >= vControllerDelay_KeypadPreview)
                {
                    ControllerPreview(controllerInput);

                    vControllerDelay_KeypadPreview = Environment.TickCount + vControllerDelayNanoTicks;
                }

                //Press arrow left key
                if (controllerInput.DPadLeft.PressedRaw)
                {
                    await KeyPressSingle((byte)KeysVirtual.Left, false);

                    ControllerUsed = true;
                    ControllerDelayShort = true;
                }
                //Press arrow right key
                else if (controllerInput.DPadRight.PressedRaw)
                {
                    await KeyPressSingle((byte)KeysVirtual.Right, false);

                    ControllerUsed = true;
                    ControllerDelayShort = true;
                }
                //Press arrow up key
                else if (controllerInput.DPadUp.PressedRaw)
                {
                    await KeyPressSingle((byte)KeysVirtual.Up, false);

                    ControllerUsed = true;
                    ControllerDelayShort = true;
                }
                //Press arrow down key
                else if (controllerInput.DPadDown.PressedRaw)
                {
                    await KeyPressSingle((byte)KeysVirtual.Down, false);

                    ControllerUsed = true;
                    ControllerDelayShort = true;
                }

                //Press button a key
                else if (controllerInput.ButtonA.PressedRaw)
                {
                    await KeyPressSingle((byte)KeysVirtual.Control, false);

                    ControllerUsed = true;
                    ControllerDelayShort = true;
                }
                //Press button b key
                else if (controllerInput.ButtonB.PressedRaw)
                {
                    await KeyPressSingle((byte)KeysVirtual.Menu, false);

                    ControllerUsed = true;
                    ControllerDelayShort = true;
                }
                //Press button y key
                else if (controllerInput.ButtonY.PressedRaw)
                {
                    await KeyPressCombo((byte)KeysVirtual.Control, (byte)KeysVirtual.Menu, false);

                    ControllerUsed = true;
                    ControllerDelayShort = true;
                }
                //Press button x key
                else if (controllerInput.ButtonX.PressedRaw)
                {
                    await KeyPressSingle((byte)KeysVirtual.Space, false);

                    ControllerUsed = true;
                    ControllerDelayShort = true;
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
            catch { }
            return ControllerUsed;
        }
    }
}