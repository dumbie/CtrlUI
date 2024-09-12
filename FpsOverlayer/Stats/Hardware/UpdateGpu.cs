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
        void UpdateGpuInformation(IList<IHardware> hardwareItems)
        {
            try
            {
                //Check if the information is visible
                bool GpuShowName = SettingLoad(vConfigurationFpsOverlayer, "GpuShowName", typeof(bool));
                bool GpuShowPercentage = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPercentage", typeof(bool));
                bool GpuShowTemperature = SettingLoad(vConfigurationFpsOverlayer, "GpuShowTemperature", typeof(bool));
                bool GpuShowTemperatureHotspot = SettingLoad(vConfigurationFpsOverlayer, "GpuShowTemperatureHotspot", typeof(bool));
                bool GpuShowMemoryUsed = SettingLoad(vConfigurationFpsOverlayer, "GpuShowMemoryUsed", typeof(bool));
                bool GpuShowMemorySpeed = SettingLoad(vConfigurationFpsOverlayer, "GpuShowMemorySpeed", typeof(bool));
                bool GpuShowCoreFrequency = SettingLoad(vConfigurationFpsOverlayer, "GpuShowCoreFrequency", typeof(bool));
                bool GpuShowFanSpeed = SettingLoad(vConfigurationFpsOverlayer, "GpuShowFanSpeed", typeof(bool));
                bool GpuShowPowerWatt = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPowerWatt", typeof(bool));
                bool GpuShowPowerVolt = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPowerVolt", typeof(bool));
                bool FanShowGpu = SettingLoad(vConfigurationFpsOverlayer, "FanShowGpu", typeof(bool));
                if (!GpuShowName && !GpuShowPercentage && !GpuShowTemperature && !GpuShowTemperatureHotspot && !GpuShowMemoryUsed && !GpuShowMemorySpeed && !GpuShowCoreFrequency && !GpuShowFanSpeed && !GpuShowPowerWatt && !GpuShowPowerVolt && !FanShowGpu)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentGpu.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Select hardware item
                IHardware hardwareItem = hardwareItems.FirstOrDefault(x => x.HardwareType == HardwareType.GpuAmd || x.HardwareType == HardwareType.GpuNvidia || x.HardwareType == HardwareType.GpuIntel);

                //Update hardware item
                hardwareItem.Update();

                string GpuName = string.Empty;
                string GpuPercentage = string.Empty;
                string GpuTemperatureCore = string.Empty;
                string GpuTemperatureHotspot = string.Empty;
                string GpuMemoryUsed = string.Empty;
                string GpuMemorySpeed = string.Empty;
                string GpuFrequency = string.Empty;
                string GpuFanSpeed = string.Empty;
                bool GpuFanSpeedSet = false;
                string GpuPowerWattage = string.Empty;
                string GpuPowerVoltage = string.Empty;

                //Set the gpu name
                if (GpuShowName)
                {
                    GpuName = hardwareItem.Name;
                }

                foreach (ISensor sensor in hardwareItem.Sensors)
                {
                    try
                    {
                        if (GpuShowPercentage && sensor.SensorType == SensorType.Load)
                        {
                            //Debug.WriteLine("GPU Load: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (sensor.Identifier.ToString().EndsWith("load/0"))
                            {
                                GpuPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
                            }
                        }
                        else if ((GpuShowTemperature || GpuShowTemperatureHotspot) && sensor.SensorType == SensorType.Temperature)
                        {
                            //Debug.WriteLine("GPU Temp: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (GpuShowTemperature && sensor.Identifier.ToString().EndsWith("temperature/0"))
                            {
                                GpuTemperatureCore = " " + sensor.Value.ToString() + "°";
                            }
                            else if (GpuShowTemperatureHotspot && sensor.Name == "GPU Hot Spot")
                            {
                                GpuTemperatureHotspot = " " + sensor.Value.ToString() + "°";
                            }
                        }
                        else if (GpuShowMemoryUsed && sensor.SensorType == SensorType.SmallData)
                        {
                            //Debug.WriteLine("GPU Mem: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (sensor.Name == "GPU Memory Used" || sensor.Name.Contains("Dedicated Memory Used"))
                            {
                                float RawMemoryUsage = (float)sensor.Value;
                                if (RawMemoryUsage < 1000)
                                {
                                    GpuMemoryUsed = " " + RawMemoryUsage.ToString("0") + "MB";
                                }
                                else
                                {
                                    GpuMemoryUsed = " " + (RawMemoryUsage / 1000).ToString("0.0") + "GB";
                                }
                            }
                        }
                        else if ((GpuShowCoreFrequency || GpuShowMemorySpeed) && sensor.SensorType == SensorType.Clock)
                        {
                            //Debug.WriteLine("GPU Frequency: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (GpuShowCoreFrequency && sensor.Name == "GPU Core")
                            {
                                GpuFrequency = " " + ((float)sensor.Value).ToString("0") + "MHz";
                            }
                            else if (GpuShowMemorySpeed && sensor.Name == "GPU Memory")
                            {
                                GpuMemorySpeed = " " + ((float)sensor.Value).ToString("0") + "MTs";
                            }
                        }
                        else if ((GpuShowFanSpeed || FanShowGpu) && (sensor.SensorType == SensorType.Fan || sensor.SensorType == SensorType.Control))
                        {
                            //Debug.WriteLine("GPU Fan: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (!GpuFanSpeedSet)
                            {
                                if (sensor.Identifier.ToString().EndsWith("fan/0"))
                                {
                                    if (GpuShowFanSpeed)
                                    {
                                        GpuFanSpeed = " " + ((float)sensor.Value).ToString("0") + "RPM";
                                    }
                                    vHardwareGpuFanSpeed = ((float)sensor.Value).ToString("0") + "RPM";
                                    GpuFanSpeedSet = true;
                                }
                                else if (sensor.Name == "GPU Fan")
                                {
                                    if (GpuShowFanSpeed)
                                    {
                                        GpuFanSpeed = " " + sensor.Value.ToString() + "%";
                                    }
                                    vHardwareGpuFanSpeed = sensor.Value.ToString() + "%";
                                    GpuFanSpeedSet = true;
                                }
                            }
                        }
                        else if (GpuShowPowerWatt && sensor.SensorType == SensorType.Power)
                        {
                            //Debug.WriteLine("GPU Wattage: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            GpuPowerWattage = " " + Convert.ToInt32(sensor.Value) + "W";
                        }
                        else if (GpuShowPowerVolt && sensor.SensorType == SensorType.Voltage)
                        {
                            //Debug.WriteLine("GPU Voltage: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            float RawPowerVoltage = (float)sensor.Value;
                            GpuPowerVoltage = " " + RawPowerVoltage.ToString("0.000") + "V";
                        }
                    }
                    catch { }
                }

                //Add temperature tags
                if (!string.IsNullOrWhiteSpace(GpuTemperatureCore) && !string.IsNullOrWhiteSpace(GpuTemperatureHotspot))
                {
                    GpuTemperatureCore += "(C)";
                    GpuTemperatureHotspot += "(H)";
                }

                bool gpuNameNullOrWhiteSpace = string.IsNullOrWhiteSpace(GpuName);
                if (!gpuNameNullOrWhiteSpace || !string.IsNullOrWhiteSpace(GpuPercentage) || !string.IsNullOrWhiteSpace(GpuTemperatureCore) || !string.IsNullOrWhiteSpace(GpuTemperatureHotspot) || !string.IsNullOrWhiteSpace(GpuFrequency) || !string.IsNullOrWhiteSpace(GpuMemorySpeed) || !string.IsNullOrWhiteSpace(GpuMemoryUsed) || !string.IsNullOrWhiteSpace(GpuFanSpeed) || !string.IsNullOrWhiteSpace(GpuPowerWattage) || !string.IsNullOrWhiteSpace(GpuPowerVoltage))
                {
                    string stringDisplay = string.Empty;
                    string stringStats = AVFunctions.StringRemoveStart(vTitleGPU + GpuPercentage + GpuTemperatureCore + GpuTemperatureHotspot + GpuFrequency + GpuMemorySpeed + GpuMemoryUsed + GpuFanSpeed + GpuPowerWattage + GpuPowerVoltage, " ");
                    if (string.IsNullOrWhiteSpace(stringStats))
                    {
                        stringDisplay = GpuName;
                    }
                    else
                    {
                        if (gpuNameNullOrWhiteSpace)
                        {
                            stringDisplay = stringStats;
                        }
                        else
                        {
                            stringDisplay = GpuName + "\n" + stringStats;
                        }
                    }

                    AVActions.DispatcherInvoke(delegate
                    {
                        textblock_CurrentGpu.Text = stringDisplay;
                        stackpanel_CurrentGpu.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentGpu.Visibility = Visibility.Collapsed;
                    });
                }
            }
            catch { }
        }
    }
}