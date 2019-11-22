using System.Diagnostics;
using System.IO;
using System.Windows;
using static ArnoldVinkCode.ProcessFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close all processes Win32
        void CloseAllProcessesWin32ByDataBindApp(DataBindApp launchApp)
        {
            try
            {
                Popup_Show_Status("Closing", "Closing " + launchApp.Name);
                Debug.WriteLine("Closing Win32 processes: " + launchApp.Name + " / " + launchApp.ProcessId + " / " + launchApp.WindowHandle);

                //Close the process or app
                bool ClosedProcess = CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(launchApp.PathExe), false);
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