using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessWin32Functions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task<bool> RestartPrepareWin32(DataBindApp dataBindApp, ProcessMulti processMulti, bool useLaunchArgument)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataBindApp.PathExe))
                {
                    Popup_Show_Status("Close", "Failed restarting " + dataBindApp.Name);
                    Debug.WriteLine("Failed to restart process: " + dataBindApp.Name);
                    return false;
                }

                Popup_Show_Status("Switch", "Restarting " + dataBindApp.Name);
                Debug.WriteLine("Restarting Win32 application: " + dataBindApp.Name + " / " + processMulti.Identifier + " / " + processMulti.WindowHandle);

                //Set the launch argument
                string launchArgument = string.Empty;
                if (useLaunchArgument)
                {
                    Debug.WriteLine("Setting restart argument: " + processMulti.Argument);
                    launchArgument = processMulti.Argument;
                }

                await RestartProcessWin32(processMulti.Identifier, dataBindApp.PathExe, dataBindApp.PathLaunch, launchArgument);
                return true;
            }
            catch { }
            return false;
        }
    }
}