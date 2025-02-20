using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task LoadListLaunchers()
        {
            try
            {
                //Check if application is activated
                if (!vAppActivated)
                {
                    return;
                }

                //Check if already refreshing
                if (vBusyRefreshingLaunchers)
                {
                    Debug.WriteLine("Launchers are already refreshing, cancelling.");
                    return;
                }

                //Check last update time
                long updateTime = GetSystemTicksMs();
                long updateOffset = updateTime - vLastUpdateLaunchers;
                if (updateOffset < 30000)
                {
                    //Debug.WriteLine("Launchers recently refreshed, cancelling.");
                    return;
                }

                //Update refreshing status
                vLastUpdateLaunchers = updateTime;
                vBusyRefreshingLaunchers = true;

                //Show the loading gif
                AVActions.DispatcherInvoke(delegate
                {
                    gif_List_Loading.Show();
                });

                //Check if loading first time
                bool firstLoad = !List_Launchers.Any();

                //Clear the app remove check list
                vLauncherAppAvailableCheck.Clear();

                //Scan and add library from Steam
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibrarySteam", typeof(bool)))
                {
                    await SteamScanAddLibrary();
                }

                //Scan and add library from EA Desktop
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryEADesktop", typeof(bool)))
                {
                    await EADesktopScanAddLibrary();
                }

                //Scan and add library from Epic
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryEpic", typeof(bool)))
                {
                    await EpicScanAddLibrary();
                }

                //Scan and add library from Ubisoft
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryUbisoft", typeof(bool)))
                {
                    await UbisoftScanAddLibrary();
                }

                //Scan and add library from GoG
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryGoG", typeof(bool)))
                {
                    await GoGScanAddLibrary();
                }

                //Scan and add library from Battle.net
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryBattleNet", typeof(bool)))
                {
                    await BattleNetScanAddLibrary();
                }

                //Scan and add library from Rockstar
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryRockstar", typeof(bool)))
                {
                    await RockstarScanAddLibrary();
                }

                //Scan and add library from Amazon
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryAmazon", typeof(bool)))
                {
                    await AmazonScanAddLibrary();
                }

                //Scan and add library from UWP
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryUwp", typeof(bool)))
                {
                    await UwpScanAddLibrary();
                }

                //Scan and add library from IndieGala
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryIndieGala", typeof(bool)))
                {
                    await IndieGalaScanAddLibrary();
                }

                //Scan and add library from Itch.io
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryItchIO", typeof(bool)))
                {
                    await ItchIOScanAddLibrary();
                }

                //Scan and add library from Humble
                if (SettingLoad(vConfigurationCtrlUI, "ShowLibraryHumble", typeof(bool)))
                {
                    await HumbleScanAddLibrary();
                }

                //Remove deleted launcher applications
                Func<DataBindApp, bool> filterLauncherApp = x => x.Category == AppCategory.Launcher && !vLauncherAppAvailableCheck.Any(y => y == x.PathExe || y == x.AppUserModelId);
                await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherApp);
                await ListBoxRemoveAll(lb_Search, List_Search, filterLauncherApp);

                //First load functions
                if (firstLoad)
                {
                    SortFunction<DataBindApp> sortFuncName = new SortFunction<DataBindApp>();
                    sortFuncName.function = x => x.Name;

                    List<SortFunction<DataBindApp>> orderListLaunchers = new List<SortFunction<DataBindApp>>();
                    orderListLaunchers.Add(sortFuncName);

                    SortObservableCollection(lb_Launchers, List_Launchers, orderListLaunchers, null);

                    AVActions.DispatcherInvoke(delegate
                    {
                        lb_Launchers.SelectedIndex = 0;
                    });
                }

                //Hide the loading gif
                if (vBusyRefreshingCount() == 1)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        gif_List_Loading.Hide();
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading launchers: " + ex.Message);
            }
            finally
            {
                //Update list load status
                vListLoadedLaunchers = true;

                //Update the refreshing status
                vBusyRefreshingLaunchers = false;
            }
        }
    }
}