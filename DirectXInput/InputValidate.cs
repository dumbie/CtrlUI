using System;
using System.Diagnostics;
using System.Linq;
using static LibraryShared.Classes;
using static LibraryShared.CRC32;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Validate controller input data
        bool ControllerValidateInputData(ControllerStatus controllerStatus)
        {
            try
            {
                //Validate read input data MD5 hash
                if (controllerStatus.Details.Wireless)
                {
                    if (controllerStatus.SupportedCurrent.CodeName == "SonyPS4DualShock" || controllerStatus.SupportedCurrent.CodeName == "SonyPS5DualSense")
                    {
                        //Compute MD5
                        int checksumOffset = controllerStatus.SupportedCurrent.OffsetWireless + (int)controllerStatus.SupportedCurrent.OffsetHeader.Checksum;
                        byte[] checksumInput = controllerStatus.InputReport.Take(checksumOffset).ToArray();
                        byte[] checksumCompute = ComputeHashCRC32(0x8C2C830C, checksumInput, false);

                        //Compare MD5
                        byte check0 = controllerStatus.InputReport[checksumOffset];
                        if (checksumCompute[0] != check0) { return false; }
                        byte check1 = controllerStatus.InputReport[checksumOffset + 1];
                        if (checksumCompute[1] != check1) { return false; }
                        byte check2 = controllerStatus.InputReport[checksumOffset + 2];
                        if (checksumCompute[2] != check2) { return false; }
                        byte check3 = controllerStatus.InputReport[checksumOffset + 3];
                        if (checksumCompute[3] != check3) { return false; }
                    }
                }

                //Return result
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to validate read input data: " + ex.Message);
                return false;
            }
        }
    }
}