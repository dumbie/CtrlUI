using Newtonsoft.Json;
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
                                await GoGAddApplication(gogGamePath, gogGameInfo, icoFilePath);
                            }
                            catch { }
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

        async Task GoGAddApplication(string gogGamePath, GoGGameInfo gogGameInfo, string icoFilePath)
        {
            try
            {
                //Check if information is dlc
                if (!gogGameInfo.playTasks.Any())
                {
                    Debug.WriteLine("Skipping GoG dlc: " + gogGameInfo.name);
                    return;
                }

                //Get primary playtask
                GoGPlayTasks primaryTask = gogGameInfo.playTasks.Where(x => x.isPrimary).FirstOrDefault();

                //Get executable launch path
                string runCommand = Path.Combine(gogGamePath, primaryTask.path);
                runCommand = runCommand.Replace("/", "\\");
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("GoG app already in list: " + appIds);
                    return;
                }

                //Get application name
                string appName = gogGameInfo.name;

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + appName);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get application launch argument
                string launchArgument = primaryTask.arguments;

                //Get application image
                string appImage = string.Empty;
                GoGPlayTasks playtaskIcon = gogGameInfo.playTasks.Where(x => x.icon != null && !string.IsNullOrWhiteSpace(x.icon)).FirstOrDefault();
                if (playtaskIcon != null)
                {
                    appImage = Path.Combine(gogGamePath, playtaskIcon.icon);
                    //Debug.WriteLine("Set GoG image to: " + appImage);
                }
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, icoFilePath, "GoG" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.GoG,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    Argument = launchArgument,
                    StatusLauncher = vImagePreloadGoG
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added GoG app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding GoG app: " + gogGameInfo.name);
            }
        }
    }
}