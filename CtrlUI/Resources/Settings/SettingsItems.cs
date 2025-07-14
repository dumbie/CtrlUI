using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVArrayFunctions;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Items - Application Settings
        void Settings_Items()
        {
            try
            {
                //Launcher settings
                var appLauncherArray = EnumToEnumArray<AppLauncher>().Where(x => x != AppLauncher.Unknown);
                foreach (AppLauncher appLauncher in appLauncherArray)
                {
                    try
                    {
                        BitmapImage imageBitmap = null;
                        if (appLauncher == AppLauncher.FourGame)
                        {
                            imageBitmap = vImagePreload4Game;
                        }
                        else if (appLauncher == AppLauncher.Amazon)
                        {
                            imageBitmap = vImagePreloadAmazon;
                        }
                        else if (appLauncher == AppLauncher.Ankama)
                        {
                            imageBitmap = vImagePreloadAnkama;
                        }
                        else if (appLauncher == AppLauncher.Arc)
                        {
                            imageBitmap = vImagePreloadArc;
                        }
                        else if (appLauncher == AppLauncher.Asobimo)
                        {
                            imageBitmap = vImagePreloadAsobimo;
                        }
                        else if (appLauncher == AppLauncher.BattleNet)
                        {
                            imageBitmap = vImagePreloadBattleNet;
                        }
                        else if (appLauncher == AppLauncher.BigFish)
                        {
                            imageBitmap = vImagePreloadBigFish;
                        }
                        else if (appLauncher == AppLauncher.DLsite)
                        {
                            imageBitmap = vImagePreloadDLsite;
                        }
                        else if (appLauncher == AppLauncher.EADesktop)
                        {
                            imageBitmap = vImagePreloadEADesktop;
                        }
                        else if (appLauncher == AppLauncher.Elixir)
                        {
                            imageBitmap = vImagePreloadElixir;
                        }
                        else if (appLauncher == AppLauncher.Epic)
                        {
                            imageBitmap = vImagePreloadEpic;
                        }
                        else if (appLauncher == AppLauncher.Fawkes)
                        {
                            imageBitmap = vImagePreloadFawkes;
                        }
                        else if (appLauncher == AppLauncher.Gameforge)
                        {
                            imageBitmap = vImagePreloadGameforge;
                        }
                        else if (appLauncher == AppLauncher.GameJolt)
                        {
                            imageBitmap = vImagePreloadGameJolt;
                        }
                        else if (appLauncher == AppLauncher.Glyph)
                        {
                            imageBitmap = vImagePreloadGlyph;
                        }
                        else if (appLauncher == AppLauncher.GoG)
                        {
                            imageBitmap = vImagePreloadGoG;
                        }
                        else if (appLauncher == AppLauncher.GooglePlay)
                        {
                            imageBitmap = vImagePreloadGooglePlay;
                        }
                        else if (appLauncher == AppLauncher.HikariField)
                        {
                            imageBitmap = vImagePreloadHikariField;
                        }
                        else if (appLauncher == AppLauncher.HoYoPlay)
                        {
                            imageBitmap = vImagePreloadHoYoPlay;
                        }
                        else if (appLauncher == AppLauncher.Humble)
                        {
                            imageBitmap = vImagePreloadHumble;
                        }
                        else if (appLauncher == AppLauncher.HyperPlay)
                        {
                            imageBitmap = vImagePreloadHyperPlay;
                        }
                        else if (appLauncher == AppLauncher.IndieGala)
                        {
                            imageBitmap = vImagePreloadIndieGala;
                        }
                        else if (appLauncher == AppLauncher.ItchIO)
                        {
                            imageBitmap = vImagePreloadItchIO;
                        }
                        else if (appLauncher == AppLauncher.Jagex)
                        {
                            imageBitmap = vImagePreloadJagex;
                        }
                        else if (appLauncher == AppLauncher.LegacyGames)
                        {
                            imageBitmap = vImagePreloadLegacyGames;
                        }
                        else if (appLauncher == AppLauncher.LoadingBay)
                        {
                            imageBitmap = vImagePreloadLoadingBay;
                        }
                        else if (appLauncher == AppLauncher.MyGames)
                        {
                            imageBitmap = vImagePreloadMyGames;
                        }
                        else if (appLauncher == AppLauncher.NCSoft)
                        {
                            imageBitmap = vImagePreloadNCSoft;
                        }
                        else if (appLauncher == AppLauncher.Netmarble)
                        {
                            imageBitmap = vImagePreloadNetmarble;
                        }
                        else if (appLauncher == AppLauncher.Nexon)
                        {
                            imageBitmap = vImagePreloadNexon;
                        }
                        else if (appLauncher == AppLauncher.Oculus)
                        {
                            imageBitmap = vImagePreloadOculus;
                        }
                        else if (appLauncher == AppLauncher.OpenLoot)
                        {
                            imageBitmap = vImagePreloadOpenLoot;
                        }
                        else if (appLauncher == AppLauncher.Paradox)
                        {
                            imageBitmap = vImagePreloadParadox;
                        }
                        else if (appLauncher == AppLauncher.Plarium)
                        {
                            imageBitmap = vImagePreloadPlarium;
                        }
                        else if (appLauncher == AppLauncher.Riot)
                        {
                            imageBitmap = vImagePreloadRiot;
                        }
                        else if (appLauncher == AppLauncher.RobotCache)
                        {
                            imageBitmap = vImagePreloadRobotCache;
                        }
                        else if (appLauncher == AppLauncher.Rockstar)
                        {
                            imageBitmap = vImagePreloadRockstar;
                        }
                        else if (appLauncher == AppLauncher.Steam)
                        {
                            imageBitmap = vImagePreloadSteam;
                        }
                        else if (appLauncher == AppLauncher.Stove)
                        {
                            imageBitmap = vImagePreloadStove;
                        }
                        else if (appLauncher == AppLauncher.Ubisoft)
                        {
                            imageBitmap = vImagePreloadUbisoft;
                        }
                        else if (appLauncher == AppLauncher.UWP)
                        {
                            imageBitmap = vImagePreloadMicrosoft;
                        }
                        else if (appLauncher == AppLauncher.Vive)
                        {
                            imageBitmap = vImagePreloadVive;
                        }
                        else if (appLauncher == AppLauncher.VKPlay)
                        {
                            imageBitmap = vImagePreloadVKPlay;
                        }
                        else if (appLauncher == AppLauncher.Wargaming)
                        {
                            imageBitmap = vImagePreloadWargaming;
                        }
                        else if (appLauncher == AppLauncher.WildTangent)
                        {
                            imageBitmap = vImagePreloadWildTangent;
                        }

                        string settingName = "ShowLibrary" + appLauncher.ToString();
                        bool settingEnabled = SettingLoad(vConfigurationCtrlUI, settingName, typeof(bool));
                        listbox_LauncherSetting.Items.Add(new LauncherSetting() { AppLauncher = appLauncher, ImageBitmap = imageBitmap, Name = settingName, Enabled = settingEnabled });
                    }
                    catch { }
                }
                listbox_LauncherSetting.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to set setting items: " + ex.Message);
            }
        }
    }
}