using ArnoldVinkStyles;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle app list keyboard/controller tapped
        private async void ListView_Apps_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //Check which key is pressed
                if (e.Key == VirtualKey.Space)
                {
                    //Get clicked item
                    DataBindApp clickedObject = AVListView.GetRoutedListViewItemObject<DataBindApp>(e);

                    await ListView_Apps_LeftClick(clickedObject);
                }
                else if (e.Key == VirtualKey.Delete || e.Key == VirtualKey.Back)
                {
                    //Get clicked item
                    DataBindApp clickedObject = AVListView.GetRoutedListViewItemObject<DataBindApp>(e);
                    int clickedIndex = AVListView.GetListViewItemObjectIndex((ListView)sender, clickedObject);

                    await ListView_Apps_RightClick((ListView)sender, clickedIndex, clickedObject);
                }
                else if (e.Key == VirtualKey.Insert)
                {
                    await Popup_Show_AddExe();
                }
            }
            catch { }
        }

        //Handle app list mouse/touch tapped
        private async void ListView_Apps_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check which mouse button is pressed
                if (vMousePressDownRight)
                {
                    //Get clicked item
                    DataBindApp clickedObject = AVListView.GetRoutedListViewItemObject<DataBindApp>(e);
                    int clickedIndex = AVListView.GetListViewItemObjectIndex((ListView)sender, clickedObject);

                    await ListView_Apps_RightClick((ListView)sender, clickedIndex, clickedObject);
                }
                else if (vMousePressDownLeft)
                {
                    //Get clicked item
                    DataBindApp clickedObject = AVListView.GetRoutedListViewItemObject<DataBindApp>(e);

                    await ListView_Apps_LeftClick(clickedObject);
                }
            }
            catch { }
        }

        //Handle app list left click
        private async Task ListView_Apps_LeftClick(DataBindApp dataBindApp)
        {
            try
            {
                //Check clicked object
                if (dataBindApp == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                await CheckApplicationLaunchMode(dataBindApp);
            }
            catch { }
        }

        //Handle app list right click
        private async Task ListView_Apps_RightClick(ListView listView, int listViewSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                //Check clicked object
                if (dataBindApp == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                if (dataBindApp.Category == AppCategory.Process)
                {
                    await SelectProcessAction(dataBindApp, null);
                }
                else if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    await RightClickShortcut(listView, listViewSelectedIndex, dataBindApp);
                }
                else if (dataBindApp.Category == AppCategory.Launcher)
                {
                    await RightClickLauncher(listView, listViewSelectedIndex, dataBindApp);
                }
                else if (dataBindApp.Category == AppCategory.Gallery)
                {
                    await RightClickGallery(listView, listViewSelectedIndex, dataBindApp);
                }
                else
                {
                    await RightClickApplication(listView, listViewSelectedIndex, dataBindApp);
                }
            }
            catch { }
        }

        private string ApplicationRunningTimeString(int runningTime, string appCategory)
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

        private string ApplicationLastLaunchTimeString(string lastLaunch, string appCategory)
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