using ArnoldVinkCode;
using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowStats
    {
        void UpdateBatteryInformation(IList<IHardware> hardwareItems)
        {
            try
            {
                //Check if the information is visible
                bool showPercentage = SettingLoad(vConfigurationFpsOverlayer, "BatShowPercentage", typeof(bool));
                if (!showPercentage)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentBat.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Select hardware item
                IHardware hardwareItem = hardwareItems.FirstOrDefault(x => x.HardwareType == HardwareType.Battery);

                //Check hardware item
                if (hardwareItem == null)
                {
                    return;
                }

                //Update hardware item
                hardwareItem.Update();

                string BatteryPercentage = string.Empty;
                string BatteryStatus = string.Empty;
                foreach (ISensor sensor in hardwareItem.Sensors)
                {
                    try
                    {
                        if (sensor.SensorType == SensorType.Level)
                        {
                            if (sensor.Name == "Charge Level")
                            {
                                //Debug.WriteLine("Bat Level: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                BatteryPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
                            }
                        }
                        else if (sensor.SensorType == SensorType.Power)
                        {
                            if (sensor.Name == "Charge Rate")
                            {
                                //Debug.WriteLine("Bat Charge Rate: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                BatteryStatus = " (Charging)";
                            }
                        }
                        else if (sensor.SensorType == SensorType.TimeSpan)
                        {
                            //Debug.WriteLine("Bat Estimated Time: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            BatteryStatus = " (" + AVFunctions.SecondsToHms(Convert.ToInt32(sensor.Value), true, false) + ")";
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(BatteryPercentage) || !string.IsNullOrWhiteSpace(BatteryStatus))
                {
                    string stringDisplay = AVFunctions.StringRemoveStart(vTitleBAT + BatteryPercentage + BatteryStatus, " ");
                    AVActions.DispatcherInvoke(delegate
                    {
                        textblock_CurrentBat.Text = stringDisplay;
                        stackpanel_CurrentBat.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentBat.Visibility = Visibility.Collapsed;
                    });
                }
            }
            catch { }
        }
    }
}