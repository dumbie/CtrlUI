using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVUwpAppx;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an UWP or Win32Store application from databindapp
        async Task<bool> PrepareProcessLauncherUwpAndWin32StoreAsync(DataBindApp dataBindApp, bool silent, bool launchKeyboard)
        {
            bool appLaunched = false;
            try
            {
                //Launch the application
                appLaunched = await PrepareProcessLauncherUwpAndWin32StoreAsync(dataBindApp.Name, dataBindApp.PathExe, dataBindApp.Argument, silent, launchKeyboard);

                //Update last launch date
                if (appLaunched)
                {
                    dataBindApp.LastLaunch = DateTime.Now.ToString(vAppCultureInfo);
                    //Debug.WriteLine("Updated last launch date: " + dataBindApp.LastLaunch);
                    JsonSaveApplications();
                }
            }
            catch { }
            return appLaunched;
        }

        //Launch an UWP or Win32Store application manually
        async Task<bool> PrepareProcessLauncherUwpAndWin32StoreAsync(string appTitle, string pathExe, string argument, bool silent, bool launchKeyboard)
        {
            try
            {
                //Check if the application exists
                if (GetUwpAppPackageByAppUserModelId(pathExe) == null)
                {
                    await Notification_Send_Status("Close", "Application not found");
                    Debug.WriteLine("Launch application not found.");
                    return false;
                }

                //Show launching message
                if (!silent)
                {
                    await Notification_Send_Status("AppLaunch", "Launching " + appTitle);
                    //Debug.WriteLine("Launching UWP or Win32Store: " + appTitle + "/" + pathExe);
                }

                //Minimize the CtrlUI window
                await AppWindowMinimize(true, true);

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
                    await KeyboardControllerHideShow(true);
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