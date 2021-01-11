using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the quick action prompt
        async Task QuickActionPrompt()
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerQuickLaunch = new DataBindString();

                //Get the current quick launch application
                try
                {
                    DataBindApp QuickLaunchApp = CombineAppLists(false, false, false).Where(x => x.QuickLaunch).FirstOrDefault();
                    if (QuickLaunchApp != null)
                    {
                        AnswerQuickLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        AnswerQuickLaunch.Name = "Quick launch " + QuickLaunchApp.Name;
                        Answers.Add(AnswerQuickLaunch);
                    }
                }
                catch { }

                DataBindString AnswerSortApps = new DataBindString();
                AnswerSortApps.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Sorting.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerSortApps.Name = "Sort applications by name or number and date";
                Answers.Add(AnswerSortApps);

                DataBindString AnswerLaunchExe = new DataBindString();
                AnswerLaunchExe.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRunExe.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerLaunchExe.Name = "Launch an executable file from disk";
                Answers.Add(AnswerLaunchExe);

                DataBindString AnswerLaunchUwp = new DataBindString();
                AnswerLaunchUwp.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRunStore.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerLaunchUwp.Name = "Launch a Windows store application";
                Answers.Add(AnswerLaunchUwp);

                DataBindString AnswerFileManager = new DataBindString();
                AnswerFileManager.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerFileManager.Name = "Show file browser and manager";
                Answers.Add(AnswerFileManager);

                DataBindString AnswerStartMenu = new DataBindString();
                AnswerStartMenu.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Windows.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerStartMenu.Name = "Show the Windows start menu";
                Answers.Add(AnswerStartMenu);

                DataBindString AnswerControlMedia = new DataBindString();
                AnswerControlMedia.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/MediaPlayPause.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerControlMedia.Name = "Control playing media and volume";
                Answers.Add(AnswerControlMedia);

                DataBindString AnswerFpsOverlayer = new DataBindString();
                AnswerFpsOverlayer.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Fps.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerFpsOverlayer.Name = "Show or hide the fps overlayer";
                Answers.Add(AnswerFpsOverlayer);

                DataBindString messageResult = await Popup_Show_MessageBox("Quick action", "* You can change the quick launch application in the CtrlUI settings.", "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerQuickLaunch)
                    {
                        await LaunchQuickLaunchApp();
                    }
                    else if (messageResult == AnswerSortApps)
                    {
                        await SortAppListsSwitch(false);
                    }
                    else if (messageResult == AnswerLaunchExe)
                    {
                        await RunExecutableFile();
                    }
                    else if (messageResult == AnswerLaunchUwp)
                    {
                        await RunStoreApplication();
                    }
                    else if (messageResult == AnswerFileManager)
                    {
                        await ShowFileManager();
                    }
                    else if (messageResult == AnswerStartMenu)
                    {
                        await ShowWindowStartMenu();
                    }
                    else if (messageResult == AnswerControlMedia)
                    {
                        await MediaControllerHideShow(true);
                    }
                    else if (messageResult == AnswerFpsOverlayer)
                    {
                        await LaunchCloseFpsOverlayer();
                    }
                }
            }
            catch { }
        }

        ////Launch windows File Explorer
        //async Task LaunchWindowsFileExplorer()
        //{
        //    try
        //    {
        //        //await ProcessLauncherWin32Prepare(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\explorer.exe", "", "", true, true, false);
        //        //await ProcessLauncherUwpPrepare("File Explorer", "c5e2524a-ea46-4f67-841f-6a9465d9d515_cw5n1h2txyewy!App", string.Empty, false, false);
        //    }
        //    catch { }
        //}

        //Launch the set quick launch app
        async Task LaunchQuickLaunchApp()
        {
            try
            {
                DataBindApp quickLaunchApp = CombineAppLists(false, false, false).Where(x => x.QuickLaunch).FirstOrDefault();
                if (quickLaunchApp != null)
                {
                    //Check which launch method needs to be used
                    await LaunchProcessSelector(quickLaunchApp);
                }
                else
                {
                    await Notification_Send_Status("AppLaunch", "Please set a quick launch app");
                    Debug.WriteLine("Please set a quick launch app");
                }
            }
            catch
            {
                await Notification_Send_Status("AppLaunch", "Please set a quick launch app");
                Debug.WriteLine("Please set a quick launch app");
            }
        }
    }
}