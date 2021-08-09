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
        //Show the quick launch prompt
        async Task QuickLaunchPrompt()
        {
            try
            {
                //Get the current quick launch application
                DataBindApp quickLaunchApp = CombineAppLists(false, false, false).Where(x => x.QuickLaunch).FirstOrDefault();

                //Prompt user to quick launch application
                List<DataBindString> Answers = new List<DataBindString>();

                DataBindString AnswerLaunch = new DataBindString();
                AnswerLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerLaunch.Name = "Launch application";
                Answers.Add(AnswerLaunch);

                DataBindString messageResult = await Popup_Show_MessageBox("Quick launch", "* You can change the quick launch application in the CtrlUI settings.", "Do you want to quick launch " + quickLaunchApp.Name + "?", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerLaunch)
                    {
                        await LaunchQuickLaunchApp();
                    }
                }
            }
            catch 
            {
                await Notification_Send_Status("AppLaunch", "Please set a quick launch app");
                Debug.WriteLine("Please set a quick launch app");
            }
        }

        //Launch the set quick launch app
        async Task LaunchQuickLaunchApp()
        {
            try
            {
                //Get the current quick launch application
                DataBindApp quickLaunchApp = CombineAppLists(false, false, false).Where(x => x.QuickLaunch).FirstOrDefault();

                //Quick launch application
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