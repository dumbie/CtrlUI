using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
        private class ItchIOVerdict
        {
            public class Candidate
            {
                public string path { get; set; }
                public int depth { get; set; }
                public string flavor { get; set; }
                public string arch { get; set; }
                public int size { get; set; }
                public WindowsInfo windowsInfo { get; set; }
            }

            public class WindowsInfo
            {
                public bool gui { get; set; }
            }

            public string basePath { get; set; }
            public int totalSize { get; set; }
            public List<Candidate> candidates { get; set; }
        }

        private class ItchIOApp
        {
            public long Identifier { get; set; }
            public string Title { get; set; }
            public string ExecutablePath { get; set; }
            public string VerdictJson { get; set; }
        }

        async Task ItchIOScanAddLibrary()
        {
            try
            {
                //Get database path
                string commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string databasePath = Path.Combine(commonApplicationDataPath, "itch\\db\\butler.db");

                //Create sql connection
                List<ItchIOApp> listApps = new List<ItchIOApp>();
                using (SQLiteConnection sqLiteConnection = new SQLiteConnection("Data Source=" + databasePath + ";Mode=ReadOnly"))
                {
                    //Open sql connection
                    await sqLiteConnection.OpenAsync();

                    //Read caves
                    using (SQLiteCommand sqlComm = new SQLiteCommand(@"SELECT game_id,verdict FROM caves", sqLiteConnection))
                    {
                        SQLiteDataReader executeReader = sqlComm.ExecuteReader();
                        while (await executeReader.ReadAsync())
                        {
                            try
                            {
                                ItchIOApp itchIOApp = new ItchIOApp()
                                {
                                    Identifier = (long)executeReader["game_id"],
                                    VerdictJson = (string)executeReader["verdict"]
                                };
                                listApps.Add(itchIOApp);
                            }
                            catch { }
                        }
                    }

                    //Read games
                    using (SQLiteCommand sqlComm = new SQLiteCommand(@"SELECT id,title FROM games", sqLiteConnection))
                    {
                        SQLiteDataReader executeReader = sqlComm.ExecuteReader();
                        while (await executeReader.ReadAsync())
                        {
                            try
                            {
                                ItchIOApp itchIOApp = listApps.FirstOrDefault(x => x.Identifier == (long)executeReader["id"]);
                                if (itchIOApp != null)
                                {
                                    itchIOApp.Title = (string)executeReader["title"];
                                }
                            }
                            catch { }
                        }
                    }

                    //Close sql connection
                    sqLiteConnection.Close();
                }

                //Process applications
                foreach (ItchIOApp itchIOApp in listApps)
                {
                    try
                    {
                        //Convert json
                        ItchIOVerdict verdictApp = JsonConvert.DeserializeObject<ItchIOVerdict>(itchIOApp.VerdictJson);
                        ItchIOVerdict.Candidate candidateApp = verdictApp.candidates.FirstOrDefault();
                        itchIOApp.ExecutablePath = Path.Combine(verdictApp.basePath, candidateApp.path);

                        //Add application to launcher list
                        await ItchIOAddApplication(itchIOApp);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding ItchIO library: " + ex.Message);
            }
        }

        async Task ItchIOAddApplication(ItchIOApp itchIOApp)
        {
            try
            {
                //Check if application is installed
                if (!File.Exists(itchIOApp.ExecutablePath))
                {
                    Debug.WriteLine("ItchIO game is not installed: " + itchIOApp.Title);
                    return;
                }

                //Add application to available list
                vLauncherAppAvailableCheck.Add(itchIOApp.ExecutablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == itchIOApp.ExecutablePath.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("ItchIO app already in list: " + itchIOApp.Title);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = itchIOApp.Title.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher is on the blacklist skipping: " + itchIOApp.Title);
                    await ListBoxRemoveAll(lb_Launchers, List_Launchers, x => x.Name.ToLower() == appNameLower);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { itchIOApp.Title, itchIOApp.ExecutablePath, "ItchIO" }, vImageSourceFoldersAppsCombined, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.ItchIO,
                    Name = itchIOApp.Title,
                    ImageBitmap = iconBitmapImage,
                    PathExe = itchIOApp.ExecutablePath,
                    StatusLauncherImage = vImagePreloadItchIO
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added ItchIO app: " + itchIOApp.Title);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding ItchIO app: " + ex.Message);
            }
        }
    }
}