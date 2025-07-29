using ArnoldVinkStyles;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle app list mouse/touch tapped
        async void ListView_Apps_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check if an actual ListViewItem is clicked
                if (!AVInterface.CheckClickedListViewItem(e))
                {
                    return;
                }

                //Check which mouse button is pressed
                if (vMousePressDownRight)
                {
                    await ListView_Apps_RightClick(sender);
                }
                else if (vMousePressDownLeft)
                {
                    await ListView_Apps_LeftClick(sender);
                }
            }
            catch { }
        }

        //Handle app list left click
        async Task ListView_Apps_LeftClick(object sender)
        {
            try
            {
                ListView ListboxSender = (ListView)sender;
                if (ListboxSender.SelectedItems.Count > 0 && ListboxSender.SelectedIndex != -1)
                {
                    //Check which launch mode needs to be used
                    DataBindApp SelectedItem = (DataBindApp)ListboxSender.SelectedItem;
                    await CheckApplicationLaunchMode(SelectedItem);
                }
            }
            catch
            {
                await Notification_Show_Status("Close", "Failed to launch or show app");
                Debug.WriteLine("Failed launching or showing the application.");
            }
        }

        //Handle app list right click
        async Task ListView_Apps_RightClick(object sender)
        {
            try
            {
                ListView listboxSender = (ListView)sender;
                int listboxSelectedIndex = listboxSender.SelectedIndex;
                if (listboxSender.SelectedItems.Count > 0 && listboxSelectedIndex != -1)
                {
                    DataBindApp selectedItem = (DataBindApp)listboxSender.SelectedItem;
                    if (selectedItem.Category == AppCategory.Process)
                    {
                        await SelectProcessAction(selectedItem, null);
                    }
                    else if (selectedItem.Category == AppCategory.Shortcut)
                    {
                        await RightClickShortcut(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                    else if (selectedItem.Category == AppCategory.Launcher)
                    {
                        await RightClickLauncher(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                    else if (selectedItem.Category == AppCategory.Gallery)
                    {
                        await RightClickGallery(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                    else
                    {
                        await RightClickApplication(listboxSender, listboxSelectedIndex, selectedItem);
                    }
                }
            }
            catch { }
        }

        string ApplicationRunningTimeString(int runningTime, string appCategory)
        {
            try
            {
                if (runningTime <= -2) { return string.Empty; }
                else if (runningTime == -1) { return appCategory + " has been running for an unknown duration."; }
                else if (runningTime == 0) { return appCategory + " has been running for less than a minute."; }
                else if (runningTime < 60) { return appCategory + " has been running for a total of " + runningTime + " minutes."; }
                else if (runningTime < 120)
                {
                    TimeSpan RunningTimeSpan = TimeSpan.FromMinutes(runningTime);
                    return appCategory + " has been running for a total of 1 hour and " + Convert.ToInt32(RunningTimeSpan.Minutes) + " minutes.";
                }
                else
                {
                    TimeSpan RunningTimeSpan = TimeSpan.FromMinutes(runningTime);
                    return appCategory + " has been running for a total of " + Convert.ToInt32(RunningTimeSpan.TotalHours) + " hours and " + Convert.ToInt32(RunningTimeSpan.Minutes) + " minutes.";
                }
            }
            catch
            {
                return appCategory + " has been running for an unknown duration.";
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
    }
}