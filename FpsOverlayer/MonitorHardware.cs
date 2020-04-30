using ArnoldVinkCode;
using LibreHardwareMonitor.Hardware;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
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
                        //Update the current monitor
                        UpdateCurrentMonitor();

                        //Update the current hardware
                        string NetworkUsage = string.Empty;
                        float NetworkUpFloat = 0;
                        float NetworkDownFloat = 0;

                        foreach (IHardware hardwareItem in vHardwareComputer.Hardware)
                        {
                            try
                            {
                                if (hardwareItem.HardwareType == HardwareType.Cpu)
                                {
                                    hardwareItem.Update();
                                    string CpuPercentage = string.Empty;
                                    string CpuTemperature = string.Empty;
                                    string CpuFrequency = string.Empty;
                                    string CpuPower = string.Empty;

                                    foreach (ISensor sensor in hardwareItem.Sensors)
                                    {
                                        try
                                        {
                                            if (Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowPercentage"]) && sensor.SensorType == SensorType.Load)
                                            {
                                                //Debug.WriteLine("CPU Load: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                                if (sensor.Identifier.ToString().EndsWith("load/0"))
                                                {
                                                    CpuPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
                                                }
                                            }
                                            else if (Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowTemperature"]) && sensor.SensorType == SensorType.Temperature)
                                            {
                                                //Debug.WriteLine("CPU Temp: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                                if (sensor.Name == "CPU Package")
                                                {
                                                    CpuTemperature = " " + sensor.Value.ToString() + "°";
                                                }
                                            }
                                            else if (Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowCoreFrequency"]) && sensor.SensorType == SensorType.Clock)
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
                                            else if (Convert.ToBoolean(ConfigurationManager.AppSettings["CpuShowPowerUsage"]) && sensor.SensorType == SensorType.Power)
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

                                    if (!string.IsNullOrWhiteSpace(CpuTemperature) || !string.IsNullOrWhiteSpace(CpuPercentage) || !string.IsNullOrWhiteSpace(CpuFrequency) || !string.IsNullOrWhiteSpace(CpuPower))
                                    {
                                        string StringDisplay = AVFunctions.StringRemoveStart(vTitleCPU + CpuPercentage + CpuTemperature + CpuFrequency + CpuPower, " ");
                                        AVActions.ActionDispatcherInvoke(delegate
                                        {
                                            textblock_CurrentCpu.Text = StringDisplay;
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
                                            if (Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowPercentage"]) && sensor.SensorType == SensorType.Load)
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

                                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowUsed"]))
                                    {
                                        MemoryBytes += " " + RawMemoryUsed.ToString("0.0") + "GB";
                                    }
                                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowFree"]))
                                    {
                                        MemoryBytes += " " + RawMemoryFree.ToString("0.0") + "GB";
                                    }
                                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["MemShowTotal"]))
                                    {
                                        MemoryBytes += " " + Convert.ToInt32(RawMemoryUsed + RawMemoryFree) + "GB";
                                    }

                                    if (!string.IsNullOrWhiteSpace(MemoryPercentage) || !string.IsNullOrWhiteSpace(MemoryBytes))
                                    {
                                        string StringDisplay = AVFunctions.StringRemoveStart(vTitleMEM + MemoryPercentage + MemoryBytes, " ");
                                        AVActions.ActionDispatcherInvoke(delegate
                                        {
                                            textblock_CurrentMem.Text = StringDisplay;
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

                                if (hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAmd)
                                {
                                    hardwareItem.Update();
                                    string GpuPercentage = string.Empty;
                                    string GpuTemperature = string.Empty;
                                    string GpuMemory = string.Empty;
                                    string GpuFrequency = string.Empty;
                                    string GpuFanSpeed = string.Empty;

                                    foreach (ISensor sensor in hardwareItem.Sensors)
                                    {
                                        try
                                        {
                                            if (Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowPercentage"]) && sensor.SensorType == SensorType.Load)
                                            {
                                                //Debug.WriteLine("GPU Load: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                                if (sensor.Identifier.ToString().EndsWith("load/0"))
                                                {
                                                    GpuPercentage = " " + Convert.ToInt32(sensor.Value) + "%";
                                                }
                                            }
                                            else if (Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowTemperature"]) && sensor.SensorType == SensorType.Temperature)
                                            {
                                                //Debug.WriteLine("GPU Temp: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                                if (sensor.Identifier.ToString().EndsWith("temperature/0"))
                                                {
                                                    GpuTemperature = " " + sensor.Value.ToString() + "°";
                                                }
                                            }
                                            else if (Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowMemoryUsed"]) && sensor.SensorType == SensorType.SmallData)
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
                                            else if (Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowCoreFrequency"]) && sensor.SensorType == SensorType.Clock)
                                            {
                                                //Debug.WriteLine("GPU Frequency: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                                if (sensor.Name == "GPU Core")
                                                {
                                                    GpuFrequency = " " + ((float)sensor.Value).ToString("0") + "MHz";
                                                }
                                            }
                                            else if (Convert.ToBoolean(ConfigurationManager.AppSettings["GpuShowFanSpeed"]) && sensor.SensorType == SensorType.Fan)
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

                                    if (!string.IsNullOrWhiteSpace(GpuPercentage) || !string.IsNullOrWhiteSpace(GpuTemperature) || !string.IsNullOrWhiteSpace(GpuFrequency) || !string.IsNullOrWhiteSpace(GpuMemory) || !string.IsNullOrWhiteSpace(GpuFanSpeed))
                                    {
                                        string StringDisplay = AVFunctions.StringRemoveStart(vTitleGPU + GpuPercentage + GpuTemperature + GpuFrequency + GpuMemory + GpuFanSpeed, " ");
                                        AVActions.ActionDispatcherInvoke(delegate
                                        {
                                            textblock_CurrentGpu.Text = StringDisplay;
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

                                if (hardwareItem.HardwareType == HardwareType.Network)
                                {
                                    hardwareItem.Update();
                                    foreach (ISensor sensor in hardwareItem.Sensors)
                                    {
                                        try
                                        {
                                            if (sensor.SensorType == SensorType.Throughput)
                                            {
                                                //Debug.WriteLine("Network Data: " + sensor.Name + "/" + sensor.Identifier + "/" + sensor.Value.ToString());
                                                if (sensor.Identifier.ToString().EndsWith("throughput/7"))
                                                {
                                                    NetworkUpFloat += (float)sensor.Value;
                                                }
                                                else if (sensor.Identifier.ToString().EndsWith("throughput/8"))
                                                {
                                                    NetworkDownFloat += (float)sensor.Value;
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                            catch { }
                        }

                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["NetShowCurrentUsage"]))
                        {
                            string NetworkDownString = AVFunctions.ConvertBytesSizeToString(NetworkDownFloat) + " DL ";
                            string NetworkUpString = AVFunctions.ConvertBytesSizeToString(NetworkUpFloat) + " UP";
                            NetworkUsage += " " + NetworkDownString + NetworkUpString;
                        }

                        if (!string.IsNullOrWhiteSpace(NetworkUsage))
                        {
                            string StringDisplay = AVFunctions.StringRemoveStart(vTitleNET + NetworkUsage, " ");
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                textblock_CurrentNet.Text = StringDisplay;
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
                    catch { }
                    await Task.Delay(1500);
                }
            }
            catch { }
        }

        //Update the current monitor
        void UpdateCurrentMonitor()
        {
            try
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["MonShowResolution"]))
                {
                    //Improve add refresh rate support
                    //Get the current active screen
                    int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                    Screen targetScreen = GetScreenByNumber(monitorNumber, out bool monitorSuccess);

                    //Get the screen resolution
                    int screenWidth = targetScreen.Bounds.Width;
                    int screenHeight = targetScreen.Bounds.Height;
                    string screenResolutionString = " " + screenWidth + "x" + screenHeight;

                    //Get the screen refresh rate
                    string screenRefreshRateString = string.Empty;
                    int screenRefreshRateInt = GetScreenRefreshRate(targetScreen);
                    if (screenRefreshRateInt > 0)
                    {
                        screenRefreshRateString = " @ " + screenRefreshRateInt + "Hz";
                    }

                    //Update the screen resolution
                    string StringDisplay = AVFunctions.StringRemoveStart(vTitleMON + screenResolutionString + screenRefreshRateString, " ");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_CurrentMon.Text = StringDisplay;
                        stackpanel_CurrentMon.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        stackpanel_CurrentMon.Visibility = Visibility.Collapsed;
                    });
                }
            }
            catch { }
        }
    }
}