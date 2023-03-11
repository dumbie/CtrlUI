using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RightClickProcess(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                //Get the process multi
                ProcessMulti processMulti = dataBindApp.ProcessMulti.FirstOrDefault();

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

                bool defaultArgument = !string.IsNullOrWhiteSpace(dataBindApp.Argument) || dataBindApp.Category == AppCategory.Shortcut || dataBindApp.Category == AppCategory.Emulator || dataBindApp.LaunchFilePicker;
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
                    if (dataBindApp.Category == AppCategory.Emulator && !dataBindApp.LaunchSkipRom)
                    {
                        launchInformation += "\nDefault argument: Select a rom";
                    }
                    else if (dataBindApp.Category != AppCategory.Emulator && dataBindApp.LaunchFilePicker)
                    {
                        launchInformation += "\nDefault argument: Select a file";
                    }
                    else
                    {
                        launchInformation += "\nDefault argument: " + dataBindApp.Argument;
                    }
                }

                //Get process running time
                string processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "Process");
                if (!string.IsNullOrWhiteSpace(processRunningTimeString))
                {
                    launchInformation += "\n" + processRunningTimeString;
                }

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", launchInformation, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerShow)
                    {
                        await ShowProcessWindowAuto(dataBindApp, processMulti);
                    }
                    else if (messageResult == AnswerClose)
                    {
                        await CloseSingleProcessAuto(processMulti, dataBindApp, false, true);
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
            }
            catch { }
        }
    }
}