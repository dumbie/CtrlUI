using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessWin32StoreFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task<bool> PrepareRestartProcessWin32Store(DataBindApp dataBindApp, ProcessMulti processMulti, bool useLaunchArgument, bool launchKeyboard)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataBindApp.PathExe))
                {
                    await Notification_Send_Status("Close", "Failed restarting " + dataBindApp.Name);
                    Debug.WriteLine("Failed to restart process: " + dataBindApp.Name);
                    return false;
                }

                await Notification_Send_Status("AppRestart", "Restarting " + dataBindApp.Name);
                Debug.WriteLine("Restarting Win32Store application: " + dataBindApp.Name + " / " + processMulti.Identifier + " / " + processMulti.WindowHandle);

                //Set the launch argument
                string launchArgument = string.Empty;
                if (useLaunchArgument)
                {
                    Debug.WriteLine("Setting restart argument: " + processMulti.Argument);
                    launchArgument = processMulti.Argument;
                }

                //Restart the process
                Process restartProcess = await RestartProcessWin32Store(dataBindApp.NameExe, dataBindApp.PathExe, processMulti.Identifier, launchArgument);
                if (restartProcess == null)
                {
                    await Notification_Send_Status("Close", "Failed restarting " + dataBindApp.Name);
                    Debug.WriteLine("Failed to restart process: " + dataBindApp.Name);
                    return false;
                }

                //Launch the keyboard controller
                if (launchKeyboard)
                {
                    await KeyboardControllerHideShow(true);
                }
            }
            catch { }
            return false;
        }
    }
}