using System;
using System.Diagnostics;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private static bool InputUpdateAccelerometer(ControllerStatus controller)
        {
            try
            {
                if (controller.SupportedCurrent.OffsetHeader.Accelerometer != null)
                {
                    //Set controller header offset
                    int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                    byte accelByte0 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer];
                    byte accelByte1 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 1];
                    byte accelByte2 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 2];
                    byte accelByte3 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 3];
                    byte accelByte4 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 4];
                    byte accelByte5 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 5];

                    short accelX = (short)((ushort)(accelByte1 << 8) | accelByte0);
                    short accelY = (short)((ushort)(accelByte3 << 8) | accelByte2);
                    short accelZ = (short)((ushort)(accelByte5 << 8) | accelByte4);

                    controller.InputCurrent.AccelX = -accelX / (float)8192;
                    controller.InputCurrent.AccelY = -accelY / (float)8192;
                    controller.InputCurrent.AccelZ = -accelZ / (float)8192;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update accelerometer input: " + ex.Message);
                return false;
            }
        }
    }
}