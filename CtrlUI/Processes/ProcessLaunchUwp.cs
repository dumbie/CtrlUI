using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVUwpAppx;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an UWP or Win32Store application from databindapp
        async Task<bool> PrepareProcessLauncherUwpAndWin32StoreAsync(DataBindApp dataBindApp, string launchArgument, bool silent, bool launchKeyboard)
        {
            bool appLaunched = false;
            try
            {
                //Set the app title
                string appTitle = dataBindApp.Name;

                //Check the launch argument
                if (string.IsNullOrWhiteSpace(launchArgument))
                {
                    launchArgument = dataBindApp.Argument;
                }
                else
                {
                    //Update the app title
                    if (dataBindApp.Category == AppCategory.Emulator)
                    {
                        appTitle += " with rom";
                    }
                    else if (dataBindApp.LaunchFilePicker)
                    {
                        appTitle += " with file";
                    }
                }

                //Launch the application
                appLaunched = await PrepareProcessLauncherUwpAndWin32StoreAsync(appTitle, dataBindApp.AppUserModelId, launchArgument, silent, launchKeyboard);

                //Update last launch date
                if (appLaunched)
                {
                    dataBindApp.LastLaunch = DateTime.Now.ToString(vAppCultureInfo);
                    //Debug.WriteLine("Updated last launch date: " + dataBindApp.LastLaunch);
                    JsonSaveList_Applications();
                }
            }
            catch { }
            return appLaunched;
        }

        //Launch an UWP or Win32Store application manually
        async Task<bool> PrepareProcessLauncherUwpAndWin32StoreAsync(string appTitle, string appUserModelId, string launchArgument, bool silent, bool launchKeyboard)
        {
            try
            {
                //Check if the application exists
                if (GetUwpAppPackageByAppUserModelId(appUserModelId) == null)
                {
                    Notification_Show_Status("Close", "Application not found");
                    Debug.WriteLine("Launch application not found.");
                    return false;
                }

                //Show launching message
                if (!silent)
                {
                    Notification_Show_Status("AppLaunch", "Launching " + appTitle);
                    //Debug.WriteLine("Launching UWP or Win32Store: " + appTitle + "/" + pathExe);
                }

                //Check Chromium DPI launch argument
                launchArgument = CheckChromiumDpiLaunchArgument(string.Empty, appUserModelId, launchArgument);

                //Minimize CtrlUI window
                await AppWindowMinimize(true, true);

                //Launch the UWP or Win32Store application
                bool launchSuccess = AVProcess.Launch_UwpApplication(appUserModelId, launchArgument);
                if (!launchSuccess)
                {
                    Notification_Show_Status("Close", "Failed launching " + appTitle);
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
                Notification_Show_Status("Close", "Failed launching " + appTitle);
                return false;
            }
        }
    }
}