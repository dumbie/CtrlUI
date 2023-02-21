using ArnoldVinkCode;
using System.Threading.Tasks;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an url protocol from databindapp
        async Task<bool> PrepareProcessLauncherUrlProtocolAsync(DataBindApp dataBindApp, bool silent, bool runAsAdmin, bool launchKeyboard)
        {
            try
            {
                //Show launching message
                if (!silent)
                {
                    await Notification_Send_Status("AppLaunch", "Launching " + dataBindApp.Name);
                    //Debug.WriteLine("Launching url protocol: " + dataBindApp.PathExe + " / " + dataBindApp.PathLaunch);
                }

                //Launch the url protocol
                int processId = AVProcessTool.Launch_Exe(dataBindApp.PathExe, dataBindApp.PathLaunch, string.Empty, false, runAsAdmin, false);
                if (processId <= 0)
                {
                    //Show failed launch messagebox
                    await ShowProcessLaunchFailedMessage();
                    return false;
                }

                //Minimize the CtrlUI window
                await AppWindowMinimize(true, true);

                //Launch the keyboard controller
                if (launchKeyboard)
                {
                    await ShowHideKeyboardController(true);
                }

                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await ShowProcessLaunchFailedMessage();
                return false;
            }
        }
    }
}