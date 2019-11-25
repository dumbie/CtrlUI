using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check if process has multiple processes
        async Task<ProcessMulti> CheckProcessMultiWin32AndWin32Store(string processName, string executableName, ProcessType processType, bool selectProcess)
        {
            try
            {
                List<DataBindString> multiAnswers = new List<DataBindString>();
                Process[] multiVariables = GetProcessesByNameOrTitle(Path.GetFileNameWithoutExtension(executableName), false, true);
                if (multiVariables.Any())
                {
                    if (selectProcess && multiVariables.Count() > 1)
                    {
                        foreach (Process multiProcess in multiVariables)
                        {
                            try
                            {
                                //Get the process title
                                string ProcessTitle = GetWindowTitleFromProcess(multiProcess);
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

                        DataBindString Result = await Popup_Show_MessageBox(processName + " has multiple running instances", "", "Please select the instance that you wish to interact with:", multiAnswers);
                        if (Result != null)
                        {
                            if (Result == Answer2)
                            {
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Type = processType;
                                processMultiNew.Action = "CloseAll";
                                return processMultiNew;
                            }
                            else if (Result == cancelString)
                            {
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Type = processType;
                                processMultiNew.Action = "Cancel";
                                return processMultiNew;
                            }
                            else
                            {
                                ProcessMulti returnProcess = ConvertProcessToProcessMulti(processType, multiVariables[multiAnswers.IndexOf(Result)]);
                                returnProcess.Count = multiVariables.Count();
                                return returnProcess;
                            }
                        }
                        else
                        {
                            ProcessMulti processMultiNew = new ProcessMulti();
                            processMultiNew.Type = processType;
                            processMultiNew.Action = "Cancel";
                            return processMultiNew;
                        }
                    }
                    else
                    {
                        ProcessMulti returnProcess = ConvertProcessToProcessMulti(processType, multiVariables.FirstOrDefault());
                        returnProcess.Count = multiVariables.Count();
                        return returnProcess;
                    }
                }
            }
            catch { }
            return null;
        }

        //Check process status before launching (True = Continue)
        async Task<bool> CheckLaunchProcessWin32andWin32Store(DataBindApp dataBindApp)
        {
            try
            {
                Debug.WriteLine("Checking launch process Win32 or Win32Store: " + dataBindApp.Name + " / " + dataBindApp.ProcessMulti.Identifier + " / " + dataBindApp.ProcessMulti.WindowHandle);

                //Check the multi process results
                if (dataBindApp.ProcessMulti == null) { return true; }
                if (dataBindApp.ProcessMulti.Action == "Cancel") { return false; }
                if (dataBindApp.ProcessMulti.Action == "CloseAll")
                {
                    //Close all processes Win32 or Win32Store
                    await CloseAllProcessesWin32AndWin32StoreByDataBindApp(dataBindApp, true, false);
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

                //Check application runtime
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
                        //Check if process has multiple windows
                        IntPtr processWindowHandle = await CheckProcessWindowsWin32AndWin32Store(dataBindApp);
                        if (processWindowHandle != IntPtr.Zero)
                        {
                            //Minimize the CtrlUI window
                            if (ConfigurationManager.AppSettings["MinimizeAppOnShow"] == "True") { await AppMinimize(true); }

                            //Force focus on the app
                            FocusProcessWindowPrepare(dataBindApp.Name, dataBindApp.ProcessMulti.Identifier, processWindowHandle, 0, false, false, false);

                            //Launch the keyboard controller
                            if (dataBindApp.LaunchKeyboard)
                            {
                                LaunchKeyboardController(true);
                            }
                        }
                        else
                        {
                            Popup_Show_Status("Close", "Application has no window");
                            Debug.WriteLine("Show application has no window.");
                        }
                        return false;
                    }
                    else if (Result == Answer2)
                    {
                        await CloseSingleProcessWin32AndWin32StoreByDataBindApp(dataBindApp, true, false);
                        return false;
                    }
                    else if (Result == Answer3)
                    {
                        //Check the application restart type
                        if (dataBindApp.ProcessMulti.Type == ProcessType.Win32Store)
                        {
                            await RestartPrepareWin32Store(dataBindApp);
                        }
                        else
                        {
                            await RestartPrepareWin32(dataBindApp);
                        }
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

        //Check if process has multiple windows
        async Task<IntPtr> CheckProcessWindowsWin32AndWin32Store(DataBindApp dataBindApp)
        {
            try
            {
                int processThreadCount = dataBindApp.ProcessMulti.Threads.Count;
                if (processThreadCount > 1)
                {
                    Debug.WriteLine("Found window threads: " + processThreadCount);

                    List<DataBindString> multiAnswers = new List<DataBindString>();
                    List<IntPtr> multiVariables = new List<IntPtr>();
                    foreach (ProcessThread threadProcess in dataBindApp.ProcessMulti.Threads)
                    {
                        foreach (IntPtr threadWindowHandle in EnumThreadWindows(threadProcess.Id))
                        {
                            try
                            {
                                //Validate the window handle
                                if (threadWindowHandle == dataBindApp.ProcessMulti.WindowHandle || ValidateWindowHandle(threadWindowHandle))
                                {
                                    //Get window title
                                    string ClassNameString = GetWindowTitleFromWindowHandle(threadWindowHandle);
                                    if (threadWindowHandle == dataBindApp.ProcessMulti.WindowHandle) { ClassNameString += " (Main Window)"; }
                                    if (multiAnswers.Where(x => x.Name.ToLower() == ClassNameString.ToLower()).Any()) { ClassNameString += " (" + multiAnswers.Count + ")"; }

                                    DataBindString Answer1 = new DataBindString();
                                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                                    Answer1.Name = ClassNameString;

                                    //Add window to selection
                                    if (threadWindowHandle == dataBindApp.ProcessMulti.WindowHandle)
                                    {
                                        multiAnswers.Insert(0, Answer1);
                                        multiVariables.Insert(0, threadWindowHandle);
                                    }
                                    else
                                    {
                                        multiAnswers.Add(Answer1);
                                        multiVariables.Add(threadWindowHandle);
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    //Check if there are multiple answers
                    if (multiVariables.Count == 1)
                    {
                        Debug.WriteLine("There is only one visible window, returning the default window.");
                        return multiVariables.FirstOrDefault();
                    }
                    else if (multiVariables.Count == 0)
                    {
                        return IntPtr.Zero;
                    }

                    DataBindString cancelString = new DataBindString();
                    cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                    cancelString.Name = "Cancel";
                    multiAnswers.Add(cancelString);

                    //Ask which window needs to be shown
                    DataBindString Result = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple windows open", "", "Please select the window that you wish to be shown:", multiAnswers);
                    if (Result != null)
                    {
                        if (Result == cancelString)
                        {
                            return IntPtr.Zero;
                        }
                        else
                        {
                            return multiVariables[multiAnswers.IndexOf(Result)];
                        }
                    }
                    else
                    {
                        return IntPtr.Zero;
                    }
                }
                else
                {
                    Debug.WriteLine("Single window thread process.");
                    return dataBindApp.ProcessMulti.WindowHandle;
                }
            }
            catch
            {
                return IntPtr.Zero;
            }
        }
    }
}