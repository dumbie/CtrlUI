using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.ImageFunctions;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch a Win32 application manually
        async Task<bool> LaunchProcessManuallyWin32(string pathExe, string pathLaunch, string argument, bool silent, bool allowMinimize, bool runAsAdmin, bool createNoWindow)
        {
            try
            {
                //Check if the application exists
                if (!File.Exists(pathExe))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("App exe not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    Debug.WriteLine("Launch executable not found");
                    return false;
                }

                //Show launching message
                if (!silent)
                {
                    Popup_Show_Status("App", "Launching " + Path.GetFileNameWithoutExtension(pathExe));
                    //Debug.WriteLine("Launching Win32: " + Path.GetFileNameWithoutExtension(pathExe));
                }

                //Launch the Win32 application
                ProcessLauncherWin32(pathExe, pathLaunch, argument, runAsAdmin, createNoWindow);

                //Minimize the CtrlUI window
                if (allowMinimize && Convert.ToBoolean(ConfigurationManager.AppSettings["MinimizeAppOnShow"])) { await AppMinimize(true); }
                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }

        //Launch a Win32 databind app
        async Task<bool> LaunchProcessDatabindWin32(DataBindApp dataBindApp)
        {
            try
            {
                //Check if the application exists
                if (!File.Exists(dataBindApp.PathExe))
                {
                    dataBindApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("App exe not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    Debug.WriteLine("Launch executable not found");
                    return false;
                }
                else
                {
                    dataBindApp.StatusAvailable = Visibility.Collapsed;
                }

                //Show application launch message
                Popup_Show_Status("App", "Launching " + dataBindApp.Name);
                Debug.WriteLine("Launching Win32: " + dataBindApp.Name + " cat: " + dataBindApp.Category + " path: " + dataBindApp.PathExe + " arg: " + dataBindApp.Argument);

                //Launch the Win32 application
                await LaunchProcessManuallyWin32(dataBindApp.PathExe, dataBindApp.PathLaunch, dataBindApp.Argument, true, true, false, false);
                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }

        //Launch a win32 databind app with filepicker
        async Task<bool> LaunchProcessDatabindWin32FilePicker(DataBindApp dataBindApp)
        {
            try
            {
                //Check if the application exe file exists
                if (!File.Exists(dataBindApp.PathExe))
                {
                    dataBindApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("App exe not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    return false;
                }
                else
                {
                    dataBindApp.StatusAvailable = Visibility.Collapsed;
                }

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
                if (vFilePickerCancelled) { return false; }

                string LaunchArguments = string.Empty;
                if (!string.IsNullOrWhiteSpace(vFilePickerResult.PathFile))
                {
                    LaunchArguments = dataBindApp.Argument + " \"" + vFilePickerResult.PathFile + "\"";
                    Popup_Show_Status("App", "Launching " + dataBindApp.Name + " with selected file");
                    Debug.WriteLine("Launching app: " + dataBindApp.Name + " file: " + LaunchArguments);
                }
                else
                {
                    LaunchArguments = dataBindApp.Argument;
                    Popup_Show_Status("App", "Launching " + dataBindApp.Name);
                    Debug.WriteLine("Launching app: " + dataBindApp.Name + " without a file");
                }

                //Launch the Win32 application
                await LaunchProcessManuallyWin32(dataBindApp.PathExe, dataBindApp.PathLaunch, LaunchArguments, true, true, false, false);
                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }

        //Launch a win32 databind emulator with filepicker
        async Task<bool> LaunchProcessDatabindWin32Emulator(DataBindApp dataBindApp)
        {
            try
            {
                //Check if the application exe file exists
                if (!File.Exists(dataBindApp.PathExe))
                {
                    dataBindApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("App exe not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    return false;
                }
                else
                {
                    dataBindApp.StatusAvailable = Visibility.Collapsed;
                }

                //Check if the rom folder location exists
                if (!Directory.Exists(dataBindApp.PathRoms))
                {
                    dataBindApp.StatusAvailable = Visibility.Visible;

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Rom folder not found, please edit the application", "", "You can do this by interacting with the application and than click on the 'Edit app' button.", Answers);
                    return false;
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
                if (vFilePickerCancelled) { return false; }

                string LaunchArguments = string.Empty;
                if (!string.IsNullOrWhiteSpace(vFilePickerResult.PathFile))
                {
                    LaunchArguments = dataBindApp.Argument + " \"" + vFilePickerResult.PathFile + "\"";
                    Popup_Show_Status("App", "Launching " + dataBindApp.Name + " with the rom");
                    Debug.WriteLine("Launching emulator: " + dataBindApp.Name + " rom: " + LaunchArguments);
                }
                else
                {
                    Popup_Show_Status("App", "Launching " + dataBindApp.Name);
                    Debug.WriteLine("Launching emulator: " + dataBindApp.Name + " without a rom");
                }

                //Launch the Win32 application
                await LaunchProcessManuallyWin32(dataBindApp.PathExe, dataBindApp.PathLaunch, LaunchArguments, true, true, false, false);
                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }
    }
}