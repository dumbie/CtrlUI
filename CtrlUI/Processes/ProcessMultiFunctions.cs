using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check process windows
        async Task<IntPtr> CheckProcessWindowsAuto(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    return processMulti.WindowHandle;
                }
                else if (processMulti.Type == ProcessType.Win32 || processMulti.Type == ProcessType.Win32Store)
                {
                    return await CheckProcessWindowsWin32AndWin32Store(dataBindApp, processMulti);
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        //Check if databind paths are available
        async Task<bool> CheckDatabindPathAuto(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    //Check if the application exists
                    if (UwpGetAppPackageByAppUserModelId(dataBindApp.PathExe) == null)
                    {
                        await Notification_Send_Status("Close", "Application not found");
                        Debug.WriteLine("Launch application not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }
                else if (dataBindApp.LaunchFilePicker)
                {
                    //Check if the application exists
                    if (!File.Exists(dataBindApp.PathExe))
                    {
                        await Notification_Send_Status("Close", "Executable not found");
                        Debug.WriteLine("Launch executable not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }
                else if (dataBindApp.Category == AppCategory.Emulator)
                {
                    //Check if the rom folder exists
                    if (!Directory.Exists(dataBindApp.PathRoms))
                    {
                        await Notification_Send_Status("Close", "Rom folder not found");
                        Debug.WriteLine("Rom folder not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }

                    //Check if the application exists
                    if (!File.Exists(dataBindApp.PathExe))
                    {
                        await Notification_Send_Status("Close", "Executable not found");
                        Debug.WriteLine("Launch executable not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }
                else
                {
                    //Check if the application exists
                    if (!File.Exists(dataBindApp.PathExe))
                    {
                        await Notification_Send_Status("Close", "Executable not found");
                        Debug.WriteLine("Launch executable not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }

                //Paths are available update status
                dataBindApp.StatusAvailable = Visibility.Collapsed;
            }
            catch { }
            return true;
        }

        //Launch databind process
        async Task LaunchProcessDatabindAuto(DataBindApp dataBindApp)
        {
            try
            {
                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower());
                bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                //Check if databind paths are available
                if (!await CheckDatabindPathAuto(dataBindApp)) { return; }

                //Launch the databind process
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    await EnableHDRDatabindAuto(dataBindApp);
                    await PrepareProcessLauncherUwpAndWin32StoreAsync(dataBindApp, false, true, keyboardLaunch);
                }
                else if (dataBindApp.LaunchFilePicker)
                {
                    string launchArgument = await GetLaunchArgumentFilePicker(dataBindApp);
                    if (launchArgument == "Cancel") { return; }
                    await EnableHDRDatabindAuto(dataBindApp);
                    await PrepareProcessLauncherWin32Async(dataBindApp, launchArgument, false, true, false, false, keyboardLaunch);
                }
                else if (dataBindApp.Category == AppCategory.Emulator)
                {
                    string launchArgument = await GetLaunchArgumentEmulator(dataBindApp);
                    if (launchArgument == "Cancel") { return; }
                    await EnableHDRDatabindAuto(dataBindApp);
                    await PrepareProcessLauncherWin32Async(dataBindApp, launchArgument, false, true, false, false, keyboardLaunch);
                }
                else
                {
                    await EnableHDRDatabindAuto(dataBindApp);
                    await PrepareProcessLauncherWin32Async(dataBindApp, string.Empty, false, true, false, false, keyboardLaunch);
                }
            }
            catch { }
        }

        //Enable monitor HDR
        async Task EnableHDRDatabindAuto(DataBindApp dataBindApp)
        {
            try
            {
                //Check executable name
                string executableName = string.Empty;
                string executableNameRaw = string.Empty;
                if (string.IsNullOrWhiteSpace(dataBindApp.NameExe))
                {
                    executableName = Path.GetFileNameWithoutExtension(dataBindApp.PathExe).ToLower();
                    executableNameRaw = dataBindApp.PathExe.ToLower();
                }
                else
                {
                    executableName = Path.GetFileNameWithoutExtension(dataBindApp.NameExe).ToLower();
                    executableNameRaw = dataBindApp.NameExe.ToLower();
                }

                //Enable monitor HDR
                bool enabledHDR = vCtrlHDRProcessName.Any(x => x.String1.ToLower() == executableName || x.String1.ToLower() == executableNameRaw);
                if (enabledHDR)
                {
                    await AllMonitorSwitchHDR(true, true);
                }
            }
            catch { }
        }

        //Restart the process
        async Task RestartProcessAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool useLaunchArgument)
        {
            try
            {
                //Check the application category
                if (!useLaunchArgument && dataBindApp.Category != AppCategory.Process)
                {
                    await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                    await LaunchProcessDatabindAuto(dataBindApp);
                    return;
                }

                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower());
                bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                //Restart the process
                if (processMulti.Type == ProcessType.UWP)
                {
                    await PrepareRestartProcessUwp(dataBindApp, processMulti, useLaunchArgument, keyboardLaunch);
                }
                else if (processMulti.Type == ProcessType.Win32Store)
                {
                    await PrepareRestartProcessWin32Store(dataBindApp, processMulti, useLaunchArgument, keyboardLaunch);
                }
                else
                {
                    await PrepareRestartProcessWin32(dataBindApp, processMulti, useLaunchArgument, keyboardLaunch);
                }
            }
            catch { }
        }

        //Close single process
        async Task CloseSingleProcessAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    await CloseSingleProcessUwp(dataBindApp, processMulti, resetProcess, removeProcess);
                }
                else if (processMulti.Type == ProcessType.Win32 || processMulti.Type == ProcessType.Win32Store)
                {
                    await CloseSingleProcessWin32AndWin32Store(dataBindApp, processMulti, resetProcess, removeProcess);
                }
            }
            catch { }
        }

        //Close all processes
        async Task CloseAllProcessesAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    await CloseAllProcessesUwp(dataBindApp, resetProcess, removeProcess);
                }
                else
                {
                    await CloseAllProcessesWin32AndWin32Store(dataBindApp, resetProcess, removeProcess);
                }
            }
            catch { }
        }
    }
}