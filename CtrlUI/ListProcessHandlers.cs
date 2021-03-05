using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.ProcessClasses;
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

                //Get process details
                string processDetails = dataBindApp.PathExe;
                if (!string.IsNullOrWhiteSpace(dataBindApp.NameExe))
                {
                    processDetails += " (" + dataBindApp.NameExe + ")";
                }

                //Get process running time
                string processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "process");
                if (string.IsNullOrWhiteSpace(processRunningTimeString))
                {
                    processRunningTimeString = processDetails;
                }
                else
                {
                    processRunningTimeString += "\n" + processDetails;
                }

                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerShow = new DataBindString();
                AnswerShow.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMiniMaxi.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerShow.Name = "Show application";
                Answers.Add(AnswerShow);

                DataBindString AnswerClose = new DataBindString();
                AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerClose.Name = "Close application";
                Answers.Add(AnswerClose);

                DataBindString AnswerLaunch = new DataBindString();
                AnswerLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerLaunch.Name = "Launch new instance";
                Answers.Add(AnswerLaunch);

                DataBindString AnswerRestartCurrent = new DataBindString();
                if (!string.IsNullOrWhiteSpace(processMulti.Argument))
                {
                    AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerRestartCurrent.Name = "Restart application";
                    AnswerRestartCurrent.NameSub = "(Current argument)";
                    Answers.Add(AnswerRestartCurrent);
                }

                DataBindString AnswerRestartWithout = new DataBindString();
                AnswerRestartWithout.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRestartWithout.Name = "Restart application";
                if (!string.IsNullOrWhiteSpace(dataBindApp.Argument) || dataBindApp.Category == AppCategory.Shortcut || dataBindApp.Category == AppCategory.Emulator || dataBindApp.LaunchFilePicker)
                {
                    AnswerRestartWithout.NameSub = "(Default argument)";
                }
                else
                {
                    AnswerRestartWithout.NameSub = "(Without argument)";
                }
                Answers.Add(AnswerRestartWithout);

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTimeString, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerShow)
                    {
                        await ShowProcessWindow(dataBindApp, processMulti);
                    }
                    else if (messageResult == AnswerClose)
                    {
                        await CloseSingleProcessAuto(processMulti, dataBindApp, false, true);
                    }
                    else if (messageResult == AnswerRestartCurrent)
                    {
                        await RestartProcessAuto(processMulti, dataBindApp, true);
                    }
                    else if (messageResult == AnswerRestartWithout)
                    {
                        await RestartProcessAuto(processMulti, dataBindApp, false);
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