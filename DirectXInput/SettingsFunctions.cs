using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Processes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Reset the blocked controller path list
        void Btn_SearchNewControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vControllerBlockedPaths = new List<string>();
                Debug.WriteLine("Reset the blocked controller path list.");
            }
            catch { }
        }

        //Open Windows Game controller settings
        void btn_CheckControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessLauncherWin32(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\control.exe", "", "joy.cpl");
            }
            catch { }
        }

        //Open Windows device manager
        void btn_CheckDeviceManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessLauncherWin32(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\devmgmt.msc", "", "");
            }
            catch { }
        }

        //Check for available update
        async void btn_Settings_CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ResCurrentVersion = await AVDownloader.DownloadStringAsync(5000, "DirectXInput", null, new Uri("http://download.arnoldvink.com/CtrlUI.zip-version.txt" + "?nc=" + Environment.TickCount));
                if (!string.IsNullOrWhiteSpace(ResCurrentVersion) && ResCurrentVersion != Assembly.GetEntryAssembly().FullName.Split('=')[1].Split(',')[0])
                {
                    int Result = await MessageBoxPopup("A newer version has been found: v" + ResCurrentVersion, "Do you want to update the application to the newest version now?", "Update now", "Cancel", "", "");
                    if (Result == 1)
                    {
                        if (!CheckRunningProcessByName("Updater", false))
                        {
                            ProcessLauncherWin32("Updater.exe", "", "");
                            await Application_Exit(true);
                        }
                    }
                }
                else { await MessageBoxPopup("No new application update has been found.", "", "Ok", "", "", ""); }
            }
            catch { await MessageBoxPopup("Failed to check for the latest application version", "Please check your internet connection and try again.", "Ok", "", "", ""); }
        }

        //Create startup shortcut
        void ManageShortcutStartup()
        {
            try
            {
                //Set application shortcut paths
                string TargetFilePath_Admin = Assembly.GetEntryAssembly().CodeBase.Replace(".exe", "-Admin.exe");
                string TargetName_Admin = Assembly.GetEntryAssembly().GetName().Name;
                string TargetFileShortcut_Admin = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + TargetName_Admin + ".url";

                //Check if the shortcut already exists
                if (!File.Exists(TargetFileShortcut_Admin))
                {
                    Debug.WriteLine("Adding application to Windows startup");
                    using (StreamWriter StreamWriter = new StreamWriter(TargetFileShortcut_Admin))
                    {
                        StreamWriter.WriteLine("[InternetShortcut]");
                        StreamWriter.WriteLine("URL=" + TargetFilePath_Admin);
                        StreamWriter.WriteLine("IconFile=" + TargetFilePath_Admin.Replace("file:///", ""));
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }
                }
                else
                {
                    Debug.WriteLine("Removing application from Windows startup");
                    if (File.Exists(TargetFileShortcut_Admin)) { File.Delete(TargetFileShortcut_Admin); }
                }
            }
            catch { }
        }

        //Install drivers buttons
        async void btn_Settings_InstallDrivers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Message_InstallDrivers();
            }
            catch { }
        }
    }
}