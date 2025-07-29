using ArnoldVinkStyles;
using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml.Input;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        public async void LauncherSettingSave()
        {
            try
            {
                //Get launcher setting
                LauncherSetting launcherSet = listView_LauncherSetting.SelectedItem as LauncherSetting;

                //Switch enabled setting
                launcherSet.Enabled = !launcherSet.Enabled;

                //Save launcher setting
                SettingSave(vConfigurationCtrlUI, launcherSet.Name, launcherSet.Enabled);

                //Remove launcher apps
                if (!launcherSet.Enabled)
                {
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == launcherSet.AppLauncher;
                    await ListViewRemoveAll(listView_Launchers, List_Launchers, filterLauncherApp);
                    await ListViewRemoveAll(listView_Search, List_Search, filterLauncherApp);
                }

                Debug.WriteLine("Set launcher setting: " + launcherSet.Name + "/" + launcherSet.Enabled);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LauncherSettingSave error: " + ex.Message);
            }
        }

        //Handle launcher setting keyboard/controller tapped
        void ListView_LauncherSetting_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                if (e.Key == VirtualKey.Space)
                {
                    LauncherSettingSave();
                }
            }
            catch { }
        }

        //Handle launcher setting mouse/touch tapped
        void ListView_LauncherSetting_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check if an actual ListViewItem is clicked
                if (!AVInterface.CheckClickedListViewItem(e))
                {
                    return;
                }

                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    LauncherSettingSave();
                }
            }
            catch { }
        }
    }
}