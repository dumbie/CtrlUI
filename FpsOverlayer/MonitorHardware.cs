using ArnoldVinkCode;
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
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

                        //Update hardware information
                        UpdateGpuInformation(vHardwareComputer.Hardware);
                        UpdateFanInformation(vHardwareComputer.Hardware);
                        UpdateCpuInformation(vHardwareComputer.Hardware);
                        UpdateMemoryInformation(vHardwareComputer.Hardware);
                        UpdateBatteryInformation(vHardwareComputer.Hardware);
                        UpdateNetworkInformation(vHardwareComputer.Hardware);
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
    }
}