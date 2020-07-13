using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Reset HidGuardian to defaults
        public void HidGuardianResetDefaults()
        {
            try
            {
                //Remove all previous processes
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    registryKeyLocalMachine.DeleteSubKeyTree(@"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters\Whitelist");
                }
            }
            catch { }
            try
            {
                //Create empty Whitelist key
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    registryKeyLocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters\Whitelist");
                }
            }
            catch { }
            try
            {
                //Create empty AffectedDevices value
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyLocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters", true))
                    {
                        string stringAffectedDevices = openSubKey.GetValue("AffectedDevices") as string;
                        if (stringAffectedDevices == null)
                        {
                            openSubKey.SetValue("AffectedDevices", string.Empty);
                        }
                    }
                }
            }
            catch { }
        }

        //Allow DirectXInput process in HidGuardian
        public void HidGuardianAllowProcess()
        {
            try
            {
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    registryKeyLocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters\Whitelist\" + vProcessCurrent.Id);
                }

                Debug.WriteLine("Allowed DirectXInput process in HidGuardian.");
            }
            catch { }
        }

        //Allow the controller in HidGuardian
        public void HidGuardianAllowController(ControllerDetails ConnectedController)
        {
            try
            {
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyLocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters", true))
                    {
                        string[] stringArray = openSubKey.GetValue("AffectedDevices") as string[];
                        List<string> stringList = (stringArray != null) ? new List<string>(stringArray) : new List<string>();
                        if (!stringList.Contains(ConnectedController.HardwareId))
                        {
                            stringList.Add(ConnectedController.HardwareId);
                            openSubKey.SetValue("AffectedDevices", stringList.ToArray());
                            Debug.WriteLine("Added HidGuardian controller: " + ConnectedController.HardwareId);
                        }
                    }
                }
            }
            catch { }
        }

        //Release the controller from HidGuardian
        public void HidGuardianReleaseController(ControllerDetails ConnectedController)
        {
            try
            {
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyLocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters", true))
                    {
                        string[] stringArray = openSubKey.GetValue("AffectedDevices") as string[];
                        List<string> stringList = (stringArray != null) ? new List<string>(stringArray) : new List<string>();
                        if (stringList.Contains(ConnectedController.HardwareId))
                        {
                            stringList.Remove(ConnectedController.HardwareId);
                            openSubKey.SetValue("AffectedDevices", stringList.ToArray());
                            Debug.WriteLine("Released HidGuardian controller: " + ConnectedController.HardwareId);
                        }
                    }
                }
            }
            catch { }
        }
    }
}