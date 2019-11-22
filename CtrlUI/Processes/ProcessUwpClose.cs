using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close all processes UWP
        async Task CloseAllProcessesUwpByDataBindApp(DataBindApp launchApp)
        {
            try
            {
                Popup_Show_Status("Closing", "Closing " + launchApp.Name);
                Debug.WriteLine("Closing UWP processes: " + launchApp.Name + " / " + launchApp.ProcessId + " / " + launchApp.WindowHandle);

                //Get the process id by window handle
                if (launchApp.ProcessId <= 0)
                {
                    launchApp.ProcessId = await GetUwpProcessIdByWindowHandle(launchApp.Name, launchApp.PathExe, launchApp.WindowHandle);
                }

                //Close the process or app
                bool ClosedProcess = CloseProcessById(launchApp.ProcessId);
                if (ClosedProcess)
                {
                    //Updating running status
                    launchApp.StatusRunning = Visibility.Collapsed;
                    launchApp.StatusSuspended = Visibility.Collapsed;
                    launchApp.RunningTimeLastUpdate = 0;

                    //Update process count
                    launchApp.ProcessRunningCount = string.Empty;
                }
                else
                {
                    Popup_Show_Status("Closing", "Failed to close the app");
                    Debug.WriteLine("Failed to close the application.");
                }
            }
            catch { }
        }
    }
}