using System;
using System.Diagnostics;
using System.Windows.Media;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Controller update led color
        void ControllerLedColor(ControllerStatus Controller)
        {
            try
            {
                //Check if the controller is connected
                if (Controller == null || !Controller.Connected())
                {
                    return;
                }

                //Load battery settings
                bool batteryBlinkLedSetting = SettingLoad(vConfigurationDirectXInput, "BatteryLowBlinkLed", typeof(bool));
                int batteryLowLevelSetting = SettingLoad(vConfigurationDirectXInput, "BatteryLowLevel", typeof(int));

                //Check led battery blink and if battery is low
                if (batteryBlinkLedSetting)
                {
                    if (!Controller.ColorLedBlink && Controller.BatteryCurrent.BatteryPercentage <= batteryLowLevelSetting && Controller.BatteryCurrent.BatteryStatus == BatteryStatus.Normal)
                    {
                        Controller.ColorLedBlink = true;
                    }
                    else
                    {
                        Controller.ColorLedBlink = false;
                    }
                }
                else
                {
                    Controller.ColorLedBlink = false;
                }

                //Set controller led color
                if (Controller.ColorLedBlink)
                {
                    Controller.ColorLedCurrentR = 0;
                    Controller.ColorLedCurrentG = 0;
                    Controller.ColorLedCurrentB = 0;
                }
                else
                {
                    Color controllerColor = (Color)Controller.Color;
                    double controllerLedBrightness = Convert.ToDouble(Controller.Details.Profile.LedBrightness) / 100;
                    Controller.ColorLedCurrentR = Convert.ToByte(controllerColor.R * controllerLedBrightness);
                    Controller.ColorLedCurrentG = Convert.ToByte(controllerColor.G * controllerLedBrightness);
                    Controller.ColorLedCurrentB = Convert.ToByte(controllerColor.B * controllerLedBrightness);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update controller led color: " + ex.Message);
            }
        }
    }
}