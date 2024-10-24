using ArnoldVinkCode;
using System.Threading.Tasks;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

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

                //Check app category
                string exePath = string.Empty;
                if (dataBindApp.Category == AppCategory.Gallery)
                {
                    exePath = dataBindApp.PathGallery;
                }
                else
                {
                    exePath = dataBindApp.PathExe;
                }

                //Launch url protocol
                bool launchSuccess = AVProcess.Launch_ShellExecute(exePath, dataBindApp.PathLaunch, dataBindApp.Argument, dataBindApp.LaunchAsAdmin);
                if (!launchSuccess)
                {
                    await Notification_Send_Status("Close", "Failed launching " + dataBindApp.Name);
                    return false;
                }

                //Launch keyboard controller
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