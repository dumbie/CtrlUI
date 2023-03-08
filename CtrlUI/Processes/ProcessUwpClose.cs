using ArnoldVinkCode;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVProcess;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close single UWP process
        async Task<bool> CloseSingleProcessUwp(DataBindApp dataBindApp, ProcessMulti processMulti, bool resetProcess, bool removeProcess)
        {
            try
            {
                await Notification_Send_Status("AppClose", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing UWP process: " + dataBindApp.Name);

                //Close the process
                bool closedProcess = AVProcessTool.Close_ProcessMessageHwnd(processMulti.WindowHandleMain);

                //Check if process closed
                if (closedProcess)
                {
                    await Notification_Send_Status("AppClose", "Closed " + dataBindApp.Name);
                    Debug.WriteLine("Closed UWP process: " + dataBindApp.Name);

                    //Reset the process running status
                    if (resetProcess)
                    {
                        dataBindApp.StatusRunning = Visibility.Collapsed;
                        dataBindApp.StatusSuspended = Visibility.Collapsed;
                        dataBindApp.RunningProcessCount = string.Empty;
                        dataBindApp.RunningTimeLastUpdate = 0;
                        dataBindApp.ProcessMulti.Clear();
                    }

                    //Remove the process from the list
                    if (removeProcess)
                    {
                        await RemoveAppFromList(dataBindApp, false, false, true);
                    }

                    return true;
                }
                else
                {
                    await Notification_Send_Status("AppClose", "Failed to close application");
                    Debug.WriteLine("Failed to close the application.");
                    return false;
                }
            }
            catch { }
            return false;
        }

        //Close all processes UWP
        async Task<bool> CloseAllProcessesUwp(DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                //Get the multi process
                ProcessMulti processMulti = dataBindApp.ProcessMulti.FirstOrDefault();

                await Notification_Send_Status("AppClose", "Closing " + dataBindApp.Name);
                Debug.WriteLine("Closing all UWP processes: " + dataBindApp.Name + " / " + processMulti.Identifier);

                //Close the process
                bool closedProcess = AVProcessTool.Close_ProcessTreeId(processMulti.Identifier);

                //Check if process closed
                if (closedProcess)
                {
                    await Notification_Send_Status("AppClose", "Closed " + dataBindApp.Name);
                    Debug.WriteLine("Closed all UWP processes: " + dataBindApp.Name);

                    //Reset the process running status
                    if (resetProcess)
                    {
                        dataBindApp.StatusRunning = Visibility.Collapsed;
                        dataBindApp.StatusSuspended = Visibility.Collapsed;
                        dataBindApp.RunningProcessCount = string.Empty;
                        dataBindApp.RunningTimeLastUpdate = 0;
                        dataBindApp.ProcessMulti.Clear();
                    }

                    //Remove the process from the list
                    if (removeProcess)
                    {
                        await RemoveAppFromList(dataBindApp, false, false, true);
                    }

                    return true;
                }
                else
                {
                    await Notification_Send_Status("AppClose", "Failed to close application");
                    Debug.WriteLine("Failed to close the application.");
                    return false;
                }
            }
            catch { }
            return false;
        }
    }
}