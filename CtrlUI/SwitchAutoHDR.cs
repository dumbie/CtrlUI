using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using static ArnoldVinkCode.AVProcess;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Enable Windows Auto HDR feature
        void EnableWindowsAutoHDRFeature()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey regKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Open Microsoft subkey
                    using (RegistryKey microsoftSubKey = regKeyCurrentUser.CreateSubKey("Software\\Microsoft\\DirectX\\UserGpuPreferences", true))
                    {
                        //Get global settings value
                        string globalSettingsString = microsoftSubKey.GetValue("DirectXUserGlobalSettings")?.ToString();

                        //Set global settings value
                        if (!string.IsNullOrWhiteSpace(globalSettingsString))
                        {
                            if (globalSettingsString.Contains("AutoHDREnable=0"))
                            {
                                globalSettingsString = globalSettingsString.Replace("AutoHDREnable=0", "AutoHDREnable=1");
                            }
                            if (globalSettingsString.Contains("SwapEffectUpgradeEnable=0"))
                            {
                                globalSettingsString = globalSettingsString.Replace("SwapEffectUpgradeEnable=0", "SwapEffectUpgradeEnable=1");
                            }

                            if (!globalSettingsString.Contains("AutoHDREnable"))
                            {
                                globalSettingsString = "AutoHDREnable=1;" + globalSettingsString;
                            }
                            if (!globalSettingsString.Contains("SwapEffectUpgradeEnable"))
                            {
                                globalSettingsString = "SwapEffectUpgradeEnable=1;" + globalSettingsString;
                            }
                            microsoftSubKey.SetValue("DirectXUserGlobalSettings", globalSettingsString);
                        }
                        else
                        {
                            microsoftSubKey.SetValue("DirectXUserGlobalSettings", "AutoHDREnable=1;SwapEffectUpgradeEnable=1;");
                        }
                    }
                }

                Debug.WriteLine("Enabled Windows Auto HDR feature.");
            }
            catch
            {
                Debug.WriteLine("Failed to enable Windows Auto HDR feature.");
                Notification_Show_Status("MonitorHDR", "Failed enabling Windows Auto HDR feature");
            }
        }

        //Check Auto HDR for application
        bool CheckApplicationAutoHDR(DataBindApp dataBindApp)
        {
            try
            {
                //Set application name
                string d3DName = string.Empty;
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    Debug.WriteLine("UWP and Win32Store applications do not support Windows Auto HDR.");
                    return false;
                }
                else
                {
                    d3DName = Path.GetFileName(dataBindApp.PathExe);
                }

                //Open the Windows registry
                using (RegistryKey regKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey applicationSubKey = regKeyCurrentUser.CreateSubKey("Software\\Microsoft\\Direct3D\\" + d3DName, false))
                    {
                        string currentD3DBehaviors = applicationSubKey.GetValue("D3DBehaviors")?.ToString();
                        if (currentD3DBehaviors.Contains("BufferUpgradeOverride=1"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed to check Windows Auto HDR for application.");
                return false;
            }
        }

        //Enable Auto HDR for unsupported application
        void EnableApplicationAutoHDR(DataBindApp dataBindApp)
        {
            try
            {
                //Set application name
                string d3DName = string.Empty;
                string d3DBehaviors = "DisableBufferUpgrade=0;BufferUpgradeOverride=1;BufferUpgradeEnable10Bit=1;";
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    //d3DName = dataBindApp.AppUserModelId;
                    Debug.WriteLine("UWP and Win32Store applications do not support Windows Auto HDR.");
                    return;
                }
                else
                {
                    d3DName = Path.GetFileName(dataBindApp.PathExe);
                }

                //Open the Windows registry
                using (RegistryKey regKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Create application subkey
                    using (RegistryKey applicationSubKey = regKeyCurrentUser.CreateSubKey("Software\\Microsoft\\Direct3D\\" + d3DName, true))
                    {
                        applicationSubKey.SetValue("Name", d3DName);
                        applicationSubKey.SetValue("D3DBehaviors", d3DBehaviors);
                    }
                }

                Debug.WriteLine("Enabled Windows Auto HDR support for: " + d3DName + "/" + d3DBehaviors);
                Notification_Show_Status("MonitorHDR", "Enabled Auto HDR, restart application");
            }
            catch
            {
                Debug.WriteLine("Failed to enable Windows Auto HDR for application.");
                Notification_Show_Status("MonitorHDR", "Failed enabling application Auto HDR");
            }
        }

        //Disable Auto HDR for unsupported application
        void DisableApplicationAutoHDR(DataBindApp dataBindApp)
        {
            try
            {
                //Set application name
                string d3DName = string.Empty;
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    //d3DName = dataBindApp.AppUserModelId;
                    Debug.WriteLine("UWP and Win32Store applications do not support Windows Auto HDR.");
                    return;
                }
                else
                {
                    d3DName = Path.GetFileName(dataBindApp.PathExe);
                }

                //Open the Windows registry
                using (RegistryKey regKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    regKeyCurrentUser.DeleteSubKey("Software\\Microsoft\\Direct3D\\" + d3DName, false);
                }

                Debug.WriteLine("Disabled Windows Auto HDR support for: " + d3DName);
                Notification_Show_Status("MonitorHDR", "Disabled Auto HDR, restart application");
            }
            catch
            {
                Debug.WriteLine("Failed to disable Windows Auto HDR for application.");
            }
        }
    }
}