﻿using ArnoldVinkCode;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;

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
                AVActions.TaskStartLoop(UpdateStatsFps, vTask_UpdateStatsFps);

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

        void UpdateFpsVisibility()
        {
            //Check the total available frames and last added frame time
            if (!vListFrameTimes.Any() || (GetSystemTicksMs() - vLastFrameTimeUpdate) >= 1000)
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    stackpanel_CurrentFrametime.Visibility = Visibility.Collapsed;
                    stackpanel_CurrentFps.Visibility = Visibility.Collapsed;
                });
            }
            else
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    stackpanel_CurrentFrametime.Visibility = Visibility.Visible;
                    bool stringEmpty = string.IsNullOrWhiteSpace(textblock_CurrentFps.Text) || textblock_CurrentFps.Text == vTitleFPS;
                    if (!stringEmpty)
                    {
                        stackpanel_CurrentFps.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_CurrentFps.Visibility = Visibility.Collapsed;
                    }
                });
            }
        }

        async Task UpdateStatsFps()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateStatsFps))
                {
                    try
                    {
                        //Update fps visibility
                        UpdateFpsVisibility();

                        //Calculate the current fps (1sec)
                        double CurrentFrameTimes = vListFrameTimes.Take(100).Average();
                        int CurrentFramesPerSecond = Convert.ToInt32(1000 / CurrentFrameTimes);

                        //Calculate the average fps (10sec)
                        double AverageFrameTimes = vListFrameTimes.Take(1000).Average();
                        int AverageFramesPerSecond = Convert.ToInt32(1000 / AverageFrameTimes);

                        //Convert fps to string
                        string StringCurrentFramesPerSecond = string.Empty;
                        if (SettingLoad(vConfigurationFpsOverlayer, "FpsShowCurrentFps", typeof(bool))) { StringCurrentFramesPerSecond = " " + CurrentFramesPerSecond.ToString() + "FPS"; }
                        string StringCurrentFrameTimes = string.Empty;
                        if (SettingLoad(vConfigurationFpsOverlayer, "FpsShowCurrentLatency", typeof(bool))) { StringCurrentFrameTimes = " " + CurrentFrameTimes.ToString("0.00") + "MS"; }
                        string StringAverageFramesPerSecond = string.Empty;
                        if (SettingLoad(vConfigurationFpsOverlayer, "FpsShowAverageFps", typeof(bool))) { StringAverageFramesPerSecond = " " + AverageFramesPerSecond.ToString() + "AVG"; }

                        //Update the fps counter
                        Debug.WriteLine("(" + vTargetProcess.Identifier + ") MS" + CurrentFrameTimes.ToString("0.00") + " / FPS " + CurrentFramesPerSecond + " / AVG " + AverageFramesPerSecond);
                        string StringDisplay = vTitleFPS + StringCurrentFramesPerSecond + StringCurrentFrameTimes + StringAverageFramesPerSecond;
                        StringDisplay = AVFunctions.StringRemoveStart(StringDisplay, " ");
                        StringDisplay = AVFunctions.StringRemoveEnd(StringDisplay, " ");

                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            textblock_CurrentFps.Text = StringDisplay;
                        });
                    }
                    catch { }
                    finally
                    {
                        //Delay the loop task
                        await TaskDelayLoop(1000, vTask_UpdateStatsFps);
                    }
                }
            }
            catch { }
        }

        void UpdateStatsFrameTimeGraph(double frameTime)
        {
            try
            {
                double yPoint = frameTime;
                double xPoint = vFrametimeCurrent;
                if (xPoint != 0)
                {
                    xPoint += vFrametimeAccuracy;
                }
                vFrametimeCurrent++;

                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Check point height
                    if (yPoint < 2)
                    {
                        yPoint = 2;
                    }
                    else if (yPoint > 48)
                    {
                        yPoint = 48;
                    }

                    //Add frametime point
                    vPointFrameTimes.Add(new Point(xPoint, yPoint));
                    stackpanel_CurrentFrametime.ScrollToRightEnd();

                    //Cleanup frametime points (10sec)
                    if (vPointFrameTimes.Count > 1000)
                    {
                        vPointFrameTimes.RemoveAt(0);
                    }
                });

                //Debug.WriteLine("Added frametime to graph: " + frameTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding frametime to graph: " + ex.Message);
            }
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
                double timeElapsed = traceEvent.TimeStampRelativeMSec;
                double timeBetween = timeElapsed - vLastFrameTimeStamp;
                vLastFrameTimeUpdate = GetSystemTicksMs();
                vLastFrameTimeStamp = timeElapsed;

                //Check frametime
                if (timeBetween < 1000)
                {
                    //Add frametime to list
                    vListFrameTimes.Insert(0, timeBetween);

                    //Cleanup frametimes (10sec)
                    if (vListFrameTimes.Count > 1000)
                    {
                        vListFrameTimes.RemoveAt(1001);
                    }

                    //Add frametime to graph
                    UpdateStatsFrameTimeGraph(timeBetween);

                    //Debug.WriteLine("Added frametime to list: " + timeBetween);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding frametime to list: " + ex.Message);
            }
        }
    }
}