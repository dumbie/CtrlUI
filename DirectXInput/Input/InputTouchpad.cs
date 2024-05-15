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

                    byte touchByte0 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad];
                    byte touchByte1 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 1];
                    byte touchByte2 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 2];
                    byte touchByte3 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 3];

                    if ((touchByte0 & 0x80) == 0)
                    {
                        controller.InputCurrent.TouchpadActive = 1;
                    }
                    else
                    {
                        controller.InputCurrent.TouchpadActive = 0;
                    }

                    controller.InputCurrent.TouchpadId = (byte)(touchByte0 & 0x7F);
                    controller.InputCurrent.TouchpadX = ((ushort)(touchByte2 & 0x0F) << 8) | touchByte1;
                    controller.InputCurrent.TouchpadY = (touchByte3 << 4) | ((ushort)(touchByte2 & 0xF0) >> 4);
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