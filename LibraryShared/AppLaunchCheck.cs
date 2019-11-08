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
        public static void Application_LaunchCheck(string ApplicationName, string ProcessName, bool PriorityRealTime, bool skipFileCheck)
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

                //Set the application priority to realtime
                if (PriorityRealTime)
                {
                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                }

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
                    string[] ApplicationFiles = { "CtrlUI.exe", "CtrlUI.exe.Config", "CtrlUI-Admin.exe", "CtrlUI-Admin.exe.Config", "DirectXInput.exe", "DirectXInput.exe.Config", "DirectXInput-Admin.exe", "DirectXInput-Admin.exe.Config", "DriverInstaller.exe", "DriverInstaller.exe.Config", "FpsOverlayer.exe", "FpsOverlayer.exe.Config", "FpsOverlayer-Admin.exe", "FpsOverlayer-Admin.exe.Config", "FpsOverlayer-Launcher.exe", "FpsOverlayer-Launcher.exe.Config", "KeyboardController.exe", "KeyboardController.exe.Config", "KeyboardController-Admin.exe", "KeyboardController-Admin.exe.Config", "KeyboardController-Launcher.exe", "KeyboardController-Launcher.exe.Config", "Updater.exe", "Updater.exe.Config" };
                    string[] ProfileFiles = { "Profiles\\Apps.json", "Profiles\\AppsBlacklistProcess.json", "Profiles\\AppsBlacklistShortcut.json", "Profiles\\AppsBlacklistShortcutUri.json", "Profiles\\AppsOtherLaunchers.json", "Profiles\\AppsOtherTools.json", "Profiles\\FileLocations.json", "Profiles\\Controllers.json", "Profiles\\ControllersSupported.json", "Profiles\\FpsBlacklistProcess.json", "Profiles\\FpsPositionProcess.json" };
                    string[] ResourcesFiles = { "Resources\\ArnoldVinkCertificate.cer", "Resources\\LibraryShared.dll", "Resources\\LibraryUsb.dll" };
                    string[] AssetsFiles = { "Assets\\Background.png", "Assets\\BoxArt.png" };
                    ApplicationFiles = ApplicationFiles.Concat(ProfileFiles).Concat(ResourcesFiles).Concat(AssetsFiles).ToArray();
                    foreach (string CheckFile in ApplicationFiles)
                    {
                        if (!File.Exists(CheckFile))
                        {
                            MessageBox.Show("File: " + CheckFile + " could not be found, please check your installation.", ApplicationName);
                            Environment.Exit(0);
                            return;
                        }
                    }
                }

                //Check - If the updater has been updated
                if (File.Exists("UpdaterNew.exe"))
                {
                    try
                    {
                        Debug.WriteLine("Renaming: UpdaterNew.exe to Updater.exe");
                        if (File.Exists("Updater.exe")) { File.Delete("Updater.exe"); }
                        File.Move("UpdaterNew.exe", "Updater.exe");
                    }
                    catch { }
                }

                //Check - If the updater failed to cleanup
                if (File.Exists("AppUpdate.zip"))
                {
                    try
                    {
                        File.Delete("AppUpdate.zip");
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}