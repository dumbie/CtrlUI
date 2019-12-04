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
                                    if (threadWindowHandle == processMulti.WindowHandle)
                                    {
                                        windowSubString = "Main Window";
                                    }

                                    //Check the window state
                                    if (processWindowState.windowShowCommand == WindowShowCommand.Minimized)
                                    {
                                        windowSubString += " (Minimized)";
                                    }

                                    DataBindString Answer1 = new DataBindString();
                                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
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

                    DataBindString Answer2 = new DataBindString();
                    Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                    Answer2.Name = "Close all the windows";
                    multiAnswers.Add(Answer2);

                    DataBindString cancelString = new DataBindString();
                    cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                    cancelString.Name = "Cancel";
                    multiAnswers.Add(cancelString);

                    //Ask which window needs to be shown
                    DataBindString Result = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple windows open", "", "Please select the window that you wish to be shown:", multiAnswers);
                    if (Result != null)
                    {
                        if (Result == Answer2)
                        {
                            return new IntPtr(-100); //CloseAll
                        }
                        else if (Result == cancelString)
                        {
                            return new IntPtr(-200); //Cancel
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