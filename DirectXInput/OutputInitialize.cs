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
                    byte[] outputReport = new byte[75];
                    outputReport[0] = 0xA2;
                    outputReport[1] = 0x31;
                    outputReport[2] = 0x02;
                    outputReport[3] = 0xFF;
                    outputReport[4] = 0x08;

                    //Add CRC32 to bytes array
                    byte[] outputReportCRC32 = ByteArrayAddCRC32(outputReport, 74);

                    //Send data to the controller
                    bool bytesWritten = Controller.HidDevice.WriteBytesFile(outputReportCRC32);
                    Debug.WriteLine("Initialized Bluetooth controller: SonyPS5DualSense: " + bytesWritten);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialize game controller: " + ex.Message);
            }
        }
    }
}