using System;
using System.Diagnostics;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private static bool InputUpdateTouchpad(ControllerStatus controller)
        {
            try
            {
                if (controller.SupportedCurrent.OffsetHeader.Touchpad != null)
                {
                    //Set controller header offset
                    int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                    //Touchpad 1
                    byte touch1Byte0 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad];
                    byte touch1Byte1 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 1];
                    byte touch1Byte2 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 2];
                    byte touch1Byte3 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 3];
                    if ((touch1Byte0 & 0x80) == 0)
                    {
                        controller.InputCurrent.Touchpad1Active = 1;
                    }
                    else
                    {
                        controller.InputCurrent.Touchpad1Active = 0;
                    }
                    controller.InputCurrent.Touchpad1Id = (byte)(touch1Byte0 & 0x7F);
                    controller.InputCurrent.Touchpad1X = ((ushort)(touch1Byte2 & 0x0F) << 8) | touch1Byte1;
                    controller.InputCurrent.Touchpad1Y = (touch1Byte3 << 4) | ((ushort)(touch1Byte2 & 0xF0) >> 4);

                    //Touchpad 2
                    byte touch2Byte0 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 4];
                    byte touch2Byte1 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 5];
                    byte touch2Byte2 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 6];
                    byte touch2Byte3 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 7];
                    if ((touch2Byte0 & 0x80) == 0)
                    {
                        controller.InputCurrent.Touchpad2Active = 1;
                    }
                    else
                    {
                        controller.InputCurrent.Touchpad2Active = 0;
                    }
                    controller.InputCurrent.Touchpad2Id = (byte)(touch2Byte0 & 0x7F);
                    controller.InputCurrent.Touchpad2X = ((ushort)(touch2Byte2 & 0x0F) << 8) | touch2Byte1;
                    controller.InputCurrent.Touchpad2Y = (touch2Byte3 << 4) | ((ushort)(touch2Byte2 & 0xF0) >> 4);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update touchpad input: " + ex.Message);
                return false;
            }
        }
    }
}