using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVUwpAppx;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check process windows
        async Task<IntPtr> CheckProcessWindowsAuto(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    return processMulti.WindowHandleMain;
                }
                else if (processMulti.Type == ProcessType.Win32 || processMulti.Type == ProcessType.Win32Store)
                {
                    return await CheckProcessWindowsWin32AndWin32Store(dataBindApp, processMulti);
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        //Check if databind paths are available
        async Task<bool> CheckDatabindPathAuto(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    //Check if the application exists
                    if (GetUwpAppPackageByAppUserModelId(dataBindApp.AppUserModelId) == null)
                    {
                        await Notification_Send_Status("Close", "Application not found");
                        Debug.WriteLine("Launch application not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }
                else if (dataBindApp.Category == AppCategory.Emulator && !dataBindApp.LaunchSkipRom)
                {
                    //Check if the rom folder exists
                    if (!Directory.Exists(dataBindApp.PathRoms))
                    {
                        await Notification_Send_Status("Close", "Rom folder not found");
                        Debug.WriteLine("Rom folder not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }

                    //Check if application executable exists
                    if (!File.Exists(dataBindApp.PathExe))
                    {
                        await Notification_Send_Status("Close", "Executable not found");
                        Debug.WriteLine("Launch executable not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }
                else
                {
                    //Check if application executable exists
                    if (!File.Exists(dataBindApp.PathExe))
                    {
                        await Notification_Send_Status("Close", "Executable not found");
                        Debug.WriteLine("Launch executable not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }

                //Paths are available update status
                dataBindApp.StatusAvailable = Visibility.Collapsed;
            }
            catch { }
            return true;
        }

        //Check process status before launching
        async Task CheckLaunchProcessStatus(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                Debug.WriteLine("Checking launch process: " + dataBindApp.Name);

                //Focus or Close when process is already running
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerShow = new DataBindString();
                AnswerShow.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMiniMaxi.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerShow.Name = "Show application";
                Answers.Add(AnswerShow);

                DataBindString AnswerClose = new DataBindString();
                AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerClose.Name = "Close application";
                Answers.Add(AnswerClose);

                DataBindString AnswerLaunch = new DataBindString();
                AnswerLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerLaunch.Name = "Launch new instance";
                Answers.Add(AnswerLaunch);

                DataBindString AnswerRestartCurrent = new DataBindString();
                if (!string.IsNullOrWhiteSpace(processMulti.Argument))
                {
                    AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerRestartCurrent.Name = "Restart application";
                    AnswerRestartCurrent.NameSub = "(Current argument *)";
                    Answers.Add(AnswerRestartCurrent);
                }

                DataBindString AnswerRestartDefault = new DataBindString();
                if (!string.IsNullOrWhiteSpace(dataBindApp.Argument) || dataBindApp.Category == AppCategory.Shortcut || dataBindApp.Category == AppCategory.Emulator || dataBindApp.LaunchFilePicker)
                {
                    AnswerRestartDefault.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerRestartDefault.Name = "Restart application";
                    AnswerRestartDefault.NameSub = "(Default argument)";
                    Answers.Add(AnswerRestartDefault);
                }

                DataBindString AnswerRestartWithout = new DataBindString();
                AnswerRestartWithout.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRestartWithout.Name = "Restart application";
                AnswerRestartWithout.NameSub = "(Without argument)";
                Answers.Add(AnswerRestartWithout);

                //Get launch information
                string launchInformation = string.Empty;
                if (processMulti.Type == ProcessType.UWP || processMulti.Type == ProcessType.Win32Store)
                {
                    launchInformation = processMulti.AppUserModelId;
                }
                else
                {
                    launchInformation = processMulti.ExePath;
                }

                //Add launch argument
                if (!string.IsNullOrWhiteSpace(processMulti.Argument))
                {
                    launchInformation += " *(" + processMulti.Argument + ")";
                }

                //Get process running time and last launch time
                string processRunningTimeString = string.Empty;
                string lastLaunchTimeString = string.Empty;
                if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "shortcut process");
                }
                else
                {
                    processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "application process");
                    lastLaunchTimeString = ApplicationLastLaunchTimeString(dataBindApp.LastLaunch, "Application");
                }

                //Set the running time string
                bool runningTimeEmpty = string.IsNullOrWhiteSpace(processRunningTimeString);
                bool launchTimeEmpty = string.IsNullOrWhiteSpace(lastLaunchTimeString);
                if (runningTimeEmpty && launchTimeEmpty)
                {
                    processRunningTimeString = launchInformation;
                }
                else
                {
                    if (!launchTimeEmpty)
                    {
                        processRunningTimeString += "\n" + lastLaunchTimeString;
                    }
                    processRunningTimeString += "\n" + launchInformation;
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTimeString, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerShow)
                    {
                        await ShowProcessWindowAuto(dataBindApp, processMulti);
                    }
                    else if (messageResult == AnswerClose)
                    {
                        await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                    }
                    else if (messageResult == AnswerRestartCurrent)
                    {
                        await RestartProcessAuto(processMulti, dataBindApp, true, false, false);
                    }
                    else if (messageResult == AnswerRestartDefault)
                    {
                        await RestartProcessAuto(processMulti, dataBindApp, false, true, false);
                    }
                    else if (messageResult == AnswerRestartWithout)
                    {
                        await RestartProcessAuto(processMulti, dataBindApp, false, false, true);
                    }
                    else if (messageResult == AnswerLaunch)
                    {
                        await LaunchProcessDatabindAuto(dataBindApp);
                    }
                }
                else
                {
                    Debug.WriteLine("Cancelling the process action.");
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Close", "Failed showing or closing application");
                Debug.WriteLine("Failed closing or showing the application: " + ex.Message);
            }
        }
    }
}