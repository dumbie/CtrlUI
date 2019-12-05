using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.ProcessClasses;
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
                    //Check which launch method needs to be used
                    DataBindApp SelectedItem = (DataBindApp)ListboxSender.SelectedItem;
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

        async Task RightClickShortcut(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                //Get the process running time
                string processRunningTimeString = ApplicationRuntimeString(dataBindApp.RunningTime, "shortcut process");

                List<DataBindString> Answers = new List<DataBindString>();

                DataBindString AnswerRemove = new DataBindString();
                AnswerRemove.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Remove.png" }, IntPtr.Zero, -1);
                AnswerRemove.Name = "Remove shortcut file";
                Answers.Add(AnswerRemove);

                DataBindString AnswerRename = new DataBindString();
                AnswerRename.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Rename.png" }, IntPtr.Zero, -1);
                AnswerRename.Name = "Rename shortcut file";
                Answers.Add(AnswerRename);

                DataBindString AnswerHide = new DataBindString();
                AnswerHide.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Hide.png" }, IntPtr.Zero, -1);
                AnswerHide.Name = "Hide shortcut file";
                Answers.Add(AnswerHide);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTimeString, dataBindApp.PathExe, Answers);
                if (Result != null)
                {
                    if (Result == AnswerRemove)
                    {
                        Popup_Show_Status("Minus", "Removed shortcut " + dataBindApp.Name);
                        Debug.WriteLine("Removing shortcut: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                        //Remove shortcut file if exists
                        if (File.Exists(dataBindApp.ShortcutPath))
                        {
                            //Delete the shortcut file
                            File.Delete(dataBindApp.ShortcutPath);

                            //Remove application from the list
                            await RemoveAppFromList(dataBindApp, false, false, true);
                        }

                        //Select the previous index
                        await FocusOnListbox(listboxSender, false, false, listboxSelectedIndex);
                    }
                    else if (Result == AnswerRename)
                    {
                        await RenameShortcutFile(dataBindApp);
                    }
                    else if (Result == AnswerHide)
                    {
                        Popup_Show_Status("Hide", "Hiding shortcut " + dataBindApp.Name);
                        Debug.WriteLine("Hiding shortcut: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                        //Add shortcut file to the ignore list
                        vAppsBlacklistShortcut.Add(dataBindApp.Name);
                        JsonSaveAppsBlacklistShortcut();

                        //Remove application from the list
                        await RemoveAppFromList(dataBindApp, false, false, true);

                        //Select the previous index
                        await FocusOnListbox(listboxSender, false, false, listboxSelectedIndex);
                    }
                }
            }
            catch { }
        }

        //Rename the shortcut file
        async Task RenameShortcutFile(DataBindApp dataBindApp)
        {
            try
            {
                Popup_Show_Status("Rename", "Renaming shortcut");
                Debug.WriteLine("Renaming shortcut: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Rename shortcut", dataBindApp.Name, "Rename the shortcut file");
                if (!string.IsNullOrWhiteSpace(textInputString))
                {
                    string shortcutDirectory = Path.GetDirectoryName(dataBindApp.ShortcutPath);
                    string fileExtension = Path.GetExtension(dataBindApp.ShortcutPath);
                    string newFileName = shortcutDirectory + "\\" + textInputString + fileExtension;

                    File.Move(dataBindApp.ShortcutPath, newFileName);
                    dataBindApp.Name = textInputString;
                    dataBindApp.ShortcutPath = newFileName;

                    Popup_Show_Status("Rename", "Renamed shortcut");
                    Debug.WriteLine("Renamed shortcut file to: " + textInputString);
                }
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Rename", "Failed renaming");
                Debug.WriteLine("Failed renaming shortcut: " + ex.Message);
            }
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
                        //Show application edit popup
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
                //Get the process multi
                ProcessMulti processMulti = dataBindApp.ProcessMulti.FirstOrDefault();

                //Get the process running time
                string processRunningTime = ApplicationRuntimeString(dataBindApp.RunningTime, "process");

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

                DataBindString Result = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTime, "", Answers);
                if (Result != null)
                {
                    if (Result == Answer0)
                    {
                        await ShowProcessWindow(dataBindApp, processMulti);
                    }
                    else if (Result == Answer1)
                    {
                        if (processMulti.Type == ProcessType.UWP)
                        {
                            await CloseSingleProcessUwp(dataBindApp, processMulti, false, true);
                        }
                        else
                        {
                            await CloseSingleProcessWin32AndWin32Store(dataBindApp, processMulti, false, true);
                        }
                    }
                    else if (Result == Answer2)
                    {
                        //Restart the process
                        if (processMulti.Type == ProcessType.UWP)
                        {
                            await RestartPrepareUwp(dataBindApp, processMulti);
                        }
                        else if (processMulti.Type == ProcessType.Win32Store)
                        {
                            await RestartPrepareWin32Store(dataBindApp, processMulti);
                        }
                        else
                        {
                            await RestartPrepareWin32(dataBindApp, processMulti);
                        }

                        //Refresh the application lists
                        await RefreshApplicationLists(true, false, false, false, false, false, false);

                        //Select the previous index
                        await FocusOnListbox(listboxSender, false, false, listboxSelectedIndex);
                    }
                }
            }
            catch { }
        }

        string ApplicationRuntimeString(int runningTime, string appCategory)
        {
            try
            {
                if (runningTime == -2) { return string.Empty; }
                else if (runningTime == -1) { return "This " + appCategory + " has been running for an unknown duration."; }
                else if (runningTime == 0) { return "This " + appCategory + " has not yet run for longer than a minute."; }
                else if (runningTime < 60) { return "This " + appCategory + " has been running for a total of " + runningTime + " minutes."; }
                else if (runningTime < 120)
                {
                    TimeSpan RunningTimeSpan = TimeSpan.FromMinutes(runningTime);
                    return "This " + appCategory + " has been running for a total of 1 hour and " + Convert.ToInt32(RunningTimeSpan.Minutes) + " minutes.";
                }
                else
                {
                    TimeSpan RunningTimeSpan = TimeSpan.FromMinutes(runningTime);
                    return "This " + appCategory + " has been running for a total of " + Convert.ToInt32(RunningTimeSpan.TotalHours) + " hours and " + Convert.ToInt32(RunningTimeSpan.Minutes) + " minutes.";
                }
            }
            catch
            {
                return "This " + appCategory + " has been running for an unknown duration.";
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