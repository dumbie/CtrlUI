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
        bool InputValidateData(ControllerStatus controllerStatus)
        {
            try
            {
                if (controllerStatus.SupportedCurrent.CodeName == "NintendoSwitchPro")
                {
                    //Check controller report mode
                    byte check0 = controllerStatus.ControllerDataInput[0];
                    if (check0 != 0x30) { return false; }
                }
                else if (controllerStatus.SupportedCurrent.CodeName == "SonyPS4DualShock")
                {
                    if (controllerStatus.Details.Wireless)
                    {
                        //Compute MD5
                        int checksumOffset = controllerStatus.SupportedCurrent.OffsetWireless + (int)controllerStatus.SupportedCurrent.OffsetHeader.Checksum;
                        byte[] checksumInput = controllerStatus.ControllerDataInput.Take(checksumOffset).ToArray();

                        //Read MD5
                        byte check0 = controllerStatus.ControllerDataInput[checksumOffset];
                        byte check1 = controllerStatus.ControllerDataInput[checksumOffset + 1];
                        byte check2 = controllerStatus.ControllerDataInput[checksumOffset + 2];
                        byte check3 = controllerStatus.ControllerDataInput[checksumOffset + 3];

                        //Compare 8BitDo static hash
                        if (check0 == 169 && check1 == 47 && check2 == 73 && check3 == 54) { return true; }

                        //Compare computed MD5 hash
                        byte[] checksumCompute = ComputeHashCRC32(0x8C2C830C, checksumInput, false);
                        if (checksumCompute[0] != check0) { return false; }
                        if (checksumCompute[1] != check1) { return false; }
                        if (checksumCompute[2] != check2) { return false; }
                        if (checksumCompute[3] != check3) { return false; }
                    }
                }
                else if (controllerStatus.SupportedCurrent.CodeName == "SonyPS5DualSense")
                {
                    if (controllerStatus.Details.Wireless)
                    {
                        //Compute MD5
                        int checksumOffset = controllerStatus.SupportedCurrent.OffsetWireless + (int)controllerStatus.SupportedCurrent.OffsetHeader.Checksum;
                        byte[] checksumInput = controllerStatus.ControllerDataInput.Take(checksumOffset).ToArray();
                        byte[] checksumCompute = ComputeHashCRC32(0x8C2C830C, checksumInput, false);

                        //Compare computed MD5 hash
                        byte check0 = controllerStatus.ControllerDataInput[checksumOffset];
                        if (checksumCompute[0] != check0) { return false; }
                        byte check1 = controllerStatus.ControllerDataInput[checksumOffset + 1];
                        if (checksumCompute[1] != check1) { return false; }
                        byte check2 = controllerStatus.ControllerDataInput[checksumOffset + 2];
                        if (checksumCompute[2] != check2) { return false; }
                        byte check3 = controllerStatus.ControllerDataInput[checksumOffset + 3];
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