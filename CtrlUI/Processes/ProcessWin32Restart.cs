using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessWin32Functions;
using static ArnoldVinkCode.ProcessWin32StoreFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RestartPrepareWin32Store(ProcessMulti processMulti, DataBindApp dataBindApp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataBindApp.PathExe))
                {
                    Popup_Show_Status("Close", "Failed restarting " + dataBindApp.Name);
                    Debug.WriteLine("Failed to restart process: " + dataBindApp.Name);
                    return;
                }

                Popup_Show_Status("Switch", "Restarting " + dataBindApp.Name);
                Debug.WriteLine("Restarting Win32Store application: " + dataBindApp.Name + " / " + processMulti.ProcessId + " / " + processMulti.WindowHandle);

                await RestartProcessWin32Store(processMulti.ProcessId, dataBindApp.NameExe, dataBindApp.PathExe, dataBindApp.Argument);
            }
            catch { }
        }

        async Task RestartPrepareWin32(ProcessMulti processMulti, DataBindApp dataBindApp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataBindApp.PathExe))
                {
                    Popup_Show_Status("Close", "Failed restarting " + dataBindApp.Name);
                    Debug.WriteLine("Failed to restart process: " + dataBindApp.Name);
                    return;
                }

                Popup_Show_Status("Switch", "Restarting " + dataBindApp.Name);
                Debug.WriteLine("Restarting Win32 application: " + dataBindApp.Name + " / " + processMulti.ProcessId + " / " + processMulti.WindowHandle);

                string LaunchArgument = dataBindApp.Argument;
                if (dataBindApp.Category == AppCategory.Emulator)
                {
                    if (string.IsNullOrWhiteSpace(dataBindApp.RomPath))
                    {
                        LaunchArgument = string.Empty;
                    }
                    else
                    {
                        LaunchArgument += dataBindApp.RomPath;
                    }
                }

                await RestartProcessWin32(processMulti.ProcessId, dataBindApp.PathExe, dataBindApp.PathLaunch, LaunchArgument);
            }
            catch { }
        }
    }
}