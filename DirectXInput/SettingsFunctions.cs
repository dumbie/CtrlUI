using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using static ArnoldVinkCode.AVFiles;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Reset temp blocked controller path list
        void Btn_SearchNewControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reset temp blocked controller path list
                vControllerTempBlockPaths.Clear();

                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Controller";
                notificationDetails.Text = "Searching for controllers";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                Debug.WriteLine("Reset temp blocked controller path list.");
            }
            catch { }
        }

        //Allow all the ignored controllers
        void Btn_AllowIgnoredControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Allow all the ignored controllers
                foreach (ControllerProfile profile in vDirectControllersProfile)
                {
                    profile.ControllerIgnore = false;
                }

                //Save changes to Json file
                JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");

                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Controller";
                notificationDetails.Text = "Allowing controllers";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                Debug.WriteLine("Showing all the ignored controllers.");
            }
            catch { }
        }

        //Open Windows Game controller settings
        void btn_CheckControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("joy.cpl");
            }
            catch { }
        }

        //Open Windows device manager
        void btn_CheckDeviceManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("devmgmt.msc");
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
                    File_Delete(targetFileShortcut);
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