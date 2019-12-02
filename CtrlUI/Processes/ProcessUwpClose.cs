using System.Diagnostics;
using System.Linq;
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
        async Task CloseSingleProcessUwp(DataBindApp dataBindApp, ProcessMulti processMulti, bool resetProcess, bool removeProcess)
        {
            try
            {
                Popup_Show_Status("Closing", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing UWP process: " + dataBindApp.Name);

                //Close the process
                bool closedProcess = await CloseProcessUwpByWindowHandleOrProcessId(dataBindApp.Name, processMulti.Identifier, processMulti.WindowHandle);
                if (closedProcess)
                {
                    Popup_Show_Status("Closing", "Closed " + dataBindApp.Name);
                    Debug.WriteLine("Closed UWP process: " + dataBindApp.Name);

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
        async Task CloseAllProcessesUwp(DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                //Get the multi process
                ProcessMulti processMulti = dataBindApp.ProcessMulti.FirstOrDefault();

                Popup_Show_Status("Closing", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing all UWP processes: " + dataBindApp.Name + " / " + processMulti.Identifier);

                //Close the process
                bool closedProcess = CloseProcessById(processMulti.Identifier);
                if (closedProcess)
                {
                    Popup_Show_Status("Closing", "Closed " + dataBindApp.Name);
                    Debug.WriteLine("Closed all UWP processes: " + dataBindApp.Name);

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