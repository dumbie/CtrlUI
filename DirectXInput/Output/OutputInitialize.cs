using System;
using System.Diagnostics;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Initialize controller
        private void ControllerInitialize(ControllerStatus Controller)
        {
            try
            {
                if (Controller.SupportedCurrent.CodeName == "SonyPS3DualShock" || Controller.SupportedCurrent.CodeName == "SonyPS3MoveNavigation")
                {
                    //Wired USB Output - DualShock 3 or Move Navigation 3
                    byte[] outputReport = new byte[2];
                    outputReport[0] = 0x42;
                    outputReport[1] = 0x0C;

                    bool bytesWritten = Controller.WinUsbDevice.WriteBytesTransfer(0x21, 0x09, 0x3F4, outputReport);
                    Debug.WriteLine("Initialized USB controller: SonyPS3DualShock or SonyPS3MoveNavigation: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyPS5DualSense" && Controller.Details.Wireless)
                {
                    //Bluetooth Output - DualSense 5
                    byte[] outputReport = new byte[Controller.ControllerDataOutput.Length];
                    outputReport[0] = 0xA2;
                    outputReport[1] = 0x31;
                    outputReport[2] = 0x02;
                    outputReport[3] = 0xFF;
                    outputReport[4] = 0x08;

                    //Replace CRC32 in bytes array
                    ByteArrayCRC32Replace(ref outputReport, 0, 1, 74);

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(outputReport);
                    Debug.WriteLine("Initialized Bluetooth controller: SonyPS5DualSense: " + bytesWritten);
                }
                else if (Controller.SupportedCurrent.CodeName == "NintendoSwitchPro")
                {
                    bool bytesWritten = false;
                    byte[] outputReport = new byte[Controller.ControllerDataOutput.Length];
                    outputReport[0] = 0x01;
                    outputReport[1] = 0xFF;
                    outputReport[2] = 0x00;
                    outputReport[3] = 0x01;
                    outputReport[4] = 0x40;
                    outputReport[5] = 0x40;
                    outputReport[6] = 0x00;
                    outputReport[7] = 0x01;
                    outputReport[8] = 0x40;
                    outputReport[9] = 0x40;

                    //Set full report mode
                    outputReport[10] = 0x03;
                    outputReport[11] = 0x30;
                    //Send data to the controller
                    bytesWritten = Controller.HidDevice.WriteBytesFile(outputReport);
                    Debug.WriteLine("Initialized controller report mode: NintendoSwitchPro: " + bytesWritten);

                    //Set player led position
                    outputReport[10] = 0x30;
                    switch (Controller.NumberId)
                    {
                        case 0: { outputReport[11] = 0x01; break; }
                        case 1: { outputReport[11] = 0x02; break; }
                        case 2: { outputReport[11] = 0x04; break; }
                        case 3: { outputReport[11] = 0x08; break; }
                    }
                    //Send data to the controller
                    bytesWritten = Controller.HidDevice.WriteBytesFile(outputReport);
                    Debug.WriteLine("Initialized controller player led: NintendoSwitchPro: " + bytesWritten);

                    //Enable motion
                    outputReport[10] = 0x40;
                    outputReport[11] = 0x01;
                    //Send data to the controller
                    bytesWritten = Controller.HidDevice.WriteBytesFile(outputReport);
                    Debug.WriteLine("Initialized controller motion enable: NintendoSwitchPro: " + bytesWritten);

                    //Enable rumble
                    outputReport[10] = 0x48;
                    outputReport[11] = 0x01;
                    //Send data to the controller
                    bytesWritten = Controller.HidDevice.WriteBytesFile(outputReport);
                    Debug.WriteLine("Initialized controller vibration: NintendoSwitchPro: " + bytesWritten);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialize game controller: " + ex.Message);
            }
        }
    }
}