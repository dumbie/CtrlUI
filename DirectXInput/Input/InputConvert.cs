﻿using LibraryUsb;
using System;
using static ArnoldVinkCode.AVInputOutputClass;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //65535 = 0xFFFF Hex = 255 * 257 Int
        byte TranslateByte_0xFF(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0xFF); }
        byte TranslateByte_0x0F(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0x0F); }
        byte TranslateByte_0x10(int rawOffset, int rawState) { return Convert.ToByte((rawState >> rawOffset) & 0x10); }

        //Update and prepare virtual input data
        void PrepareVirtualInputDataEmpty(ControllerStatus controller)
        {
            try
            {
                //Set byte array
                controller.VirtualDataInput = new byte[(int)VigemBusDevice.ByteArraySizes.Input];
                controller.VirtualDataInput[0] = (byte)VigemBusDevice.ByteArraySizes.Input; //Size
                controller.VirtualDataInput[4] = (byte)(controller.NumberVirtual() + 1); //SerialNo
            }
            catch { }
        }

        //Update and prepare virtual input data
        void PrepareVirtualInputDataCurrent(ControllerStatus controller)
        {
            try
            {
                //Set byte array
                controller.VirtualDataInput = new byte[(int)VigemBusDevice.ByteArraySizes.Input];
                controller.VirtualDataInput[0] = (byte)VigemBusDevice.ByteArraySizes.Input; //Size
                controller.VirtualDataInput[4] = (byte)(controller.NumberVirtual() + 1); //SerialNo

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
                controller.VirtualDataInput[10] = controller.InputCurrent.TriggerLeft;
                controller.VirtualDataInput[11] = controller.InputCurrent.TriggerRight;

                //DPad
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw) { controller.VirtualDataInput[8] |= (1 << 2); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw) { controller.VirtualDataInput[8] |= (1 << 0); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw) { controller.VirtualDataInput[8] |= (1 << 3); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw) { controller.VirtualDataInput[8] |= (1 << 1); }

                //Buttons
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.A].PressedRaw) { controller.VirtualDataInput[9] |= (1 << 4); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.B].PressedRaw) { controller.VirtualDataInput[9] |= (1 << 5); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.X].PressedRaw) { controller.VirtualDataInput[9] |= (1 << 6); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Y].PressedRaw) { controller.VirtualDataInput[9] |= (1 << 7); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Back].PressedRaw) { controller.VirtualDataInput[8] |= (1 << 5); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Start].PressedRaw) { controller.VirtualDataInput[8] |= (1 << 4); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Guide].PressedRaw) { controller.VirtualDataInput[9] |= (1 << 2); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ShoulderLeft].PressedRaw) { controller.VirtualDataInput[9] |= (1 << 0); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ShoulderRight].PressedRaw) { controller.VirtualDataInput[9] |= (1 << 1); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeft].PressedRaw) { controller.VirtualDataInput[8] |= (1 << 6); }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRight].PressedRaw) { controller.VirtualDataInput[8] |= (1 << 7); }
            }
            catch { }
        }
    }
}