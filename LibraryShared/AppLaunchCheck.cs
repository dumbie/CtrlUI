using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessFunctions;

namespace LibraryShared
{
    public partial class AppStartupCheck
    {
        public static string[] ApplicationFiles = { "CtrlUI.exe", "CtrlUI.exe.config", "CtrlUI.exe.csettings", "CtrlUI-Admin.exe", "CtrlUI-Admin.exe.config", "DirectXInput.exe", "DirectXInput.exe.config", "DirectXInput.exe.csettings", "DirectXInput-Admin.exe", "DirectXInput-Admin.exe.Config", "DriverInstaller.exe", "DriverInstaller.exe.Config", "FpsOverlayer.exe", "FpsOverlayer.exe.config", "FpsOverlayer.exe.csettings", "FpsOverlayer-Admin.exe", "FpsOverlayer-Admin.exe.config", "FpsOverlayer-Launcher.exe", "FpsOverlayer-Launcher.exe.config", "Updater.exe", "Updater.exe.config" };
        public static string[] ProfileFiles = { "Profiles/CtrlApplications.json", "Profiles/CtrlIgnoreProcessName.json", "Profiles/CtrlIgnoreLauncherName.json", "Profiles/CtrlIgnoreShortcutName.json", "Profiles/CtrlIgnoreShortcutUri.json", "Profiles/CtrlCloseLaunchers.json", "Profiles/DirectCloseTools.json", "Profiles/CtrlLocationsFile.json", "Profiles/CtrlLocationsShortcut.json", "Profiles/DirectControllersProfile.json", "Profiles/DirectControllersSupported.json", "Profiles/DirectKeypadMapping.json", "Profiles/DirectControllersIgnored.json", "Profiles/FpsPositionProcessName.json" };
        public static string[] ResourcesFiles = { "Resources/ArnoldVinkCertificate.cer", "Resources/LibraryShared.dll", "Resources/LibraryUsb.dll", "Resources/LibreHardwareMonitorLib.dll", "Resources/Newtonsoft.Json.dll" };
        public static string[] AssetsDefaultFiles = { "Assets/Default/Background.png", "Assets/Default/BackgroundLive.mp4", "Assets/BoxArt.png", "Assets/Default/Apps/Battle.net.png", "Assets/Default/Apps/Bethesda.png", "Assets/Default/Apps/DirectXInput.png", "Assets/Default/Apps/FpsOverlayer.png", "Assets/Default/Apps/Epic.png", "Assets/Default/Apps/GoG.png", "Assets/Default/Apps/Rockstar.png", "Assets/Default/Apps/Discord.png", "Assets/Default/Apps/Kodi.png", "Assets/Default/Apps/Edge.png", "Assets/Default/Apps/EA Desktop.png", "Assets/Default/Apps/Remote Play.png", "Assets/Default/Apps/Spotify.png", "Assets/Default/Apps/Steam.png", "Assets/Default/Apps/Unknown.png", "Assets/Default/Apps/Ubisoft.png", "Assets/Default/Apps/Xbox.png" };

        public static async Task Application_LaunchCheck(string applicationName, ProcessPriorityClass priorityLevel, bool skipFileCheck, bool focusActiveProcess)
        {
            try
            {
                Debug.WriteLine("Checking application status.");
                Process currentProcess = Process.GetCurrentProcess();
                string processName = currentProcess.ProcessName;
                Process[] activeProcesses = Process.GetProcessesByName(processName);

                //Check - If application is already running
                if (activeProcesses.Length > 1)
                {
                    Debug.WriteLine("Application is already running.");

                    //Show the active process
                    if (focusActiveProcess)
                    {
                        foreach (Process activeProcess in activeProcesses)
                        {
                            try
                            {
                                if (currentProcess.Id != activeProcess.Id)
                                {
                                    Debug.WriteLine("Showing active process: " + activeProcess.Id);
                                    await FocusProcessWindow(applicationName, activeProcess.Id, activeProcess.MainWindowHandle, 0, false, false);
                                }
                            }
                            catch { }
                        }
                    }

                    //Close the current process
                    Debug.WriteLine("Closing the process.");
                    Environment.Exit(0);
                    return;
                }

                //Set the working directory to executable directory
                try
                {
                    Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                }
                catch { }

                //Set the application priority level
                try
                {
                    currentProcess.PriorityClass = priorityLevel;
                }
                catch { }

                //Check - Windows version check
                if (AVFunctions.DevOsVersion() < 10)
                {
                    MessageBox.Show(applicationName + " only supports Windows 10 or newer.", applicationName);
                    Environment.Exit(0);
                    return;
                }

                //Check for missing application files
                if (!skipFileCheck)
                {
                    ApplicationFiles = ApplicationFiles.Concat(ProfileFiles).Concat(ResourcesFiles).Concat(AssetsDefaultFiles).ToArray();
                    foreach (string checkFile in ApplicationFiles)
                    {
                        try
                        {
                            if (!File.Exists(checkFile))
                            {
                                MessageBox.Show("File: " + checkFile + " could not be found, please check your installation.", applicationName);
                                Environment.Exit(0);
                                return;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }
    }
}