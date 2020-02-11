using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check if process has multiple windows
        async Task<IntPtr> CheckProcessWindowsWin32AndWin32Store(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                int processThreadCount = processMulti.Threads.Count;
                if (processThreadCount > 1)
                {
                    Debug.WriteLine("Found window threads: " + processThreadCount);

                    List<DataBindString> multiAnswers = new List<DataBindString>();
                    List<IntPtr> multiVariables = new List<IntPtr>();
                    foreach (ProcessThread threadProcess in processMulti.Threads)
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
                                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MiniMaxi.png" }, IntPtr.Zero, -1, 0);
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

                    DataBindString Answer2 = new DataBindString();
                    Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1, 0);
                    Answer2.Name = "Close application";
                    multiAnswers.Add(Answer2);

                    DataBindString Answer3 = new DataBindString();
                    Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Switch.png" }, IntPtr.Zero, -1, 0);
                    Answer3.Name = "Restart application";
                    multiAnswers.Add(Answer3);

                    DataBindString Answer4 = new DataBindString();
                    Answer4.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1, 0);
                    Answer4.Name = "Launch new instance";
                    multiAnswers.Add(Answer4);

                    //Ask which window needs to be shown
                    DataBindString messageResult = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple windows open", "", "Please select the window that you wish to be shown:", multiAnswers);
                    if (messageResult != null)
                    {
                        if (messageResult == Answer4)
                        {
                            //Launch new instance
                            return new IntPtr(-50);
                        }
                        else if (messageResult == Answer3)
                        {
                            //Restart the application
                            return new IntPtr(-75);
                        }
                        else if (messageResult == Answer2)
                        {
                            //Close the application
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
                    Debug.WriteLine("Single window thread process.");
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