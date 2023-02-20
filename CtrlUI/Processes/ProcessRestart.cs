using ArnoldVinkCode;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task<bool> PrepareRestartProcess(DataBindApp dataBindApp, ProcessMulti processMulti, bool useLaunchArgument, bool launchKeyboard)
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
                Debug.WriteLine("Restarting Win32 application: " + dataBindApp.Name + " / " + processMulti.Identifier + " / " + processMulti.WindowHandle);

                //Set the launch argument
                string launchArgument = string.Empty;
                if (useLaunchArgument)
                {
                    Debug.WriteLine("Setting restart argument: " + processMulti.Argument);
                    launchArgument = processMulti.Argument;
                }

                //Restart the process
                int processId = AVProcessTool.Restart_ProcessId(processMulti.Identifier, launchArgument);
                if (processId <= 0)
                {
                    await Notification_Send_Status("Close", "Failed restarting " + dataBindApp.Name);
                    Debug.WriteLine("Failed to restart process: " + dataBindApp.Name);
                    return false;
                }

                //Minimize the CtrlUI window
                await AppWindowMinimize(true, true);

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