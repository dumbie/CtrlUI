using ArnoldVinkCode;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using static DriverInstaller.AppVariables;
using static LibraryUsb.DeviceManager;
using static LibraryUsb.NativeMethods_Guid;
using static LibraryUsb.NativeMethods_SetupApi;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        //Install the required drivers
        async void button_Driver_Install_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                async void TaskAction()
                {
                    try
                    {
                        await InstallRequiredDrivers();
                    }
                    catch { }
                }
                await AVActions.TaskStart(TaskAction);
            }
            catch { }
        }

        async Task InstallRequiredDrivers()
        {
            try
            {
                //Disable the buttons
                ElementEnableDisable(button_Driver_Install, false);
                ElementEnableDisable(button_Driver_Uninstall, false);
                ElementEnableDisable(button_Driver_Close, false);
                ProgressBarUpdate(5, false);

                //Close running controller tools
                await CloseControllerTools();
                ProgressBarUpdate(10, false);

                //Start the driver installation
                TextBoxAppend("Starting the driver installation.");
                ProgressBarUpdate(20, false);

                //Install Virtual Bus Driver
                ProgressBarUpdate(40, false);
                InstallVirtualBus();

                //Install HidGuardian Driver
                ProgressBarUpdate(60, false);
                InstallHidGuardian();

                //Install DS3 USB Driver
                ProgressBarUpdate(75, false);
                InstallDualShock3();

                //Remove Xbox Controllers
                ProgressBarUpdate(90, false);
                RemoveXboxControllers();

                TextBoxAppend("Driver installation completed.");
                TextBoxAppend("--- System reboot may be required ---");
                ProgressBarUpdate(100, false);

                //Close the application
                await Application_Exit("Closing the driver installer in a bit.", true);
            }
            catch { }
        }

        void InstallVirtualBus()
        {
            try
            {
                if (DeviceCreate("System", GuidClassSystem, @"Root\ScpVBus"))
                {
                    TextBoxAppend("Virtual Bus Driver created.");
                }

                if (DriverInstallInf(@"Resources\Drivers\ScpVBus\ScpVBus.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("Virtual Bus Driver installed.");
                }
                else
                {
                    TextBoxAppend("Virtual Bus Driver not installed.");
                }
            }
            catch { }
        }

        void InstallDualShock3()
        {
            try
            {
                if (DriverInstallInf(@"Resources\Drivers\Ds3Controller\Ds3Controller.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("DS3 USB Driver installed.");
                }
                else
                {
                    TextBoxAppend("DS3 USB Driver not installed.");
                }
            }
            catch { }
        }

        void InstallHidGuardian()
        {
            try
            {
                if (DeviceCreate("System", GuidClassSystem, @"Root\HidGuardian"))
                {
                    TextBoxAppend("HidGuardian Driver created.");
                }

                if (DriverInstallInf(@"Resources\Drivers\HidGuardian\HidGuardian.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("HidGuardian Driver installed.");
                }
                else
                {
                    TextBoxAppend("HidGuardian Driver not installed.");
                }

                AddUpperFilter("HidGuardian");
            }
            catch { }
        }

        void AddUpperFilter(string filterName)
        {
            try
            {
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyLocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{" + GuidClassHidClass.ToString() + "}", true))
                    {
                        string[] stringArray = openSubKey.GetValue("UpperFilters") as string[];
                        List<string> stringList = (stringArray != null) ? new List<string>(stringArray) : new List<string>();
                        if (!stringList.Contains(filterName))
                        {
                            stringList.Add(filterName);
                            openSubKey.SetValue("UpperFilters", stringList.ToArray());
                            TextBoxAppend("Added upper filter: " + filterName);
                        }
                    }
                }
            }
            catch { }
        }
    }
}