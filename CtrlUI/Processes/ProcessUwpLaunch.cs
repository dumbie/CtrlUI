using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an UWP or Win32Store application from databindapp
        async Task<bool> PrepareProcessLauncherUwpAndWin32StoreAsync(DataBindApp dataBindApp, bool silent, bool allowMinimize, bool launchKeyboard)
        {
            try
            {
                return await PrepareProcessLauncherUwpAndWin32StoreAsync(dataBindApp.Name, dataBindApp.PathExe, dataBindApp.Argument, silent, allowMinimize, launchKeyboard);
            }
            catch { }
            return false;
        }

        //Launch an UWP or Win32Store application manually
        async Task<bool> PrepareProcessLauncherUwpAndWin32StoreAsync(string appName, string pathExe, string argument, bool silent, bool allowMinimize, bool launchKeyboard)
        {
            try
            {
                //Check if the application exists
                if (UwpGetAppPackageByAppUserModelId(pathExe) == null)
                {
                    Popup_Show_Status("Close", "Application not found");
                    Debug.WriteLine("Launch application not found, possibly uninstalled.");
                    return false;
                }

                //Show launching message
                if (!silent)
                {
                    Popup_Show_Status("App", "Launching " + appName);
                    //Debug.WriteLine("Launching UWP or Win32Store: " + appName + "/" + pathExe);
                }

                //Minimize the CtrlUI window
                if (allowMinimize && Convert.ToBoolean(ConfigurationManager.AppSettings["MinimizeAppOnShow"])) { await AppMinimize(true); }

                //Launch the UWP or Win32Store application
                Process launchProcess = await ProcessLauncherUwpAndWin32StoreAsync(pathExe, argument);
                if (launchProcess == null)
                {
                    //Show failed launch messagebox
                    await LaunchProcessFailed();
                    return false;
                }

                //Launch the keyboard controller
                if (launchKeyboard)
                {
                    await LaunchKeyboardController(true);
                }

                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }
    }
}