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

                    short gyroPitch = (short)((ushort)(gyroByte1 << 8) | gyroByte0);
                    short gyroYaw = (short)((ushort)(gyroByte3 << 8) | gyroByte2);
                    short gyroRoll = (short)((ushort)(gyroByte5 << 8) | gyroByte4);

                    controller.InputCurrent.GyroPitch = gyroPitch / (float)16;
                    controller.InputCurrent.GyroYaw = -gyroYaw / (float)16;
                    controller.InputCurrent.GyroRoll = -gyroRoll / (float)16;
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