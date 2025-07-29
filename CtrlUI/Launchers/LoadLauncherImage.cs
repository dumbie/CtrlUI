using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        public async Task<BitmapImage> LoadLauncherImage(AppLauncher appLauncher, int imageWidth, int imageHeight)
        {
            try
            {
                //Set launcher image paths
                string[] filePaths = null;
                if (appLauncher == AppLauncher.FourGame)
                {
                    filePaths = ["4Game"];
                }
                else if (appLauncher == AppLauncher.Amazon)
                {
                    filePaths = ["Amazon"];
                }
                else if (appLauncher == AppLauncher.Ankama)
                {
                    filePaths = ["Ankama"];
                }
                else if (appLauncher == AppLauncher.Arc)
                {
                    filePaths = ["Arc"];
                }
                else if (appLauncher == AppLauncher.Asobimo)
                {
                    filePaths = ["Asobimo"];
                }
                else if (appLauncher == AppLauncher.BattleNet)
                {
                    filePaths = ["Battle.net"];
                }
                else if (appLauncher == AppLauncher.BigFish)
                {
                    filePaths = ["Big Fish"];
                }
                else if (appLauncher == AppLauncher.DLsite)
                {
                    filePaths = ["DLsite"];
                }
                else if (appLauncher == AppLauncher.EADesktop)
                {
                    filePaths = ["EA Desktop"];
                }
                else if (appLauncher == AppLauncher.Elixir)
                {
                    filePaths = ["Elixir"];
                }
                else if (appLauncher == AppLauncher.Epic)
                {
                    filePaths = ["Epic"];
                }
                else if (appLauncher == AppLauncher.Fawkes)
                {
                    filePaths = ["Fawkes"];
                }
                else if (appLauncher == AppLauncher.Gameforge)
                {
                    filePaths = ["Gameforge"];
                }
                else if (appLauncher == AppLauncher.GameJolt)
                {
                    filePaths = ["Game Jolt"];
                }
                else if (appLauncher == AppLauncher.Glyph)
                {
                    filePaths = ["GlyphClient"];
                }
                else if (appLauncher == AppLauncher.GoG)
                {
                    filePaths = ["GoG"];
                }
                else if (appLauncher == AppLauncher.GooglePlay)
                {
                    filePaths = ["Google Play"];
                }
                else if (appLauncher == AppLauncher.HikariField)
                {
                    filePaths = ["Hikari Field"];
                }
                else if (appLauncher == AppLauncher.HoYoPlay)
                {
                    filePaths = ["HoYoPlay"];
                }
                else if (appLauncher == AppLauncher.Humble)
                {
                    filePaths = ["Humble"];
                }
                else if (appLauncher == AppLauncher.HyperPlay)
                {
                    filePaths = ["HyperPlay"];
                }
                else if (appLauncher == AppLauncher.IndieGala)
                {
                    filePaths = ["IndieGala"];
                }
                else if (appLauncher == AppLauncher.ItchIO)
                {
                    filePaths = ["ItchIO"];
                }
                else if (appLauncher == AppLauncher.Jagex)
                {
                    filePaths = ["Jagex"];
                }
                else if (appLauncher == AppLauncher.LegacyGames)
                {
                    filePaths = ["Legacy Games"];
                }
                else if (appLauncher == AppLauncher.LoadingBay)
                {
                    filePaths = ["Loading Bay"];
                }
                else if (appLauncher == AppLauncher.MyGames)
                {
                    filePaths = ["MyGames"];
                }
                else if (appLauncher == AppLauncher.NCSoft)
                {
                    filePaths = ["NCSoft"];
                }
                else if (appLauncher == AppLauncher.Netmarble)
                {
                    filePaths = ["Netmarble"];
                }
                else if (appLauncher == AppLauncher.Nexon)
                {
                    filePaths = ["Nexon"];
                }
                else if (appLauncher == AppLauncher.Oculus)
                {
                    filePaths = ["Oculus"];
                }
                else if (appLauncher == AppLauncher.OpenLoot)
                {
                    filePaths = ["Open Loot"];
                }
                else if (appLauncher == AppLauncher.Paradox)
                {
                    filePaths = ["Paradox"];
                }
                else if (appLauncher == AppLauncher.Plarium)
                {
                    filePaths = ["Plarium"];
                }
                else if (appLauncher == AppLauncher.Riot)
                {
                    filePaths = ["Riot"];
                }
                else if (appLauncher == AppLauncher.RobotCache)
                {
                    filePaths = ["RobotCache"];
                }
                else if (appLauncher == AppLauncher.Rockstar)
                {
                    filePaths = ["Rockstar"];
                }
                else if (appLauncher == AppLauncher.Steam)
                {
                    filePaths = ["Steam"];
                }
                else if (appLauncher == AppLauncher.Stove)
                {
                    filePaths = ["Stove"];
                }
                else if (appLauncher == AppLauncher.Ubisoft)
                {
                    filePaths = ["Ubisoft"];
                }
                else if (appLauncher == AppLauncher.UWP)
                {
                    filePaths = ["Microsoft"];
                }
                else if (appLauncher == AppLauncher.Vive)
                {
                    filePaths = ["Vive"];
                }
                else if (appLauncher == AppLauncher.VKPlay)
                {
                    filePaths = ["VK Play"];
                }
                else if (appLauncher == AppLauncher.Wargaming)
                {
                    filePaths = ["Wargaming"];
                }
                else if (appLauncher == AppLauncher.WildTangent)
                {
                    filePaths = ["WildTangent"];
                }

                //Return launcher bitmap image
                return await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = filePaths,
                    SearchPaths = vImageSourceFoldersAppsCombined,
                    BackupPath = vImageBackupSource,
                    Width = imageWidth,
                    Height = imageHeight,
                    Dispatcher = this.Dispatcher
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading launcher icon: " + ex.Message);
                return null;
            }
        }
    }
}