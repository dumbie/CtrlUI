﻿using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.FocusFunctions;

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
                await Notification_Send_Status("Close", "Failed to launch or show app");
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
                    else if (selectedItem.Category == AppCategory.Launcher)
                    {
                        await RightClickLauncher(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                    else
                    {
                        await RightClickList(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                }
            }
            catch { }
        }

        async Task RightClickList(ListBox listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                Debug.WriteLine("Right clicked app: " + dataBindApp.Name + " from: " + listboxSender.Name);

                //Show the messagebox popup with options
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerEdit = new DataBindString();
                AnswerEdit.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Edit.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerEdit.Name = "Edit this application details";
                Answers.Add(AnswerEdit);

                DataBindString AnswerRemove = new DataBindString();
                AnswerRemove.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRemove.Name = "Remove application from list";
                Answers.Add(AnswerRemove);

                DataBindString AnswerMove = new DataBindString();
                AnswerMove.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Move.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerMove.Name = "Move application position in list";
                Answers.Add(AnswerMove);

                //Get process details
                string processDetails = dataBindApp.PathExe;
                if (!string.IsNullOrWhiteSpace(dataBindApp.NameExe))
                {
                    processDetails += " (" + dataBindApp.NameExe + ")";
                }

                //Get process running time and last launch time
                string processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "application");
                string lastLaunchTimeString = ApplicationLastLaunchTimeString(dataBindApp.LastLaunch, "Application");

                //Set the running time string
                bool runningTimeEmpty = string.IsNullOrWhiteSpace(processRunningTimeString);
                bool launchTimeEmpty = string.IsNullOrWhiteSpace(lastLaunchTimeString);
                if (runningTimeEmpty && launchTimeEmpty)
                {
                    processRunningTimeString = processDetails;
                }
                else
                {
                    if (!launchTimeEmpty)
                    {
                        processRunningTimeString += "\n" + lastLaunchTimeString;
                    }
                    processRunningTimeString += "\n" + processDetails;
                }

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTimeString, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerEdit)
                    {
                        //Show application edit popup
                        await Popup_Show_AppEdit(dataBindApp);
                    }
                    else if (messageResult == AnswerRemove)
                    {
                        //Remove application from the list
                        await RemoveAppFromList(dataBindApp, true, true, false);

                        //Select the previous index
                        await ListboxFocusIndex(listboxSender, false, false, listboxSelectedIndex, vProcessCurrent.MainWindowHandle);
                    }
                    else if (messageResult == AnswerMove)
                    {
                        //Show application move popup
                        await Popup_Show_AppMove(dataBindApp);
                    }
                }
            }
            catch { }
        }

        string ApplicationRunningTimeString(int runningTime, string appCategory)
        {
            try
            {
                if (runningTime == -2) { return string.Empty; }
                else if (runningTime == -1) { return "This " + appCategory + " has been running for an unknown duration."; }
                else if (runningTime == 0) { return "This " + appCategory + " has been running for less than a minute."; }
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

        string ApplicationLastLaunchTimeString(string lastLaunch, string appCategory)
        {
            try
            {
                DateTime lastLaunchDateTime = DateTime.Parse(lastLaunch, vAppCultureInfo);
                return appCategory + " last launched on " + lastLaunchDateTime.ToString("d MMMM yyyy", vAppCultureInfo) + " at " + lastLaunchDateTime.ToShortTimeString();
            }
            catch
            {
                return string.Empty;
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