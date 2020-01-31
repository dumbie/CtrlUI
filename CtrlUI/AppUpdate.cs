using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check for available application update
        public async Task CheckForAppUpdate(bool silentCheck)
        {
            try
            {
                if (!vCheckingForUpdate)
                {
                    vCheckingForUpdate = true;

                    string ResCurrentVersion = await AVDownloader.DownloadStringAsync(5000, "CtrlUI", null, new Uri("http://download.arnoldvink.com/CtrlUI.zip-version.txt" + "?nc=" + Environment.TickCount));
                    if (!string.IsNullOrWhiteSpace(ResCurrentVersion) && ResCurrentVersion != Assembly.GetEntryAssembly().FullName.Split('=')[1].Split(',')[0])
                    {
                        if (silentCheck)
                        {
                            Popup_Show_Status("Refresh", "Update available");
                        }
                        else
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Refresh.png" }, IntPtr.Zero, -1, 0);
                            Answer1.Name = "Update now";
                            Answers.Add(Answer1);

                            DataBindString messageResult = await Popup_Show_MessageBox("A newer version has been found: v" + ResCurrentVersion, "", "Do you want to update the application to the newest version now?", Answers);
                            if (messageResult != null && messageResult == Answer1)
                            {
                                await ProcessLauncherWin32Async("Updater.exe", "", "", false, false);
                                await Application_Exit(true);
                            }
                        }
                    }
                    else
                    {
                        if (!silentCheck)
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                            Answer1.Name = "Alright";
                            Answers.Add(Answer1);

                            await Popup_Show_MessageBox("No new application update has been found", "", "", Answers);
                        }
                    }

                    //Set the last application update check date
                    SettingSave("AppUpdateCheck", DateTime.Now.ToString(vAppCultureInfo));
                    vCheckingForUpdate = false;
                }
            }
            catch
            {
                vCheckingForUpdate = false;
                if (!silentCheck)
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Failed to check for application update", "", "Please check your internet connection and try again.", Answers);
                }
            }
        }
    }
}
