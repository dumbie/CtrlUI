using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch a Win32 application from databindapp
        async Task<bool> PrepareProcessLauncherWin32Async(DataBindApp dataBindApp, string launchArgument, bool silent, bool allowMinimize, bool runAsAdmin, bool createNoWindow, bool launchKeyboard)
        {
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

                //Check if the application exists
                if (!File.Exists(dataBindApp.PathExe))
                {
                    await Notification_Send_Status("Close", "Executable not found");
                    Debug.WriteLine("Launch executable not found.");
                    dataBindApp.StatusAvailable = Visibility.Visible;
                    return false;
                }

                return await PrepareProcessLauncherWin32Async(appTitle, dataBindApp.PathExe, dataBindApp.PathLaunch, launchArgument, silent, allowMinimize, runAsAdmin, createNoWindow, launchKeyboard);
            }
            catch { }
            return false;
        }

        //Launch a Win32 application manually
        async Task<bool> PrepareProcessLauncherWin32Async(string appTitle, string pathExe, string pathLaunch, string launchArgument, bool silent, bool allowMinimize, bool runAsAdmin, bool createNoWindow, bool launchKeyboard)
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
                if (launchProcess == null)
                {
                    //Show failed launch messagebox
                    await LaunchProcessFailed();
                    return false;
                }

                //Minimize the CtrlUI window
                if (allowMinimize && Convert.ToBoolean(ConfigurationManager.AppSettings["MinimizeAppOnShow"]))
                {
                    await AppMinimize(true);
                }

                //Launch the keyboard controller
                if (launchKeyboard)
                {
                    await LaunchKeyboardController(true);
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
                //Check if the rom folder location exists
                if (!Directory.Exists(dataBindApp.PathRoms))
                {
                    dataBindApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Check.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Rom folder not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    return "Cancel";
                }
                else
                {
                    dataBindApp.StatusAvailable = Visibility.Collapsed;
                }

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