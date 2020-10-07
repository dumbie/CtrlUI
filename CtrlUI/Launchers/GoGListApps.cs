using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task GoGScanAddLibrary()
        {
            try
            {
                //Get launcher paths
                string commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string gogConfigPath = Path.Combine(commonApplicationDataPath, "GOG.com\\Galaxy\\config.json");

                //Get applications path from json config
                string gogConfigJson = File.ReadAllText(gogConfigPath);
                GoGConfig gogConfigDeserial = JsonConvert.DeserializeObject<GoGConfig>(gogConfigJson);

                //Add applications from json path
                string[] gameFolders = Directory.GetDirectories(gogConfigDeserial.libraryPath, "*");
                foreach (string infoFolder in gameFolders)
                {
                    try
                    {
                        string[] infoFiles = Directory.GetFiles(infoFolder, "goggame*.info");
                        foreach (string infoFile in infoFiles)
                        {
                            try
                            {
                                string icoFilePath = infoFile.Replace(".info", ".ico");
                                string gogGamePath = Path.GetDirectoryName(infoFile);
                                string infoFileString = File.ReadAllText(infoFile);
                                GoGGameInfo gogGameInfo = JsonConvert.DeserializeObject<GoGGameInfo>(infoFileString);
                                await GoGAddApplication(gogGamePath, icoFilePath, gogGameInfo);
                            }
                            catch
                            {
                                Debug.WriteLine("Failed to deserialize GoG game.");
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding GoG library: " + ex.Message);
            }
        }

        async Task GoGAddApplication(string gogGamePath, string icoFilePath, GoGGameInfo gogGameInfo)
        {
            try
            {
                //Check if information is dlc
                if (!gogGameInfo.playTasks.Any())
                {
                    Debug.WriteLine("Skipping GoG dlc: " + gogGameInfo.name);
                    return;
                }

                //Add game playtasks
                IEnumerable<GoGPlayTasks> gameTasks = gogGameInfo.playTasks.Where(x => x.isPrimary || x.category == GoGAppCategory.game);
                foreach (GoGPlayTasks gameTask in gameTasks)
                {
                    try
                    {
                        //Get executable launch path
                        string runCommand = Path.Combine(gogGamePath, gameTask.path);
                        runCommand = runCommand.Replace("/", "\\");
                        runCommand = Regex.Replace(runCommand, @"\s*(\\){2,}\s*", "\\");
                        vLauncherAppAvailableCheck.Add(runCommand);

                        //Check if application is already added
                        DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                        if (launcherExistCheck != null)
                        {
                            //Debug.WriteLine("GoG app already in list: " + appIds);
                            continue;
                        }

                        //Get application name
                        string appName = gameTask.name;
                        if (string.IsNullOrWhiteSpace(appName))
                        {
                            appName = gogGameInfo.name;
                        }

                        //Check application name
                        string appNameLower = appName.ToLower();
                        string[] nameFilterCheck = { "run", "launch", "launcher" };
                        bool replaceName = nameFilterCheck.Any(x => appNameLower.StartsWith(x));
                        if (replaceName)
                        {
                            appName = gogGameInfo.name;
                            appNameLower = appName.ToLower();
                        }

                        //Check if application name is ignored
                        if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                        {
                            //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                            await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                            continue;
                        }

                        //Get application launch argument
                        string launchArgument = gameTask.arguments;

                        //Get application image
                        string appImage = string.Empty;
                        GoGPlayTasks playtaskIcon = gogGameInfo.playTasks.Where(x => !string.IsNullOrWhiteSpace(x.icon)).FirstOrDefault();
                        if (playtaskIcon != null)
                        {
                            appImage = Path.Combine(gogGamePath, playtaskIcon.icon);
                            //Debug.WriteLine("Set GoG image to: " + appImage);
                        }
                        BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, icoFilePath, "GoG" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                        //Check the application category
                        Visibility categoryLauncher = gameTask.category == GoGAppCategory.launcher ? Visibility.Visible : Visibility.Collapsed;

                        //Add the application to the list
                        DataBindApp dataBindApp = new DataBindApp()
                        {
                            Category = AppCategory.Launcher,
                            Launcher = AppLauncher.GoG,
                            Name = appName,
                            ImageBitmap = iconBitmapImage,
                            PathExe = runCommand,
                            Argument = launchArgument,
                            StatusLauncher = vImagePreloadGoG,
                            StatusUrlProtocol = categoryLauncher
                        };

                        await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                        //Debug.WriteLine("Added GoG game: " + appName + "/" + gameTask.category);
                    }
                    catch { }
                }
            }
            catch
            {
                Debug.WriteLine("Failed adding GoG game: " + gogGameInfo.name);
            }
        }
    }
}