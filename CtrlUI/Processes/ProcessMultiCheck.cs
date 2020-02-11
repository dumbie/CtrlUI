using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check process status before launching (True = Continue)
        async Task<bool> CheckLaunchProcessStatus(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                Debug.WriteLine("Checking launch process: " + dataBindApp.Name);

                //Focus or Close when process is already running
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MiniMaxi.png" }, IntPtr.Zero, -1, 0);
                Answer1.Name = "Show application";
                Answers.Add(Answer1);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1, 0);
                Answer2.Name = "Close application";
                Answers.Add(Answer2);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Switch.png" }, IntPtr.Zero, -1, 0);
                Answer3.Name = "Restart application";
                Answers.Add(Answer3);

                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1, 0);
                Answer4.Name = "Launch new instance";
                Answers.Add(Answer4);

                //Get the process running time
                string applicationRuntime = string.Empty;
                if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    applicationRuntime = ApplicationRuntimeString(dataBindApp.RunningTime, "shortcut process");
                }
                else
                {
                    applicationRuntime = ApplicationRuntimeString(dataBindApp.RunningTime, "application");
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", applicationRuntime, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == Answer1)
                    {
                        Debug.WriteLine("Showing the application.");
                        await ShowProcessWindow(dataBindApp, processMulti);
                        return false;
                    }
                    else if (messageResult == Answer2)
                    {
                        Debug.WriteLine("Closing the application.");
                        await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                        return false;
                    }
                    else if (messageResult == Answer3)
                    {
                        Debug.WriteLine("Restarting the application.");
                        return await RestartPrepareAuto(processMulti, dataBindApp);
                    }
                    else if (messageResult == Answer4)
                    {
                        Debug.WriteLine("Running new application instance.");
                        return await LaunchProcessDatabindAuto(dataBindApp);
                    }
                }
                else
                {
                    Debug.WriteLine("Cancelling the process action.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Close", "Failed showing or closing application");
                Debug.WriteLine("Failed closing or showing the application: " + ex.Message);
            }
            return true;
        }
    }
}