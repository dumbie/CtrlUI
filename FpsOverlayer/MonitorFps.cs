using ArnoldVinkCode;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

namespace FpsOverlayer
{
    public partial class WindowMain
    {
        void StartMonitorFps()
        {
            try
            {
                vTraceEventSession = new TraceEventSession("FpsOverlayer");
                vTraceEventSession.EnableProvider(vProvider_DxgKrnl.ToString());
                vTraceEventSession.Source.AllEvents += ProcessEvents;

                AVActions.TaskStart(TaskTraceEventSource);
                AVActions.TaskStartLoop(LoopTraceEventOutput, vTask_TraceEventOutput);

                Debug.WriteLine("Started monitoring fps.");
            }
            catch { }
        }

        void TaskTraceEventSource()
        {
            try
            {
                vTraceEventSession.Source.Process();
            }
            catch { }
        }

        async Task LoopTraceEventOutput()
        {
            try
            {
                while (!vTask_TraceEventOutput.TaskStopRequest)
                {
                    try
                    {
                        //Check the total available frames and last added frame time
                        int TotalFrameTimes = vListFrameTime.Count;
                        bool SkipCurrentFrames = (GetSystemTicksMs() - vLastFrameTimeAdded) >= 1000;
                        if (SkipCurrentFrames || TotalFrameTimes <= 0)
                        {
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                stackpanel_CurrentFps.Visibility = Visibility.Collapsed;
                            });

                            //Delay the loop task
                            await TaskDelayLoop(1000, vTask_TraceEventOutput);
                            continue;
                        }

                        //Reverse the frames list
                        IEnumerable<double> ReversedFrameList = vListFrameTime.AsEnumerable().Reverse();

                        //Calculate the current fps
                        double CurrentFrameTimes = ReversedFrameList.Take(100).Average(); //1sec
                        int CurrentFramesPerSecond = Convert.ToInt32(1000 / CurrentFrameTimes);

                        //Calculate the average fps
                        double AverageFrameTimes = ReversedFrameList.Average();
                        int AverageFramesPerSecond = Convert.ToInt32(1000 / AverageFrameTimes);

                        //Convert fps to string
                        string StringCurrentFramesPerSecond = string.Empty;
                        if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "FpsShowCurrentFps"))) { StringCurrentFramesPerSecond = " " + CurrentFramesPerSecond.ToString() + "FPS"; }
                        string StringCurrentFrameTimes = string.Empty;
                        if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "FpsShowCurrentLatency"))) { StringCurrentFrameTimes = " " + CurrentFrameTimes.ToString("0.00") + "MS"; }
                        string StringAverageFramesPerSecond = string.Empty;
                        if (Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "FpsShowAverageFps"))) { StringAverageFramesPerSecond = " " + AverageFramesPerSecond.ToString() + "AVG"; }

                        //Update the fps counter
                        Debug.WriteLine("(" + vTargetProcess.Identifier + ") MS" + CurrentFrameTimes.ToString("0.00") + " / FPS " + CurrentFramesPerSecond + " / AVG " + AverageFramesPerSecond);
                        string StringDisplay = AVFunctions.StringRemoveStart(vTitleFPS + StringCurrentFramesPerSecond + StringCurrentFrameTimes + StringAverageFramesPerSecond, " ");

                        if (!string.IsNullOrWhiteSpace(StringDisplay))
                        {
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                textblock_CurrentFps.Text = StringDisplay;
                                stackpanel_CurrentFps.Visibility = Visibility.Visible;
                            });
                        }
                        else
                        {
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                stackpanel_CurrentFps.Visibility = Visibility.Collapsed;
                            });
                        }
                    }
                    catch { }

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_TraceEventOutput);
                }
            }
            catch { }
        }

        void ProcessEvents(TraceEvent traceEvent)
        {
            try
            {
                //Check the event id
                if ((int)traceEvent.ID != vEventID_DxgKrnlPresent)
                {
                    //Debug.WriteLine("DxgKrnl skipping invalid frame.");
                    return;
                }

                //Check the process id
                if (traceEvent.ProcessID != vTargetProcess.Identifier)
                {
                    //Debug.WriteLine("Event process is not foreground window.");
                    return;
                }

                //Calculate new frame time
                double TimeElapsed = traceEvent.TimeStampRelativeMSec;
                double TimeBetween = TimeElapsed - vLastFrameTimeStamp;
                vLastFrameTimeAdded = GetSystemTicksMs();
                vLastFrameTimeStamp = TimeElapsed;

                //Add frame time to the list
                if (TimeBetween < 1000)
                {
                    if (vListFrameTime.Count > 1000) { vListFrameTime.RemoveAt(0); } //10sec
                    vListFrameTime.Add(TimeBetween);
                    //Debug.WriteLine("Added new frame time to the list: " + TimeBetween);
                }
            }
            catch { }
        }
    }
}