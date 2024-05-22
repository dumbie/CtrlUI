using System;
using System.Diagnostics;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private static bool InputUpdateGyroscope(ControllerStatus controller)
        {
            try
            {
                if (controller.SupportedCurrent.OffsetHeader.Gyroscope != null)
                {
                    //Set controller header offset
                    int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                    byte gyroByte0 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope];
                    byte gyroByte1 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 1];
                    byte gyroByte2 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 2];
                    byte gyroByte3 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 3];
                    byte gyroByte4 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 4];
                    byte gyroByte5 = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 5];

                    short gyroGroup1 = (short)((ushort)(gyroByte1 << 8) | gyroByte0);
                    short gyroGroup2 = (short)((ushort)(gyroByte3 << 8) | gyroByte2);
                    short gyroGroup3 = (short)((ushort)(gyroByte5 << 8) | gyroByte4);

                    if (controller.SupportedCurrent.CodeName == "NintendoSwitchPro")
                    {
                        controller.InputCurrent.GyroPitch = -gyroGroup2 / (float)16;
                        controller.InputCurrent.GyroYaw = -gyroGroup3 / (float)16;
                        controller.InputCurrent.GyroRoll = gyroGroup1 / (float)16;
                    }
                    else
                    {
                        controller.InputCurrent.GyroPitch = gyroGroup1 / (float)16;
                        controller.InputCurrent.GyroYaw = -gyroGroup2 / (float)16;
                        controller.InputCurrent.GyroRoll = -gyroGroup3 / (float)16;
                    }

                    //Debug.WriteLine("Gyroscope Pitch" + controller.InputCurrent.GyroPitch + " Yaw" + controller.InputCurrent.GyroYaw + " Roll" + controller.InputCurrent.GyroRoll);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update gyroscope input: " + ex.Message);
                return false;
            }
        }
    }
}