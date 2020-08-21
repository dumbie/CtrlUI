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
        public static string[] vBattleNetUidBlacklist = { "agent", "bna", "battle.net" };
        public static string[] vBattleNetBranchReplace = { "retail" };

        string BattleNetLauncherExePath()
        {
            try
            {
                //Open the Windows registry
                RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

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

                //Close and dispose the registry
                registryKeyLocalMachine.Dispose();
            }
            catch { }
            return string.Empty;
        }

        async Task BattleNetScanAddLibrary()
        {
            try
            {
                //Get launcher icon image
                BitmapImage launcherImage = FileToBitmapImage(new string[] { "Battle.net" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 10, 0);

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
                            await BattleNetAddApplication(productInstall, launcherImage, launcherExePath);
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

        async Task BattleNetAddApplication(ProductInstall productInstall, BitmapImage launcherImage, string launcherExePath)
        {
            try
            {
                //Get application details
                //Improve find way to load proper uid
                string appUid = productInstall.uid;
                string installDir = productInstall.settings.installPath;

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

                //Set application launch argument
                string launchArgument = "--exec=\"launch_uid " + appUid + "\"";
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

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, "Battle.Net" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.BattleNet,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = launcherExePath,
                    Argument = launchArgument,
                    StatusLauncher = launcherImage,
                    LaunchKeyboard = true
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