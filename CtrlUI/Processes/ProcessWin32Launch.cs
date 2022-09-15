using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch a Win32 application from databindapp
        async Task<bool> PrepareProcessLauncherWin32Async(DataBindApp dataBindApp, string launchArgument, bool silent, bool runAsAdmin, bool createNoWindow, bool launchKeyboard)
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

                //Chromium DPI launch argument
                string processName = Path.GetFileNameWithoutExtension(dataBindApp.PathExe);
                if (vCtrlChromiumBrowsers.Any(x => x.String1 == processName))
                {
                    //Get the current active screen
                    int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                    DisplayMonitor displayMonitorSettings = GetSingleMonitorEnumDisplay(monitorNumber);

                    //Get the current screen dpi
                    double screenDPI = displayMonitorSettings.DpiScaleHorizontal;
                    double chromiumDPI = Convert.ToDouble(Setting_Load(vConfigurationCtrlUI, "AdjustChromiumDpi"));

                    //Update the launch argument
                    string stringDPI = (screenDPI + chromiumDPI).ToString(vAppCultureInfo);
                    launchArgument += " -force-device-scale-factor=" + stringDPI;

                    Debug.WriteLine("Chromium dpi scale factor: " + stringDPI);
                }

                //Launch the application
                appLaunched = await PrepareProcessLauncherWin32Async(appTitle, dataBindApp.PathExe, dataBindApp.PathLaunch, launchArgument, silent, runAsAdmin, createNoWindow, launchKeyboard, false);

                //Update last launch date
                if (appLaunched)
                {
                    dataBindApp.LastLaunch = DateTime.Now.ToString(vAppCultureInfo);
                    //Debug.WriteLine("Updated last launch date: " + dataBindApp.LastLaunch);
                    JsonSaveApplications();
                }
            }
            catch { }
            return appLaunched;
        }

        //Launch a Win32 application manually
        async Task<bool> PrepareProcessLauncherWin32Async(string appTitle, string pathExe, string pathLaunch, string launchArgument, bool silent, bool runAsAdmin, bool createNoWindow, bool launchKeyboard, bool ignoreFailed)
        {
            try
            {
                //Check if the application exists
                if (!File.Exists(pathExe))
                {
                    await Notification_Send_Status("Close", "Executable not found");
                    Debug.WriteLine("Launch executable not found.");
                    return false;
                }

                //Show launching message
                if (!silent)
                {
                    await Notification_Send_Status("AppLaunch", "Launching " + appTitle);
                    //Debug.WriteLine("Launching Win32: " + appTitle + "/" + pathExe);
                }

                //Launch the Win32 application
                Process launchProcess = await ProcessLauncherWin32Async(pathExe, pathLaunch, launchArgument, runAsAdmin, createNoWindow);
                if (!ignoreFailed && launchProcess == null)
                {
                    //Show failed launch messagebox
                    await LaunchProcessFailed();
                    return false;
                }

                //Minimize the CtrlUI window
                await AppWindowMinimize(true, true);

                //Launch the keyboard controller
                if (launchKeyboard)
                {
                    await KeyboardControllerHideShow(true);
                }

                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }

        //Get launch argument for filepicker
        async Task<string> GetLaunchArgumentFilePicker(DataBindApp dataBindApp)
        {
            try
            {
                //Select a file from list to launch
                vFilePickerFilterIn = new List<string>();
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "File Selection";
                vFilePickerDescription = "Please select a file to load in " + dataBindApp.Name + ":";
                vFilePickerShowNoFile = true;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
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
                vFilePickerFilterIn = new List<string>();
                vFilePickerFilterOut = new List<string> { "jpg", "png" };
                vFilePickerTitle = "Rom Selection";
                vFilePickerDescription = "Please select a rom file to load in " + dataBindApp.Name + ":";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = true;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
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