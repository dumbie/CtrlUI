using ArnoldVinkCode;
using LibraryShared;
using Microsoft.Win32;
using ProtoBuf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Classes
        private class BattleNetLaunchIdConvert
        {
            public string UID { get; set; }
            public string LaunchID { get; set; }
        }

        //Arrays
        private static string[] vBattleNetUidBlacklist = { "agent", "agent_beta", "bna", "battle.net" };
        private static string[] vBattleNetBranchReplace = { "retail" };
        private static BattleNetLaunchIdConvert[] vBattleNetLaunchIdentifiers =
        {
            //Test LaunchId with battlenet://LaunchId
            new BattleNetLaunchIdConvert { UID = "wlby", LaunchID = "WLBY" }, //Crash Bandicoot 4: It's About Time
            new BattleNetLaunchIdConvert { UID = "rtro", LaunchID = "RTRO" }, //Blizzard Arcade Collection
            new BattleNetLaunchIdConvert { UID = "heroes", LaunchID = "Hero" }, //Heroes of the Storm
            new BattleNetLaunchIdConvert { UID = "prometheus", LaunchID = "Pro" }, //Overwatch
            new BattleNetLaunchIdConvert { UID = "s1", LaunchID = "S1" }, //StarCraft: Remastered
            new BattleNetLaunchIdConvert { UID = "s2", LaunchID = "S2" }, //StarCraft II
            new BattleNetLaunchIdConvert { UID = "hs_beta", LaunchID = "WTCG" }, //Hearthstone
            new BattleNetLaunchIdConvert { UID = "w3", LaunchID = "W3" }, //Warcraft III: Reforged
            new BattleNetLaunchIdConvert { UID = "wow", LaunchID = "WoW" }, //World of Warcraft
            //new LaunchIdConvert { UID = "wow_classic", LaunchID = "WoWC" }, //World of Warcraft Classic Expansion
            //new LaunchIdConvert { UID = "wow_classic_era", LaunchID = "WoWC" }, //World of Warcraft Classic Basegame
            new BattleNetLaunchIdConvert { UID = "anbs", LaunchID = "ANBS" }, //Diablo: Immortal
            new BattleNetLaunchIdConvert { UID = "osi", LaunchID = "OSI" }, //Diablo II: Resurrected
            new BattleNetLaunchIdConvert { UID = "d3cn", LaunchID = "D3CN" }, //Diablo III China
            new BattleNetLaunchIdConvert { UID = "diablo3", LaunchID = "D3" }, //Diablo III
            new BattleNetLaunchIdConvert { UID = "fenris", LaunchID = "Fen" }, //Diablo IV
            new BattleNetLaunchIdConvert { UID = "fore", LaunchID = "FORE" }, //Call of Duty: Vanguard
            new BattleNetLaunchIdConvert { UID = "auks", LaunchID = "AUKS" }, //Call of Duty: Modern Warfare II
            new BattleNetLaunchIdConvert { UID = "lazarus", LaunchID = "LAZR" }, //Call of Duty: Modern Warfare II Campaign
            new BattleNetLaunchIdConvert { UID = "odin", LaunchID = "ODIN" }, //Call of Duty: Modern Warfare
            new BattleNetLaunchIdConvert { UID = "zeus", LaunchID = "ZEUS" }, //Call of Duty: Black Ops Cold War
            new BattleNetLaunchIdConvert { UID = "viper", LaunchID = "VIPR" }, //Call of Duty: Black Ops 4
        };

        string BattleNetLauncherExePath()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    //Search for Battle.net install directory
                    using (RegistryKey RegKeyBattle = registryKeyLocalMachine.OpenSubKey("Software\\Blizzard Entertainment\\Battle.net\\Capabilities"))
                    {
                        if (RegKeyBattle != null)
                        {
                            string RegKeyExePath = RegKeyBattle.GetValue("ApplicationIcon").ToString().Replace("\"", "").Replace(",0", "");
                            if (File.Exists(RegKeyExePath))
                            {
                                return RegKeyExePath;
                            }
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        async Task BattleNetScanAddLibrary()
        {
            try
            {
                //Get launcher paths
                string commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string productDatabasePath = Path.Combine(commonApplicationDataPath, "Battle.net\\Agent\\product.db");
                string launcherExePath = BattleNetLauncherExePath();

                using (FileStream productDatabaseFile = File.OpenRead(productDatabasePath))
                {
                    BattleNetProductDatabase productDatabase = Serializer.Deserialize<BattleNetProductDatabase>(productDatabaseFile);
                    foreach (ProductInstall productInstall in productDatabase.productInstall)
                    {
                        try
                        {
                            await BattleNetAddApplication(productInstall, launcherExePath);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding BattleNet library: " + ex.Message);
            }
        }

        async Task BattleNetAddApplication(ProductInstall productInstall, string launcherExePath)
        {
            try
            {
                //Get application details
                string appUid = productInstall.uid;
                string installDir = productInstall.settings.installPath;
                //Debug.WriteLine("BattleNet uid: " + appUid + " / " + installDir);

                //Check if application id is in blacklist
                if (vBattleNetUidBlacklist.Contains(appUid))
                {
                    Debug.WriteLine("BattleNet uid is blacklisted: " + appUid);
                    return;
                }

                //Check if application is installed
                if (!Directory.Exists(installDir))
                {
                    Debug.WriteLine("BattleNet game is not installed: " + appUid);
                    return;
                }

                //Set launch argument and convert UID to LaunchID
                //Improve find way to automatically load LaunchID
                bool launchKeyboard = false;
                string launchArgument = string.Empty;
                BattleNetLaunchIdConvert launchIdConvert = vBattleNetLaunchIdentifiers.FirstOrDefault(x => x.UID == appUid);
                if (launchIdConvert != null)
                {
                    launchArgument = "--exec=\"launch " + launchIdConvert.LaunchID + "\"";
                }
                else
                {
                    launchArgument = "--exec=\"launch_uid " + appUid + "\"";
                    launchKeyboard = true;
                }

                //Add application to available list
                vLauncherAppAvailableCheck.Add(launcherExePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Argument) && x.Argument.ToLower() == launchArgument.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("BattleNet app already in list: " + appUid);
                    return;
                }

                //Get application branch
                string appBranch = productInstall.settings.branch;
                appBranch = appBranch.Replace("_", string.Empty);
                foreach (string branchReplace in vBattleNetBranchReplace)
                {
                    appBranch = appBranch.Replace(branchReplace, string.Empty);
                }
                appBranch = AVFunctions.StringToTitleCase(appBranch);

                //Get application name
                string appName = Path.GetFileName(installDir);
                if (!string.IsNullOrWhiteSpace(appBranch))
                {
                    appName += " (" + appBranch + ")";
                }

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Battle.Net" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.BattleNet,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = launcherExePath,
                    Argument = launchArgument,
                    StatusLauncherImage = vImagePreloadBattleNet,
                    LaunchKeyboard = launchKeyboard
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added BattleNet app: " + appName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding BattleNet app: " + ex.Message);
            }
        }
    }
}