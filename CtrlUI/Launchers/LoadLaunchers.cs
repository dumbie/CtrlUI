using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task LoadLauncherApplications()
        {
            try
            {
                //Check if already refreshing
                if (vBusyRefreshingLaunchers)
                {
                    Debug.WriteLine("Launchers are already refreshing, cancelling.");
                    return;
                }

                //Update the refreshing status
                vBusyRefreshingLaunchers = true;

                //Show the loading gif
                AVActions.ActionDispatcherInvoke(delegate
                {
                    gif_Launchers_Loading.Show();
                });

                //Check if sorting is required
                bool sortByName = !List_Launchers.Any();

                //Clear the app remove check list
                vLauncherAppAvailableCheck.Clear();

                //Scan and add library from Steam
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibrarySteam")))
                {
                    await SteamScanAddLibrary();
                }

                //Scan and add library from EA Desktop
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryEADesktop")))
                {
                    await EADesktopScanAddLibrary();
                }

                //Scan and add library from Epic
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryEpic")))
                {
                    await EpicScanAddLibrary();
                }

                //Scan and add library from Ubisoft
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryUbisoft")))
                {
                    await UbisoftScanAddLibrary();
                }

                //Scan and add library from GoG
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryGoG")))
                {
                    await GoGScanAddLibrary();
                }

                //Scan and add library from Battle.net
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryBattleNet")))
                {
                    await BattleNetScanAddLibrary();
                }

                //Scan and add library from Bethesda
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryBethesda")))
                {
                    await BethesdaScanAddLibrary();
                }

                //Scan and add library from Rockstar
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryRockstar")))
                {
                    await RockstarScanAddLibrary();
                }

                //Scan and add library from UWP games
                if (Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowLibraryUwp")))
                {
                    await UwpScanAddLibrary();
                }

                //Remove deleted launcher applications
                Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && !vLauncherAppAvailableCheck.Any(y => y == x.PathExe);
                await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                await ListBoxRemoveAll(lb_Search, List_Search, filterLauncherApp);

                //Sort applications and select first item
                if (sortByName)
                {
                    SortObservableCollection(lb_Launchers, List_Launchers, x => x.Name, null, true);
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        lb_Launchers.SelectedIndex = 0;
                    });
                }

                //Hide the loading gif
                AVActions.ActionDispatcherInvoke(delegate
                {
                    gif_Launchers_Loading.Hide();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading launchers: " + ex.Message);
            }
            //Update the refreshing status
            vBusyRefreshingLaunchers = false;
        }
    }
}