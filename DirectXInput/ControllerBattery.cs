using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

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
                IEnumerable<ControllerSupported> TargetController = List_ControllerSupported.Where(x => x.ProductIDs.Any(z => z.ToLower() == Controller.Connected.Profile.ProductID.ToLower() && x.VendorID.ToLower() == Controller.Connected.Profile.VendorID.ToLower()));
                if (TargetController.Any(x => x.CodeName == "SonyDualShock4") && Controller.Connected.Wireless)
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
                else if (TargetController.Any(x => x.CodeName == "SonyDualShock4") && !Controller.Connected.Wireless)
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
        async Task ControllerLowBattery(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected != null && Controller.InputReport != null && Controller.BatteryPercentageCurrent > 0)
                {
                    int LowBatteryLevelRange = 20;
                    //Debug.WriteLine("Checking if controller " + Controller.NumberId + " has a low battery level " + Controller.BatteryPercentageCurrent + "/" + Controller.BatteryPercentagePrevious);
                    if (Controller.BatteryPercentageCurrent <= LowBatteryLevelRange && (Controller.BatteryPercentagePrevious > LowBatteryLevelRange || Controller.BatteryPercentagePrevious == -1))
                    {
                        Debug.WriteLine("Controller " + Controller.NumberId + " has a low battery level.");
                        PlayInterfaceSound("BatteryLow", true);
                        await Task.Delay(1000);
                        PlayInterfaceSound("BatteryLow", true);
                        await Task.Delay(1000);
                        PlayInterfaceSound("BatteryLow", true);
                    }

                    Controller.BatteryPercentagePrevious = Controller.BatteryPercentageCurrent;
                }
            }
            catch { }
        }

        //Check for timed out controllers
        async Task CheckControllersLowBattery()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["PlaySoundBatteryLow"]))
                {
                    await ControllerLowBattery(vController0);
                    await ControllerLowBattery(vController1);
                    await ControllerLowBattery(vController2);
                    await ControllerLowBattery(vController3);
                }
            }
            catch { }
        }
    }
}