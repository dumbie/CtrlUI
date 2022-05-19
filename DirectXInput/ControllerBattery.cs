using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVClassConverters;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Read controller battery level
        void ControllerReadBatteryLevel(ControllerStatus Controller)
        {
            try
            {
                //Check which controller is connected
                if (Controller.SupportedCurrent.CodeName == "SonyPS5DualSense" && Controller.Details.Wireless)
                {
                    //Bluetooth - DualSense 5
                    int batteryLevelOffset = Controller.SupportedCurrent.OffsetWireless + (int)Controller.SupportedCurrent.OffsetHeader.BatteryLevel;
                    byte batteryLevelReport = Controller.InputReport[batteryLevelOffset];
                    int batteryStatusOffset = Controller.SupportedCurrent.OffsetWireless + (int)Controller.SupportedCurrent.OffsetHeader.BatteryStatus;
                    byte batteryStatusReport = Controller.InputReport[batteryStatusOffset];

                    bool batteryCharging = batteryStatusReport != 0;
                    if (batteryCharging)
                    {
                        Controller.BatteryCurrent.BatteryPercentage = -1;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                    }
                    else
                    {
                        int batteryPercentage = TranslateByte_0x0F(0, batteryLevelReport) * 10 + 10;
                        if (batteryPercentage > 100) { batteryPercentage = 100; }
                        Controller.BatteryCurrent.BatteryPercentage = batteryPercentage;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Normal;
                    }
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyPS5DualSense" && !Controller.Details.Wireless)
                {
                    //Wired USB - DualSense 5
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyPS4DualShock" && Controller.Details.Wireless)
                {
                    //Bluetooth - DualShock 4
                    int batteryOffset = Controller.SupportedCurrent.OffsetWireless + (int)Controller.SupportedCurrent.OffsetHeader.BatteryLevel;
                    byte batteryReport = Controller.InputReport[batteryOffset];

                    bool batteryCharging = TranslateByte_0x10(0, batteryReport) != 0;
                    if (batteryCharging)
                    {
                        Controller.BatteryCurrent.BatteryPercentage = -1;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                    }
                    else
                    {
                        int batteryPercentage = TranslateByte_0x0F(0, batteryReport) * 10 + 10;
                        if (batteryPercentage > 100) { batteryPercentage = 100; }
                        Controller.BatteryCurrent.BatteryPercentage = batteryPercentage;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Normal;
                    }
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyPS4DualShock" && !Controller.Details.Wireless)
                {
                    //Wired USB - DualShock 4
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyPS3DualShock" && !Controller.Details.Wireless)
                {
                    //Wired USB - DualShock 3
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                }
                else
                {
                    //Unknown controller
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Unknown;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read the battery level: " + ex.Message);
                Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Unknown;
            }
        }

        //Check controller for low battery level
        async Task ControllerLowBattery(ControllerStatus Controller, bool forceUpdate)
        {
            try
            {
                //Debug.WriteLine("Checking if controller " + Controller.NumberId + " has a low battery level " + Controller.BatteryPercentageCurrent + "/" + Controller.BatteryPercentagePrevious);
                string controllerNumberDisplay = (Controller.NumberId + 1).ToString();

                //Check if the controller is connected
                if (Controller == null || !Controller.Connected())
                {
                    return;
                }

                //Check the current battery level
                bool batteryLevelChanged = Controller.BatteryCurrent.BatteryPercentage != Controller.BatteryPrevious.BatteryPercentage || Controller.BatteryCurrent.BatteryStatus != Controller.BatteryPrevious.BatteryStatus;
                bool batteryLevelLow = Controller.BatteryCurrent.BatteryPercentage <= Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "BatteryLowLevel")) && Controller.BatteryCurrent.BatteryStatus == BatteryStatus.Normal;

                //Check if battery level changed
                if (forceUpdate || batteryLevelChanged)
                {
                    Debug.WriteLine("Controller " + Controller.NumberId + " battery level changed.");

                    //Check if battery level is low
                    if (batteryLevelLow)
                    {
                        Debug.WriteLine("Controller " + Controller.NumberId + " has a low battery level.");

                        //Battery level notification
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryLowShowNotification")))
                        {
                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "Battery/BatteryVerDis20";
                            notificationDetails.Text = "Controller (" + controllerNumberDisplay + ") battery " + Controller.BatteryCurrent.BatteryPercentage + "%";
                            notificationDetails.Color = Controller.Color;
                            await App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                        }

                        //Battery level sound
                        if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryLowPlaySound")))
                        {
                            PlayInterfaceSound(vConfigurationCtrlUI, "BatteryLow", true, false);
                        }
                    }
                }

                //Update the previous battery level
                CloneObjectShallow(Controller.BatteryCurrent, out Controller.BatteryPrevious);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed checking low battery level: " + ex.Message);
            }
        }

        //Check all controllers for low battery level
        async Task CheckAllControllersLowBattery(bool forceUpdate)
        {
            try
            {
                await ControllerLowBattery(vController0, forceUpdate);
                await ControllerLowBattery(vController1, forceUpdate);
                await ControllerLowBattery(vController2, forceUpdate);
                await ControllerLowBattery(vController3, forceUpdate);
            }
            catch { }
        }
    }
}