using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkStyles.AVImage;
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
                DataBindApp quickLaunchApp = CombineAppLists(true, true, true, false, false, false, false).FirstOrDefault(x => x.QuickLaunch);

                //Prompt user to quick launch application
                List<DataBindString> Answers = new List<DataBindString>();

                DataBindString AnswerLaunch = new DataBindString();
                AnswerLaunch.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/AppLaunch.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerLaunch.Name = "Launch application";
                Answers.Add(AnswerLaunch);

                DataBindString messageResult = await Popup_Show_MessageBox("Quick launch", "* You can change the quick launch application in the CtrlUI settings.", "Do you want to launch " + quickLaunchApp.Name + "?", Answers);
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
                await Notification_Show_Status("AppLaunch", "Please set a quick launch app");
                Debug.WriteLine("Please set a quick launch app");
            }
        }

        //Launch the set quick launch app
        async Task LaunchQuickLaunchApp()
        {
            try
            {
                //Get the current quick launch application
                DataBindApp quickLaunchApp = CombineAppLists(true, true, true, false, false, false, false).FirstOrDefault(x => x.QuickLaunch);

                //Quick launch application
                if (quickLaunchApp != null)
                {
                    //Check which launch mode needs to be used
                    await CheckApplicationLaunchMode(quickLaunchApp);
                }
                else
                {
                    await Notification_Show_Status("AppLaunch", "Please set a quick launch app");
                    Debug.WriteLine("Please set a quick launch app");
                }
            }
            catch
            {
                await Notification_Show_Status("AppLaunch", "Please set a quick launch app");
                Debug.WriteLine("Please set a quick launch app");
            }
        }
    }
}