using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static CtrlUI.Classes;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task AnkamaScanAddLibrary()
        {
            try
            {
                //Get launcher paths
                string roamingDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string repositoriesPath = Path.Combine(roamingDataPath, "zaap\\repositories\\production");

                //Get all release json files
                string[] releaseFiles = Directory.GetFiles(repositoriesPath, "release.json", SearchOption.AllDirectories).Where(x => !x.Contains("\\data\\")).ToArray();
                foreach (string releaseFile in releaseFiles)
                {
                    try
                    {
                        //Read root release json file
                        string rootReleaseString = File.ReadAllText(releaseFile);
                        AnkamaInstall rootReleaseJson = JsonConvert.DeserializeObject<AnkamaInstall>(rootReleaseString);

                        //Check if game is installed
                        if (rootReleaseJson.location != "false" && rootReleaseJson.installedFragments.Any())
                        {
                            //Read data release json file
                            string dataReleaseString = File.ReadAllText(releaseFile.Replace("release.json", "data\\release.json"));
                            AnkamaData dataReleaseJson = JsonConvert.DeserializeObject<AnkamaData>(dataReleaseString);

                            //Add application to list
                            string appName = dataReleaseJson.displayName;
                            string appImage = Path.Combine(roamingDataPath, "zaap\\repositories\\production\\" + rootReleaseJson.gameUid + "\\" + rootReleaseJson.name + "\\data\\logo.png");
                            string runCommand = "zaap://app/games/game/" + rootReleaseJson.gameUid + "/" + rootReleaseJson.name + "?launch";
                            await AnkamaAddApplication(appName, appImage, runCommand);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Ankama library: " + ex.Message);
            }
        }

        async Task AnkamaAddApplication(string appName, string appImage, string runCommand)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == runCommand.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + appName);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = appName.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + appName);
                    return;
                }

                //Load application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, appImage, "Ankama" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Ankama,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncherImage = vImagePreloadAnkama
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Ankama game: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Ankama game: " + appName);
            }
        }
    }
}