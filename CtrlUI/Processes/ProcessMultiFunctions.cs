using System;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check process window
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

        //Launch new process
        async Task<bool> LaunchProcessDatabindAuto(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.Category == AppCategory.Emulator)
                {
                    return await LaunchProcessDatabindWin32Emulator(dataBindApp);
                }
                else if (dataBindApp.LaunchFilePicker)
                {
                    return await LaunchProcessDatabindWin32FilePicker(dataBindApp);
                }
                else if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    return await LaunchProcessDatabindUwpAndWin32Store(dataBindApp);
                }
                else
                {
                    return await LaunchProcessDatabindWin32(dataBindApp);
                }
            }
            catch { }
            return false;
        }

        //Restart the process
        async Task<bool> RestartPrepareAuto(ProcessMulti processMulti, DataBindApp dataBindApp)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    return await RestartPrepareUwp(dataBindApp, processMulti);
                }
                else if (processMulti.Type == ProcessType.Win32Store)
                {
                    return await RestartPrepareWin32Store(dataBindApp, processMulti);
                }
                else
                {
                    return await RestartPrepareWin32(dataBindApp, processMulti);
                }
            }
            catch { }
            return false;
        }

        //Close single process
        async Task<bool> CloseSingleProcessAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    return await CloseSingleProcessUwp(dataBindApp, processMulti, resetProcess, removeProcess);
                }
                else if (processMulti.Type == ProcessType.Win32 || processMulti.Type == ProcessType.Win32Store)
                {
                    return await CloseSingleProcessWin32AndWin32Store(dataBindApp, processMulti, resetProcess, removeProcess);
                }
            }
            catch { }
            return false;
        }

        //Close all processes
        async Task<bool> CloseAllProcessesAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    return await CloseAllProcessesUwp(dataBindApp, resetProcess, removeProcess);
                }
                else
                {
                    return await CloseAllProcessesWin32AndWin32Store(dataBindApp, resetProcess, removeProcess);
                }
            }
            catch { }
            return false;
        }
    }
}