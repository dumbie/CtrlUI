﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RightClickLauncher(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                Debug.WriteLine("Right clicked launcher: " + dataBindApp.Name + " from: " + listboxSender.Name);

                List<DataBindString> Answers = new List<DataBindString>();

                DataBindString AnswerShowGameInfo = new DataBindString();
                AnswerShowGameInfo.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Star.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerShowGameInfo.Name = "Show game information";
                Answers.Add(AnswerShowGameInfo);

                DataBindString AnswerHowLongToBeat = new DataBindString();
                AnswerHowLongToBeat.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Timer.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerHowLongToBeat.Name = "How long to beat information";
                Answers.Add(AnswerHowLongToBeat);

                DataBindString AnswerHide = new DataBindString();
                AnswerHide.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Hide.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerHide.Name = "Hide application from list";
                Answers.Add(AnswerHide);

                //Get launch information
                string launchInformation = string.Empty;
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    launchInformation = dataBindApp.AppUserModelId + " (" + dataBindApp.NameExe + ")";
                }
                else
                {
                    launchInformation = dataBindApp.PathExe;
                }

                //Add launch argument
                if (!string.IsNullOrWhiteSpace(dataBindApp.Argument))
                {
                    launchInformation += "\nLaunch argument: " + dataBindApp.Argument;
                }

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", launchInformation, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerHowLongToBeat)
                    {
                        await Popup_Show_HowLongToBeat(dataBindApp.Name);
                    }
                    else if (messageResult == AnswerShowGameInfo)
                    {
                        await Popup_Show_GameInformation(dataBindApp.Name, dataBindApp);
                    }
                    else if (messageResult == AnswerHide)
                    {
                        await HideLauncherApp(listboxSender, listboxSelectedIndex, dataBindApp);
                    }
                }
            }
            catch { }
        }

        //Hide launcher app
        async Task HideLauncherApp(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                await Notification_Send_Status("Hide", "Hiding launcher " + dataBindApp.Name);
                Debug.WriteLine("Hiding launcher by name: " + dataBindApp.Name);

                //Create new profile shared
                ProfileShared profileShared = new ProfileShared();
                profileShared.String1 = dataBindApp.Name;

                //Add shortcut file to the ignore list
                vCtrlIgnoreLauncherName.Add(profileShared);
                JsonSaveObject(vCtrlIgnoreLauncherName, @"Profiles\User\CtrlIgnoreLauncherName.json");

                //Remove application from the list
                await RemoveAppFromList(dataBindApp, false, false, true);

                //Select the previous index
                await ListBoxFocusIndex(listboxSender, false, listboxSelectedIndex, vProcessCurrent.WindowHandleMain);
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Hide", "Failed hiding");
                Debug.WriteLine("Failed hiding launcher: " + ex.Message);
            }
        }
    }
}