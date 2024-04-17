using LibraryUsb;
using System;
using static LibraryShared.Classes;
using static LibraryUsb.VigemBusDevice;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //65535 = 0xFFFF Hex = 255 * 257 Int
        byte TranslateByte_0xFF(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0xFF); }
        byte TranslateByte_0x0F(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0x0F); }
        byte TranslateByte_0x10(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0x10); }

        //Lookup button group offset
        int LookupButtonGroupOffset(int targetGroup, ControllerSupported controllerSupported)
        {
            try
            {
                switch (targetGroup)
                {
                    case 1: return (int)controllerSupported.OffsetHeader.ButtonsGroup1;
                    case 2: return (int)controllerSupported.OffsetHeader.ButtonsGroup2;
                    case 3: return (int)controllerSupported.OffsetHeader.ButtonsGroup3;
                }
            }
            catch { }
            return 0;
        }

        //Update and prepare XInput byte data
        void PrepareXInputDataEmpty(ControllerStatus controller)
        {
            try
            {
                //Set byte array
                controller.VirtualDataInput = new byte[(int)ByteArraySizes.Input];
                controller.VirtualDataInput[0] = (byte)ByteArraySizes.Input; //Size
                controller.VirtualDataInput[4] = (byte)(controller.NumberId + 1); //SerialNo
            }
            catch { }
        }

        //Update and prepare XInput byte data
        void PrepareXInputDataCurrent(ControllerStatus controller)
        {
            try
            {
                //Set byte array
                controller.VirtualDataInput = new byte[(int)ByteArraySizes.Input];
                controller.VirtualDataInput[0] = (byte)ByteArraySizes.Input; //Size
                controller.VirtualDataInput[4] = (byte)(controller.NumberId + 1); //SerialNo

                //Thumb Left
                controller.VirtualDataInput[12] = TranslateByte_0xFF(0, controller.InputCurrent.ThumbLeftX);
                controller.VirtualDataInput[13] = TranslateByte_0xFF(8, controller.InputCurrent.ThumbLeftX);
                controller.VirtualDataInput[14] = TranslateByte_0xFF(0, controller.InputCurrent.ThumbLeftY);
                controller.VirtualDataInput[15] = TranslateByte_0xFF(8, controller.InputCurrent.ThumbLeftY);

                //Thumb Right
                controller.VirtualDataInput[16] = TranslateByte_0xFF(0, controller.InputCurrent.ThumbRightX);
                controller.VirtualDataInput[17] = TranslateByte_0xFF(8, controller.InputCurrent.ThumbRightX);
                controller.VirtualDataInput[18] = TranslateByte_0xFF(0, controller.InputCurrent.ThumbRightY);
                controller.VirtualDataInput[19] = TranslateByte_0xFF(8, controller.InputCurrent.ThumbRightY);

                //Triggers
                if (!controller.Details.Profile.UseButtonTriggers)
                {
                    controller.VirtualDataInput[10] = controller.InputCurrent.TriggerLeft;
                    controller.VirtualDataInput[11] = controller.InputCurrent.TriggerRight;
                }
                else
                {
                    if (controller.InputCurrent.ButtonTriggerLeft.PressedRaw) { controller.VirtualDataInput[10] = 255; }
                    if (controller.InputCurrent.ButtonTriggerRight.PressedRaw) { controller.VirtualDataInput[11] = 255; }
                }

                //DPad
                if (controller.InputCurrent.DPadLeft.PressedRaw) { controller.VirtualDataInput[8] |= (1 << 2); }
                if (controller.InputCurrent.DPadUp.PressedRaw) { controller.VirtualDataInput[8] |= (1 << 0); }
                if (controller.InputCurrent.DPadRight.PressedRaw) { controller.VirtualDataInput[8] |= (1 << 3); }
                if (controller.InputCurrent.DPadDown.PressedRaw) { controller.VirtualDataInput[8] |= (1 << 1); }

                //Buttons
                if (controller.InputCurrent.ButtonBack.PressedRaw) { controller.VirtualDataInput[8] |= (1 << 5); }
                if (controller.InputCurrent.ButtonStart.PressedRaw) { controller.VirtualDataInput[8] |= (1 << 4); }
                if (controller.InputCurrent.ButtonThumbLeft.PressedRaw) { controller.VirtualDataInput[8] |= (1 << 6); }
                if (controller.InputCurrent.ButtonThumbRight.PressedRaw) { controller.VirtualDataInput[8] |= (1 << 7); }
                if (controller.InputCurrent.ButtonShoulderLeft.PressedRaw) { controller.VirtualDataInput[9] |= (1 << 0); }
                if (controller.InputCurrent.ButtonShoulderRight.PressedRaw) { controller.VirtualDataInput[9] |= (1 << 1); }
                if (controller.InputCurrent.ButtonGuide.PressedRaw) { controller.VirtualDataInput[9] |= (1 << 2); }
                if (controller.InputCurrent.ButtonA.PressedRaw) { controller.VirtualDataInput[9] |= (1 << 4); }
                if (controller.InputCurrent.ButtonB.PressedRaw) { controller.VirtualDataInput[9] |= (1 << 5); }
                if (controller.InputCurrent.ButtonX.PressedRaw) { controller.VirtualDataInput[9] |= (1 << 6); }
                if (controller.InputCurrent.ButtonY.PressedRaw) { controller.VirtualDataInput[9] |= (1 << 7); }
            }
            catch { }
        }
    }
}