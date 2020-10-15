using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Add first launch applications automatically
        async Task AddFirstLaunchApps()
        {
            try
            {
                //Set application first launch to false
                Setting_Save(vConfigurationCtrlUI, "AppFirstLaunch", "False");

                //Open the Windows registry
                RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

                //Search for Microsoft Edge install and add to the list
                using (RegistryKey RegKeyEdge = registryKeyLocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\App Paths\\msedge.exe"))
                {
                    if (RegKeyEdge != null)
                    {
                        string RegKeyExePath = RegKeyEdge.GetValue(null).ToString();
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Edge", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath), LaunchKeyboard = true };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Edge.IsEnabled = false;
                            grid_Popup_Welcome_button_Edge.Opacity = 0.40;
                        }
                    }
                }

                //Search for Discord install and add to the list
                string DiscordExePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord\\Update.exe";
                if (File.Exists(DiscordExePath))
                {
                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Discord", NameExe = "Discord.exe", Argument = "--processStart Discord.exe", PathExe = DiscordExePath, PathLaunch = Path.GetDirectoryName(DiscordExePath), LaunchKeyboard = true };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    grid_Popup_Welcome_button_Discord.IsEnabled = false;
                    grid_Popup_Welcome_button_Discord.Opacity = 0.40;
                }

                //Search for Spotify install and add to the list
                string SpotifyExePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\Spotify.exe";
                if (File.Exists(SpotifyExePath))
                {
                    //Add application to the list
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Spotify", PathExe = SpotifyExePath, PathLaunch = Path.GetDirectoryName(SpotifyExePath), LaunchKeyboard = true };
                    await AddAppToList(dataBindApp, true, true);

                    //Disable the icon after selection
                    grid_Popup_Welcome_button_Spotify.IsEnabled = false;
                    grid_Popup_Welcome_button_Spotify.Opacity = 0.40;
                }

                //Search for Kodi install and add to the list
                using (RegistryKey RegKeyKodi = registryKeyCurrentUser.OpenSubKey("Software\\Kodi"))
                {
                    if (RegKeyKodi != null)
                    {
                        string RegKeyExePath = RegKeyKodi.GetValue(null).ToString() + "\\Kodi.exe";
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Kodi", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Kodi.IsEnabled = false;
                            grid_Popup_Welcome_button_Kodi.Opacity = 0.40;
                        }
                    }
                }

                //Add Xbox uwp application to the list
                if (UwpGetAppPackageByAppUserModelId("Microsoft.GamingApp_8wekyb3d8bbwe!Microsoft.Xbox.App") != null)
                {
                    DataBindApp dataBindAppXbox = new DataBindApp() { Type = ProcessType.UWP, Category = AppCategory.App, Name = "Xbox", NameExe = "XboxPcApp.exe", PathExe = "Microsoft.GamingApp_8wekyb3d8bbwe!Microsoft.Xbox.App" };
                    await AddAppToList(dataBindAppXbox, true, true);
                }

                //Search for PS Remote Play install and add to the list
                using (RegistryKey RegKeyPSRemote = registryKeyLocalMachine.OpenSubKey("SOFTWARE\\Sony Corporation\\PS Remote Play"))
                {
                    if (RegKeyPSRemote != null)
                    {
                        string RegKeyExePath = RegKeyPSRemote.GetValue("Path").ToString() + "RemotePlay.exe";
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.App, Name = "Remote Play", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_PSRemote.IsEnabled = false;
                            grid_Popup_Welcome_button_PSRemote.Opacity = 0.40;
                        }
                    }
                }

                //Search for Steam install and add to the list
                using (RegistryKey RegKeySteam = registryKeyCurrentUser.OpenSubKey("Software\\Valve\\Steam"))
                {
                    if (RegKeySteam != null)
                    {
                        string RegKeyExePath = RegKeySteam.GetValue("SteamExe").ToString();
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Steam", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath), Argument = "-bigpicture", QuickLaunch = true };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Steam.IsEnabled = false;
                            grid_Popup_Welcome_button_Steam.Opacity = 0.40;
                        }
                    }
                }

                //Search for GoG install and add to the list
                using (RegistryKey RegKeyGoG = registryKeyLocalMachine.OpenSubKey("Software\\GOG.com\\GalaxyClient\\paths"))
                {
                    if (RegKeyGoG != null)
                    {
                        string RegKeyExePath = RegKeyGoG.GetValue("client").ToString() + "\\GalaxyClient.exe";
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "GoG", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_GoG.IsEnabled = false;
                            grid_Popup_Welcome_button_GoG.Opacity = 0.40;
                        }
                    }
                }

                //Search for Epic install and add to the list
                using (RegistryKey RegKeyEpic = registryKeyLocalMachine.OpenSubKey("Software\\Classes\\com.epicgames.launcher\\DefaultIcon"))
                {
                    if (RegKeyEpic != null)
                    {
                        string RegKeyExePath = RegKeyEpic.GetValue(null).ToString().Replace(",0", string.Empty);
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Epic Games", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Epic.IsEnabled = false;
                            grid_Popup_Welcome_button_Epic.Opacity = 0.40;
                        }
                    }
                }

                //Search for Origin install and add to the list
                using (RegistryKey RegKeyOrigin = registryKeyLocalMachine.OpenSubKey("Software\\Origin"))
                {
                    if (RegKeyOrigin != null)
                    {
                        string RegKeyExePath = RegKeyOrigin.GetValue("ClientPath").ToString();
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Origin", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Origin.IsEnabled = false;
                            grid_Popup_Welcome_button_Origin.Opacity = 0.40;
                        }
                    }
                }

                //Search for Uplay install and add to the list
                using (RegistryKey RegKeyUplay = registryKeyLocalMachine.OpenSubKey("Software\\Ubisoft\\Launcher"))
                {
                    if (RegKeyUplay != null)
                    {
                        string RegKeyExePath = RegKeyUplay.GetValue("InstallDir").ToString() + "upc.exe";
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Uplay", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Uplay.IsEnabled = false;
                            grid_Popup_Welcome_button_Uplay.Opacity = 0.40;
                        }
                    }
                }

                //Search for Battle.net install and add to the list
                using (RegistryKey RegKeyBattle = registryKeyLocalMachine.OpenSubKey("Software\\Blizzard Entertainment\\Battle.net\\Capabilities"))
                {
                    if (RegKeyBattle != null)
                    {
                        string RegKeyExePath = RegKeyBattle.GetValue("ApplicationIcon").ToString().Replace("\"", "").Replace(",0", "");
                        if (File.Exists(RegKeyExePath))
                        {
                            //Add application to the list
                            DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = AppCategory.Game, Name = "Battle.net", PathExe = RegKeyExePath, PathLaunch = Path.GetDirectoryName(RegKeyExePath) };
                            await AddAppToList(dataBindApp, true, true);

                            //Disable the icon after selection
                            grid_Popup_Welcome_button_Battle.IsEnabled = false;
                            grid_Popup_Welcome_button_Battle.Opacity = 0.40;
                        }
                    }
                }

                //Close and dispose the registry
                registryKeyLocalMachine.Dispose();
                registryKeyCurrentUser.Dispose();

                //Show the welcome screen popup
                await Popup_Show(grid_Popup_Welcome, grid_Popup_Welcome_button_Start);
            }
            catch { }
        }
    }
}