using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.CRC32;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Send information to gyro dsu
        async Task SendGyroInformation(UdpEndPointDetails endPoint, ControllerStatus controller)
        {
            try
            {
                //Check if client endpoint is set
                if (endPoint == null || !endPoint.Active) { return; }
                //Debug.WriteLine("Sending controller " + controller.NumberId + " information to dsu client.");

                //Set message header
                byte[] sendBytes = new byte[32];
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
                byte[] typeBytes = BitConverter.GetBytes((uint)DsuMessageType.DSUS_PortInfo);
                sendBytes[16] = typeBytes[0];
                sendBytes[17] = typeBytes[1];
                sendBytes[18] = typeBytes[2];
                sendBytes[19] = typeBytes[3];

                //Set controller status
                sendBytes[20] = (byte)controller.NumberId;
                sendBytes[21] = controller.Connected() ? (byte)DsuState.Connected : (byte)DsuState.Disconnected;
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

                //Compute and set CRC32 hash
                byte[] checksum = ComputeHashCRC32(0, sendBytes, false);
                sendBytes[8] = checksum[0];
                sendBytes[9] = checksum[1];
                sendBytes[10] = checksum[2];
                sendBytes[11] = checksum[3];

                //Send bytes to dsu client
                if (!await vArnoldVinkSockets.UdpClientSendBytesServer(endPoint.IPEndPoint, sendBytes, vArnoldVinkSockets.vSocketTimeout))
                {
                    Debug.WriteLine("Failed to send information bytes to dsu client.");
                }
            }
            catch { }
        }

        //Send empty input to gyro dsu
        async Task SendGyroMotionEmpty(ControllerStatus controller)
        {
            try
            {
                //Check if client endpoint is set
                if (controller.GyroDsuClientEndPoint == null || !controller.GyroDsuClientEndPoint.Active) { return; }
                //Debug.WriteLine("Sending gyro motion " + controller.NumberId + " to dsu client.");

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
                sendBytes[21] = controller.Connected() ? (byte)DsuState.Connected : (byte)DsuState.Disconnected;
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

                //Compute and set CRC32 hash
                byte[] checksum = ComputeHashCRC32(0, sendBytes, false);
                sendBytes[8] = checksum[0];
                sendBytes[9] = checksum[1];
                sendBytes[10] = checksum[2];
                sendBytes[11] = checksum[3];

                //Send bytes to dsu client
                if (!await vArnoldVinkSockets.UdpClientSendBytesServer(controller.GyroDsuClientEndPoint.IPEndPoint, sendBytes, vArnoldVinkSockets.vSocketTimeout))
                {
                    Debug.WriteLine("Failed to send motion bytes to dsu client.");
                }
            }
            catch { }
        }

        //Send controller input to gyro dsu
        async Task<bool> SendGyroMotionController(ControllerStatus controller)
        {
            try
            {
                //Check if client endpoint is set
                if (controller.GyroDsuClientEndPoint == null || !controller.GyroDsuClientEndPoint.Active)
                {
                    //Debug.WriteLine("No gyro motion end point found to send input.");
                    return false;
                }
                else
                {
                    //Debug.WriteLine("Sending gyro motion " + controller.NumberId + " to dsu client.");
                }

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
                sendBytes[21] = controller.Connected() ? (byte)DsuState.Connected : (byte)DsuState.Disconnected;
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
                sendBytes[30] = (byte)DsuBattery.Full;

                //Set packet number
                byte[] packetNumberBytes = BitConverter.GetBytes(controller.GyroDsuClientPacketNumber);
                sendBytes[32] = packetNumberBytes[0];
                sendBytes[33] = packetNumberBytes[1];
                sendBytes[34] = packetNumberBytes[2];
                sendBytes[35] = packetNumberBytes[3];
                if (controller.GyroDsuClientPacketNumber >= 100000)
                {
                    controller.GyroDsuClientPacketNumber = 0;
                }
                else
                {
                    controller.GyroDsuClientPacketNumber++;
                }

                //Set DPad
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw)
                {
                    sendBytes[36] |= 0x10;
                    sendBytes[47] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw)
                {
                    sendBytes[36] |= 0x40;
                    sendBytes[45] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw)
                {
                    sendBytes[36] |= 0x80;
                    sendBytes[44] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw)
                {
                    sendBytes[36] |= 0x20;
                    sendBytes[46] = 0xFF;
                }

                //Set buttons (Start, Back and Thumb)
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Back].PressedRaw)
                {
                    sendBytes[36] |= 0x01;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Start].PressedRaw)
                {
                    sendBytes[36] |= 0x08;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeft].PressedRaw)
                {
                    sendBytes[36] |= 0x02;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRight].PressedRaw)
                {
                    sendBytes[36] |= 0x04;
                }

                //Set buttons (ABXY, Shoulder, Trigger)
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.A].PressedRaw)
                {
                    sendBytes[37] |= 0x40;
                    sendBytes[49] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.B].PressedRaw)
                {
                    sendBytes[37] |= 0x20;
                    sendBytes[50] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.X].PressedRaw)
                {
                    sendBytes[37] |= 0x80;
                    sendBytes[48] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Y].PressedRaw)
                {
                    sendBytes[37] |= 0x10;
                    sendBytes[51] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ShoulderLeft].PressedRaw)
                {
                    sendBytes[37] |= 0x04;
                    sendBytes[53] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ShoulderRight].PressedRaw)
                {
                    sendBytes[37] |= 0x08;
                    sendBytes[52] = 0xFF;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.TriggerLeft].PressedRaw)
                {
                    sendBytes[37] |= 0x01;
                }
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.TriggerRight].PressedRaw)
                {
                    sendBytes[37] |= 0x02;
                }

                //Set button Guide
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Guide].PressedRaw)
                {
                    sendBytes[38] = 0xFF;
                }

                //Set button Touchpad
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.One].PressedRaw)
                {
                    sendBytes[39] = 0xFF;
                }

                //Set left stick
                sendBytes[40] = (byte)controller.InputCurrent.ThumbLeftX;
                sendBytes[41] = (byte)controller.InputCurrent.ThumbLeftY;

                //Set right stick
                sendBytes[42] = (byte)controller.InputCurrent.ThumbRightX;
                sendBytes[43] = (byte)controller.InputCurrent.ThumbRightY;

                //Set triggers
                sendBytes[54] = controller.InputCurrent.TriggerRight;
                sendBytes[55] = controller.InputCurrent.TriggerLeft;

                //Set touchpad 1
                byte[] touch1XBytes = BitConverter.GetBytes(controller.InputCurrent.Touchpad1X);
                byte[] touch1YBytes = BitConverter.GetBytes(controller.InputCurrent.Touchpad1Y);
                sendBytes[56] = controller.InputCurrent.Touchpad1Active;
                sendBytes[57] = controller.InputCurrent.Touchpad1Id;
                sendBytes[58] = touch1XBytes[0];
                sendBytes[59] = touch1XBytes[1];
                sendBytes[60] = touch1YBytes[0];
                sendBytes[61] = touch1YBytes[1];

                //Set touchpad 2
                byte[] touch2XBytes = BitConverter.GetBytes(controller.InputCurrent.Touchpad2X);
                byte[] touch2YBytes = BitConverter.GetBytes(controller.InputCurrent.Touchpad2Y);
                sendBytes[62] = controller.InputCurrent.Touchpad2Active;
                sendBytes[63] = controller.InputCurrent.Touchpad2Id;
                sendBytes[64] = touch2XBytes[0];
                sendBytes[65] = touch2XBytes[1];
                sendBytes[66] = touch2YBytes[0];
                sendBytes[67] = touch2YBytes[1];

                //Set timestamp
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
                byte[] checksum = ComputeHashCRC32(0, sendBytes, false);
                sendBytes[8] = checksum[0];
                sendBytes[9] = checksum[1];
                sendBytes[10] = checksum[2];
                sendBytes[11] = checksum[3];

                //Send bytes to dsu client
                if (!await vArnoldVinkSockets.UdpClientSendBytesServer(controller.GyroDsuClientEndPoint.IPEndPoint, sendBytes, vArnoldVinkSockets.vSocketTimeout))
                {
                    //Debug.WriteLine("Failed sending motion bytes to dsu client.");
                    return false;
                }
                else
                {
                    //Debug.WriteLine("Sended motion bytes to dsu client.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed sending motion to dsu client: " + ex.Message);
                return false;
            }
        }
    }
}