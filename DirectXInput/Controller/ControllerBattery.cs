using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
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
                //Check if controller is connected
                if (!Controller.Connected())
                {
                    //Debug.WriteLine("Controller is not connected skipping battery level check: " + Controller.NumberId);
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Unknown;
                    return;
                }

                //Check if controller has read data
                if (!Controller.ControllerDataRead)
                {
                    //Debug.WriteLine("Controller has no data skipping battery level check: " + Controller.NumberId);
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Unknown;
                    return;
                }

                //Check which controller is connected
                if (!Controller.Details.Wireless)
                {
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyPS5DualSense")
                {
                    //Bluetooth - SonyPS5DualSense
                    int batteryLevelOffset = Controller.SupportedCurrent.OffsetWireless + (int)Controller.SupportedCurrent.OffsetHeader.BatteryLevel;
                    byte batteryLevelReport = Controller.ControllerDataInput[batteryLevelOffset];
                    int batteryStatusOffset = Controller.SupportedCurrent.OffsetWireless + (int)Controller.SupportedCurrent.OffsetHeader.BatteryStatus;
                    byte batteryStatusReport = Controller.ControllerDataInput[batteryStatusOffset];

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
                else if (Controller.SupportedCurrent.CodeName == "SonyPS4DualShock")
                {
                    //Bluetooth - SonyPS4DualShock
                    int batteryOffset = Controller.SupportedCurrent.OffsetWireless + (int)Controller.SupportedCurrent.OffsetHeader.BatteryLevel;
                    byte batteryReport = Controller.ControllerDataInput[batteryOffset];

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
                else if (Controller.SupportedCurrent.CodeName == "NintendoSwitchPro")
                {
                    //Bluetooth - NintendoSwitchPro
                    int batteryOffset = Controller.SupportedCurrent.OffsetWireless + (int)Controller.SupportedCurrent.OffsetHeader.BatteryLevel;
                    byte batteryReport = Controller.ControllerDataInput[batteryOffset];

                    bool batteryCharging = TranslateByte_0x10(0, batteryReport) != 0;
                    if (batteryCharging)
                    {
                        Controller.BatteryCurrent.BatteryPercentage = -1;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                    }
                    else
                    {
                        int batteryPercentage = ((batteryReport) >> 4) * 10;
                        if (batteryPercentage > 100) { batteryPercentage = 100; }
                        Controller.BatteryCurrent.BatteryPercentage = batteryPercentage;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Normal;
                    }
                }
                else if (Controller.SupportedCurrent.CodeName == "8BitDoPro2")
                {
                    //Bluetooth - 8BitDoPro2
                    int batteryOffset = Controller.SupportedCurrent.OffsetWireless + (int)Controller.SupportedCurrent.OffsetHeader.BatteryLevel;
                    byte batteryReport = Controller.ControllerDataInput[batteryOffset];

                    Controller.BatteryCurrent.BatteryPercentage = batteryReport;
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Normal;
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
        void ControllerLowBattery(ControllerStatus Controller, bool forceUpdate)
        {
            try
            {
                //Debug.WriteLine("Checking if controller " + Controller.NumberId + " has a low battery level " + Controller.BatteryPercentageCurrent + "/" + Controller.BatteryPercentagePrevious);

                //Check if controller is connected
                if (!Controller.Connected())
                {
                    return;
                }

                //Check controller display number
                string controllerNumberDisplay = Controller.NumberDisplay().ToString();

                //Check controller current battery level
                bool batteryLevelChanged = Controller.BatteryCurrent.BatteryPercentage != Controller.BatteryPrevious.BatteryPercentage || Controller.BatteryCurrent.BatteryStatus != Controller.BatteryPrevious.BatteryStatus;
                bool batteryLevelLow = Controller.BatteryCurrent.BatteryPercentage <= SettingLoad(vConfigurationDirectXInput, "BatteryLowLevel", typeof(int)) && Controller.BatteryCurrent.BatteryStatus == BatteryStatus.Normal;

                //Check if battery level changed
                if (forceUpdate || batteryLevelChanged)
                {
                    Debug.WriteLine("Controller " + Controller.NumberId + " battery level changed.");

                    //Check if battery level is low
                    if (batteryLevelLow)
                    {
                        Debug.WriteLine("Controller " + Controller.NumberId + " has a low battery level.");

                        //Battery level notification
                        if (SettingLoad(vConfigurationDirectXInput, "BatteryLowShowNotification", typeof(bool)))
                        {
                            NotificationDetails notificationDetails = new NotificationDetails();
                            notificationDetails.Icon = "Battery/BatteryVerDis20";
                            notificationDetails.Text = "Controller (" + controllerNumberDisplay + ") battery " + Controller.BatteryCurrent.BatteryPercentage + "%";
                            notificationDetails.Color = Controller.Color;
                            vWindowOverlay.Notification_Show_Status(notificationDetails);
                        }

                        //Battery level sound
                        if (SettingLoad(vConfigurationDirectXInput, "BatteryLowPlaySound", typeof(bool)))
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
        void CheckAllControllersLowBattery(bool forceUpdate)
        {
            try
            {
                ControllerLowBattery(vController0, forceUpdate);
                ControllerLowBattery(vController1, forceUpdate);
                ControllerLowBattery(vController2, forceUpdate);
                ControllerLowBattery(vController3, forceUpdate);
            }
            catch { }
        }
    }
}