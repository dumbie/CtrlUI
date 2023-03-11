using ArnoldVinkCode;
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
        //Check if databind paths are available
        async Task<bool> CheckDatabindPathAuto(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.Category == AppCategory.Emulator && !dataBindApp.LaunchSkipRom)
                {
                    //Check if the rom folder exists
                    if (!Directory.Exists(dataBindApp.PathRoms))
                    {
                        await Notification_Send_Status("Close", "Rom folder not found");
                        Debug.WriteLine("Rom folder not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }

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

                bool currentArgument = !string.IsNullOrWhiteSpace(processMulti.Argument);
                DataBindString AnswerRestartCurrent = new DataBindString();
                if (currentArgument)
                {
                    AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerRestartCurrent.Name = "Restart application";
                    AnswerRestartCurrent.NameSub = "(Current argument)";
                    Answers.Add(AnswerRestartCurrent);
                }

                bool availableArgument = !string.IsNullOrWhiteSpace(dataBindApp.Argument);
                bool emulatorArgument = dataBindApp.Category == AppCategory.Emulator && !dataBindApp.LaunchSkipRom;
                bool filepickerArgument = dataBindApp.Category != AppCategory.Emulator && dataBindApp.LaunchFilePicker;
                bool defaultArgument = availableArgument || emulatorArgument || filepickerArgument;
                DataBindString AnswerRestartDefault = new DataBindString();
                if (defaultArgument)
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

                //Add process identifier
                if (processMulti.Identifier != 0)
                {
                    launchInformation += " (" + processMulti.Identifier + ")";
                }

                //Add launch argument
                if (currentArgument)
                {
                    launchInformation += "\nCurrent argument: " + AVFunctions.StringCut(processMulti.Argument, 50, "...");
                }
                if (defaultArgument)
                {
                    if (emulatorArgument)
                    {
                        launchInformation += "\nDefault argument: Select a rom";
                    }
                    else if (filepickerArgument)
                    {
                        launchInformation += "\nDefault argument: Select a file";
                    }
                    else
                    {
                        launchInformation += "\nDefault argument: " + dataBindApp.Argument;
                    }
                }

                if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    //Get process running time
                    string processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "Shortcut process");
                    if (!string.IsNullOrWhiteSpace(processRunningTimeString))
                    {
                        launchInformation += "\n" + processRunningTimeString;
                    }
                }
                else
                {
                    //Get process running time
                    string processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "Application process");
                    if (!string.IsNullOrWhiteSpace(processRunningTimeString))
                    {
                        launchInformation += "\n" + processRunningTimeString;
                    }

                    //Get process last launch time
                    string lastLaunchTimeString = ApplicationLastLaunchTimeString(dataBindApp.LastLaunch, "Application");
                    if (!string.IsNullOrWhiteSpace(lastLaunchTimeString))
                    {
                        launchInformation += "\n" + lastLaunchTimeString;
                    }
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", launchInformation, "", Answers);
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