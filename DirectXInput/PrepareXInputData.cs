using System;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //65535 = 0xFFFF Hex = 255 * 257 Int
        byte TranslateByte_0xFF(int RawOffset, int RawJoyState) { return Convert.ToByte((RawJoyState >> RawOffset) & 0xFF); }
        byte TranslateByte_0x0F(int RawOffset, int RawJoyState) { return Convert.ToByte((RawJoyState >> RawOffset) & 0x0F); }
        byte TranslateByte_0x10(int RawOffset, int RawJoyState) { return Convert.ToByte((RawJoyState >> RawOffset) & 0x10); }

        //Update and prepare XInput byte data
        void PrepareXInputData(ControllerStatus Controller, bool Empty)
        {
            try
            {
                //Set the package header
                Controller.XInputData[0] = 0x1C;
                Controller.XInputData[4] = (byte)(Controller.NumberId + 1);
                Controller.XInputData[9] = 0x14;
                for (int i = 10; i < Controller.XInputData.Length; i++) { Controller.XInputData[i] = 0x00; }
                if (Empty) { return; }

                //Thumb Left
                Controller.XInputData[14] = TranslateByte_0xFF(0, Controller.InputCurrent.ThumbLeftX);
                Controller.XInputData[15] = TranslateByte_0xFF(8, Controller.InputCurrent.ThumbLeftX);
                Controller.XInputData[16] = TranslateByte_0xFF(0, Controller.InputCurrent.ThumbLeftY);
                Controller.XInputData[17] = TranslateByte_0xFF(8, Controller.InputCurrent.ThumbLeftY);

                //Thumb Right
                Controller.XInputData[18] = TranslateByte_0xFF(0, Controller.InputCurrent.ThumbRightX);
                Controller.XInputData[19] = TranslateByte_0xFF(8, Controller.InputCurrent.ThumbRightX);
                Controller.XInputData[20] = TranslateByte_0xFF(0, Controller.InputCurrent.ThumbRightY);
                Controller.XInputData[21] = TranslateByte_0xFF(8, Controller.InputCurrent.ThumbRightY);

                //Triggers
                if (!Controller.Details.Profile.UseButtonTriggers)
                {
                    Controller.XInputData[12] = Controller.InputCurrent.TriggerLeft;
                    Controller.XInputData[13] = Controller.InputCurrent.TriggerRight;
                }
                else
                {
                    if (Controller.InputCurrent.ButtonTriggerLeft.PressedRaw) { Controller.XInputData[12] = 255; }
                    if (Controller.InputCurrent.ButtonTriggerRight.PressedRaw) { Controller.XInputData[13] = 255; }
                }

                //D-Pad
                if (Controller.InputCurrent.DPadLeft.PressedRaw) { Controller.XInputData[10] |= (byte)(1 << 2); }
                if (Controller.InputCurrent.DPadUp.PressedRaw) { Controller.XInputData[10] |= (byte)(1 << 0); }
                if (Controller.InputCurrent.DPadRight.PressedRaw) { Controller.XInputData[10] |= (byte)(1 << 3); }
                if (Controller.InputCurrent.DPadDown.PressedRaw) { Controller.XInputData[10] |= (byte)(1 << 1); }

                //Buttons
                if (Controller.InputCurrent.ButtonA.PressedRaw) { Controller.XInputData[11] |= (byte)(1 << 4); }
                if (Controller.InputCurrent.ButtonB.PressedRaw) { Controller.XInputData[11] |= (byte)(1 << 5); }
                if (Controller.InputCurrent.ButtonX.PressedRaw) { Controller.XInputData[11] |= (byte)(1 << 6); }
                if (Controller.InputCurrent.ButtonY.PressedRaw) { Controller.XInputData[11] |= (byte)(1 << 7); }

                if (Controller.InputCurrent.ButtonBack.PressedRaw) { Controller.XInputData[10] |= (byte)(1 << 5); }
                if (Controller.InputCurrent.ButtonStart.PressedRaw) { Controller.XInputData[10] |= (byte)(1 << 4); }
                if (Controller.InputCurrent.ButtonGuide.PressedRaw) { Controller.XInputData[11] |= (byte)(1 << 2); }

                if (Controller.InputCurrent.ButtonShoulderLeft.PressedRaw) { Controller.XInputData[11] |= (byte)(1 << 0); }
                if (Controller.InputCurrent.ButtonShoulderRight.PressedRaw) { Controller.XInputData[11] |= (byte)(1 << 1); }

                if (Controller.InputCurrent.ButtonThumbLeft.PressedRaw) { Controller.XInputData[10] |= (byte)(1 << 6); }
                if (Controller.InputCurrent.ButtonThumbRight.PressedRaw) { Controller.XInputData[10] |= (byte)(1 << 7); }
            }
            catch { }
        }
    }
}