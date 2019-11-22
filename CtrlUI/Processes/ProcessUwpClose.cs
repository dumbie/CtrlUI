using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close single UWP process
        async Task CloseSingleProcessUwpByDataBindApp(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                Popup_Show_Status("Closing", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing UWP application: " + dataBindApp.Name);

                //Close the process
                bool ClosedProcess = false;
                if (processMulti != null)
                {
                    ClosedProcess = await CloseProcessUwpByWindowHandleOrProcessId(dataBindApp.Name, processMulti.ProcessId, processMulti.WindowHandle);
                }
                else
                {
                    ClosedProcess = await CloseProcessUwpByWindowHandleOrProcessId(dataBindApp.Name, dataBindApp.ProcessId, dataBindApp.WindowHandle);
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

        //Close all processes UWP
        async Task CloseAllProcessesUwpByDataBindApp(DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                Popup_Show_Status("Closing", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing UWP processes: " + dataBindApp.Name + " / " + dataBindApp.ProcessId + " / " + dataBindApp.WindowHandle);

                //Get the process id by window handle
                if (dataBindApp.ProcessId <= 0)
                {
                    dataBindApp.ProcessId = await GetUwpProcessIdByWindowHandle(dataBindApp.Name, dataBindApp.PathExe, dataBindApp.WindowHandle);
                }

                //Close the process or app
                bool ClosedProcess = CloseProcessById(dataBindApp.ProcessId);
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