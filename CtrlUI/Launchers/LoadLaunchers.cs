using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVArrayFunctions;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVSortObservableCollection;
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

                //Scan and add libraries
                var appLauncherArray = EnumToEnumArray<AppLauncher>().Where(x => x != AppLauncher.Unknown);
                foreach (AppLauncher appLauncher in appLauncherArray)
                {
                    try
                    {
                        string settingName = "ShowLibrary" + appLauncher.ToString();
                        if (SettingLoad(vConfigurationCtrlUI, settingName, typeof(bool)))
                        {
                            if (appLauncher == AppLauncher.FourGame)
                            {
                                await FourGameScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Amazon)
                            {
                                await AmazonScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Ankama)
                            {
                                await AnkamaScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Arc)
                            {
                                await ArcScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Asobimo)
                            {
                                await AsobimoScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.BattleNet)
                            {
                                await BattleNetScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.BigFish)
                            {
                                await BigFishScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.DLsite)
                            {
                                await DLsiteScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.EADesktop)
                            {
                                await EADesktopScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Elixir)
                            {
                                await ElixirScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Epic)
                            {
                                await EpicScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Fawkes)
                            {
                                await FawkesScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Gameforge)
                            {
                                await GameforgeScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.GameJolt)
                            {
                                await GameJoltScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Glyph)
                            {
                                await GlyphScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.GoG)
                            {
                                await GoGScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.GooglePlay)
                            {
                                await GooglePlayScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.HikariField)
                            {
                                await HikariFieldScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.HoYoPlay)
                            {
                                await HoYoPlayScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Humble)
                            {
                                await HumbleScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.HyperPlay)
                            {
                                await HyperPlayScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.IndieGala)
                            {
                                await IndieGalaScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.ItchIO)
                            {
                                await ItchIOScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Jagex)
                            {
                                await JagexScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.LegacyGames)
                            {
                                await LegacyGamesScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.LoadingBay)
                            {
                                await LoadingBayScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.MyGames)
                            {
                                await MyGamesScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.NCSoft)
                            {
                                await NCSoftScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Netmarble)
                            {
                                await NetmarbleScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Nexon)
                            {
                                await NexonScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Oculus)
                            {
                                await OculusScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Paradox)
                            {
                                await ParadoxScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.OpenLoot)
                            {
                                await OpenLootScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Plarium)
                            {
                                await PlariumScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Riot)
                            {
                                await RiotScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.RobotCache)
                            {
                                await RobotCacheScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Rockstar)
                            {
                                await RockstarScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Steam)
                            {
                                await SteamScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Stove)
                            {
                                await StoveScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Ubisoft)
                            {
                                await UbisoftScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.UWP)
                            {
                                await UwpScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Vive)
                            {
                                await ViveScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.VKPlay)
                            {
                                await VkPlayScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.Wargaming)
                            {
                                await WargamingScanAddLibrary();
                            }
                            else if (appLauncher == AppLauncher.WildTangent)
                            {
                                await WildTangentScanAddLibrary();
                            }
                        }
                    }
                    catch { }
                }

                //Remove deleted launcher applications
                Func<DataBindApp, bool> filterLauncherDeleted = x => x.Category == AppCategory.Launcher && !vLauncherAppAvailableCheck.Any(y => y == x.PathExe || y == x.AppUserModelId);
                await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherDeleted);
                await ListBoxRemoveAll(lb_Search, List_Search, filterLauncherDeleted);

                //Remove ignored launcher applications
                Func<DataBindApp, bool> filterLauncherIgnored = x => x.Category == AppCategory.Launcher && vCtrlIgnoreLauncherName.Any(y => y.String1.ToLower() == x.Name.ToLower());
                await ListBoxRemoveAll(lb_Launchers, List_Launchers, filterLauncherIgnored);
                await ListBoxRemoveAll(lb_Search, List_Search, filterLauncherIgnored);

                //First load functions
                if (firstLoad)
                {
                    //Sort list by name
                    SortFunction<DataBindApp> sortFuncName = new SortFunction<DataBindApp>();
                    sortFuncName.Function = x => x.Name;
                    SortObservableCollection(lb_Launchers, List_Launchers, sortFuncName, null);

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