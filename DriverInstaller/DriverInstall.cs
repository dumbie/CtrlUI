using ArnoldVinkCode;
using Microsoft.Win32;
using System;
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
                ProgressBarUpdate(5, false);
                ElementEnableDisable(button_Driver_Install, false);
                ElementEnableDisable(button_Driver_Uninstall, false);
                ElementEnableDisable(button_Driver_Cleanup, false);
                ElementEnableDisable(button_Driver_Close, false);

                //Close running controller tools
                ProgressBarUpdate(10, false);
                await CloseControllerTools();

                //Start the driver installation
                ProgressBarUpdate(20, false);
                TextBoxAppend("Starting the driver installation.");

                //Remove unused devices and drivers
                ProgressBarUpdate(30, false);
                RemoveUnusedVigemVirtualBus();
                RemoveUnusedScpVirtualBus();
                RemoveUnusedXboxControllers();
                RemoveUnusedDS3Controllers();
                RemoveUnusedFakerInputDevices();
                UninstallHidGuardian();

                //Install FakerInput Driver
                ProgressBarUpdate(40, false);
                InstallFakerInput();

                //Install Virtual Bus Driver
                ProgressBarUpdate(55, false);
                InstallVirtualBus();

                //Install HidHide Driver
                ProgressBarUpdate(70, false);
                InstallHidHide();

                //Install DS3 USB Driver
                ProgressBarUpdate(85, false);
                InstallDualShock3();

                ProgressBarUpdate(100, false);
                TextBoxAppend("Driver installation completed.");
                TextBoxAppend("--- System reboot may be required ---");

                //Close the application
                await Application_Exit("Closing the driver installer in a bit.", true);
            }
            catch { }
        }

        void InstallFakerInput()
        {
            try
            {
                if (DeviceCreateNode("System", GuidClassSystem, @"Root\FakerInput"))
                {
                    TextBoxAppend("FakerInput Node created.");
                }

                if (DriverInstallInf(@"Resources\Drivers\FakerInput\x64\FakerInput.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("FakerInput Driver installed.");
                }
                else
                {
                    TextBoxAppend("FakerInput Driver not installed.");
                }
            }
            catch { }
        }

        void InstallVirtualBus()
        {
            try
            {
                if (DeviceCreateNode("System", GuidClassSystem, @"Nefarius\ViGEmBus\Gen1"))
                {
                    TextBoxAppend("Virtual Bus Node created.");
                }

                string osSystem = Environment.Is64BitOperatingSystem ? "x64" : "x86";
                if (DriverInstallInf(@"Resources\Drivers\ViGEmBus\" + osSystem + @"\ViGEmBus.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
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
                    TextBoxAppend("DualShock 3 USB Driver installed.");
                }
                else
                {
                    TextBoxAppend("DualShock 3 USB Driver not installed.");
                }
            }
            catch { }
        }

        void InstallHidHide()
        {
            try
            {
                if (DeviceCreateNode("System", GuidClassSystem, @"Root\HidHide"))
                {
                    TextBoxAppend("HidHide Node created.");
                }

                if (DriverInstallInf(@"Resources\Drivers\HidHide\x64\HidHide.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("HidHide Driver installed.");
                }
                else
                {
                    TextBoxAppend("HidHide Driver not installed.");
                }

                AddUpperFilter("HidHide");
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