using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Read controller battery level
        void ControllerUpdateBatteryLevel(ControllerStatus Controller)
        {
            try
            {
                //Check which controller is connected
                IEnumerable<ControllerSupported> TargetController = List_ControllerSupported.Where(x => x.ProductIDs.Any(z => z.ToLower() == Controller.Details.Profile.ProductID.ToLower() && x.VendorID.ToLower() == Controller.Details.Profile.VendorID.ToLower()));
                if (TargetController.Any(x => x.CodeName == "SonyDualShock4") && Controller.Details.Wireless)
                {
                    //Bluetooth - DualShock 4
                    int BatteryOffset = 30 + Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset;
                    byte BatteryReport = Controller.InputReport[BatteryOffset];

                    bool BatteryCharging = TranslateByte_0x10(0, BatteryReport) != 0;
                    if (BatteryCharging)
                    {
                        Controller.BatteryPercentageCurrent = -2;
                    }
                    else
                    {
                        int RawBattery = TranslateByte_0x0F(0, BatteryReport) * 10 + 10;
                        if (RawBattery > 100) { RawBattery = 100; }
                        Controller.BatteryPercentageCurrent = RawBattery;
                    }
                }
                else if (TargetController.Any(x => x.CodeName == "SonyDualShock4") && !Controller.Details.Wireless)
                {
                    //Wired USB - DualShock 4
                    Controller.BatteryPercentageCurrent = -2;
                }
                else
                {
                    //Incompatible controllers
                    Controller.BatteryPercentageCurrent = -1;
                }
            }
            catch
            {
                Controller.BatteryPercentageCurrent = -1;
            }
        }

        //Check controller for low battery level
        void ControllerLowBattery(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected() && Controller.InputReport != null && Controller.BatteryPercentageCurrent > 0)
                {
                    int LowBatteryLevelRange = 20;
                    //Debug.WriteLine("Checking if controller " + Controller.NumberId + " has a low battery level " + Controller.BatteryPercentageCurrent + "/" + Controller.BatteryPercentagePrevious);
                    if (Controller.BatteryPercentageCurrent <= LowBatteryLevelRange && (Controller.BatteryPercentagePrevious > LowBatteryLevelRange || Controller.BatteryPercentagePrevious == -1))
                    {
                        Debug.WriteLine("Controller " + Controller.NumberId + " has a low battery level.");
                        PlayInterfaceSound(vInterfaceSoundVolume, "BatteryLow", true);
                    }

                    Controller.BatteryPercentagePrevious = Controller.BatteryPercentageCurrent;
                }
            }
            catch { }
        }

        //Check for timed out controllers
        void CheckControllersLowBattery()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["PlaySoundBatteryLow"]))
                {
                    ControllerLowBattery(vController0);
                    ControllerLowBattery(vController1);
                    ControllerLowBattery(vController2);
                    ControllerLowBattery(vController3);
                }
            }
            catch { }
        }
    }
}