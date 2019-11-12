using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using static DriverInstaller.DeviceManager;
using static LibraryUsb.NativeMethods_SetupApi;

namespace DriverInstaller
{
    public partial class WindowMain : Window
    {
        //Application Variables
        Guid ClassGuid_Hid = new Guid("745A17A0-74D3-11D0-B6FE-00A0C90F57DA");
        Guid ClassGuid_System = new Guid("4D36E97D-E325-11CE-BFC1-08002BE10318");
        bool RebootRequired = false;

        async Task InstallRequiredDrivers()
        {
            try
            {
                TextBoxAppend("Starting the driver installation.");
                ProgressBarUpdate(20, false);

                //Install Virtual Bus Driver
                ProgressBarUpdate(40, false);
                InstallVirtualBus();

                //Install HidGuardian Driver
                ProgressBarUpdate(60, false);
                InstallHidGuardian();

                //Install DS3 USB Driver
                ProgressBarUpdate(80, false);
                InstallDualShock3();

                TextBoxAppend("Driver installation completed.");
                TextBoxAppend("--- System reboot may be required ---");
                ProgressBarUpdate(90, false);

                //Close the application
                await Application_Exit("Closing the driver installer in a bit.");
            }
            catch { }
        }

        void InstallVirtualBus()
        {
            try
            {
                if (Create("System", ClassGuid_System, @"Root\ScpVBus"))
                {
                    TextBoxAppend("Virtual Bus Driver created.");
                }

                if (Install(@"Resources\Drivers\ScpVBus\ScpVBus.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref RebootRequired))
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
                if (Install(@"Resources\Drivers\Ds3Controller\Ds3Controller.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref RebootRequired))
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
                if (Create("System", ClassGuid_System, @"Root\HidGuardian"))
                {
                    TextBoxAppend("HidGuardian Driver created.");
                }

                if (Install(@"Resources\Drivers\HidGuardian\HidGuardian.inf", DIIRFLAG.DIIRFLAG_FORCE_INF, ref RebootRequired))
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

        void AddUpperFilter(string FilterName)
        {
            try
            {
                using (RegistryKey RegisteryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey OpenSubKey = RegisteryKeyLocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{" + ClassGuid_Hid.ToString() + "}", true))
                    {
                        string[] stringArray = OpenSubKey.GetValue("UpperFilters") as string[];
                        List<string> stringList = (stringArray != null) ? new List<string>(stringArray) : new List<string>();
                        if (!stringList.Contains(FilterName))
                        {
                            stringList.Add(FilterName);
                            OpenSubKey.SetValue("UpperFilters", stringList.ToArray());
                            TextBoxAppend("Added upper filter: " + FilterName);
                        }
                    }
                }
            }
            catch { }
        }
    }
}