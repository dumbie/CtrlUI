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
        async Task CloseSingleProcessUwpByDataBindApp(DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                Popup_Show_Status("Closing", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing UWP application: " + dataBindApp.Name);

                //Close the process or app
                bool closedProcess = await CloseProcessUwpByWindowHandleOrProcessId(dataBindApp.Name, dataBindApp.ProcessMulti.Identifier, dataBindApp.ProcessMulti.WindowHandle);
                if (closedProcess)
                {
                    //Reset the process running status
                    if (resetProcess)
                    {
                        dataBindApp.StatusRunning = Visibility.Collapsed;
                        dataBindApp.StatusSuspended = Visibility.Collapsed;
                        dataBindApp.RunningProcessCount = string.Empty;
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
                Debug.WriteLine("Closing UWP processes: " + dataBindApp.Name + " / " + dataBindApp.ProcessMulti.Identifier + " / " + dataBindApp.ProcessMulti.WindowHandle);

                //Get the process id by window handle
                if (dataBindApp.ProcessMulti.Identifier <= 0)
                {
                    int processId = await GetUwpProcessIdByWindowHandle(dataBindApp.Name, dataBindApp.PathExe, dataBindApp.ProcessMulti.WindowHandle);
                    dataBindApp.ProcessMulti.Identifier = processId;
                }

                //Close the process or app
                bool closedProcess = CloseProcessById(dataBindApp.ProcessMulti.Identifier);
                if (closedProcess)
                {
                    //Reset the process running status
                    if (resetProcess)
                    {
                        dataBindApp.StatusRunning = Visibility.Collapsed;
                        dataBindApp.StatusSuspended = Visibility.Collapsed;
                        dataBindApp.RunningProcessCount = string.Empty;
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