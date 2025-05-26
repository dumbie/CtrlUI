using ArnoldVinkCode;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVDevices.Enumerate;
using static ArnoldVinkCode.AVDevices.Interop;
using static DriverInstaller.AppVariables;
using static LibraryUsb.NativeMethods_Guid;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        //Install the required drivers
        void button_Driver_Install_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        await InstallRequiredDrivers();
                    }
                    catch { }
                }
                AVActions.TaskStartBackground(TaskAction);
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
                TextBoxAppend("--- There may be manual install popups ---");

                //Remove unused devices and drivers
                ProgressBarUpdate(30, false);
                RemoveUnusedVigemG1VirtualBus();
                RemoveUnusedVigemG2VirtualBus();
                RemoveUnusedScpVirtualBus();
                RemoveUnusedXboxControllers();
                RemoveUnusedDS3Controllers();
                RemoveUnusedFakerInputDevices();

                //Install FakerInput Driver
                ProgressBarUpdate(40, false);
                InstallFakerInput();

                //Uninstall Virtual Bus Driver
                ProgressBarUpdate(50, false);
                UninstallScpVirtualBus();
                UninstallVigemG2VirtualBus();

                //Install Virtual Bus Driver
                ProgressBarUpdate(60, false);
                InstallVigemG1VirtualBus();

                //Uninstall HidHide Driver
                ProgressBarUpdate(70, false);
                UninstallHidGuardian();

                //Install HidHide Driver
                ProgressBarUpdate(80, false);
                InstallHidHide();

                //Install DS3 USB Driver
                ProgressBarUpdate(90, false);
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

                if (DriverInstallInf(@"Drivers\FakerInput\x64\FakerInput.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
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

        void InstallVigemG2VirtualBus()
        {
            try
            {
                if (DeviceCreateNode("System", GuidClassSystem, @"Nefarius\VirtualPad"))
                {
                    TextBoxAppend("ViGEm G2 Virtual Bus Node created.");
                }

                if (DriverInstallInf(@"Drivers\VirtualPad\nssvpd.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("ViGEm G2 Virtual Bus Driver installed.");
                }
                else
                {
                    TextBoxAppend("ViGEm G2 Virtual Bus Driver not installed.");
                }
            }
            catch { }
        }

        void InstallVigemG1VirtualBus()
        {
            try
            {
                if (DeviceCreateNode("System", GuidClassSystem, @"Nefarius\ViGEmBus\Gen1"))
                {
                    TextBoxAppend("ViGEm G1 Virtual Bus Node created.");
                }

                if (DriverInstallInf(@"Drivers\ViGEmBus\x64\ViGEmBus.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("ViGEm G1 Virtual Bus Driver installed.");
                }
                else
                {
                    TextBoxAppend("ViGEm G1 Virtual Bus Driver not installed.");
                }
            }
            catch { }
        }

        void InstallScpVirtualBus()
        {
            try
            {
                if (DeviceCreateNode("System", GuidClassSystem, @"Root\ScpVBus"))
                {
                    TextBoxAppend("Scp Virtual Bus Node created.");
                }

                if (DriverInstallInf(@"Drivers\ScpVBus\x64\ScpVBus.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
                {
                    TextBoxAppend("Scp Virtual Bus Driver installed.");
                }
                else
                {
                    TextBoxAppend("Scp Virtual Bus Driver not installed.");
                }
            }
            catch { }
        }

        void InstallDualShock3()
        {
            try
            {
                if (DriverInstallInf(@"Drivers\Ds3Controller\Ds3Controller.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
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

                if (DriverInstallInf(@"Drivers\HidHide\x64\HidHide.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref vRebootRequired))
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