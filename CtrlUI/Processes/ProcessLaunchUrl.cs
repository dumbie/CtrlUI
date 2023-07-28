using ArnoldVinkCode;
using System.Threading.Tasks;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an url protocol from databindapp
        async Task<bool> PrepareProcessLauncherUrlProtocolAsync(DataBindApp dataBindApp, bool silent, bool launchKeyboard)
        {
            try
            {
                //Show launching message
                if (!silent)
                {
                    await Notification_Send_Status("AppLaunch", "Launching " + dataBindApp.Name);
                    //Debug.WriteLine("Launching url protocol: " + dataBindApp.PathExe + " / " + dataBindApp.PathLaunch);
                }

                //Minimize CtrlUI window
                await AppWindowMinimize(true, true);

                //Launch the url protocol
                bool launchSuccess = AVProcess.Launch_ShellExecute(dataBindApp.PathExe, dataBindApp.PathLaunch, dataBindApp.Argument, dataBindApp.LaunchAsAdmin);
                if (!launchSuccess)
                {
                    await Notification_Send_Status("Close", "Failed launching " + dataBindApp.Name);
                    return false;
                }

                //Launch the keyboard controller
                if (launchKeyboard)
                {
                    await ShowHideKeyboardController(true);
                }

                return true;
            }
            catch
            {
                await Notification_Send_Status("Close", "Failed launching " + dataBindApp.Name);
                return false;
            }
        }
    }
}