using ArnoldVinkCode;
using LibreHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVDisplayMonitor;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

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
                vHardwareComputer.IsMotherboardEnabled = true;
                vHardwareComputer.IsBatteryEnabled = true;
                vHardwareComputer.Open();

                AVActions.TaskStartLoop(LoopMonitorHardware, vTask_MonitorHardware);

                Debug.WriteLine("Started monitoring hardware.");
            }
            catch { }
        }

        async Task LoopMonitorHardware()
        {
            try
            {
                while (TaskCheckLoop(vTask_MonitorHardware))
                {
                    try
                    {
                        //Update the monitor information
                        UpdateMonitorInformation();

                        //Network usage variables
                        float networkUpFloat = 0;
                        float networkDownFloat = 0;

                        //Fan speed variables
                        float cpuFanSpeedFloat = 0;

                        //Update the hardware information
                        foreach (IHardware hardwareItem in vHardwareComputer.Hardware)
                        {
                            try
                            {
                                UpdateFanInformation(hardwareItem, ref cpuFanSpeedFloat);
                                UpdateCpuInformation(hardwareItem, cpuFanSpeedFloat);
                                UpdateGpuInformation(hardwareItem);
                                UpdateMemoryInformation(hardwareItem);
                                UpdateBatteryInformation(hardwareItem);
                                UpdateNetworkInformation(hardwareItem, ref networkUpFloat, ref networkDownFloat);
                            }
                            catch { }
                        }
                    }
                    catch { }
                    finally
                    {
                        //Delay the loop task
                        int hardwareUpdateRate = Convert.ToInt32(Setting_Load(vConfigurationFpsOverlayer, "HardwareUpdateRateMs"));
                        await TaskDelayLoop(hardwareUpdateRate, vTask_MonitorHardware);
                    }
                }
            }
            catch { }
        }

        //Update the network information
        void UpdateNetworkInformation(IHardware hardwareItem, ref float networkUpFloat, ref float networkDownFloat)
        {
            try
            {
                //Check if the information is visible
                bool showCurrentUsage = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "NetShowCurrentUsage"));
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

        //Update the battery information
        void UpdateBatteryInformation(IHardware hardwareItem)
        {
            try
            {
                //Check if the information is visible
                bool showPercentage = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BatShowPercentage"));
                if (!showPercentage)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentBat.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                if (hardwareItem.HardwareType == HardwareType.Battery)
                {
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
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            textblock_CurrentBat.Text = stringDisplay;
                            stackpanel_CurrentBat.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            stackpanel_CurrentBat.Visibility = Visibility.Collapsed;
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
                bool showPercentage = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowPercentage"));
                bool showUsed = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowUsed"));
                bool showFree = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowFree"));
                bool showTotal = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MemShowTotal"));
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
                bool showName = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowName"));
                bool showPercentage = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowPercentage"));
                bool showTemperature = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowTemperature"));
                bool showMemoryUsed = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowMemoryUsed"));
                bool showCoreFrequency = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowCoreFrequency"));
                bool showFanSpeed = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowFanSpeed"));
                bool showPowerWatt = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowPowerWatt"));
                bool showPowerVolt = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "GpuShowPowerVolt"));
                if (!showName && !showPercentage && !showTemperature && !showMemoryUsed && !showCoreFrequency && !showFanSpeed && !showPowerWatt && !showPowerVolt)
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
                    string GpuPowerWattage = string.Empty;
                    string GpuPowerVoltage = string.Empty;

                    //Set the gpu name
                    if (showName)
                    {
                        GpuName = hardwareItem.Name;
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
                                if (sensor.Name == "GPU Memory Used" || sensor.Name.Contains("Dedicated Memory Used"))
                                {
                                    float RawMemoryUsage = (float)sensor.Value;
                                    if (RawMemoryUsage < 1000)
                                    {
                                        GpuMemory = " " + RawMemoryUsage.ToString("0") + "MB";
                                    }
                                    else
                                    {
                                        GpuMemory = " " + (RawMemoryUsage / 1000).ToString("0.0") + "GB";
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
                            else if (showFanSpeed && (sensor.SensorType == SensorType.Fan || sensor.SensorType == SensorType.Control))
                            {
                                //Debug.WriteLine("GPU Fan: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                if (string.IsNullOrWhiteSpace(GpuFanSpeed))
                                {
                                    if (sensor.Identifier.ToString().EndsWith("fan/0"))
                                    {
                                        GpuFanSpeed = " " + sensor.Value.ToString() + "RPM";
                                    }
                                    else if (sensor.Name == "GPU Fan")
                                    {
                                        GpuFanSpeed = " " + sensor.Value.ToString() + "%";
                                    }
                                }
                            }
                            else if (showPowerWatt && sensor.SensorType == SensorType.Power)
                            {
                                //Debug.WriteLine("GPU Wattage: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                GpuPowerWattage = " " + Convert.ToInt32(sensor.Value) + "W";
                            }
                            else if (showPowerVolt && sensor.SensorType == SensorType.Voltage)
                            {
                                //Debug.WriteLine("GPU Voltage: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                float RawPowerVoltage = (float)sensor.Value;
                                if (RawPowerVoltage <= 0)
                                {
                                    GpuPowerVoltage = " 0V";
                                }
                                else
                                {
                                    GpuPowerVoltage = " " + RawPowerVoltage.ToString("0.000") + "V";
                                }
                            }
                        }
                        catch { }
                    }

                    bool gpuNameNullOrWhiteSpace = string.IsNullOrWhiteSpace(GpuName);
                    if (!gpuNameNullOrWhiteSpace || !string.IsNullOrWhiteSpace(GpuPercentage) || !string.IsNullOrWhiteSpace(GpuTemperature) || !string.IsNullOrWhiteSpace(GpuFrequency) || !string.IsNullOrWhiteSpace(GpuMemory) || !string.IsNullOrWhiteSpace(GpuFanSpeed) || !string.IsNullOrWhiteSpace(GpuPowerWattage) || !string.IsNullOrWhiteSpace(GpuPowerVoltage))
                    {
                        string stringDisplay = string.Empty;
                        string stringStats = AVFunctions.StringRemoveStart(vTitleGPU + GpuPercentage + GpuTemperature + GpuFrequency + GpuMemory + GpuFanSpeed + GpuPowerWattage + GpuPowerVoltage, " ");
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

        //Update the fan information
        void UpdateFanInformation(IHardware hardwareItem, ref float cpuFanSpeedFloat)
        {
            try
            {
                //Check if the information is visible
                bool showCpuShowFanSpeed = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowFanSpeed"));
                if (!showCpuShowFanSpeed)
                {
                    return;
                }

                if (hardwareItem.HardwareType == HardwareType.Motherboard)
                {
                    hardwareItem.Update();
                    foreach (IHardware subHardware in hardwareItem.SubHardware)
                    {
                        try
                        {
                            subHardware.Update();
                            foreach (ISensor sensor in subHardware.Sensors)
                            {
                                //Debug.WriteLine("Fan: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                try
                                {
                                    if (sensor.Name.Contains("CPU") && sensor.Identifier.ToString().EndsWith("fan/0"))
                                    {
                                        cpuFanSpeedFloat = (float)sensor.Value;
                                        break;
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        //Update the cpu information
        void UpdateCpuInformation(IHardware hardwareItem, float fanSpeedFloat)
        {
            try
            {
                //Check if the information is visible
                bool showName = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowName"));
                bool showPercentage = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowPercentage"));
                bool showTemperature = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowTemperature"));
                bool showCoreFrequency = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowCoreFrequency"));
                bool showPowerWatt = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowPowerWatt"));
                bool showPowerVolt = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowPowerVolt"));
                bool showFanSpeed = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "CpuShowFanSpeed"));
                if (!showName && !showPercentage && !showTemperature && !showCoreFrequency && !showPowerWatt && !showPowerVolt && !showFanSpeed)
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
                    string CpuPowerWattage = string.Empty;
                    string CpuPowerVoltage = string.Empty;
                    string CpuFanSpeed = string.Empty;

                    //Set the cpu name
                    if (showName)
                    {
                        CpuName = hardwareItem.Name;
                    }

                    //Set the cpu fan speed
                    if (showFanSpeed)
                    {
                        CpuFanSpeed = " " + Convert.ToInt32(fanSpeedFloat) + "RPM";
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
                    if (!cpuNameNullOrWhiteSpace || !string.IsNullOrWhiteSpace(CpuPercentage) || !string.IsNullOrWhiteSpace(CpuTemperature) || !string.IsNullOrWhiteSpace(CpuFrequency) || !string.IsNullOrWhiteSpace(CpuPowerWattage) || !string.IsNullOrWhiteSpace(CpuPowerVoltage) || !string.IsNullOrWhiteSpace(CpuFanSpeed))
                    {
                        string stringDisplay = string.Empty;
                        string stringStats = AVFunctions.StringRemoveStart(vTitleCPU + CpuPercentage + CpuTemperature + CpuFrequency + CpuFanSpeed + CpuPowerWattage + CpuPowerVoltage, " ");
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
                bool showResolution = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowResolution"));
                bool showDpiResolution = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowDpiResolution"));
                bool showColorBitDepth = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowColorBitDepth"));
                bool showRefreshRate = Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "MonShowRefreshRate"));
                if (!showResolution && !showColorBitDepth && !showRefreshRate)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentMon.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));

                //Get the screen resolution
                DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);
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