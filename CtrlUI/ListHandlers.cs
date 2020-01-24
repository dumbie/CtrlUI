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
using static ArnoldVinkCode.AVInteropDll;
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
                AnswerRemove.Name = "Move shortcut file to recycle bin";
                Answers.Add(AnswerRemove);

                DataBindString AnswerRename = new DataBindString();
                AnswerRename.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Rename.png" }, IntPtr.Zero, -1);
                AnswerRename.Name = "Rename the shortcut file";
                Answers.Add(AnswerRename);

                DataBindString AnswerHide = new DataBindString();
                AnswerHide.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Hide.png" }, IntPtr.Zero, -1);
                AnswerHide.Name = "Hide the shortcut file";
                Answers.Add(AnswerHide);

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTimeString, dataBindApp.PathExe, Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerRemove)
                    {
                        await RemoveShortcutFile(listboxSender, listboxSelectedIndex, dataBindApp);
                    }
                    else if (messageResult == AnswerRename)
                    {
                        await RenameShortcutFile(dataBindApp);
                    }
                    else if (messageResult == AnswerHide)
                    {
                        await HideShortcutFile(listboxSender, listboxSelectedIndex, dataBindApp);
                    }
                }
            }
            catch { }
        }

        //Hide the shortcut file
        async Task HideShortcutFile(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                Popup_Show_Status("Hide", "Hiding shortcut " + dataBindApp.Name);
                Debug.WriteLine("Hiding shortcut by name: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                //Create new profile shared
                ProfileShared profileShared = new ProfileShared();
                profileShared.String1 = dataBindApp.Name;

                //Add shortcut file to the ignore list
                vCtrlIgnoreShortcutName.Add(profileShared);
                JsonSaveObject(vCtrlIgnoreShortcutName, "CtrlIgnoreShortcutName");

                //Remove application from the list
                await RemoveAppFromList(dataBindApp, false, false, true);

                //Select the previous index
                await ListboxFocus(listboxSender, false, false, listboxSelectedIndex);
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Hide", "Failed hiding");
                Debug.WriteLine("Failed hiding shortcut: " + ex.Message);
            }
        }

        //Remove the shortcut file
        async Task RemoveShortcutFile(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                Popup_Show_Status("Minus", "Removing shortcut " + dataBindApp.Name);
                Debug.WriteLine("Removing shortcut: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                //Move the shortcut file to recycle bin
                SHFILEOPSTRUCT shFileOpstruct = new SHFILEOPSTRUCT();
                shFileOpstruct.wFunc = FILEOP_FUNC.FO_DELETE;
                shFileOpstruct.pFrom = dataBindApp.ShortcutPath + "\0\0";
                shFileOpstruct.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_ALLOWUNDO;
                int shFileResult = SHFileOperation(ref shFileOpstruct);

                //Check file operation status
                if (shFileResult == 0 && !shFileOpstruct.fAnyOperationsAborted)
                {
                    //Show the removal status notification
                    Popup_Show_Status("Minus", "Removed shortcut " + dataBindApp.Name);
                    Debug.WriteLine("Removed shortcut: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);

                    //Remove application from the list
                    await RemoveAppFromList(dataBindApp, false, false, true);
                }
                else if (shFileOpstruct.fAnyOperationsAborted)
                {
                    Popup_Show_Status("Minus", "Remove shortcut aborted");
                    Debug.WriteLine("Remove shortcut aborted: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);
                }
                else
                {
                    Popup_Show_Status("Minus", "Remove shortcut failed");
                    Debug.WriteLine("Remove shortcut failed: " + dataBindApp.Name + " path: " + dataBindApp.ShortcutPath);
                }

                //Select the previous index
                await ListboxFocus(listboxSender, false, false, listboxSelectedIndex);
            }
            catch (Exception ex)
            {
                Popup_Show_Status("Minus", "Failed removing");
                Debug.WriteLine("Failed removing shortcut: " + ex.Message);
            }
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

                //Check if file name changed
                if (textInputString == dataBindApp.Name)
                {
                    Popup_Show_Status("Rename", "File name not changed");
                    Debug.WriteLine("The file name did not change.");
                    return;
                }

                //Check the changed file name
                if (!string.IsNullOrWhiteSpace(textInputString))
                {
                    string shortcutDirectory = Path.GetDirectoryName(dataBindApp.ShortcutPath);
                    string fileExtension = Path.GetExtension(dataBindApp.ShortcutPath);
                    string newFilePath = Path.Combine(shortcutDirectory, textInputString + fileExtension);

                    File.Move(dataBindApp.ShortcutPath, newFilePath);
                    dataBindApp.Name = textInputString;
                    dataBindApp.ShortcutPath = newFilePath;

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

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", ApplicationRuntimeString(dataBindApp.RunningTime, "application"), "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == Answer1)
                    {
                        //Show application edit popup
                        await Popup_Show_AppEdit(listboxSender);
                    }
                    else if (messageResult == Answer2)
                    {
                        //Remove application from the list
                        await RemoveAppFromList(dataBindApp, true, true, false);

                        //Select the previous index
                        await ListboxFocus(listboxSender, false, false, listboxSelectedIndex);
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

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTime, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == Answer0)
                    {
                        await ShowProcessWindow(dataBindApp, processMulti);
                    }
                    else if (messageResult == Answer1)
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
                    else if (messageResult == Answer2)
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
                        await ListboxFocus(listboxSender, false, false, listboxSelectedIndex);
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