using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
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

        //Launch databind process
        async Task LaunchProcessDatabindAuto(DataBindApp dataBindApp)
        {
            try
            {
                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower());
                bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                //Launch new process
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    await PrepareProcessLauncherUwpAndWin32StoreAsync(dataBindApp, false, true, keyboardLaunch);
                }
                else if (dataBindApp.LaunchFilePicker)
                {
                    string launchArgument = await GetLaunchArgumentFilePicker(dataBindApp);
                    if (launchArgument == "Cancel") { return; }
                    await PrepareProcessLauncherWin32Async(dataBindApp, launchArgument, false, true, false, false, keyboardLaunch);
                }
                else if (dataBindApp.Category == AppCategory.Emulator)
                {
                    string launchArgument = await GetLaunchArgumentEmulator(dataBindApp);
                    if (launchArgument == "Cancel") { return; }
                    await PrepareProcessLauncherWin32Async(dataBindApp, launchArgument, false, true, false, false, keyboardLaunch);
                }
                else
                {
                    await PrepareProcessLauncherWin32Async(dataBindApp, string.Empty, false, true, false, false, keyboardLaunch);
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