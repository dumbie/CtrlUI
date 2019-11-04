using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static ArnoldVinkCode.ArnoldVinkProcesses;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle app list mouse/touch tapped
        async void ListBox_Apps_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (vMousePressDownRightClick)
                    {
                        await lb_AppList_RightClick(sender);
                    }
                    else
                    {
                        await lb_AppList_LeftClick(sender);
                    }
                }
            }
            catch { }
        }

        //Handle app list left click
        async Task lb_AppList_LeftClick(object sender)
        {
            try
            {
                ListBox ListboxSender = (sender as ListBox);
                if (ListboxSender.SelectedItems.Count > 0 && ListboxSender.SelectedIndex != -1)
                {
                    DataBindApp SelectedItem = (DataBindApp)ListboxSender.SelectedItem;

                    //Check which launch method needs to be used
                    await LaunchProcessSelector(SelectedItem);
                }
            }
            catch
            {
                Popup_Show_Status("Close", "Failed to launch or show app");
                Debug.WriteLine("Failed launching or showing the application.");
            }
        }

        //Handle app list right click
        async Task lb_AppList_RightClick(object sender)
        {
            try
            {
                ListBox ListboxSender = (sender as ListBox);
                int ListboxSelectedIndex = ListboxSender.SelectedIndex;
                if (ListboxSender.SelectedItems.Count > 0 && ListboxSelectedIndex != -1)
                {
                    DataBindApp SelectedItem = (DataBindApp)ListboxSender.SelectedItem;
                    if (SelectedItem.Category == "Process")
                    {
                        await RightClickProcess(ListboxSender, ListboxSelectedIndex, SelectedItem);
                    }
                    else if (SelectedItem.Category == "Shortcut")
                    {
                        //Get the process running time
                        string ProcessRunningTime = ApplicationRuntimeString(ProcessRuntimeMinutes(GetProcessById(SelectedItem.ProcessId)), "shortcut process");

                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Remove.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Remove shortcut file";
                        Answers.Add(Answer1);

                        DataBindString cancelString = new DataBindString();
                        cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                        cancelString.Name = "Cancel";
                        Answers.Add(cancelString);

                        DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + SelectedItem.Name + "?", ProcessRunningTime, SelectedItem.PathExe, Answers);
                        if (Result != null)
                        {
                            if (Result == Answer1)
                            {
                                Popup_Show_Status("Minus", "Removed shortcut " + SelectedItem.Name);
                                Debug.WriteLine("Removing shortcut: " + SelectedItem.Name + " path: " + SelectedItem.ShortcutPath);

                                //Remove shortcut file if exists
                                if (File.Exists(SelectedItem.ShortcutPath))
                                {
                                    //Delete the shortcut file
                                    File.Delete(SelectedItem.ShortcutPath);

                                    //Remove the shortcut from the lists
                                    List_Shortcuts.Remove(SelectedItem);
                                    List_Search.Remove(SelectedItem);

                                    //Refresh the application lists
                                    ShowHideEmptyList(true, true);
                                    ListsUpdateCount();
                                    UpdateSearchResults();
                                }

                                //Select the previous index
                                await FocusOnListbox(ListboxSender, false, false, ListboxSelectedIndex, true);
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Editing app: " + SelectedItem.Name + " from: " + ListboxSender.Name);

                        //Show the messagebox popup with options
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Edit.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Edit this application";
                        Answers.Add(Answer1);

                        DataBindString Answer2 = new DataBindString();
                        Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Remove.png" }, IntPtr.Zero, -1);
                        Answer2.Name = "Remove application";
                        Answers.Add(Answer2);

                        DataBindString cancelString = new DataBindString();
                        cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                        cancelString.Name = "Cancel";
                        Answers.Add(cancelString);

                        DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + SelectedItem.Name + "?", ApplicationRuntimeString(SelectedItem.RunningTime, "application"), "", Answers);
                        if (Result != null)
                        {
                            if (Result == Answer1)
                            {
                                await Popup_Show_AppEdit(ListboxSender);
                            }
                            else if (Result == Answer2)
                            {
                                await RemoveAppFromList(SelectedItem);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        string ApplicationRuntimeString(int RunningTime, string Category)
        {
            try
            {
                if (RunningTime == -1) { return "This " + Category + " has been running for an unknown duration."; }
                else if (RunningTime == 0) { return "This " + Category + " has not yet run for longer than a minute."; }
                else if (RunningTime < 60) { return "This " + Category + " has been running for a total of " + RunningTime + " minutes."; }
                else if (RunningTime < 120)
                {
                    TimeSpan RunningTimeSpan = TimeSpan.FromMinutes(RunningTime);
                    return "This " + Category + " has been running for a total of 1 hour and " + Convert.ToInt32(RunningTimeSpan.Minutes) + " minutes.";
                }
                else
                {
                    TimeSpan RunningTimeSpan = TimeSpan.FromMinutes(RunningTime);
                    return "This " + Category + " has been running for a total of " + Convert.ToInt32(RunningTimeSpan.TotalHours) + " hours and " + Convert.ToInt32(RunningTimeSpan.Minutes) + " minutes.";
                }
            }
            catch
            {
                return "This " + Category + " has been running for an unknown duration.";
            }
        }

        int ProcessRuntimeMinutes(Process TargetProcess)
        {
            try
            {
                return Convert.ToInt32(DateTime.Now.Subtract(TargetProcess.StartTime).TotalMinutes);
            }
            catch { return -1; }
        }

        async Task RightClickProcess(ListBox ListboxSender, int ListboxSelectedIndex, DataBindApp SelectedItem)
        {
            try
            {
                if (SelectedItem.Type == "UWP" || SelectedItem.Type == "Win32Store")
                {
                    //Get the process running time
                    string ProcessRunningTime = ApplicationRuntimeString(SelectedItem.RunningTime, "process");

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Close the process";
                    Answers.Add(Answer1);

                    DataBindString Answer2 = new DataBindString();
                    Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Switch.png" }, IntPtr.Zero, -1);
                    Answer2.Name = "Restart process";
                    Answers.Add(Answer2);

                    DataBindString cancelString = new DataBindString();
                    cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                    cancelString.Name = "Cancel";
                    Answers.Add(cancelString);

                    DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + SelectedItem.Name + "?", ProcessRunningTime, "", Answers);
                    if (Result != null)
                    {
                        if (Result == Answer1)
                        {
                            Popup_Show_Status("Closing", "Closing " + SelectedItem.Name);
                            Debug.WriteLine("Closing uwp process: " + SelectedItem.Name + " / " + SelectedItem.ProcessId + " / " + SelectedItem.WindowHandle);

                            //Close the process or app
                            bool ClosedProcess = await CloseProcessUwpByWindowHandle(SelectedItem.Name, SelectedItem.ProcessId, SelectedItem.WindowHandle);
                            if (ClosedProcess)
                            {
                                //Remove process from list
                                List_Processes.Remove(SelectedItem);
                                List_Search.Remove(SelectedItem);

                                //Refresh the application lists
                                ShowHideEmptyList(true, true);
                                ListsUpdateCount();
                                UpdateSearchResults();
                            }
                            else
                            {
                                Popup_Show_Status("Closing", "Failed to close the process");
                                Debug.WriteLine("Failed to close the uwp process.");
                            }

                            //Select the previous index
                            await FocusOnListbox(ListboxSender, false, false, ListboxSelectedIndex, true);
                        }
                        else if (Result == Answer2)
                        {
                            if (!string.IsNullOrWhiteSpace(SelectedItem.PathExe))
                            {
                                Popup_Show_Status("Switch", "Restarting " + SelectedItem.Name);
                                Debug.WriteLine("Restarting uwp process: " + SelectedItem.Name + " / Arg: " + SelectedItem.Argument);

                                await RestartProcessUwp(SelectedItem.Name, SelectedItem.PathExe, SelectedItem.Argument, SelectedItem.ProcessId, SelectedItem.WindowHandle);

                                //Remove process from list
                                List_Processes.Remove(SelectedItem);
                                List_Search.Remove(SelectedItem);

                                //Refresh the application lists
                                ShowHideEmptyList(true, true);
                                ListsUpdateCount();
                                UpdateSearchResults();

                                //Select the previous index
                                await FocusOnListbox(ListboxSender, false, false, ListboxSelectedIndex, true);
                            }
                            else
                            {
                                Popup_Show_Status("Close", "Failed restarting " + SelectedItem.Name);
                                Debug.WriteLine("Failed to restart process: " + SelectedItem.Name);
                            }
                        }
                    }
                }
                else
                {
                    //Get the process running time
                    string ProcessRunningTime = ApplicationRuntimeString(SelectedItem.RunningTime, "process");

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Close the process";
                    Answers.Add(Answer1);

                    DataBindString Answer2 = new DataBindString();
                    Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Switch.png" }, IntPtr.Zero, -1);
                    Answer2.Name = "Restart process";
                    Answers.Add(Answer2);

                    DataBindString cancelString = new DataBindString();
                    cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                    cancelString.Name = "Cancel";
                    Answers.Add(cancelString);

                    DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + SelectedItem.Name + "?", ProcessRunningTime, "", Answers);
                    if (Result != null)
                    {
                        if (Result == Answer1)
                        {
                            if (SelectedItem.ProcessId > 0)
                            {
                                Popup_Show_Status("Closing", "Closing " + SelectedItem.Name);
                                Debug.WriteLine("Closing process: " + SelectedItem.Name + " / " + SelectedItem.ProcessId + " / " + SelectedItem.WindowHandle);

                                //Close the process or app
                                bool ClosedProcess = CloseProcessById(SelectedItem.ProcessId);
                                if (ClosedProcess)
                                {
                                    //Remove process from list
                                    List_Processes.Remove(SelectedItem);
                                    List_Search.Remove(SelectedItem);

                                    //Refresh the application lists
                                    ShowHideEmptyList(true, true);
                                    ListsUpdateCount();
                                    UpdateSearchResults();
                                }
                                else
                                {
                                    Popup_Show_Status("Closing", "Failed to close the process");
                                    Debug.WriteLine("Failed to close the process.");
                                }
                            }
                            else
                            {
                                Popup_Show_Status("Closing", "Process to close is not visible");
                                Debug.WriteLine("Process to close is not visible");
                            }

                            //Select the previous index
                            await FocusOnListbox(ListboxSender, false, false, ListboxSelectedIndex, true);
                        }
                        else if (Result == Answer2)
                        {
                            if (!string.IsNullOrWhiteSpace(SelectedItem.PathExe))
                            {
                                Popup_Show_Status("Switch", "Restarting " + SelectedItem.Name);
                                Debug.WriteLine("Restarting process: " + SelectedItem.Name + " / Arg: " + SelectedItem.Argument);

                                await RestartProcessWin32(SelectedItem.ProcessId, SelectedItem.PathExe, SelectedItem.PathLaunch, SelectedItem.Argument);

                                //Remove process from list
                                List_Processes.Remove(SelectedItem);
                                List_Search.Remove(SelectedItem);

                                //Refresh the application lists
                                ShowHideEmptyList(true, true);
                                ListsUpdateCount();
                                UpdateSearchResults();

                                //Select the previous index
                                await FocusOnListbox(ListboxSender, false, false, ListboxSelectedIndex, true);
                            }
                            else
                            {
                                Popup_Show_Status("Close", "Failed restarting " + SelectedItem.Name);
                                Debug.WriteLine("Failed to restart process: " + SelectedItem.Name);
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}