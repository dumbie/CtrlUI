using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close single process Win32 and Win32Store
        async Task CloseSingleProcessWin32AndWin32StoreByDataBindApp(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                Popup_Show_Status("Closing", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing Win32 application: " + dataBindApp.Name);

                //Close the process
                bool ClosedProcess = false;
                if (processMulti != null)
                {
                    ClosedProcess = CloseProcessById(processMulti.ProcessId);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dataBindApp.NameExe))
                    {
                        ClosedProcess = CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(dataBindApp.NameExe), false);
                    }
                    else
                    {
                        ClosedProcess = CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(dataBindApp.PathExe), false);
                    }
                }

                //Check if process closed
                if (ClosedProcess)
                {
                    //Reset the process running status
                    if (resetProcess)
                    {
                        dataBindApp.StatusRunning = Visibility.Collapsed;
                        dataBindApp.StatusSuspended = Visibility.Collapsed;
                        dataBindApp.ProcessRunningCount = string.Empty;
                        dataBindApp.RunningTimeLastUpdate = 0;
                    }

                    //Remove the process from the list
                    if (removeProcess)
                    {
                        await RemoveAppFromList(dataBindApp, false, false, true);
                    }
                }
                else
                {
                    Popup_Show_Status("Closing", "Failed to close the app");
                    Debug.WriteLine("Failed to close the application.");
                }
            }
            catch { }
        }

        //Close all processes Win32 and Win32Store
        async Task CloseAllProcessesWin32AndWin32StoreByDataBindApp(DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                Popup_Show_Status("Closing", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing Win32 processes: " + dataBindApp.Name + " / " + dataBindApp.ProcessId + " / " + dataBindApp.WindowHandle);

                //Close the process
                bool ClosedProcess = false;
                if (!string.IsNullOrWhiteSpace(dataBindApp.NameExe))
                {
                    ClosedProcess = CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(dataBindApp.NameExe), false);
                }
                else
                {
                    ClosedProcess = CloseProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(dataBindApp.PathExe), false);
                }

                //Check if process closed
                if (ClosedProcess)
                {
                    //Reset the process running status
                    if (resetProcess)
                    {
                        dataBindApp.StatusRunning = Visibility.Collapsed;
                        dataBindApp.StatusSuspended = Visibility.Collapsed;
                        dataBindApp.ProcessRunningCount = string.Empty;
                        dataBindApp.RunningTimeLastUpdate = 0;
                    }

                    //Remove the process from the list
                    if (removeProcess)
                    {
                        await RemoveAppFromList(dataBindApp, false, false, true);
                    }
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