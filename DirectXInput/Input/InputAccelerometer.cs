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

                    if (controller.SupportedCurrent.CodeName == "NintendoSwitchPro")
                    {
                        short accelGroup1 = (short)((ushort)(accelByte1 << 8) | accelByte0);
                        short accelGroup2 = (short)((ushort)(accelByte3 << 8) | accelByte2);
                        short accelGroup3 = (short)((ushort)(accelByte5 << 8) | accelByte4);
                        controller.InputCurrent.AccelX = accelGroup2 / 4096.0f;
                        controller.InputCurrent.AccelY = -(accelGroup3 / 4096.0f);
                        controller.InputCurrent.AccelZ = accelGroup1 / 4096.0f;
                    }
                    else if (controller.SupportedCurrent.CodeName == "SonyPS3DualShock")
                    {
                        short accelGroup1 = (short)((ushort)(accelByte0 << 8) | accelByte1);
                        short accelGroup2 = (short)((ushort)(accelByte2 << 8) | accelByte3);
                        short accelGroup3 = (short)((ushort)(accelByte4 << 8) | accelByte5);
                        controller.InputCurrent.AccelX = -((accelGroup1 - 511.5f) / 115.0f);
                        controller.InputCurrent.AccelY = (accelGroup3 - 511.5f) / 115.0f;
                        controller.InputCurrent.AccelZ = (accelGroup2 - 511.5f) / 115.0f;
                    }
                    else
                    {
                        short accelGroup1 = (short)((ushort)(accelByte1 << 8) | accelByte0);
                        short accelGroup2 = (short)((ushort)(accelByte3 << 8) | accelByte2);
                        short accelGroup3 = (short)((ushort)(accelByte5 << 8) | accelByte4);
                        controller.InputCurrent.AccelX = -(accelGroup1 / 8192.0f);
                        controller.InputCurrent.AccelY = -(accelGroup2 / 8192.0f);
                        controller.InputCurrent.AccelZ = -(accelGroup3 / 8192.0f);
                    }

                    //Debug.WriteLine("Accelerometer X" + controller.InputCurrent.AccelX + " Y" + controller.InputCurrent.AccelY + " Z" + controller.InputCurrent.AccelZ);
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