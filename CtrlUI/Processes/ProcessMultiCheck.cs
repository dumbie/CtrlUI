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
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MiniMaxi.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Show application";
                Answers.Add(Answer1);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                Answer2.Name = "Close application";
                Answers.Add(Answer2);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Switch.png" }, IntPtr.Zero, -1);
                Answer3.Name = "Restart application";
                Answers.Add(Answer3);

                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                Answer4.Name = "Launch new instance";
                Answers.Add(Answer4);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

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
                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", applicationRuntime, "", Answers);
                if (Result != null)
                {
                    if (Result == Answer1)
                    {
                        Debug.WriteLine("Showing the application.");
                        await ShowProcessWindow(dataBindApp, processMulti);
                        return false;
                    }
                    else if (Result == Answer2)
                    {
                        Debug.WriteLine("Closing the application.");
                        if (processMulti.Type == ProcessType.UWP)
                        {
                            await CloseSingleProcessUwp(dataBindApp, processMulti, true, false);
                        }
                        else
                        {
                            await CloseSingleProcessWin32AndWin32Store(dataBindApp, processMulti, true, false);
                        }
                        return false;
                    }
                    else if (Result == Answer3)
                    {
                        Debug.WriteLine("Restarting the application.");
                        if (processMulti.Type == ProcessType.UWP)
                        {
                            return await RestartPrepareUwp(dataBindApp, processMulti);
                        }
                        else if (processMulti.Type == ProcessType.Win32Store)
                        {
                            return await RestartPrepareWin32Store(dataBindApp, processMulti);
                        }
                        else
                        {
                            return await RestartPrepareWin32(dataBindApp, processMulti);
                        }
                    }
                    else if (Result == Answer4)
                    {
                        Debug.WriteLine("Running new application instance.");
                        if (processMulti.Type == ProcessType.UWP || processMulti.Type == ProcessType.Win32Store)
                        {
                            return await LaunchProcessDatabindUwpAndWin32Store(dataBindApp);
                        }
                        else
                        {
                            return await LaunchProcessDatabindWin32(dataBindApp);
                        }
                    }
                    else if (Result == cancelString)
                    {
                        Debug.WriteLine("Cancelling the process action.");
                        return false;
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