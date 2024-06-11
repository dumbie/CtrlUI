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
    public partial class WindowMain
    {
        void UpdateMemoryInformation(IList<IHardware> hardwareItems)
        {
            try
            {
                //Check if the information is visible
                bool showName = SettingLoad(vConfigurationFpsOverlayer, "MemShowName", typeof(bool));
                bool showSpeed = SettingLoad(vConfigurationFpsOverlayer, "MemShowSpeed", typeof(bool));
                bool showPowerVolt = SettingLoad(vConfigurationFpsOverlayer, "MemShowPowerVolt", typeof(bool));
                bool showPercentage = SettingLoad(vConfigurationFpsOverlayer, "MemShowPercentage", typeof(bool));
                bool showUsed = SettingLoad(vConfigurationFpsOverlayer, "MemShowUsed", typeof(bool));
                bool showFree = SettingLoad(vConfigurationFpsOverlayer, "MemShowFree", typeof(bool));
                bool showTotal = SettingLoad(vConfigurationFpsOverlayer, "MemShowTotal", typeof(bool));
                if (!showName && !showSpeed && !showPercentage && !showUsed && !showFree && !showTotal)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentMem.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Select hardware item
                IHardware hardwareItem = hardwareItems.FirstOrDefault(x => x.HardwareType == HardwareType.Memory);

                //Update hardware item
                hardwareItem.Update();

                string MemoryName = string.Empty;
                string MemorySpeed = string.Empty;
                string MemoryPowerVolt = string.Empty;
                string MemoryPercentage = string.Empty;
                string MemoryBytes = string.Empty;
                float RawMemoryUsed = 0;
                float RawMemoryFree = 0;

                //Set the memory name
                if (showName)
                {
                    MemoryName = vHardwareMemoryName;
                }

                //Set the memory speed
                if (showSpeed)
                {
                    MemorySpeed = " " + vHardwareMemorySpeed;
                }

                //Set the memory voltage
                if (showPowerVolt)
                {
                    MemoryPowerVolt = " " + vHardwareMemoryVoltage;
                }

                foreach (ISensor sensor in hardwareItem.Sensors)
                {
                    try
                    {
                        if (showPercentage && sensor.SensorType == SensorType.Load)
                        {
                            //Debug.WriteLine("Mem Load Percentage: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (sensor.Identifier.ToString().EndsWith("load/0"))
                            {
                                MemoryPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
                            }
                        }
                        else if (sensor.SensorType == SensorType.Data)
                        {
                            //Debug.WriteLine("Mem Load Data: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (sensor.Identifier.ToString().EndsWith("data/0"))
                            {
                                RawMemoryUsed = (float)sensor.Value;
                            }
                            else if (sensor.Identifier.ToString().EndsWith("data/1"))
                            {
                                RawMemoryFree = (float)sensor.Value;
                            }
                        }
                    }
                    catch { }
                }

                if (showUsed)
                {
                    MemoryBytes += " " + RawMemoryUsed.ToString("0.0") + "GB(U)";
                }
                if (showFree)
                {
                    MemoryBytes += " " + RawMemoryFree.ToString("0.0") + "GB(F)";
                }
                if (showTotal)
                {
                    MemoryBytes += " " + Convert.ToInt32(RawMemoryUsed + RawMemoryFree) + "GB(T)";
                }

                bool memoryNameNullOrWhiteSpace = string.IsNullOrWhiteSpace(MemoryName);
                if (!memoryNameNullOrWhiteSpace || !string.IsNullOrWhiteSpace(MemorySpeed) || !string.IsNullOrWhiteSpace(MemoryPowerVolt) || !string.IsNullOrWhiteSpace(MemoryPercentage) || !string.IsNullOrWhiteSpace(MemoryBytes))
                {
                    string stringDisplay = string.Empty;
                    string stringStats = AVFunctions.StringRemoveStart(vTitleMEM + MemoryPercentage + MemorySpeed + MemoryBytes + MemoryPowerVolt, " ");

                    if (string.IsNullOrWhiteSpace(stringStats))
                    {
                        stringDisplay = MemoryName;
                    }
                    else
                    {
                        if (memoryNameNullOrWhiteSpace)
                        {
                            stringDisplay = stringStats;
                        }
                        else
                        {
                            stringDisplay = MemoryName + "\n" + stringStats;
                        }
                    }

                    AVActions.DispatcherInvoke(delegate
                    {
                        textblock_CurrentMem.Text = stringDisplay;
                        stackpanel_CurrentMem.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentMem.Visibility = Visibility.Collapsed;
                    });
                }
            }
            catch { }
        }
    }
}