using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Select process action
        async Task SelectProcessAction(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                Debug.WriteLine("Select process action: " + dataBindApp.Name + "/" + dataBindApp.Type + "/" + dataBindApp.Category);

                //Check if process is file explorer
                bool processIsExplorer = Path.GetFileName(dataBindApp.PathExe).ToLower() == "explorer.exe";
                if (!processIsExplorer && processMulti != null)
                {
                    processIsExplorer = processMulti.ExeName.ToLower() == "explorer.exe";
                }

                //Select process multi
                if (processMulti == null)
                {
                    ProcessMultiAction processMultiAction = await SelectProcessMulti(dataBindApp);
                    if (processMultiAction.Action == ProcessMultiActions.Launch)
                    {
                        Debug.WriteLine("Launching the application.");
                        await LaunchProcessDatabindAuto(dataBindApp);
                        return;
                    }
                    else if (processMultiAction.Action == ProcessMultiActions.CloseAll)
                    {
                        Debug.WriteLine("Closing all processes, skipping the launch.");
                        await CloseAllProcessesAuto(dataBindApp, true, false);
                        return;
                    }
                    else if (processMultiAction.Action == ProcessMultiActions.Cancel)
                    {
                        Debug.WriteLine("Cancelled process selection, skipping the launch.");
                        return;
                    }
                    else if (processMultiAction.Action == ProcessMultiActions.Select)
                    {
                        processMulti = processMultiAction.ProcessMulti;
                        Debug.WriteLine("Selected process: " + processMulti.ExeName);
                    }
                }

                //Set messagebox answers
                List<DataBindString> Answers = new List<DataBindString>();

                //Check window handle main
                DataBindString AnswerShow = new DataBindString();
                DataBindString AnswerHide = new DataBindString();
                if (processMulti.WindowHandleMain != IntPtr.Zero)
                {
                    AnswerShow.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMiniMaxi.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    AnswerShow.Name = "Show application";
                    Answers.Add(AnswerShow);

                    AnswerHide.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMinimize.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    AnswerHide.Name = "Hide application";
                    Answers.Add(AnswerHide);
                }

                DataBindString AnswerClose = new DataBindString();
                if (!processIsExplorer)
                {
                    AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    AnswerClose.Name = "Close application";
                    Answers.Add(AnswerClose);
                }

                DataBindString AnswerLaunch = new DataBindString();
                AnswerLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerLaunch.Name = "Launch new instance";
                Answers.Add(AnswerLaunch);

                bool availableArgument = !string.IsNullOrWhiteSpace(dataBindApp.Argument);
                bool emulatorArgument = dataBindApp.Category == AppCategory.Emulator && !dataBindApp.LaunchSkipRom;
                bool filepickerArgument = dataBindApp.Category != AppCategory.Emulator && dataBindApp.LaunchFilePicker;
                bool defaultArgument = availableArgument || emulatorArgument || filepickerArgument;
                DataBindString AnswerRestartDefault = new DataBindString();
                if (defaultArgument && !processIsExplorer)
                {
                    AnswerRestartDefault.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    AnswerRestartDefault.Name = "Restart application";
                    AnswerRestartDefault.NameSub = "(Default argument)";
                    Answers.Add(AnswerRestartDefault);
                }

                bool currentMatchesDefaultArgument = processMulti.Argument == dataBindApp.Argument;
                bool currentArgument = !string.IsNullOrWhiteSpace(processMulti.Argument);
                DataBindString AnswerRestartCurrent = new DataBindString();
                if (currentArgument && !currentMatchesDefaultArgument && !processIsExplorer)
                {
                    AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    AnswerRestartCurrent.Name = "Restart application";
                    AnswerRestartCurrent.NameSub = "(Current argument)";
                    Answers.Add(AnswerRestartCurrent);
                }

                DataBindString AnswerRestartWithout = new DataBindString();
                if (!processIsExplorer)
                {
                    AnswerRestartWithout.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    AnswerRestartWithout.Name = "Restart application";
                    AnswerRestartWithout.NameSub = "(Without argument)";
                    Answers.Add(AnswerRestartWithout);
                }

                DataBindString AnswerAutoHDR = new DataBindString();
                bool applicationAutoHDR = false;
                if (!processIsExplorer && dataBindApp.Type == ProcessType.Win32)
                {
                    AnswerAutoHDR.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/MonitorHDR.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    applicationAutoHDR = CheckApplicationAutoHDR(dataBindApp);
                    if (applicationAutoHDR)
                    {
                        AnswerAutoHDR.Name = "Disable Windows Auto HDR support";
                    }
                    else
                    {
                        AnswerAutoHDR.Name = "Enable Windows Auto HDR support";
                    }
                    Answers.Add(AnswerAutoHDR);
                }

                //Get launch information
                string launchInformation = string.Empty;
                if (processMulti.Type == ProcessType.UWP || processMulti.Type == ProcessType.Win32Store)
                {
                    launchInformation = processMulti.AppUserModelId + " (" + processMulti.ExeName + ")";
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
                if (currentArgument && !currentMatchesDefaultArgument)
                {
                    launchInformation += "\nCurrent argument: " + AVFunctions.StringCut(processMulti.Argument, 50, "...");
                }

                //Set category title
                string categoryTitle = string.Empty;
                if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    categoryTitle = "Shortcut process";
                }
                else if (dataBindApp.Category == AppCategory.Launcher)
                {
                    categoryTitle = "Launcher process";
                }
                else if (dataBindApp.Category == AppCategory.Process)
                {
                    categoryTitle = "Process";
                }
                else
                {
                    categoryTitle = "Application process";
                }

                //Get process running time
                string processRunningTimeString = ApplicationRunningTimeString(dataBindApp.StatusProcessRunningTime, categoryTitle);
                if (!string.IsNullOrWhiteSpace(processRunningTimeString))
                {
                    launchInformation += "\n" + processRunningTimeString;
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", launchInformation, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerShow)
                    {
                        await ShowProcessWindowAuto(dataBindApp, processMulti);
                    }
                    else if (messageResult == AnswerHide)
                    {
                        await HideProcessWindowAuto(dataBindApp, processMulti);
                    }
                    else if (messageResult == AnswerClose)
                    {
                        if (dataBindApp.Category == AppCategory.Process)
                        {
                            await CloseSingleProcessAuto(processMulti, dataBindApp, false, true);
                        }
                        else
                        {
                            await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                        }
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
                    else if (messageResult == AnswerAutoHDR)
                    {
                        if (applicationAutoHDR)
                        {
                            DisableApplicationAutoHDR(dataBindApp);
                        }
                        else
                        {
                            //Enable Windows auto HDR feature
                            EnableWindowsAutoHDRFeature();

                            //Allow auto HDR for application
                            EnableApplicationAutoHDR(dataBindApp);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Cancelled process select action.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed selecting process action: " + ex.Message);
            }
        }
    }
}