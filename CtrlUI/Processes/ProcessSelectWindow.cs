using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Select window if process has multiple windows
        async Task<ProcessWindowAction> SelectProcessWindow(DataBindApp dataBindApp, ProcessMulti processMulti, bool addHideAll)
        {
            try
            {
                //Get process windows
                List<IntPtr> processWindows = processMulti.WindowHandles;
                int processWindowCount = processWindows.Count;
                if (processWindowCount > 1)
                {
                    Debug.WriteLine("Found process windows: " + processWindowCount);
                    List<DataBindString> Answers = new List<DataBindString>();
                    List<IntPtr> validWindowHandles = new List<IntPtr>();
                    foreach (IntPtr windowHandle in processWindows)
                    {
                        try
                        {
                            //Validate the window handle
                            if (!Check_WindowHandleValid(windowHandle, false))
                            {
                                //Debug.WriteLine("Window handle is not valid.");
                                continue;
                            }

                            //Get the window title
                            string windowTitleString = Detail_WindowTitleByWindowHandle(windowHandle);
                            string windowSubString = windowHandle.ToString();

                            //Check window main
                            if (windowHandle == processMulti.WindowHandleMain)
                            {
                                windowSubString += " (Main)";
                            }

                            //Check window style
                            WindowStylesEx windowStyle = (WindowStylesEx)GetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_EXSTYLE).ToInt64();
                            if (windowStyle.HasFlag(WindowStylesEx.WS_EX_TOOLWINDOW))
                            {
                                windowSubString += " (Tool)";
                            }
                            else if (windowStyle.HasFlag(WindowStylesEx.WS_EX_APPWINDOW))
                            {
                                windowSubString += " (App)";
                            }
                            else if (windowStyle.HasFlag(WindowStylesEx.WS_EX_LAYERED))
                            {
                                windowSubString += " (Layer)";
                            }
                            else
                            {
                                windowSubString += " (Window)";
                            }

                            //Check window placement
                            GetWindowPlacement(windowHandle, out WindowPlacement windowPlacement);
                            if (windowPlacement.windowShowCommand == WindowShowCommand.Minimized)
                            {
                                windowSubString += " (Minimized)";
                            }

                            //Add window to selection
                            DataBindString AnswerWindow = new DataBindString();
                            AnswerWindow.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMiniMaxi.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            AnswerWindow.Name = windowTitleString;
                            AnswerWindow.NameSub = windowSubString;
                            AnswerWindow.Data1 = windowHandle;
                            if (windowHandle == processMulti.WindowHandleMain)
                            {
                                Answers.Insert(0, AnswerWindow);
                                validWindowHandles.Insert(0, windowHandle);
                            }
                            else
                            {
                                Answers.Add(AnswerWindow);
                                validWindowHandles.Add(windowHandle);
                            }
                        }
                        catch
                        {
                            Debug.WriteLine("Failed checking window handle: " + windowHandle);
                        }
                    }

                    //Check if there are multiple answers
                    if (validWindowHandles.Count == 1)
                    {
                        Debug.WriteLine("There is only one visible window, returning the window.");
                        return new ProcessWindowAction() { Action = ProcessWindowActions.Single, WindowHandle = validWindowHandles.FirstOrDefault() };
                    }
                    else if (validWindowHandles.Count == 0)
                    {
                        return new ProcessWindowAction() { Action = ProcessWindowActions.NoAction };
                    }

                    //Add hide all windows
                    DataBindString AnswerHideAll = new DataBindString();
                    if (addHideAll)
                    {
                        AnswerHideAll.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMinimize.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        AnswerHideAll.Name = "Hide all the windows";
                        Answers.Add(AnswerHideAll);
                    }

                    //Ask which window needs to be used
                    DataBindString messageResult = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple windows open", string.Empty, "Please select which window you want to interact with:", Answers);
                    if (messageResult != null)
                    {
                        if (messageResult == AnswerHideAll)
                        {
                            //Hide all windows
                            return new ProcessWindowAction() { Action = ProcessWindowActions.Multiple, WindowHandles = validWindowHandles };
                        }
                        else if (messageResult.Data1 != null)
                        {
                            //Return selected window
                            return new ProcessWindowAction() { Action = ProcessWindowActions.Single, WindowHandle = (IntPtr)messageResult.Data1 };
                        }
                    }

                    //Return cancel selection
                    return new ProcessWindowAction() { Action = ProcessWindowActions.Cancel };
                }
                else
                {
                    Debug.WriteLine("Single window process, returning main window.");
                    return new ProcessWindowAction() { Action = ProcessWindowActions.Single, WindowHandle = processMulti.WindowHandleMain };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to select process window: " + ex.Message);
                return new ProcessWindowAction() { Action = ProcessWindowActions.Cancel };
            }
        }
    }
}