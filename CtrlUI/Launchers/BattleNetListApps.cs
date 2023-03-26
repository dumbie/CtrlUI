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
        private class LaunchIdConvert
        {
            public string UID { get; set; }
            public string LaunchID { get; set; }
        }

        //Arrays
        private static string[] vBattleNetUidBlacklist = { "agent", "bna", "battle.net" };
        private static string[] vBattleNetBranchReplace = { "retail" };
        private static LaunchIdConvert[] vBattleNetLaunchIdentifiers =
        {
            //Test LaunchId with battlenet://LaunchId
            new LaunchIdConvert { UID = "wlby", LaunchID = "WLBY" }, //Crash Bandicoot 4: It's About Time
            new LaunchIdConvert { UID = "rtro", LaunchID = "RTRO" }, //Blizzard Arcade Collection
            new LaunchIdConvert { UID = "heroes", LaunchID = "Hero" }, //Heroes of the Storm
            new LaunchIdConvert { UID = "prometheus", LaunchID = "Pro" }, //Overwatch
            new LaunchIdConvert { UID = "s1", LaunchID = "S1" }, //StarCraft: Remastered
            new LaunchIdConvert { UID = "s2", LaunchID = "S2" }, //StarCraft II
            new LaunchIdConvert { UID = "hs_beta", LaunchID = "WTCG" }, //Hearthstone
            new LaunchIdConvert { UID = "w3", LaunchID = "W3" }, //Warcraft III: Reforged
            new LaunchIdConvert { UID = "wow", LaunchID = "WoW" }, //World of Warcraft
            //new LaunchIdConvert { UID = "wow_classic", LaunchID = "WoWC" }, //World of Warcraft Classic Expansion
            //new LaunchIdConvert { UID = "wow_classic_era", LaunchID = "WoWC" }, //World of Warcraft Classic Basegame
            new LaunchIdConvert { UID = "anbs", LaunchID = "ANBS" }, //Diablo: Immortal
            new LaunchIdConvert { UID = "osi", LaunchID = "OSI" }, //Diablo II: Resurrected
            new LaunchIdConvert { UID = "d3cn", LaunchID = "D3CN" }, //Diablo III China
            new LaunchIdConvert { UID = "diablo3", LaunchID = "D3" }, //Diablo III
            new LaunchIdConvert { UID = "fenris", LaunchID = "Fen" }, //Diablo IV
            new LaunchIdConvert { UID = "fore", LaunchID = "FORE" }, //Call of Duty: Vanguard
            new LaunchIdConvert { UID = "auks", LaunchID = "AUKS" }, //Call of Duty: Modern Warfare II
            new LaunchIdConvert { UID = "lazarus", LaunchID = "LAZR" }, //Call of Duty: Modern Warfare II Campaign
            new LaunchIdConvert { UID = "odin", LaunchID = "ODIN" }, //Call of Duty: Modern Warfare
            new LaunchIdConvert { UID = "zeus", LaunchID = "ZEUS" }, //Call of Duty: Black Ops Cold War
            new LaunchIdConvert { UID = "viper", LaunchID = "VIPR" }, //Call of Duty: Black Ops 4
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
                LaunchIdConvert launchIdConvert = vBattleNetLaunchIdentifiers.Where(x => x.UID == appUid).FirstOrDefault();
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
                DataBindApp launcherExistCheck = List_Launchers.Where(x => !string.IsNullOrWhiteSpace(x.Argument) && x.Argument.ToLower() == launchArgument.ToLower()).FirstOrDefault();
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
                appBranch = AVFunctions.ToTitleCase(appBranch);

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
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Battle.Net" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);

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