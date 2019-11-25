using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

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
                    else if (vMousePressDownLeftClick)
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
                ListBox ListboxSender = (ListBox)sender;
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
                ListBox listboxSender = (ListBox)sender;
                int listboxSelectedIndex = listboxSender.SelectedIndex;
                if (listboxSender.SelectedItems.Count > 0 && listboxSelectedIndex != -1)
                {
                    DataBindApp selectedItem = (DataBindApp)listboxSender.SelectedItem;
                    if (selectedItem.Category == AppCategory.Process)
                    {
                        await RightClickProcess(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                    else if (selectedItem.Category == AppCategory.Shortcut)
                    {
                        await RightClickShortcut(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                    else
                    {
                        await RightClickList(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                }
            }
            catch { }
        }

        async Task RightClickShortcut(ListBox ListboxSender, int ListboxSelectedIndex, DataBindApp SelectedItem)
        {
            try
            {
                //Get the process running time
                int processRunningTimeInt = ProcessRuntimeMinutes(GetProcessById(SelectedItem.ProcessMulti.Identifier));
                string processRunningTimeString = ApplicationRuntimeString(processRunningTimeInt, "shortcut process");

                List<DataBindString> Answers = new List<DataBindString>();

                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Remove.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Remove shortcut file";
                Answers.Add(Answer1);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Hide.png" }, IntPtr.Zero, -1);
                Answer2.Name = "Hide shortcut file";
                Answers.Add(Answer2);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + SelectedItem.Name + "?", processRunningTimeString, SelectedItem.PathExe, Answers);
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

                            //Remove application from the list
                            await RemoveAppFromList(SelectedItem, false, false, true);
                        }

                        //Select the previous index
                        await FocusOnListbox(ListboxSender, false, false, ListboxSelectedIndex);
                    }
                    else if (Result == Answer2)
                    {
                        Popup_Show_Status("Hide", "Hiding shortcut " + SelectedItem.Name);
                        Debug.WriteLine("Hiding shortcut: " + SelectedItem.Name + " path: " + SelectedItem.ShortcutPath);

                        //Add shortcut file to the ignore list
                        vAppsBlacklistShortcut.Add(SelectedItem.Name);
                        JsonSaveAppsBlacklistShortcut();

                        //Remove application from the list
                        await RemoveAppFromList(SelectedItem, false, false, true);

                        //Select the previous index
                        await FocusOnListbox(ListboxSender, false, false, ListboxSelectedIndex);
                    }
                }
            }
            catch { }
        }

        async Task RightClickList(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                Debug.WriteLine("Editing app: " + dataBindApp.Name + " from: " + listboxSender.Name);

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

                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", ApplicationRuntimeString(dataBindApp.RunningTime, "application"), "", Answers);
                if (Result != null)
                {
                    if (Result == Answer1)
                    {
                        await Popup_Show_AppEdit(listboxSender);
                    }
                    else if (Result == Answer2)
                    {
                        //Remove application from the list
                        await RemoveAppFromList(dataBindApp, true, true, false);

                        //Select the previous index
                        await FocusOnListbox(listboxSender, false, false, listboxSelectedIndex);
                    }
                }
            }
            catch { }
        }

        async Task RightClickProcess(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                //Get the process running time
                string ProcessRunningTime = ApplicationRuntimeString(dataBindApp.RunningTime, "process");

                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer0 = new DataBindString();
                Answer0.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Fullscreen.png" }, IntPtr.Zero, -1);
                Answer0.Name = "Show the process";
                Answers.Add(Answer0);

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

                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", ProcessRunningTime, "", Answers);
                if (Result != null)
                {
                    if (Result == Answer0)
                    {
                        await ShowApplicationByDataBindApp(dataBindApp);
                    }
                    else if (Result == Answer1)
                    {
                        if (dataBindApp.ProcessMulti.Type == ProcessType.UWP)
                        {
                            await CloseSingleProcessUwpByDataBindApp(dataBindApp, false, true);
                        }
                        else
                        {
                            await CloseSingleProcessWin32AndWin32StoreByDataBindApp(dataBindApp, false, true);
                        }
                    }
                    else if (Result == Answer2)
                    {
                        //Restart the process
                        if (dataBindApp.ProcessMulti.Type == ProcessType.UWP)
                        {
                            await RestartPrepareUwp(dataBindApp);
                        }
                        else if (dataBindApp.ProcessMulti.Type == ProcessType.Win32Store)
                        {
                            await RestartPrepareWin32Store(dataBindApp);
                        }
                        else
                        {
                            await RestartPrepareWin32(dataBindApp);
                        }

                        //Refresh the application lists
                        await RefreshApplicationLists(false, false, false, false, false);

                        //Select the previous index
                        await FocusOnListbox(listboxSender, false, false, listboxSelectedIndex);
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

        int ProcessRuntimeMinutes(Process targetProcess)
        {
            try
            {
                return Convert.ToInt32(DateTime.Now.Subtract(targetProcess.StartTime).TotalMinutes);
            }
            catch
            {
                return -1;
            }
        }
    }
}