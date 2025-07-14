using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
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
                LauncherSetting launcherSet = listbox_LauncherSetting.SelectedItem as LauncherSetting;

                //Switch enabled setting
                launcherSet.Enabled = !launcherSet.Enabled;

                //Save launcher setting
                SettingSave(vConfigurationCtrlUI, launcherSet.Name, launcherSet.Enabled);

                //Remove launcher apps
                if (!launcherSet.Enabled)
                {
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == launcherSet.AppLauncher;
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                    await ListBoxRemoveAll(lb_Search, List_Search, filterLauncherApp);
                }

                Debug.WriteLine("Set launcher setting: " + launcherSet.Name + "/" + launcherSet.Enabled);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LauncherSettingSave error: " + ex.Message);
            }
        }

        //Handle launcher setting keyboard/controller tapped
        void ListBox_LauncherSetting_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    LauncherSettingSave();
                }
            }
            catch { }
        }

        //Handle launcher setting mouse/touch tapped
        void ListBox_LauncherSetting_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (vMousePressDownLeft)
                    {
                        LauncherSettingSave();
                    }
                }
            }
            catch { }
        }
    }
}