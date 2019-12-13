using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;

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
                ProcessLauncherWin32(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\control.exe", "", "joy.cpl", false, false);
            }
            catch { }
        }

        //Open Windows device manager
        void btn_CheckDeviceManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessLauncherWin32(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\devmgmt.msc", "", "", false, false);
            }
            catch { }
        }

        //Create startup shortcut
        void ManageShortcutStartup()
        {
            try
            {
                //Set application shortcut paths
                string targetFilePath = Assembly.GetEntryAssembly().CodeBase.Replace(".exe", "-Admin.exe");
                string targetName = Assembly.GetEntryAssembly().GetName().Name;
                string targetFileShortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");

                //Check if the shortcut already exists
                if (!File.Exists(targetFileShortcut))
                {
                    Debug.WriteLine("Adding application to Windows startup.");
                    using (StreamWriter StreamWriter = new StreamWriter(targetFileShortcut))
                    {
                        StreamWriter.WriteLine("[InternetShortcut]");
                        StreamWriter.WriteLine("URL=" + targetFilePath);
                        StreamWriter.WriteLine("IconFile=" + targetFilePath.Replace("file:///", ""));
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }
                }
                else
                {
                    Debug.WriteLine("Removing application from Windows startup.");
                    if (File.Exists(targetFileShortcut))
                    {
                        File.Delete(targetFileShortcut);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed creating startup shortcut.");
            }
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