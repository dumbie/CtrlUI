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

namespace CtrlUI
{
    partial class WindowMain
    {
        //Select window if process has multiple windows
        async Task<IntPtr> SelectProcessWindow(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                //Get process windows
                List<IntPtr> processWindows = processMulti.WindowHandles;
                int processWindowCount = processWindows.Count;
                if (processWindowCount > 1)
                {
                    Debug.WriteLine("Found process windows: " + processWindowCount);
                    List<DataBindString> multiAnswers = new List<DataBindString>();
                    List<IntPtr> multiVariables = new List<IntPtr>();
                    foreach (IntPtr windowHandle in processWindows)
                    {
                        try
                        {
                            //Check if window handle is already added
                            string windowHandleString = windowHandle.ToString();
                            if (multiAnswers.Any(x => x.Data1.ToString() == windowHandleString))
                            {
                                //Debug.WriteLine("Duplicate window handle detected, skipping.");
                                continue;
                            }

                            //Validate the window handle
                            if (windowHandle == processMulti.WindowHandleMain || Check_ValidWindowHandle(windowHandle))
                            {
                                //Get the window state
                                WindowPlacement processWindowState = new WindowPlacement();
                                GetWindowPlacement(windowHandle, ref processWindowState);

                                //Get the window title
                                string windowTitleString = Detail_WindowTitleByWindowHandle(windowHandle);
                                string windowSubString = windowHandleString;

                                //Check window main
                                if (windowHandle == processMulti.WindowHandleMain)
                                {
                                    windowSubString += " (Main)";
                                }

                                //Check window style
                                WindowStylesEx windowStyle = (WindowStylesEx)GetWindowLongAuto(windowHandle, (int)WindowLongFlags.GWL_EXSTYLE).ToInt64();
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

                                //Add window to selection
                                DataBindString Answer1 = new DataBindString();
                                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMiniMaxi.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                                Answer1.Name = windowTitleString;
                                Answer1.NameSub = windowSubString;
                                Answer1.Data1 = windowHandleString;
                                if (windowHandle == processMulti.WindowHandleMain)
                                {
                                    multiAnswers.Insert(0, Answer1);
                                    multiVariables.Insert(0, windowHandle);
                                }
                                else
                                {
                                    multiAnswers.Add(Answer1);
                                    multiVariables.Add(windowHandle);
                                }
                            }
                        }
                        catch
                        {
                            Debug.WriteLine("Failed checking window handle: " + windowHandle);
                        }
                    }

                    //Check if there are multiple answers
                    if (multiVariables.Count == 1)
                    {
                        Debug.WriteLine("There is only one visible window, returning the window.");
                        return multiVariables.FirstOrDefault();
                    }
                    else if (multiVariables.Count == 0)
                    {
                        return IntPtr.Zero;
                    }

                    //Ask which window needs to be used
                    DataBindString messageResult = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple windows open", string.Empty, "Please select which window you want to be used:", multiAnswers);
                    if (messageResult != null)
                    {
                        //Return selected window
                        return multiVariables[multiAnswers.IndexOf(messageResult)];
                    }
                    else
                    {
                        //Cancel the selection
                        return new IntPtr(-200);
                    }
                }
                else
                {
                    Debug.WriteLine("Single window process, returning main window.");
                    return processMulti.WindowHandleMain;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to select process window: " + ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}