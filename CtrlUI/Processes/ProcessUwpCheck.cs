using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check if process has multiple processes
        async Task<ProcessMulti> CheckProcessMultiUwp(DataBindApp dataBindApp, bool selectProcess)
        {
            try
            {
                List<DataBindString> multiAnswers = new List<DataBindString>();
                List<ProcessMulti> multiVariables = UwpGetProcessMultiFromAppUserModelId(dataBindApp.PathExe);
                if (multiVariables.Any())
                {
                    if (selectProcess && multiVariables.Count > 1)
                    {
                        foreach (ProcessMulti multiProcess in multiVariables)
                        {
                            try
                            {
                                //Get the process title
                                string ProcessTitle = GetWindowTitleFromWindowHandle(multiProcess.WindowHandle);
                                if (ProcessTitle == "Unknown") { ProcessTitle += " (Hidden)"; }
                                if (multiAnswers.Where(x => x.Name.ToLower() == ProcessTitle.ToLower()).Any()) { ProcessTitle += " (" + multiAnswers.Count + ")"; }

                                DataBindString AnswerApp = new DataBindString();
                                AnswerApp.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                                AnswerApp.Name = ProcessTitle;
                                multiAnswers.Add(AnswerApp);
                            }
                            catch { }
                        }

                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Launch new instance";
                        multiAnswers.Add(Answer1);

                        DataBindString Answer2 = new DataBindString();
                        Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                        Answer2.Name = "Close all the instances";
                        multiAnswers.Add(Answer2);

                        DataBindString cancelString = new DataBindString();
                        cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                        cancelString.Name = "Cancel";
                        multiAnswers.Add(cancelString);

                        DataBindString Result = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple running instances", "", "Please select the instance that you wish to interact with:", multiAnswers);
                        if (Result != null)
                        {
                            if (Result == Answer2)
                            {
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Type = ProcessType.UWP;
                                processMultiNew.Action = "CloseAll";
                                return processMultiNew;
                            }
                            else if (Result == cancelString)
                            {
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Type = ProcessType.UWP;
                                processMultiNew.Action = "Cancel";
                                return processMultiNew;
                            }
                            else
                            {
                                ProcessMulti returnProcess = multiVariables[multiAnswers.IndexOf(Result)];
                                returnProcess.Count = multiVariables.Count();
                                return returnProcess;
                            }
                        }
                        else
                        {
                            ProcessMulti processMultiNew = new ProcessMulti();
                            processMultiNew.Type = ProcessType.UWP;
                            processMultiNew.Action = "Cancel";
                            return processMultiNew;
                        }
                    }
                    else
                    {
                        ProcessMulti returnProcess = multiVariables.FirstOrDefault();
                        returnProcess.Count = multiVariables.Count();
                        return returnProcess;
                    }
                }
            }
            catch { }
            return null;
        }

        //Check process status before launching (True = Continue)
        async Task<bool> CheckLaunchProcessUwp(DataBindApp dataBindApp)
        {
            try
            {
                Debug.WriteLine("Checking launch process UWP: " + dataBindApp.Name + " / " + dataBindApp.ProcessMulti.Identifier + " / " + dataBindApp.ProcessMulti.WindowHandle);

                //Check the multiple check result
                if (dataBindApp.ProcessMulti == null) { return true; }
                if (dataBindApp.ProcessMulti.Action == "Cancel") { return false; }
                if (dataBindApp.ProcessMulti.Action == "CloseAll")
                {
                    //Close all processes UWP
                    await CloseAllProcessesUwpByDataBindApp(dataBindApp, true, false);
                    return false;
                }

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
                string ApplicationRuntime = string.Empty;
                if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    int processRunningTimeInt = ProcessRuntimeMinutes(GetProcessById(dataBindApp.ProcessMulti.Identifier));
                    ApplicationRuntime = ApplicationRuntimeString(processRunningTimeInt, "shortcut process");
                }
                else
                {
                    ApplicationRuntime = ApplicationRuntimeString(dataBindApp.RunningTime, "application");
                }

                //Show the messagebox
                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", ApplicationRuntime, "", Answers);
                if (Result != null)
                {
                    if (Result == Answer1)
                    {
                        //Minimize the CtrlUI window
                        if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                        //Force focus on the app
                        FocusProcessWindowPrepare(dataBindApp.Name, dataBindApp.ProcessMulti.Identifier, dataBindApp.ProcessMulti.WindowHandle, 0, false, false, false);

                        //Launch the keyboard controller
                        if (dataBindApp.LaunchKeyboard)
                        {
                            LaunchKeyboardController(true);
                        }

                        return false;
                    }
                    else if (Result == Answer2)
                    {
                        await CloseSingleProcessUwpByDataBindApp(dataBindApp, true, false);
                        return false;
                    }
                    else if (Result == Answer3)
                    {
                        Popup_Show_Status("Switch", "Restarting " + dataBindApp.Name);
                        Debug.WriteLine("Restarting UWP application: " + dataBindApp.Name + " / " + dataBindApp.ProcessMulti.Identifier + " / " + dataBindApp.ProcessMulti.WindowHandle);

                        await RestartProcessUwp(dataBindApp.Name, dataBindApp.PathExe, dataBindApp.Argument, dataBindApp.ProcessMulti.Identifier, dataBindApp.ProcessMulti.WindowHandle);
                        return false;
                    }
                    else if (Result == Answer4)
                    {
                        Debug.WriteLine("Running new application instance.");
                        return true;
                    }
                    else if (Result == cancelString)
                    {
                        Debug.WriteLine("Cancelling the process action.");
                        return false;
                    }
                }
                else
                {
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