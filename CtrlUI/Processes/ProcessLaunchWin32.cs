using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch a Win32 application from databindapp
        async Task<bool> PrepareProcessLauncherWin32Async(DataBindApp dataBindApp, string launchArgument, bool silent, bool launchKeyboard)
        {
            bool appLaunched = false;
            try
            {
                //Set the app title
                string appTitle = dataBindApp.Name;

                //Check the launch argument
                if (string.IsNullOrWhiteSpace(launchArgument))
                {
                    launchArgument = dataBindApp.Argument;
                }
                else
                {
                    //Update the app title
                    if (dataBindApp.Category == AppCategory.Emulator)
                    {
                        appTitle += " with rom";
                    }
                    else if (dataBindApp.LaunchFilePicker)
                    {
                        appTitle += " with file";
                    }
                }

                //Launch the application
                appLaunched = await PrepareProcessLauncherWin32Async(appTitle, dataBindApp.PathExe, dataBindApp.PathLaunch, launchArgument, silent, dataBindApp.LaunchAsAdmin, launchKeyboard);

                //Update last launch date
                if (appLaunched)
                {
                    dataBindApp.LastLaunch = DateTime.Now.ToString(vAppCultureInfo);
                    //Debug.WriteLine("Updated last launch date: " + dataBindApp.LastLaunch);
                    JsonSaveList_Applications();
                }
            }
            catch { }
            return appLaunched;
        }

        //Launch a Win32 application manually
        async Task<bool> PrepareProcessLauncherWin32Async(string appTitle, string pathExe, string pathWork, string launchArgument, bool silent, bool launchAsAdmin, bool launchKeyboard)
        {
            try
            {
                //Check if the application exists
                if (!File.Exists(pathExe))
                {
                    Notification_Show_Status("Close", "Executable not found");
                    Debug.WriteLine("Launch executable not found.");
                    return false;
                }

                //Show launching message
                if (!silent)
                {
                    Notification_Show_Status("AppLaunch", "Launching " + appTitle);
                    //Debug.WriteLine("Launching Win32: " + appTitle + "/" + pathExe);
                }

                //Check Chromium DPI launch argument
                launchArgument = CheckChromiumDpiLaunchArgument(pathExe, string.Empty, launchArgument);

                //Minimize CtrlUI window
                await AppWindowMinimize(true, true);

                //Launch the Win32 application
                bool launchSuccess = AVProcess.Launch_ShellExecute(pathExe, pathWork, launchArgument, launchAsAdmin);
                if (!launchSuccess)
                {
                    Notification_Show_Status("Close", "Failed launching " + appTitle);
                    return false;
                }

                //Launch the keyboard controller
                if (launchKeyboard)
                {
                    await ShowHideKeyboardController(true);
                }

                return true;
            }
            catch
            {
                Notification_Show_Status("Close", "Failed launching " + appTitle);
                return false;
            }
        }

        //Get launch argument for filepicker
        async Task<string> GetLaunchArgumentFilePicker(DataBindApp dataBindApp)
        {
            try
            {
                //Select a file from list to launch
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.Title = "File Selection";
                vFilePickerSettings.Description = "Please select a file to load in " + dataBindApp.Name + ":";
                vFilePickerSettings.ShowLaunchWithoutFile = true;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return "Cancel"; }

                string launchArgument = string.Empty;
                if (!string.IsNullOrWhiteSpace(vFilePickerResult.PathFile))
                {
                    launchArgument = dataBindApp.Argument + " \"" + vFilePickerResult.PathFile + "\"";
                }

                Debug.WriteLine("Set launch argument to: " + launchArgument);
                return launchArgument;
            }
            catch { }
            return "Cancel";
        }

        //Get launch argument for emulator
        async Task<string> GetLaunchArgumentEmulator(DataBindApp dataBindApp)
        {
            try
            {
                //Select a file from list to launch
                vFilePickerSettings = new FilePickerSettings();
                vFilePickerSettings.FilterOut = new List<string> { "jpg", "png", "json" };
                vFilePickerSettings.Title = "Rom Selection";
                vFilePickerSettings.Description = "Please select a rom to load in " + dataBindApp.Name + ":";
                vFilePickerSettings.ShowEmulatorInterface = true;
                vFilePickerSettings.SourceDataBindApp = dataBindApp;
                vFilePickerSettings.RootPath = dataBindApp.PathRoms;
                await Popup_Show_FilePicker(dataBindApp.PathRoms, -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return "Cancel"; }

                string launchArgument = string.Empty;
                if (!string.IsNullOrWhiteSpace(vFilePickerResult.PathFile))
                {
                    launchArgument = dataBindApp.Argument + " \"" + vFilePickerResult.PathFile + "\"";
                }

                Debug.WriteLine("Set launch argument to: " + launchArgument);
                return launchArgument;
            }
            catch { }
            return "Cancel";
        }
    }
}