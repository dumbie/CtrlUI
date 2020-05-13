using ArnoldVinkCode;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using static DriverInstaller.AppVariables;
using static DriverInstaller.DeviceManager;
using static LibraryUsb.NativeMethods_SetupApi;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        //Uninstall the required drivers
        async void button_Driver_Uninstall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                async void TaskAction()
                {
                    try
                    {
                        await UninstallRequiredDrivers();
                    }
                    catch { }
                }
                await AVActions.TaskStart(TaskAction);
            }
            catch { }
        }

        async Task UninstallRequiredDrivers()
        {
            try
            {
                //Disable the buttons
                ElementEnableDisable(button_Driver_Install, false);
                ElementEnableDisable(button_Driver_Uninstall, false);
                ElementEnableDisable(button_Driver_Close, false);

                //Close running controller tools
                await CloseControllerTools();
                ProgressBarUpdate(10, false);

                //Start the driver uninstallation
                TextBoxAppend("Starting the driver uninstallation.");
                ProgressBarUpdate(20, false);

                //Uninstall Virtual Bus Driver
                ProgressBarUpdate(40, false);
                UninstallVirtualBus();

                //Uninstall HidGuardian Driver
                ProgressBarUpdate(60, false);
                UninstallHidGuardian();

                //Uninstall DS3 USB Driver
                ProgressBarUpdate(80, false);
                UninstallDualShock3();

                TextBoxAppend("Driver uninstallation completed.");
                TextBoxAppend("--- System reboot may be required ---");
                ProgressBarUpdate(90, false);

                //Close the application
                await Application_Exit("Closing the driver installer in a bit.", false);
            }
            catch { }
        }

        void UninstallVirtualBus()
        {
            try
            {
                if (Uninstall(@"Resources\Drivers\ScpVBus\ScpVBus.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("Virtual Bus Driver uninstalled.");
                }
                else
                {
                    TextBoxAppend("Virtual Bus Driver not uninstalled.");
                }
            }
            catch { }
        }

        void UninstallDualShock3()
        {
            try
            {
                if (Uninstall(@"Resources\Drivers\Ds3Controller\Ds3Controller.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("DS3 USB Driver uninstalled.");
                }
                else
                {
                    TextBoxAppend("DS3 USB Driver not uninstalled.");
                }
            }
            catch { }
        }

        void UninstallHidGuardian()
        {
            try
            {
                if (Uninstall(@"Resources\Drivers\HidGuardian\HidGuardian.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("HidGuardian Driver uninstalled.");
                }
                else
                {
                    TextBoxAppend("HidGuardian Driver not uninstalled.");
                }

                RemoveUpperFilter("HidGuardian");
            }
            catch { }
        }

        void RemoveUpperFilter(string filterName)
        {
            try
            {
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyLocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{" + vClassGuid_Hid.ToString() + "}", true))
                    {
                        string[] stringArray = openSubKey.GetValue("UpperFilters") as string[];
                        List<string> stringList = (stringArray != null) ? new List<string>(stringArray) : new List<string>();
                        if (stringList.Contains(filterName))
                        {
                            stringList.Remove(filterName);
                            openSubKey.SetValue("UpperFilters", stringList.ToArray());
                            TextBoxAppend("Removed upper filter: " + filterName);
                        }
                    }
                }
            }
            catch { }
        }
    }
}