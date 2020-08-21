using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        public static List<string> vLauncherAppAvailableCheck = new List<string>();

        async Task LoadLauncherApplications()
        {
            try
            {
                //Check if sorting is required
                bool sortByName = !List_Launchers.Any();

                //Clear the app remove check list
                vLauncherAppAvailableCheck.Clear();

                //Scan and add library from Steam
                await SteamScanAddLibrary();

                //Scan and add library from Origin
                await OriginScanAddLibrary();

                //Scan and add library from Epic Games
                await EpicScanAddLibrary();

                //Scan and add library from Uplay
                await UplayScanAddLibrary();

                //Scan and add library from GoG
                await GoGScanAddLibrary();

                //Scan and add library from Battle.net
                await BattleNetScanAddLibrary();

                //Scan and add library from Bethesda
                await BethesdaScanAddLibrary();

                //Scan and add library from Rockstar
                await RockstarScanAddLibrary();

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
            }
            catch { }
        }
    }
}