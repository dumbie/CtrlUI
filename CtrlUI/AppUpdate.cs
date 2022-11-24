using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using static ArnoldVinkCode.ApiGitHub;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check for available application update
        public async Task<bool> CheckForAppUpdate(bool silentCheck)
        {
            bool updateAvailable = false;
            try
            {
                //Check if already updating
                if (vBusyCheckingForUpdate)
                {
                    Debug.WriteLine("Already checking for application update, cancelling.");
                    return updateAvailable;
                }

                //Update the updating status
                vBusyCheckingForUpdate = true;

                string onlineVersion = await ApiGitHub_GetLatestVersion("dumbie", "CtrlUI");
                string currentVersion = "v" + Assembly.GetEntryAssembly().FullName.Split('=')[1].Split(',')[0];
                if (!string.IsNullOrWhiteSpace(onlineVersion) && onlineVersion != currentVersion)
                {
                    //Update status variable
                    updateAvailable = true;

                    //Insert main menu item
                    MainMenuInsertUpdate();

                    //Notification or interaction
                    if (silentCheck)
                    {
                        await Notification_Send_Status("Refresh", "CtrlUI update available");
                    }
                    else
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Refresh.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Update and restart CtrlUI";
                        Answers.Add(Answer1);

                        DataBindString messageResult = await Popup_Show_MessageBox("A newer version has been found: " + onlineVersion, "", "Do you want to update the application to the newest version now?", Answers);
                        if (messageResult != null && messageResult == Answer1)
                        {
                            await AppUpdateRestart();
                        }
                    }
                }
                else
                {
                    if (!silentCheck)
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Ok";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("No new application update has been found", "", "", Answers);
                    }
                }
            }
            catch
            {
                await AppUpdateFailed(silentCheck);
            }

            vBusyCheckingForUpdate = false;
            return updateAvailable;
        }

        //Application update check failed message
        async Task AppUpdateFailed(bool silentCheck)
        {
            try
            {
                if (!silentCheck)
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Check.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Ok";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Failed to check for application update", "", "Please check your internet connection and try again.", Answers);
                }
            }
            catch { }
        }

        //Launch updater and restart application
        async Task AppUpdateRestart()
        {
            try
            {
                await ProcessLauncherWin32Async("Updater.exe", "", "", false, false);
                await Application_Exit();
            }
            catch { }
        }
    }
}