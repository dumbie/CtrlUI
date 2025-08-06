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
        public async void Launcher_Setting_Save(LauncherSetting launcherSetting)
        {
            try
            {
                //Check clicked object
                if (launcherSetting == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                //Switch enabled setting
                launcherSetting.Enabled = !launcherSetting.Enabled;

                //Save launcher setting
                SettingSave(vConfigurationCtrlUI, launcherSetting.Name, launcherSetting.Enabled);

                //Remove launcher apps
                if (!launcherSetting.Enabled)
                {
                    Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && x.Launcher == launcherSetting.AppLauncher;
                    await ListViewRemoveAll(listView_Launchers, List_Launchers, filterLauncherApp);
                    await ListViewRemoveAll(listView_Search, List_Search, filterLauncherApp);
                }

                Debug.WriteLine("Set launcher setting: " + launcherSetting.Name + "/" + launcherSetting.Enabled);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Launcher setting save error: " + ex.Message);
            }
        }

        //Handle launcher setting keyboard/controller tapped
        void ListView_LauncherSetting_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //Check which key is pressed
                if (e.Key == VirtualKey.Space)
                {
                    //Get clicked item
                    LauncherSetting clickedObject = AVListView.GetRoutedListViewItemObject<LauncherSetting>(e);

                    Launcher_Setting_Save(clickedObject);
                }
            }
            catch { }
        }

        //Handle launcher setting mouse/touch tapped
        void ListView_LauncherSetting_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    //Get clicked item
                    LauncherSetting clickedObject = AVListView.GetRoutedListViewItemObject<LauncherSetting>(e);

                    Launcher_Setting_Save(clickedObject);
                }
            }
            catch { }
        }
    }
}