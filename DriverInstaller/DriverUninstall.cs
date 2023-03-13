using ArnoldVinkCode;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static DriverInstaller.AppVariables;
using static LibraryUsb.DeviceManager;
using static LibraryUsb.Enumerate;
using static LibraryUsb.NativeMethods_Guid;
using static LibraryUsb.NativeMethods_SetupApi;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        //Uninstall the required drivers
        void button_Driver_Uninstall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        await UninstallRequiredDrivers();
                    }
                    catch { }
                }
                AVActions.TaskStartBackground(TaskAction);
            }
            catch { }
        }

        async Task UninstallRequiredDrivers()
        {
            try
            {
                //Disable the buttons
                ProgressBarUpdate(5, false);
                ElementEnableDisable(button_Driver_Install, false);
                ElementEnableDisable(button_Driver_Uninstall, false);
                ElementEnableDisable(button_Driver_Cleanup, false);
                ElementEnableDisable(button_Driver_Close, false);

                //Close running controller tools
                ProgressBarUpdate(10, false);
                await CloseControllerTools();

                //Start the driver uninstallation
                ProgressBarUpdate(20, false);
                TextBoxAppend("Starting the driver uninstallation.");

                //Remove unused devices and drivers
                ProgressBarUpdate(30, false);
                RemoveUnusedVigemVirtualBus();
                RemoveUnusedScpVirtualBus();
                RemoveUnusedXboxControllers();
                RemoveUnusedDS3Controllers();
                RemoveUnusedFakerInputDevices();
                UninstallHidGuardian();

                //Uninstall FakerInput Driver
                ProgressBarUpdate(40, false);
                UninstallFakerInput();

                //Uninstall Virtual Bus Driver
                ProgressBarUpdate(55, false);
                UninstallVirtualBus();

                //Uninstall HidHide Driver
                ProgressBarUpdate(70, false);
                UninstallHidHide();

                //Uninstall DS3 USB Driver
                ProgressBarUpdate(85, false);
                UninstallDualShock3();

                ProgressBarUpdate(100, false);
                TextBoxAppend("Driver uninstallation completed.");
                TextBoxAppend("--- System reboot may be required ---");

                //Close the application
                await Application_Exit("Closing the driver installer in a bit.", false);
            }
            catch { }
        }

        void UninstallFakerInput()
        {
            try
            {
                List<FileInfo> infPaths = EnumerateDevicesStore("FakerInput.inf");
                foreach (FileInfo infPath in infPaths)
                {
                    try
                    {
                        if (DriverUninstallInf(infPath.FullName, DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                        {
                            TextBoxAppend("FakerInput Driver uninstalled.");
                        }
                        else
                        {
                            TextBoxAppend("FakerInput Driver not uninstalled.");
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        void UninstallVirtualBus()
        {
            try
            {
                List<FileInfo> infPaths = EnumerateDevicesStore("ViGEmBus.inf");
                foreach (FileInfo infPath in infPaths)
                {
                    try
                    {
                        if (DriverUninstallInf(infPath.FullName, DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                        {
                            TextBoxAppend("Virtual ViGEm Bus Driver uninstalled.");
                        }
                        else
                        {
                            TextBoxAppend("Virtual ViGEm Bus Driver not uninstalled.");
                        }
                    }
                    catch { }
                }

                infPaths = EnumerateDevicesStore("ScpVBus.inf");
                foreach (FileInfo infPath in infPaths)
                {
                    try
                    {
                        if (DriverUninstallInf(infPath.FullName, DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                        {
                            TextBoxAppend("Virtual ScpVBus Bus Driver uninstalled.");
                        }
                        else
                        {
                            TextBoxAppend("Virtual ScpVBus Bus Driver not uninstalled.");
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        void UninstallDualShock3()
        {
            try
            {
                List<FileInfo> infPaths = EnumerateDevicesStore("Ds3Controller.inf");
                foreach (FileInfo infPath in infPaths)
                {
                    try
                    {
                        if (DriverUninstallInf(infPath.FullName, DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                        {
                            TextBoxAppend("DualShock 3 USB Driver uninstalled.");
                        }
                        else
                        {
                            TextBoxAppend("DualShock 3 USB Driver not uninstalled.");
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        void UninstallHidGuardian()
        {
            try
            {
                List<FileInfo> infPaths = EnumerateDevicesStore("HidGuardian.inf");
                foreach (FileInfo infPath in infPaths)
                {
                    try
                    {
                        if (DriverUninstallInf(infPath.FullName, DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                        {
                            TextBoxAppend("HidGuardian Driver uninstalled.");
                        }
                        else
                        {
                            TextBoxAppend("HidGuardian Driver not uninstalled.");
                        }
                    }
                    catch { }
                }

                RemoveUpperFilter("HidGuardian");
            }
            catch { }
        }

        void UninstallHidHide()
        {
            try
            {
                List<FileInfo> infPaths = EnumerateDevicesStore("HidHide.inf");
                foreach (FileInfo infPath in infPaths)
                {
                    try
                    {
                        if (DriverUninstallInf(infPath.FullName, DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                        {
                            TextBoxAppend("HidHide Driver uninstalled.");
                        }
                        else
                        {
                            TextBoxAppend("HidHide Driver not uninstalled.");
                        }
                    }
                    catch { }
                }

                RemoveUpperFilter("HidHide");
            }
            catch { }
        }

        void RemoveUpperFilter(string filterName)
        {
            try
            {
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyLocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{" + GuidClassHidClass.ToString() + "}", true))
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