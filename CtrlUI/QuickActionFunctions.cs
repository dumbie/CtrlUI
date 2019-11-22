using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static CtrlUI.ImageFunctions;
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
                DataBindString Answer1 = new DataBindString();

                //Get the current quick launch application
                try
                {
                    DataBindApp QuickLaunchApp = CombineAppLists(false, false).Where(x => x.QuickLaunch).FirstOrDefault();
                    if (QuickLaunchApp != null)
                    {
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Quick launch " + QuickLaunchApp.Name;
                        Answers.Add(Answer1);
                    }
                }
                catch { }

                DataBindString Answer6 = new DataBindString();
                Answer6.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Sorting.png" }, IntPtr.Zero, -1);
                Answer6.Name = "Sort applications by name or number and date";
                Answers.Add(Answer6);

                DataBindString Answer5 = new DataBindString();
                Answer5.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Run.png" }, IntPtr.Zero, -1);
                Answer5.Name = "Launch an executable file from disk";
                Answers.Add(Answer5);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Media.png" }, IntPtr.Zero, -1);
                Answer2.Name = "Control playing media and volume";
                Answers.Add(Answer2);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Windows.png" }, IntPtr.Zero, -1);
                Answer3.Name = "Show the Windows start menu";
                Answers.Add(Answer3);

                //Improve: check if the xbox app is installed
                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "Xbox" }, IntPtr.Zero, -1);
                Answer4.Name = "Open Xbox Companion app";
                Answers.Add(Answer4);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                DataBindString Result = await Popup_Show_MessageBox("Quick action", "* You can change the quick launch application in the CtrlUI settings.", "", Answers);

                if (Result == Answer1)
                {
                    await LaunchQuickLaunchApp();
                }
                else if (Result == Answer6)
                {
                    SortAppLists(false, false);
                }
                else if (Result == Answer5)
                {
                    await RunExecutableFile();
                }
                else if (Result == Answer3)
                {
                    ShowWindowStartMenu();
                }
                else if (Result == Answer2)
                {
                    await Popup_Show(grid_Popup_Media, grid_Popup_Media_PlayPause, true);
                }
                else if (Result == Answer4)
                {
                    await LaunchXboxCompanion();
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

        //Launch windows Xbox Companion
        async Task LaunchXboxCompanion()
        {
            try
            {
                await LaunchProcessManuallyUwpAndWin32Store("Xbox Companion", "Microsoft.XboxApp_8wekyb3d8bbwe!Microsoft.XboxApp", string.Empty, false, false);
            }
            catch { }
        }

        //Launch the set quick launch app
        async Task LaunchQuickLaunchApp()
        {
            try
            {
                DataBindApp QuickLaunchApp = CombineAppLists(false, false).Where(x => x.QuickLaunch).FirstOrDefault();
                if (QuickLaunchApp != null)
                {
                    //Check which launch method needs to be used
                    await LaunchProcessSelector(QuickLaunchApp);
                }
                else
                {
                    Popup_Show_Status("App", "Please set a quick launch app");
                    Debug.WriteLine("Please set a quick launch app");
                }
            }
            catch
            {
                Popup_Show_Status("App", "Please set a quick launch app");
                Debug.WriteLine("Please set a quick launch app");
            }
        }
    }
}