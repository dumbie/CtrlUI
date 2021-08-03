using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task AllMonitorSwitchHDR(bool enableHDR)
        {
            try
            {
                if (enableHDR)
                {
                    await Notification_Send_Status("MonitorHDR", "Enabling monitor HDR");
                }
                else
                {
                    await Notification_Send_Status("MonitorHDR", "Disabling monitor HDR");
                }

                //Switch hdr for all monitors
                int screenCount = Screen.AllScreens.Count();
                for (int i = 0; i < screenCount; i++)
                {
                    SetMonitorHDR(i, enableHDR);
                }

                //Wait for hdr to have enabled
                await Task.Delay(500);
            }
            catch
            {
                Debug.WriteLine("Failed to switch monitor HDR.");
                await Notification_Send_Status("MonitorHDR", "Failed switching HDR");
            }
        }

        async Task SwitchDisplayMonitor()
        {
            try
            {
                Debug.WriteLine("Listing all the connected display monitors.");

                List<DataBindString> Answers = new List<DataBindString>();

                //Get all the connected display monitors
                List<DisplayMonitor> monitorsList = GetAllMonitorDisplayConfig();

                //Add all display monitors to answers list
                foreach (DisplayMonitor displayMonitor in monitorsList)
                {
                    DataBindString AnswerMonitor = new DataBindString();
                    AnswerMonitor.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/MonitorSwitch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerMonitor.Name = displayMonitor.Name;
                    Answers.Add(AnswerMonitor);
                }

                DataBindString AnswerPrimary = new DataBindString();
                AnswerPrimary.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/MonitorSwitch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerPrimary.Name = "Primary monitor";
                Answers.Add(AnswerPrimary);

                DataBindString AnswerSecondary = new DataBindString();
                AnswerSecondary.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/MonitorSwitch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerSecondary.Name = "Secondary monitor";
                Answers.Add(AnswerSecondary);

                DataBindString AnswerDuplicate = new DataBindString();
                AnswerDuplicate.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/MonitorSwitch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerDuplicate.Name = "Duplicate mode";
                Answers.Add(AnswerDuplicate);

                DataBindString AnswerExtend = new DataBindString();
                AnswerExtend.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/MonitorSwitch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerExtend.Name = "Extend mode";
                Answers.Add(AnswerExtend);

                //Show the messagebox prompt
                DataBindString messageResult = await Popup_Show_MessageBox("Switch display monitor", "", "Please select the display monitor you want to use.", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerPrimary)
                    {
                        await Notification_Send_Status("MonitorSwitch", "Switching primary monitor");
                        EnableMonitorFirst();
                    }
                    else if (messageResult == AnswerSecondary)
                    {
                        await Notification_Send_Status("MonitorSwitch", "Switching secondary monitor");
                        EnableMonitorSecond();
                    }
                    else if (messageResult == AnswerDuplicate)
                    {
                        await Notification_Send_Status("MonitorSwitch", "Cloning display monitor");
                        EnableMonitorCloneMode();
                    }
                    else if (messageResult == AnswerExtend)
                    {
                        await Notification_Send_Status("MonitorSwitch", "Extending display monitor");
                        EnableMonitorExtendMode();
                    }
                    else
                    {
                        DisplayMonitor changeDevice = monitorsList.Where(x => x.Name.ToLower() == messageResult.Name.ToLower()).FirstOrDefault();
                        if (changeDevice != null)
                        {
                            await Notification_Send_Status("MonitorSwitch", "Switching display monitor");
                            if (!SetMonitorPrimary((uint)changeDevice.Identifier))
                            {
                                await Notification_Send_Status("MonitorSwitch", "Failed switching monitor");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the display monitors: " + ex.Message);
                await Notification_Send_Status("MonitorSwitch", "No display monitors");
            }
        }

        //Prevent or allow monitor sleep
        void UpdateMonitorSleepAuto()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "MonitorPreventSleep")))
                    {
                        Debug.WriteLine("Preventing monitor to sleep.");
                        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
                    }
                    else
                    {
                        Debug.WriteLine("Allowing monitor to sleep.");
                        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                    }
                });
            }
            catch { }
        }

        //Allow monitor sleep
        void UpdateMonitorSleepAllow()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    Debug.WriteLine("Allowing monitor to sleep.");
                    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                });
            }
            catch { }
        }
    }
}