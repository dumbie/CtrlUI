using ArnoldVinkCode;
using LibreHardwareMonitor.Hardware;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVDisplayMonitor;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        void StartMonitorHardware()
        {
            try
            {
                vHardwareComputer = new Computer();
                vHardwareComputer.IsCpuEnabled = true;
                vHardwareComputer.IsGpuEnabled = true;
                vHardwareComputer.IsMemoryEnabled = true;
                vHardwareComputer.IsNetworkEnabled = true;
                vHardwareComputer.Open();

                vTaskToken_MonitorHardware = new CancellationTokenSource();
                vTask_MonitorHardware = AVActions.TaskStart(MonitorHardware, vTaskToken_MonitorHardware);

                Debug.WriteLine("Started monitoring hardware.");
            }
            catch { }
        }

        async void MonitorHardware()
        {
            try
            {
                while (TaskRunningCheck(vTaskToken_MonitorHardware))
                {
                    try
                    {
                        //Update the monitor information
                        UpdateMonitorInformation();

                        //Update the hardware information
                        foreach (IHardware hardwareItem in vHardwareComputer.Hardware)
                        {
                            try
                            {
                                UpdateCpuInformation(hardwareItem);
                                UpdateGpuInformation(hardwareItem);
                                UpdateMemoryInformation(hardwareItem);
                                UpdateNetworkInformation(hardwareItem);
                            }
                            catch { }
                        }

                        int hardwareUpdateRate = Convert.ToInt32(ConfigurationManager.AppSettings["HardwareUpdateRateMs"]);
                        await Task.Delay(hardwareUpdateRate);
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Update the network information
        void UpdateNetworkInformation(IHardware hardwareItem)
        {
            try
            {
                //Check if the information is visible
                bool showCurrentUsage = Convert.ToBoolean(ConfigurationManager.AppSettings["NetShowCurrentUsage"]);
                if (!showCurrentUsage)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentNet.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                if (hardwareItem.HardwareType == HardwareType.Network)
                {
                    hardwareItem.Update();
                    string networkUsage = string.Empty;
                    float networkUpFloat = 0;
                    float networkDownFloat = 0;

                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        try
                        {
                            if (sensor.SensorType == SensorType.Throughput)
                            {
                                //Debug.WriteLine("Network Data: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Identifier.ToString().EndsWith("throughput/7"))
                                {
                                    networkUpFloat += (float)sensor.Value;
                                }
                                else if (sensor.Identifier.ToString().EndsWith("throughput/8"))
                                {
                                    networkDownFloat += (float)sensor.Value;
                                }
                            }
                        }
                        catch { }
                    }

                    string networkDownString = AVFunctions.ConvertBytesSizeToString(networkDownFloat) + " DL ";
                    string networkUpString = AVFunctions.ConvertBytesSizeToString(networkUpFloat) + " UP";
                    networkUsage += " " + networkDownString + networkUpString;

                    if (!string.IsNullOrWhiteSpace(networkUsage))
                    {
                        string stringDisplay = AVFunctions.StringRemoveStart(vTitleNET + networkUsage, " ");
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            textblock_CurrentNet.Text = stringDisplay;
                            stackpanel_CurrentNet.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            stackpanel_CurrentNet.Visibility = Visibility.Collapsed;
                        });
                    }
                }
            }
            catch { }
        }

        //Update the memory information
        void UpdateMemoryInformation(IHardware hardwareItem)
        {
            try
            {
                //Check if the information is visible
                bool showPercentage = Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowPercentage"]);
                bool showUsed = Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowUsed"]);
                bool showFree = Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowFree"]);
                bool showTotal = Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowTotal"]);
                if (!showPercentage && !showUsed && !showFree && !showTotal)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentMem.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                if (hardwareItem.HardwareType == HardwareType.Memory)
                {
                    hardwareItem.Update();
                    string MemoryPercentage = string.Empty;
                    string MemoryBytes = string.Empty;
                    float RawMemoryUsed = 0;
                    float RawMemoryFree = 0;

                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        try
                        {
                            if (showPercentage && sensor.SensorType == SensorType.Load)
                            {
                                //Debug.WriteLine("Mem Load Perc: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                MemoryPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
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
                        MemoryBytes += " " + RawMemoryUsed.ToString("0.0") + "GB";
                    }
                    if (showFree)
                    {
                        MemoryBytes += " " + RawMemoryFree.ToString("0.0") + "GB";
                    }
                    if (showTotal)
                    {
                        MemoryBytes += " " + Convert.ToInt32(RawMemoryUsed + RawMemoryFree) + "GB";
                    }

                    if (!string.IsNullOrWhiteSpace(MemoryPercentage) || !string.IsNullOrWhiteSpace(MemoryBytes))
                    {
                        string stringDisplay = AVFunctions.StringRemoveStart(vTitleMEM + MemoryPercentage + MemoryBytes, " ");
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            textblock_CurrentMem.Text = stringDisplay;
                            stackpanel_CurrentMem.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            stackpanel_CurrentMem.Visibility = Visibility.Collapsed;
                        });
                    }
                }
            }
            catch { }
        }

        //Update the gpu information
        void UpdateGpuInformation(IHardware hardwareItem)
        {
            try
            {
                //Check if the information is visible
                bool showName = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowName"]);
                bool showPercentage = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowPercentage"]);
                bool showTemperature = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowTemperature"]);
                bool showMemoryUsed = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowMemoryUsed"]);
                bool showCoreFrequency = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowCoreFrequency"]);
                bool showFanSpeed = Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowFanSpeed"]);
                if (!showName && !showPercentage && !showTemperature && !showMemoryUsed && !showCoreFrequency && !showFanSpeed)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentGpu.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                if (hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAmd)
                {
                    hardwareItem.Update();
                    string GpuName = string.Empty;
                    string GpuPercentage = string.Empty;
                    string GpuTemperature = string.Empty;
                    string GpuMemory = string.Empty;
                    string GpuFrequency = string.Empty;
                    string GpuFanSpeed = string.Empty;

                    if (showName)
                    {
                        GpuName = hardwareItem.Name;
                    }
                    else
                    {
                        GpuName = string.Empty;
                    }

                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        try
                        {
                            if (showPercentage && sensor.SensorType == SensorType.Load)
                            {
                                //Debug.WriteLine("GPU Load: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Identifier.ToString().EndsWith("load/0"))
                                {
                                    GpuPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
                                }
                            }
                            else if (showTemperature && sensor.SensorType == SensorType.Temperature)
                            {
                                //Debug.WriteLine("GPU Temp: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Identifier.ToString().EndsWith("temperature/0"))
                                {
                                    GpuTemperature = " " + sensor.Value.ToString() + "°";
                                }
                            }
                            else if (showMemoryUsed && sensor.SensorType == SensorType.SmallData)
                            {
                                //Debug.WriteLine("GPU Mem: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Name == "GPU Memory Used")
                                {
                                    float RawMemoryUsage = (float)sensor.Value;
                                    if (RawMemoryUsage < 1024)
                                    {
                                        GpuMemory = " " + RawMemoryUsage.ToString("0") + "MB";
                                    }
                                    else
                                    {
                                        GpuMemory = " " + (RawMemoryUsage / 1024).ToString("0.0") + "GB";
                                    }
                                }
                            }
                            else if (showCoreFrequency && sensor.SensorType == SensorType.Clock)
                            {
                                //Debug.WriteLine("GPU Frequency: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Name == "GPU Core")
                                {
                                    GpuFrequency = " " + ((float)sensor.Value).ToString("0") + "MHz";
                                }
                            }
                            else if (showFanSpeed && sensor.SensorType == SensorType.Fan)
                            {
                                //Debug.WriteLine("GPU Fan: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Identifier.ToString().EndsWith("fan/0"))
                                {
                                    GpuFanSpeed = " " + sensor.Value.ToString() + "RPM";
                                }
                            }
                        }
                        catch { }
                    }

                    bool gpuNameNullOrWhiteSpace = string.IsNullOrWhiteSpace(GpuName);
                    if (!gpuNameNullOrWhiteSpace || !string.IsNullOrWhiteSpace(GpuPercentage) || !string.IsNullOrWhiteSpace(GpuTemperature) || !string.IsNullOrWhiteSpace(GpuFrequency) || !string.IsNullOrWhiteSpace(GpuMemory) || !string.IsNullOrWhiteSpace(GpuFanSpeed))
                    {
                        string stringDisplay = string.Empty;
                        string stringStats = AVFunctions.StringRemoveStart(vTitleGPU + GpuPercentage + GpuTemperature + GpuFrequency + GpuMemory + GpuFanSpeed, " ");
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

                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            textblock_CurrentGpu.Text = stringDisplay;
                            stackpanel_CurrentGpu.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            stackpanel_CurrentGpu.Visibility = Visibility.Collapsed;
                        });
                    }
                }
            }
            catch { }
        }

        //Update the cpu information
        void UpdateCpuInformation(IHardware hardwareItem)
        {
            try
            {
                //Check if the information is visible
                bool showName = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowName"]);
                bool showPercentage = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowPercentage"]);
                bool showTemperature = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowTemperature"]);
                bool showCoreFrequency = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowCoreFrequency"]);
                bool showPowerUsage = Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowPowerUsage"]);
                if (!showName && !showPercentage && !showTemperature && !showCoreFrequency && !showPowerUsage)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentCpu.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    hardwareItem.Update();
                    string CpuName = string.Empty;
                    string CpuPercentage = string.Empty;
                    string CpuTemperature = string.Empty;
                    string CpuFrequency = string.Empty;
                    string CpuPower = string.Empty;

                    if (showName)
                    {
                        CpuName = hardwareItem.Name;
                    }
                    else
                    {
                        CpuName = string.Empty;
                    }

                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        try
                        {
                            if (showPercentage && sensor.SensorType == SensorType.Load)
                            {
                                //Debug.WriteLine("CPU Load: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Identifier.ToString().EndsWith("load/0"))
                                {
                                    CpuPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
                                }
                            }
                            else if (showTemperature && sensor.SensorType == SensorType.Temperature)
                            {
                                //Debug.WriteLine("CPU Temp: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Name == "CPU Package")
                                {
                                    CpuTemperature = " " + sensor.Value.ToString() + "°";
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
                            else if (showPowerUsage && sensor.SensorType == SensorType.Power)
                            {
                                //Debug.WriteLine("CPU Power: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (sensor.Identifier.ToString().EndsWith("power/0"))
                                {
                                    CpuPower = " " + Convert.ToInt32(sensor.Value) + "W";
                                }
                            }
                        }
                        catch { }
                    }

                    bool cpuNameNullOrWhiteSpace = string.IsNullOrWhiteSpace(CpuName);
                    if (!cpuNameNullOrWhiteSpace || !string.IsNullOrWhiteSpace(CpuPercentage) || !string.IsNullOrWhiteSpace(CpuTemperature) || !string.IsNullOrWhiteSpace(CpuFrequency) || !string.IsNullOrWhiteSpace(CpuPower))
                    {
                        string stringDisplay = string.Empty;
                        string stringStats = AVFunctions.StringRemoveStart(vTitleCPU + CpuPercentage + CpuTemperature + CpuFrequency + CpuPower, " ");
                        if (string.IsNullOrWhiteSpace(stringStats))
                        {
                            stringDisplay = CpuName;
                        }
                        else
                        {
                            if (cpuNameNullOrWhiteSpace)
                            {
                                stringDisplay = stringStats;
                            }
                            else
                            {
                                stringDisplay = CpuName + "\n" + stringStats;
                            }
                        }

                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            textblock_CurrentCpu.Text = stringDisplay;
                            stackpanel_CurrentCpu.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            stackpanel_CurrentCpu.Visibility = Visibility.Collapsed;
                        });
                    }
                }
            }
            catch { }
        }

        //Update the monitor information
        void UpdateMonitorInformation()
        {
            try
            {
                //Check if the information is visible
                bool showResolution = Convert.ToBoolean(ConfigurationManager.AppSettings["MonShowResolution"]);
                bool showDpiResolution = Convert.ToBoolean(ConfigurationManager.AppSettings["MonShowDpiResolution"]);
                bool showColorBitDepth = Convert.ToBoolean(ConfigurationManager.AppSettings["MonShowColorBitDepth"]);
                bool showRefreshRate = Convert.ToBoolean(ConfigurationManager.AppSettings["MonShowRefreshRate"]);
                if (!showResolution && !showColorBitDepth && !showRefreshRate)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentMon.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Get the current active screen
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);

                //Get the screen resolution
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);
                string screenResolutionString = string.Empty;
                if (showResolution)
                {
                    if (showDpiResolution)
                    {
                        screenResolutionString = " " + displayMonitorSettings.WidthDpi + "x" + displayMonitorSettings.HeightDpi;
                    }
                    else
                    {
                        screenResolutionString = " " + displayMonitorSettings.WidthNative + "x" + displayMonitorSettings.HeightNative;
                    }
                }

                //Get the screen color bit depth
                string screenColorBitDepthString = string.Empty;
                if (showColorBitDepth)
                {
                    screenColorBitDepthString = " " + displayMonitorSettings.BitDepth + "Bits";
                }

                //Get the screen refresh rate
                string screenRefreshRateString = string.Empty;
                if (showRefreshRate)
                {
                    int screenRefreshRateInt = displayMonitorSettings.RefreshRate;
                    if (screenRefreshRateInt > 0)
                    {
                        if (showResolution || showColorBitDepth)
                        {
                            screenRefreshRateString = " @ " + screenRefreshRateInt + "Hz";
                        }
                        else
                        {
                            screenRefreshRateString = " " + screenRefreshRateInt + "Hz";
                        }
                    }
                }

                //Update the screen resolution
                string stringDisplay = AVFunctions.StringRemoveStart(vTitleMON + screenResolutionString + screenColorBitDepthString + screenRefreshRateString, " ");
                AVActions.ActionDispatcherInvoke(delegate
                {
                    textblock_CurrentMon.Text = stringDisplay;
                    stackpanel_CurrentMon.Visibility = Visibility.Visible;
                });
            }
            catch { }
        }
    }
}