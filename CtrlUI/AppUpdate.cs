using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ApiGitHub;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Clean application update files
        public async Task UpdateCleanup()
        {
            try
            {
                Debug.WriteLine("Cleaning application update.");

                //Close running application updater
                if (Close_ProcessesByName("Updater.exe", true))
                {
                    await Task.Delay(1000);
                }

                //Check if the updater has been updated
                File_Move("Resources/UpdaterReplace.exe", "Updater.exe", true);
                File_Move("Updater/UpdaterReplace.exe", "Updater.exe", true);
            }
            catch { }
        }

        //Launch updater and restart application
        public void UpdateRestart()
        {
            try
            {
                Launch_ShellExecute("Updater.exe", "", "-ProcessLaunch", true);
                Environment.Exit(0);
            }
            catch { }
        }

        //Check for available application update
        public async Task<bool> UpdateCheck(bool onlyNotification)
        {
            try
            {
                Debug.WriteLine("Checking for application update.");

                string onlineVersion = await ApiGitHub_GetLatestVersion("dumbie", "CtrlUI");
                string currentVersion = "v" + AVFunctions.ApplicationVersion();
                if (!string.IsNullOrWhiteSpace(onlineVersion) && onlineVersion != currentVersion)
                {
                    //Insert main menu item
                    MainMenuInsertUpdate();

                    //Notification or interaction
                    if (onlyNotification)
                    {
                        await Notification_Send_Status("Refresh", "CtrlUI update available");
                    }
                    else
                    {
                        List<DataBindString> messageBoxAnswers = new List<DataBindString>();
                        DataBindString AnswerUpdateRestart = new DataBindString();
                        AnswerUpdateRestart.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Refresh.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        AnswerUpdateRestart.Name = "Update and restart CtrlUI";
                        messageBoxAnswers.Add(AnswerUpdateRestart);

                        DataBindString messageResult = await Popup_Show_MessageBox("Newer version has been found: " + onlineVersion, "", "Would you like to update the application to the newest version available?", messageBoxAnswers);
                        if (messageResult != null)
                        {
                            if (messageResult == AnswerUpdateRestart)
                            {
                                UpdateRestart();
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    if (!onlyNotification)
                    {
                        List<DataBindString> messageBoxAnswers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        Answer1.Name = "Ok";
                        messageBoxAnswers.Add(Answer1);

                        await Popup_Show_MessageBox("No new application update has been found", "", "", messageBoxAnswers);
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed checking for application update: " + ex.Message);
                return false;
            }
        }
    }
}