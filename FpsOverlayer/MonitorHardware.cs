using ArnoldVinkCode;
using LibreHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVSettings;
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
                vHardwareComputer.IsMotherboardEnabled = true;
                vHardwareComputer.IsBatteryEnabled = true;
                vHardwareComputer.Open();
                UpdateSMBiosInformation(vHardwareComputer.SMBios);

                AVActions.TaskStartLoop(LoopMonitorHardware, vTask_MonitorHardware);

                Debug.WriteLine("Started monitoring hardware.");
            }
            catch { }
        }

        //Update SMBios information
        void UpdateSMBiosInformation(SMBios smBios)
        {
            try
            {
                //Set motherboard name
                vHardwareMotherboardName = smBios.Board.ManufacturerName + " " + smBios.Board.ProductName;

                //Filter motherboard manufacturer
                vHardwareMotherboardName = vHardwareMotherboardName.Replace("To be filled by O.E.M.", "O.E.M.");
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Technology", string.Empty);
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Ltd.", string.Empty);
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Ltd", string.Empty);
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Co.,", string.Empty);
                vHardwareMotherboardName = vHardwareMotherboardName.Replace(" Co.", string.Empty);

                //Set memory details
                int memoryCount = 0;
                foreach (MemoryDevice memoryDevice in smBios.MemoryDevices)
                {
                    if (memoryDevice.Size > 0)
                    {
                        memoryCount++;
                        vHardwareMemoryName = memoryDevice.ManufacturerName + " " + memoryDevice.PartNumber + " (" + memoryCount + "X) " + memoryDevice.Type;
                        vHardwareMemorySpeed = memoryDevice.ConfiguredSpeed + "MTs";
                        vHardwareMemoryVoltage = (memoryDevice.ConfiguredVoltage / 1000F).ToString("0.000") + "V";
                    }
                }
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

                        //Processor variables
                        bool cpuDone = false;
                        float cpuFanSpeedFloat = 0;

                        //Videocard variables
                        bool gpuDone = false;

                        //Update the hardware information
                        foreach (IHardware hardwareItem in vHardwareComputer.Hardware)
                        {
                            try
                            {
                                UpdateFanInformation(hardwareItem, ref cpuFanSpeedFloat);
                                UpdateCpuInformation(hardwareItem, cpuFanSpeedFloat, ref cpuDone);
                                UpdateGpuInformation(hardwareItem, ref gpuDone);
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
                        int hardwareUpdateRate = SettingLoad(vConfigurationFpsOverlayer, "HardwareUpdateRateMs", typeof(int));
                        await TaskDelay(hardwareUpdateRate, vTask_MonitorHardware);
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
                bool showCurrentUsage = SettingLoad(vConfigurationFpsOverlayer, "NetShowCurrentUsage", typeof(bool));
                if (!showCurrentUsage)
                {
                    AVActions.DispatcherInvoke(delegate
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
                        AVActions.DispatcherInvoke(delegate
                        {
                            textblock_CurrentNet.Text = stringDisplay;
                            stackpanel_CurrentNet.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        AVActions.DispatcherInvoke(delegate
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
                bool showPercentage = SettingLoad(vConfigurationFpsOverlayer, "BatShowPercentage", typeof(bool));
                if (!showPercentage)
                {
                    AVActions.DispatcherInvoke(delegate
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
            }
            catch { }
        }

        //Update the memory information
        void UpdateMemoryInformation(IHardware hardwareItem)
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

                if (hardwareItem.HardwareType == HardwareType.Memory)
                {
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
            }
            catch { }
        }

        //Update the gpu information
        void UpdateGpuInformation(IHardware hardwareItem, ref bool gpuDone)
        {
            try
            {
                //Check if information is already set
                if (gpuDone) { return; }

                //Check if the information is visible
                bool showName = SettingLoad(vConfigurationFpsOverlayer, "GpuShowName", typeof(bool));
                bool showPercentage = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPercentage", typeof(bool));
                bool showTemperature = SettingLoad(vConfigurationFpsOverlayer, "GpuShowTemperature", typeof(bool));
                bool showMemoryUsed = SettingLoad(vConfigurationFpsOverlayer, "GpuShowMemoryUsed", typeof(bool));
                bool showCoreFrequency = SettingLoad(vConfigurationFpsOverlayer, "GpuShowCoreFrequency", typeof(bool));
                bool showFanSpeed = SettingLoad(vConfigurationFpsOverlayer, "GpuShowFanSpeed", typeof(bool));
                bool showPowerWatt = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPowerWatt", typeof(bool));
                bool showPowerVolt = SettingLoad(vConfigurationFpsOverlayer, "GpuShowPowerVolt", typeof(bool));
                if (!showName && !showPercentage && !showTemperature && !showMemoryUsed && !showCoreFrequency && !showFanSpeed && !showPowerWatt && !showPowerVolt)
                {
                    AVActions.DispatcherInvoke(delegate
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
                                GpuPowerVoltage = " " + RawPowerVoltage.ToString("0.000") + "V";
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

                        gpuDone = true;
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
            }
            catch { }
        }

        //Update the fan information
        void UpdateFanInformation(IHardware hardwareItem, ref float cpuFanSpeedFloat)
        {
            try
            {
                //Check if the information is visible
                bool showCpuShowFanSpeed = SettingLoad(vConfigurationFpsOverlayer, "CpuShowFanSpeed", typeof(bool));
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
        void UpdateCpuInformation(IHardware hardwareItem, float fanSpeedFloat, ref bool cpuDone)
        {
            try
            {
                //Check if information is already set
                if (cpuDone) { return; }

                //Check if the information is visible
                bool showCpuName = SettingLoad(vConfigurationFpsOverlayer, "CpuShowName", typeof(bool));
                bool showBoardName = SettingLoad(vConfigurationFpsOverlayer, "BoardShowName", typeof(bool));
                bool showPercentage = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPercentage", typeof(bool));
                bool showTemperature = SettingLoad(vConfigurationFpsOverlayer, "CpuShowTemperature", typeof(bool));
                bool showCoreFrequency = SettingLoad(vConfigurationFpsOverlayer, "CpuShowCoreFrequency", typeof(bool));
                bool showPowerWatt = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPowerWatt", typeof(bool));
                bool showPowerVolt = SettingLoad(vConfigurationFpsOverlayer, "CpuShowPowerVolt", typeof(bool));
                bool showFanSpeed = SettingLoad(vConfigurationFpsOverlayer, "CpuShowFanSpeed", typeof(bool));
                if (!showCpuName && !showBoardName && !showPercentage && !showTemperature && !showCoreFrequency && !showPowerWatt && !showPowerVolt && !showFanSpeed)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentCpu.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
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
                        CpuFanSpeed = " " + Convert.ToInt32(fanSpeedFloat) + "RPM";
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

                        cpuDone = true;
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
            }
            catch { }
        }

        //Update the monitor information
        void UpdateMonitorInformation()
        {
            try
            {
                //Check if the information is visible
                bool showResolution = SettingLoad(vConfigurationFpsOverlayer, "MonShowResolution", typeof(bool));
                bool showDpiResolution = SettingLoad(vConfigurationFpsOverlayer, "MonShowDpiResolution", typeof(bool));
                bool showColorBitDepth = SettingLoad(vConfigurationFpsOverlayer, "MonShowColorBitDepth", typeof(bool));
                bool showRefreshRate = SettingLoad(vConfigurationFpsOverlayer, "MonShowRefreshRate", typeof(bool));
                if (!showResolution && !showColorBitDepth && !showRefreshRate)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentMon.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));

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
                AVActions.DispatcherInvoke(delegate
                {
                    textblock_CurrentMon.Text = stringDisplay;
                    stackpanel_CurrentMon.Visibility = Visibility.Visible;
                });
            }
            catch { }
        }
    }
}