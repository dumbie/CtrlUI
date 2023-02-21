using ArnoldVinkCode;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Restart the process
        async Task RestartProcessAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool currentArgument, bool defaultArgument, bool withoutArgument)
        {
            try
            {
                //Check the application category
                if (defaultArgument)
                {
                    await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                    await LaunchProcessDatabindAuto(dataBindApp);
                    return;
                }

                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                bool keyboardProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower() || x.String1.ToLower() == dataBindApp.PathExe.ToLower());
                bool keyboardLaunch = (keyboardProcess || dataBindApp.LaunchKeyboard) && vControllerAnyConnected();

                //Set restart arguments
                string newArguments = string.Empty;
                if (currentArgument)
                {
                    newArguments = processMulti.Argument;
                }

                //Restart the process
                await PrepareRestartProcess(dataBindApp, processMulti, newArguments, withoutArgument, keyboardLaunch);
            }
            catch { }
        }

        //Restart the process
        async Task<bool> PrepareRestartProcess(DataBindApp dataBindApp, ProcessMulti processMulti, string newArguments, bool withoutArguments, bool launchKeyboard)
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

                //Restart the process
                int processId = AVProcessTool.Restart_ProcessId(processMulti.Identifier, newArguments, withoutArguments);
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
                    await ShowHideKeyboardController(true);
                }
            }
            catch { }
            return false;
        }
    }
}