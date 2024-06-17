using ArnoldVinkCode;
using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        void UpdateFanInformation(IList<IHardware> hardwareItems)
        {
            try
            {
                //Check if the information is visible
                bool FanShowCpu = SettingLoad(vConfigurationFpsOverlayer, "FanShowCpu", typeof(bool));
                bool FanShowGpu = SettingLoad(vConfigurationFpsOverlayer, "FanShowGpu", typeof(bool));
                bool FanShowSystem = SettingLoad(vConfigurationFpsOverlayer, "FanShowSystem", typeof(bool));
                bool CpuShowFanSpeed = SettingLoad(vConfigurationFpsOverlayer, "CpuShowFanSpeed", typeof(bool));
                if (!FanShowCpu && !FanShowGpu && !FanShowSystem && !CpuShowFanSpeed)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentFan.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Select hardware item
                IHardware hardwareItem = hardwareItems.FirstOrDefault(x => x.HardwareType == HardwareType.Motherboard);

                //Update hardware item
                hardwareItem.Update();

                //Load hardware information
                List<string> systemFans = new List<string>();
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
                                if ((CpuShowFanSpeed || FanShowCpu) && sensor.Name.Contains("CPU") && sensor.Identifier.ToString().EndsWith("fan/0"))
                                {
                                    vHardwareCpuFanSpeed = ((float)sensor.Value).ToString("0") + "RPM";
                                }
                                else if ((sensor.Name.Contains("System") || sensor.Name.Contains("Optional")) && sensor.Identifier.ToString().Contains("/fan/"))
                                {
                                    float RawFanSpeed = (float)sensor.Value;
                                    if (RawFanSpeed > 0)
                                    {
                                        systemFans.Add(RawFanSpeed.ToString("0") + "RPM");
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                string stringStats = string.Empty;
                if (FanShowCpu && !string.IsNullOrWhiteSpace(vHardwareCpuFanSpeed))
                {
                    stringStats += " (C)" + vHardwareCpuFanSpeed;
                }
                if (FanShowGpu && !string.IsNullOrWhiteSpace(vHardwareGpuFanSpeed))
                {
                    stringStats += " (G)" + vHardwareGpuFanSpeed;
                }
                if (FanShowSystem && systemFans.Any())
                {
                    if (string.IsNullOrWhiteSpace(stringStats))
                    {
                        stringStats += " " + string.Join(" ", systemFans);
                    }
                    else
                    {
                        stringStats += " (S)" + string.Join(" ", systemFans);
                    }
                }

                if (!string.IsNullOrWhiteSpace(stringStats))
                {
                    string stringDisplay = AVFunctions.StringRemoveStart(vTitleFAN + stringStats, " ");
                    AVActions.DispatcherInvoke(delegate
                    {
                        textblock_CurrentFan.Text = stringDisplay;
                        stackpanel_CurrentFan.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_CurrentFan.Visibility = Visibility.Collapsed;
                    });
                }
            }
            catch { }
        }
    }
}