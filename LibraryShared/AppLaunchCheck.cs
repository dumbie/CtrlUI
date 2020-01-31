using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace LibraryShared
{
    public partial class AppLaunchCheck
    {
        public static string[] ApplicationFiles = { "CtrlUI.exe", "CtrlUI.exe.Config", "CtrlUI-Admin.exe", "CtrlUI-Admin.exe.Config", "DirectXInput.exe", "DirectXInput.exe.Config", "DirectXInput-Admin.exe", "DirectXInput-Admin.exe.Config", "DriverInstaller.exe", "DriverInstaller.exe.Config", "FpsOverlayer.exe", "FpsOverlayer.exe.Config", "FpsOverlayer-Admin.exe", "FpsOverlayer-Admin.exe.Config", "FpsOverlayer-Launcher.exe", "FpsOverlayer-Launcher.exe.Config", "KeyboardController.exe", "KeyboardController.exe.Config", "KeyboardController-Admin.exe", "KeyboardController-Admin.exe.Config", "KeyboardController-Launcher.exe", "KeyboardController-Launcher.exe.Config", "Updater.exe", "Updater.exe.Config" };
        public static string[] ProfileFiles = { "Profiles\\CtrlApplications.json", "Profiles\\CtrlIgnoreProcessName.json", "Profiles\\CtrlIgnoreShortcutName.json", "Profiles\\CtrlIgnoreShortcutUri.json", "Profiles\\CtrlCloseLaunchers.json", "Profiles\\DirectCloseTools.json", "Profiles\\CtrlLocationsFile.json", "Profiles\\CtrlLocationsShortcut.json", "Profiles\\DirectControllersProfile.json", "Profiles\\DirectControllersSupported.json", "Profiles\\FpsIgnoreProcessName.json", "Profiles\\FpsPositionProcessName.json" };
        public static string[] ResourcesFiles = { "Resources\\ArnoldVinkCertificate.cer", "Resources\\LibraryShared.dll", "Resources\\LibraryUsb.dll", "Resources\\LibreHardwareMonitorLib.dll", "Resources\\Newtonsoft.Json.dll" };
        public static string[] AssetsRootFiles = { "Assets\\Background.png", "Assets\\BackgroundLive.mp4", "Assets\\BoxArt.png" };
        public static string[] AssetsAppsFiles = { "Assets\\Apps\\Battle.net.png", "Assets\\Apps\\Bethesda.png", "Assets\\Apps\\DirectXInput.png", "Assets\\Apps\\Epic.png", "Assets\\Apps\\GoG.png", "Assets\\Apps\\Kodi.png", "Assets\\Apps\\Microsoft Edge.png", "Assets\\Apps\\Origin.png", "Assets\\Apps\\Remote Play.png", "Assets\\Apps\\Spotify.png", "Assets\\Apps\\Steam.png", "Assets\\Apps\\Unknown.png", "Assets\\Apps\\Uplay.png", "Assets\\Apps\\Xbox.png" };

        public static void Application_LaunchCheck(string ApplicationName, string ProcessName, ProcessPriorityClass priorityLevel, bool skipFileCheck)
        {
            try
            {
                Debug.WriteLine("Checking application status.");

                //Check - If application is already running
                if (Process.GetProcessesByName(ProcessName).Length > 1)
                {
                    Debug.WriteLine("Application is already running.");
                    Environment.Exit(0);
                    return;
                }

                //Set the working directory to executable directory
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

                //Set the application priority level
                Process.GetCurrentProcess().PriorityClass = priorityLevel;

                //Check - Windows version check
                if (AVFunctions.DevOsVersion() < 10)
                {
                    MessageBox.Show(ApplicationName + " only supports Windows 10 or newer.", ApplicationName);
                    Environment.Exit(0);
                    return;
                }

                //Check for missing application files
                if (!skipFileCheck)
                {
                    ApplicationFiles = ApplicationFiles.Concat(ProfileFiles).Concat(ResourcesFiles).Concat(AssetsRootFiles).Concat(AssetsAppsFiles).ToArray();
                    foreach (string checkFile in ApplicationFiles)
                    {
                        if (!File.Exists(checkFile))
                        {
                            MessageBox.Show("File: " + checkFile + " could not be found, please check your installation.", ApplicationName);
                            Environment.Exit(0);
                            return;
                        }
                    }
                }
            }
            catch { }
        }
    }
}