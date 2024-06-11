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
        void UpdateCpuInformation(IList<IHardware> hardwareItems)
        {
            try
            {
                //Check if the information is visible
                bool showCpuName = SettingLoad(vConfigurationFpsOverlayer, "CpuShowName", typeof(bool));
                bool showBoardName = SettingLoad(vConfigurationFpsOverlayer, "BoardShowName", typeof(bool));
                bool showPercentage = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPercentage", typeof(bool));
                bool showTemperature = SettingLoad(vConfigurationFpsOverlayer, "CpuShowTemperature", typeof(bool));
                bool showCoreFrequency = SettingLoad(vConfigurationFpsOverlayer, "CpuShowCoreFrequency", typeof(bool));
                bool showPowerWatt = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPowerWatt", typeof(bool));
                bool showPowerVolt = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPowerVolt", typeof(bool));
                bool showFanSpeed = SettingLoad(vConfigurationFpsOverlayer, "CpuShowFanSpeed", typeof(bool));
                bool FanShowCpu = SettingLoad(vConfigurationFpsOverlayer, "FanShowCpu", typeof(bool));
                if (!showCpuName && !showBoardName && !showPercentage && !showTemperature && !showCoreFrequency && !showPowerWatt && !showPowerVolt && !showFanSpeed)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentCpu.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Select hardware item
                IHardware hardwareItem = hardwareItems.FirstOrDefault(x => x.HardwareType == HardwareType.Cpu);

                //Update hardware item
                hardwareItem.Update();

                string CpuName = string.Empty;
                string BoardName = string.Empty;
                string CpuPercentage = string.Empty;
                string CpuTemperature = string.Empty;
                string CpuFrequency = string.Empty;
                string CpuPowerWattage = string.Empty;
                string CpuPowerVoltage = string.Empty;
                string CpuFanSpeed = string.Empty;

                //Set the processor name
                if (showCpuName)
                {
                    CpuName = hardwareItem.Name;
                }

                //Set the motherboard name
                if (showBoardName)
                {
                    BoardName = vHardwareMotherboardName;
                }

                //Set the cpu fan speed
                if (showFanSpeed)
                {
                    CpuFanSpeed = " " + vHardwareCpuFanSpeed;
                }

                foreach (ISensor sensor in hardwareItem.Sensors)
                {
                    try
                    {
                        if (showPercentage && sensor.SensorType == SensorType.Load)
                        {
                            //Debug.WriteLine("CPU Load: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (sensor.Name == "CPU Total" || sensor.Name == "CPU Core")
                            {
                                CpuPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
                            }
                        }
                        else if (showTemperature && sensor.SensorType == SensorType.Temperature)
                        {
                            //Debug.WriteLine("CPU Temp: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (sensor.Name == "CPU Package" || sensor.Name == "CPU Cores")
                            {
                                float RawCpuTemperature = (float)sensor.Value;
                                CpuTemperature = " " + RawCpuTemperature.ToString("0") + "°";
                            }
                        }
                        else if (showCoreFrequency && sensor.SensorType == SensorType.Clock)
                        {
                            //Debug.WriteLine("CPU Frequency: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (sensor.Name == "CPU Core #1")
                            {
                                float RawCpuFrequency = (float)sensor.Value;
                                if (RawCpuFrequency < 1000)
                                {
                                    CpuFrequency = " " + RawCpuFrequency.ToString("0") + "MHz";
                                }
                                else
                                {
                                    CpuFrequency = " " + (RawCpuFrequency / 1000).ToString("0.00") + "GHz";
                                }
                            }
                        }
                        else if (showPowerWatt && sensor.SensorType == SensorType.Power)
                        {
                            //Debug.WriteLine("CPU Wattage: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            if (sensor.Identifier.ToString().EndsWith("power/0"))
                            {
                                CpuPowerWattage = " " + Convert.ToInt32(sensor.Value) + "W";
                            }
                        }
                        else if (showPowerVolt && sensor.SensorType == SensorType.Voltage)
                        {
                            //Debug.WriteLine("CPU Voltage: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                            float RawPowerVoltage = (float)sensor.Value;
                            if (RawPowerVoltage <= 0)
                            {
                                CpuPowerVoltage = " 0V";
                            }
                            else
                            {
                                CpuPowerVoltage = " " + RawPowerVoltage.ToString("0.000") + "V";
                            }
                        }
                    }
                    catch { }
                }

                bool cpuNameNullOrWhiteSpace = string.IsNullOrWhiteSpace(CpuName);
                bool boardNameNullOrWhiteSpace = string.IsNullOrWhiteSpace(BoardName);
                if (!cpuNameNullOrWhiteSpace || !boardNameNullOrWhiteSpace || !string.IsNullOrWhiteSpace(CpuPercentage) || !string.IsNullOrWhiteSpace(CpuTemperature) || !string.IsNullOrWhiteSpace(CpuFrequency) || !string.IsNullOrWhiteSpace(CpuPowerWattage) || !string.IsNullOrWhiteSpace(CpuPowerVoltage) || !string.IsNullOrWhiteSpace(CpuFanSpeed))
                {
                    string stringDisplay = string.Empty;
                    string stringStats = AVFunctions.StringRemoveStart(vTitleCPU + CpuPercentage + CpuTemperature + CpuFrequency + CpuFanSpeed + CpuPowerWattage + CpuPowerVoltage, " ");
                    if (string.IsNullOrWhiteSpace(stringStats))
                    {
                        if (!cpuNameNullOrWhiteSpace && !boardNameNullOrWhiteSpace)
                        {
                            stringDisplay = CpuName + "\n" + BoardName;
                        }
                        else if (!cpuNameNullOrWhiteSpace)
                        {
                            stringDisplay = CpuName;
                        }
                        else if (!boardNameNullOrWhiteSpace)
                        {
                            stringDisplay = BoardName;
                        }
                    }
                    else
                    {
                        if (cpuNameNullOrWhiteSpace && boardNameNullOrWhiteSpace)
                        {
                            stringDisplay = stringStats;
                        }
                        else
                        {
                            if (!cpuNameNullOrWhiteSpace && !boardNameNullOrWhiteSpace)
                            {
                                stringDisplay = CpuName + "\n" + BoardName + "\n" + stringStats;
                            }
                            else if (!cpuNameNullOrWhiteSpace)
                            {
                                stringDisplay = CpuName + "\n" + stringStats;
                            }
                            else if (!boardNameNullOrWhiteSpace)
                            {
                                stringDisplay = BoardName + "\n" + stringStats;
                            }
                        }
                    }

                    AVActions.DispatcherInvoke(delegate
                    {
                        textblock_CurrentCpu.Text = stringDisplay;
                        stackpanel_CurrentCpu.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentCpu.Visibility = Visibility.Collapsed;
                    });
                }
            }
            catch { }
        }
    }
}