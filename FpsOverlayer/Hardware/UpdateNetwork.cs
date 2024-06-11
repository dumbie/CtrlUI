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
        void UpdateNetworkInformation(IList<IHardware> hardwareItems)
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

                //Select hardware item
                IHardware hardwareItem = hardwareItems.FirstOrDefault(x => x.HardwareType == HardwareType.Network && x.Name == "Ethernet");

                //Update hardware item
                hardwareItem.Update();

                float networkUpFloat = 0;
                float networkDownFloat = 0;
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
            catch { }
        }
    }
}