using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.CRC32;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private async Task SendGyroMotion(ControllerStatus controller)
        {
            try
            {
                //Check if client endpoint is set
                if (controller.GyroDsuClientEndPoint == null || !controller.GyroDsuClientEndPoint.Active) { return; }
                //Debug.WriteLine("Sending gyro motion to dsu client.");

                //Set message header
                byte[] sendBytes = new byte[100];
                sendBytes[0] = (byte)'D';
                sendBytes[1] = (byte)'S';
                sendBytes[2] = (byte)'U';
                sendBytes[3] = (byte)'S';

                //Set message protocol
                byte[] protocolBytes = BitConverter.GetBytes(1001);
                sendBytes[4] = protocolBytes[0];
                sendBytes[5] = protocolBytes[1];

                //Set message length
                byte[] lengthBytes = BitConverter.GetBytes(sendBytes.Length - 16);
                sendBytes[6] = lengthBytes[0];
                sendBytes[7] = lengthBytes[1];

                //Set message type
                byte[] typeBytes = BitConverter.GetBytes((uint)DsuMessageType.DSUS_PadDataRsp);
                sendBytes[16] = typeBytes[0];
                sendBytes[17] = typeBytes[1];
                sendBytes[18] = typeBytes[2];
                sendBytes[19] = typeBytes[3];

                //Set controller status
                sendBytes[20] = (byte)controller.NumberId;
                sendBytes[21] = (byte)DsuState.Connected;
                sendBytes[22] = (byte)DsuModel.DualShock4;
                sendBytes[23] = (byte)DsuConnectionType.None;

                //Set mac address
                sendBytes[24] = 0;
                sendBytes[25] = 0;
                sendBytes[26] = 0;
                sendBytes[27] = 0;
                sendBytes[28] = 0;
                sendBytes[29] = (byte)controller.NumberId;

                //Set battery status
                sendBytes[30] = (byte)DsuBattery.None;

                //Set packet number
                byte[] packetNumberBytes = BitConverter.GetBytes(controller.GyroPacketNumber);
                sendBytes[32] = packetNumberBytes[0];
                sendBytes[33] = packetNumberBytes[1];
                sendBytes[34] = packetNumberBytes[2];
                sendBytes[35] = packetNumberBytes[3];
                if (controller.GyroPacketNumber >= 100000)
                {
                    controller.GyroPacketNumber = 0;
                }
                else
                {
                    controller.GyroPacketNumber++;
                }

                //Set time stamp
                byte[] timeStampBytes = BitConverter.GetBytes(Stopwatch.GetTimestamp() / 10);
                sendBytes[68] = timeStampBytes[0];
                sendBytes[69] = timeStampBytes[1];
                sendBytes[70] = timeStampBytes[2];
                sendBytes[71] = timeStampBytes[3];

                //Set accelerometer
                byte[] accelXBytes = BitConverter.GetBytes(controller.InputCurrent.AccelX);
                sendBytes[76] = accelXBytes[0];
                sendBytes[77] = accelXBytes[1];
                sendBytes[78] = accelXBytes[2];
                sendBytes[79] = accelXBytes[3];

                byte[] accelYBytes = BitConverter.GetBytes(controller.InputCurrent.AccelY);
                sendBytes[80] = accelYBytes[0];
                sendBytes[81] = accelYBytes[1];
                sendBytes[82] = accelYBytes[2];
                sendBytes[83] = accelYBytes[3];

                byte[] accelZBytes = BitConverter.GetBytes(controller.InputCurrent.AccelZ);
                sendBytes[84] = accelZBytes[0];
                sendBytes[85] = accelZBytes[1];
                sendBytes[86] = accelZBytes[2];
                sendBytes[87] = accelZBytes[3];

                //Set gyroscope
                byte[] gyroPitchBytes = BitConverter.GetBytes(controller.InputCurrent.GyroPitch);
                sendBytes[88] = gyroPitchBytes[0];
                sendBytes[89] = gyroPitchBytes[1];
                sendBytes[90] = gyroPitchBytes[2];
                sendBytes[91] = gyroPitchBytes[3];

                byte[] gyroYawBytes = BitConverter.GetBytes(controller.InputCurrent.GyroYaw);
                sendBytes[92] = gyroYawBytes[0];
                sendBytes[93] = gyroYawBytes[1];
                sendBytes[94] = gyroYawBytes[2];
                sendBytes[95] = gyroYawBytes[3];

                byte[] gyroRollBytes = BitConverter.GetBytes(controller.InputCurrent.GyroRoll);
                sendBytes[96] = gyroRollBytes[0];
                sendBytes[97] = gyroRollBytes[1];
                sendBytes[98] = gyroRollBytes[2];
                sendBytes[99] = gyroRollBytes[3];

                //Compute and set CRC32 hash
                byte[] checksum = ComputeHashCRC32(sendBytes, false);
                sendBytes[8] = checksum[0];
                sendBytes[9] = checksum[1];
                sendBytes[10] = checksum[2];
                sendBytes[11] = checksum[3];

                //Send bytes to dsu client
                if (!await vArnoldVinkSockets.UdpClientSendBytes(controller.GyroDsuClientEndPoint.IPEndPoint, sendBytes, 1000))
                {
                    Debug.WriteLine("Failed to send bytes to dsu client.");
                }
            }
            catch { }
        }
    }
}