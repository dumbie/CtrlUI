using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check if process has multiple windows
        async Task<IntPtr> CheckProcessWindowsWin32AndWin32Store(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                //Get threads from process
                ProcessThreadCollection processThreads = GetProcessThreads(processMulti);

                //Check threads from process
                int processThreadCount = processThreads.Count;
                if (processThreadCount > 1)
                {
                    Debug.WriteLine("Found window threads: " + processThreadCount);

                    List<DataBindString> multiAnswers = new List<DataBindString>();
                    List<IntPtr> multiVariables = new List<IntPtr>();
                    foreach (ProcessThread threadProcess in processThreads)
                    {
                        foreach (IntPtr threadWindowHandle in EnumThreadWindows(threadProcess.Id))
                        {
                            try
                            {
                                //Validate the window handle
                                if (threadWindowHandle == processMulti.WindowHandle || ValidateWindowHandle(threadWindowHandle))
                                {
                                    //Get the window state
                                    WindowPlacement processWindowState = new WindowPlacement();
                                    GetWindowPlacement(threadWindowHandle, ref processWindowState);

                                    //Get the window title
                                    string windowTitleString = GetWindowTitleFromWindowHandle(threadWindowHandle);
                                    string windowSubString = threadWindowHandle.ToString();

                                    //Check window main
                                    if (threadWindowHandle == processMulti.WindowHandle)
                                    {
                                        windowSubString += " (Main)";
                                    }

                                    //Check window style
                                    WindowStylesEx windowStyle = (WindowStylesEx)GetWindowLongAuto(threadWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE).ToInt64();
                                    if (windowStyle.HasFlag(WindowStylesEx.WS_EX_TOOLWINDOW) || windowStyle.HasFlag(WindowStylesEx.WS_EX_LAYERED))
                                    {
                                        windowSubString += " (Tool)";
                                    }
                                    else
                                    {
                                        windowSubString += " (Window)";
                                    }

                                    //Check window state
                                    if (processWindowState.windowShowCommand == WindowShowCommand.Minimized)
                                    {
                                        windowSubString += " (Minimized)";
                                    }

                                    //Check explorer window
                                    if (dataBindApp.NameExe.ToLower() == "explorer.exe")
                                    {
                                        if (windowTitleString == "Unknown" || windowStyle.HasFlag(WindowStylesEx.WS_EX_TOOLWINDOW) || windowStyle.HasFlag(WindowStylesEx.WS_EX_LAYERED))
                                        {
                                            continue;
                                        }
                                    }

                                    DataBindString Answer1 = new DataBindString();
                                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMiniMaxi.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                                    Answer1.Name = windowTitleString;
                                    Answer1.NameSub = windowSubString;

                                    //Add window to selection
                                    if (threadWindowHandle == processMulti.WindowHandle)
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
                            catch
                            {
                                Debug.WriteLine("Failed checking window handle: " + threadWindowHandle);
                            }
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

                    DataBindString AnswerClose = new DataBindString();
                    AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerClose.Name = "Close application";
                    multiAnswers.Add(AnswerClose);

                    DataBindString AnswerLaunch = new DataBindString();
                    AnswerLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerLaunch.Name = "Launch new instance";
                    multiAnswers.Add(AnswerLaunch);

                    DataBindString AnswerRestartCurrent = new DataBindString();
                    if (!string.IsNullOrWhiteSpace(processMulti.Argument))
                    {
                        AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        AnswerRestartCurrent.Name = "Restart application";
                        AnswerRestartCurrent.NameSub = "(Current argument)";
                        multiAnswers.Add(AnswerRestartCurrent);
                    }

                    DataBindString AnswerRestartWithout = new DataBindString();
                    AnswerRestartWithout.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerRestartWithout.Name = "Restart application";
                    if (!string.IsNullOrWhiteSpace(dataBindApp.Argument) || dataBindApp.Category == AppCategory.Shortcut || dataBindApp.Category == AppCategory.Emulator || dataBindApp.LaunchFilePicker)
                    {
                        AnswerRestartWithout.NameSub = "(Default argument)";
                    }
                    else
                    {
                        AnswerRestartWithout.NameSub = "(Without argument)";
                    }
                    multiAnswers.Add(AnswerRestartWithout);

                    //Ask which window needs to be shown
                    DataBindString messageResult = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple windows open", "", "Please select the window that you wish to be shown:", multiAnswers);
                    if (messageResult != null)
                    {
                        if (messageResult == AnswerLaunch)
                        {
                            return new IntPtr(-50);
                        }
                        else if (messageResult == AnswerRestartCurrent)
                        {
                            return new IntPtr(-75);
                        }
                        else if (messageResult == AnswerRestartWithout)
                        {
                            return new IntPtr(-80);
                        }
                        else if (messageResult == AnswerClose)
                        {
                            return new IntPtr(-100);
                        }
                        else
                        {
                            return multiVariables[multiAnswers.IndexOf(messageResult)];
                        }
                    }
                    else
                    {
                        //Cancel the selection
                        return new IntPtr(-200);
                    }
                }
                else
                {
                    Debug.WriteLine("Single window thread process: " + processMulti.WindowHandle);
                    return processMulti.WindowHandle;
                }
            }
            catch
            {
                return IntPtr.Zero;
            }
        }
    }
}