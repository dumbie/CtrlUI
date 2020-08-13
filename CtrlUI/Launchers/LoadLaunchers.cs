using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;

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

                //Remove deleted launcher applications
                await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => !vLauncherAppAvailableCheck.Any(y => y == x.PathExe));

                //Sort applications by name
                if (sortByName)
                {
                    SortObservableCollection(lb_Launchers, List_Launchers, x => x.Name, null, true);
                }
            }
            catch { }
        }
    }
}