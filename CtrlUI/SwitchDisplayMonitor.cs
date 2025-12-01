using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        public async Task SwitchDisplayMonitor()
        {
            try
            {
                Debug.WriteLine("Loading display monitor options.");

                //Add all options to answers list
                List<DataBindString> answersList = new List<DataBindString>();

                DataBindString answerSwitchPrimary = new DataBindString()
                {
                    Name = "Switch to primary monitor",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/MonitorPrimary.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                };
                answersList.Add(answerSwitchPrimary);

                DataBindString answerSwitchSecondary = new DataBindString()
                {
                    Name = "Switch to secondary monitor",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/MonitorSecondary.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                };
                answersList.Add(answerSwitchSecondary);

                DataBindString answerSwitchDuplicate = new DataBindString()
                {
                    Name = "Switch to duplicate mode",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/MonitorDuplicate.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                };
                answersList.Add(answerSwitchDuplicate);

                DataBindString answerSwitchExtend = new DataBindString()
                {
                    Name = "Switch to extend mode",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/MonitorExtend.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                };
                answersList.Add(answerSwitchExtend);

                DataBindString answerHdrEnable = new DataBindString()
                {
                    Name = "Enable HDR for all monitors",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/MonitorHDR.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                };
                answersList.Add(answerHdrEnable);

                DataBindString answerHdrDisable = new DataBindString()
                {
                    Name = "Disable HDR for all monitors",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/MonitorHDR.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                };
                answersList.Add(answerHdrDisable);

                DataBindString answerAutoHdrEnable = new DataBindString()
                {
                    Name = "Enable Windows Auto HDR",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/MonitorHDR.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                };
                answersList.Add(answerAutoHdrEnable);

                DataBindString answerAutoHdrDisable = new DataBindString()
                {
                    Name = "Disable Windows Auto HDR",
                    ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/MonitorHDR.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    })
                };
                answersList.Add(answerAutoHdrDisable);

                //Show messagebox prompt
                DataBindString messageResult = await Popup_Show_MessageBox("Monitor Settings", string.Empty, string.Empty, answersList);
                if (messageResult != null)
                {
                    if (messageResult == answerSwitchPrimary)
                    {
                        await Monitor_Switch_Primary();
                    }
                    else if (messageResult == answerSwitchSecondary)
                    {
                        await Monitor_Switch_Secondary();
                    }
                    else if (messageResult == answerSwitchDuplicate)
                    {
                        await Monitor_Switch_Duplicate();
                    }
                    else if (messageResult == answerSwitchExtend)
                    {
                        await Monitor_Switch_Extend();
                    }
                    else if (messageResult == answerHdrEnable)
                    {
                        await AllMonitorSwitchHDR(true, false);
                    }
                    else if (messageResult == answerHdrDisable)
                    {
                        await AllMonitorSwitchHDR(false, false);
                    }
                    else if (messageResult == answerAutoHdrEnable)
                    {
                        await WindowsAutoHDREnable();
                    }
                    else if (messageResult == answerAutoHdrDisable)
                    {
                        await WindowsAutoHDRDisable();
                    }
                }
            }
            catch { }
        }

        async Task Monitor_Switch_Extend()
        {
            try
            {
                await Notification_Show_Status("MonitorSwitch", "Extending display monitor");

                //Enable monitor extend mode
                EnableMonitorExtendMode();

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        async Task Monitor_Switch_Duplicate()
        {
            try
            {
                await Notification_Show_Status("MonitorSwitch", "Duplicating display monitor");

                //Enable monitor clone mode
                EnableMonitorCloneMode();

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        async Task Monitor_Switch_Secondary()
        {
            try
            {
                await Notification_Show_Status("MonitorSwitch", "Switching secondary monitor");

                //Switch secondary monitor
                EnableMonitorSecond();

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        async Task Monitor_Switch_Primary()
        {
            try
            {
                await Notification_Show_Status("MonitorSwitch", "Switching primary monitor");

                //Switch primary monitor
                EnableMonitorFirst();

                //Focus on CtrlUI window
                await AppWindowShow(true, true);
            }
            catch { }
        }

        //Enable monitor HDR and Windows Auto HDR
        async Task EnableHDRDatabindAuto(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.LaunchEnableDisplayHDR || dataBindApp.LaunchEnableAutoHDR)
                {
                    //Enable monitor HDR
                    await AllMonitorSwitchHDR(true, false);
                }

                if (dataBindApp.LaunchEnableAutoHDR)
                {
                    //Enable Windows auto HDR feature
                    await WindowsAutoHDREnable();

                    //Force auto HDR for application
                    await ApplicationForceAutoHDREnable(dataBindApp);
                }

                //Wait for HDR initialization
                if (dataBindApp.LaunchEnableDisplayHDR || dataBindApp.LaunchEnableAutoHDR)
                {
                    await Task.Delay(500);
                }
            }
            catch { }
        }

        //Enable or disable monitor HDR
        async Task AllMonitorSwitchHDR(bool enableHDR, bool waitHDR)
        {
            try
            {
                if (enableHDR)
                {
                    await Notification_Show_Status("MonitorHDR", "Enabling monitor HDR");
                    Debug.WriteLine("Enabling monitor HDR.");
                }
                else
                {
                    await Notification_Show_Status("MonitorHDR", "Disabling monitor HDR");
                    Debug.WriteLine("Disabling monitor HDR.");
                }

                //Switch hdr for all monitors
                int screenCount = Screen.AllScreens.Count();
                for (int i = 0; i < screenCount; i++)
                {
                    SetMonitorHDR(i, enableHDR);
                }

                //Wait for HDR initialization
                if (waitHDR)
                {
                    await Task.Delay(500);
                }
            }
            catch
            {
                Debug.WriteLine("Failed to switch monitor HDR.");
                await Notification_Show_Status("MonitorHDR", "Failed switching HDR");
            }
        }

        //Prevent or allow monitor sleep
        void UpdateMonitorSleepAuto()
        {
            try
            {
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    if (vSettings.Load("MonitorPreventSleep", typeof(bool)))
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
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    Debug.WriteLine("Allowing monitor to sleep.");
                    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                });
            }
            catch { }
        }
    }
}