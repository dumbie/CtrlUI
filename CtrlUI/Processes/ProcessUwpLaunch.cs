using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an UWP or Win32Store application manually
        async Task<bool> LaunchProcessManuallyUwpAndWin32Store(string appName, string pathExe, string argument, bool silent, bool allowMinimize)
        {
            try
            {
                //Check if the application exists
                if (UwpGetAppPackageFromAppUserModelId(pathExe) == null)
                {
                    Popup_Show_Status("Close", "Application not found");
                    Debug.WriteLine("Launch application not found, possibly uninstalled.");
                    return false;
                }

                //Show launching message
                if (!silent)
                {
                    Popup_Show_Status("App", "Launching " + appName);
                    Debug.WriteLine("Launching UWP or Win32Store: " + appName + "/" + pathExe);
                }

                //Minimize the CtrlUI window
                if (allowMinimize && ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                //Launch the UWP or Win32Store application
                ProcessLauncherUwpAndWin32Store(pathExe, argument);
                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }

        //Launch an UWP or Win32Store databind app
        async Task<bool> LaunchProcessDatabindUwpAndWin32Store(DataBindApp launchApp)
        {
            try
            {
                //Check if UWP or Win32Store process is running
                ProcessMulti processMulti = await GetProcessMultiFromDataBindApp(launchApp);
                if (processMulti != null)
                {
                    bool alreadyRunning = false;
                    if (processMulti.Type == ProcessType.UWP)
                    {
                        alreadyRunning = await CheckLaunchProcessUwp(processMulti, launchApp);
                    }
                    else if (processMulti.Type == ProcessType.Win32Store)
                    {
                        alreadyRunning = await CheckLaunchProcessWin32Store(processMulti, launchApp);
                    }

                    if (!alreadyRunning)
                    {
                        Debug.WriteLine(processMulti.Type + " process is already running, skipping the launch.");
                        return false;
                    }
                }

                //Check if the application exists
                if (UwpGetAppPackageFromAppUserModelId(launchApp.PathExe) == null)
                {
                    launchApp.StatusAvailable = Visibility.Visible;

                    Popup_Show_Status("Close", "Application not found");
                    Debug.WriteLine("Launch application not found, possibly uninstalled.");
                    return false;
                }
                else
                {
                    launchApp.StatusAvailable = Visibility.Collapsed;
                }

                //Show application launch message
                Popup_Show_Status("App", "Launching " + launchApp.Name);
                Debug.WriteLine("Launching UWP or Win32Store: " + launchApp.Name + " from: " + launchApp.Category + " path: " + launchApp.PathExe);

                //Launch the UWP application
                await LaunchProcessManuallyUwpAndWin32Store(launchApp.Name, launchApp.PathExe, launchApp.Argument, true, true);
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