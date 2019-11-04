using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        void StartMonitorProcess()
        {
            try
            {
                vTaskToken_MonitorProcess = new CancellationTokenSource();
                vTask_MonitorProcess = AVActions.TaskStart(MonitorProcess, vTaskToken_MonitorProcess);

                Debug.WriteLine("Started monitoring processes.");
            }
            catch { }
        }

        async void MonitorProcess()
        {
            try
            {
                while (TaskRunningCheck(vTaskToken_MonitorProcess))
                {
                    try
                    {
                        //Get and check the focused process
                        ProcessFocus ForegroundProcess = GetFocusedProcess();
                        if (ForegroundProcess == null || ForegroundProcess.Process == null)
                        {
                            Debug.WriteLine("No active or valid process found.");
                            ResetFpsCounter();
                            await Task.Delay(1000);
                            continue;
                        }

                        Debug.WriteLine("Checking process: (" + ForegroundProcess.Process.Id + ") " + ForegroundProcess.Process.ProcessName);

                        //Check if the foreground window has changed
                        if (vTargetProcessId == ForegroundProcess.Process.Id)
                        {
                            //Debug.WriteLine("Foreground window has not changed.");
                            UpdateProcessIdTitle(ForegroundProcess.Process);
                            await Task.Delay(1000);
                            continue;
                        }

                        Debug.WriteLine("New foreground window detected (" + ForegroundProcess.Process.Id + ") " + ForegroundProcess.Process.ProcessName);

                        //Update the stats text position
                        UpdateWindowPosition(ForegroundProcess.Process.ProcessName);

                        //Reset the fps counter for new process
                        ResetFpsCounter();

                        //Check if the foreground window is fps overlayer
                        if (vCurrentProcessId == ForegroundProcess.Process.Id)
                        {
                            //Debug.WriteLine("Current process is fps overlayer.");
                            await Task.Delay(1000);
                            continue;
                        }

                        //Check if process is not in blacklist
                        if (vFpsBlacklistProcess.Any(x => x.ToLower() == ForegroundProcess.Process.ProcessName.ToLower()))
                        {
                            //Debug.WriteLine("Current app is blacklisted: " + ForegroundProcess.Process.ProcessName);
                            await Task.Delay(1000);
                            continue;
                        }

                        //Update the current foreground id and name
                        UpdateProcessIdTitle(ForegroundProcess.Process);
                    }
                    catch { }
                    await Task.Delay(1000);
                }
            }
            catch { }
        }

        void UpdateProcessIdTitle(Process ForegroundProcess)
        {
            try
            {
                //Debug.WriteLine("Updating the process id and title.");
                vTargetProcessId = ForegroundProcess.Id;
                vTargetProcessTitle = ForegroundProcess.MainWindowTitle;
                if (string.IsNullOrWhiteSpace(vTargetProcessTitle)) { vTargetProcessTitle = ForegroundProcess.ProcessName; }
                vTargetProcessTitle = AVFunctions.StringRemoveStart(vTargetProcessTitle, " ");
            }
            catch { }
        }

        void ResetFpsCounter()
        {
            try
            {
                //Debug.WriteLine("Resetting the frames per second counter.");
                vTargetProcessId = -1;
                vTargetProcessTitle = string.Empty;
                vLastFrameTimeStamp = 0;
                vListFrameTime.Clear();
                GC.Collect();
            }
            catch { }
        }
    }
}