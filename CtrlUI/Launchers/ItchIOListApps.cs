using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static CtrlUI.Classes;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task ItchIOScanAddLibrary()
        {
            try
            {
                //Get database path
                string commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string databasePath = Path.Combine(commonApplicationDataPath, "itch\\db\\butler.db");

                //Check if database exists
                if (!File.Exists(databasePath)) { return; }

                //Create sql connection
                List<ItchIOApp> listApps = new List<ItchIOApp>();
                using (SQLiteConnection sqLiteConnection = new SQLiteConnection("Data Source=" + databasePath + ";Mode=ReadOnly"))
                {
                    //Open sql connection
                    await sqLiteConnection.OpenAsync();

                    //Read caves
                    using (SQLiteCommand sqlCommand = new SQLiteCommand(@"SELECT game_id,verdict FROM caves", sqLiteConnection))
                    {
                        using (SQLiteDataReader sqlReader = sqlCommand.ExecuteReader())
                        {
                            while (await sqlReader.ReadAsync())
                            {
                                try
                                {
                                    ItchIOApp itchIOApp = new ItchIOApp()
                                    {
                                        Identifier = (long)sqlReader["game_id"],
                                        VerdictJson = (string)sqlReader["verdict"]
                                    };
                                    listApps.Add(itchIOApp);
                                }
                                catch { }
                            }
                        }
                    }

                    //Read games
                    using (SQLiteCommand sqlCommand = new SQLiteCommand(@"SELECT id,title FROM games", sqLiteConnection))
                    {
                        using (SQLiteDataReader sqlReader = sqlCommand.ExecuteReader())
                        {
                            while (await sqlReader.ReadAsync())
                            {
                                try
                                {
                                    ItchIOApp itchIOApp = listApps.FirstOrDefault(x => x.Identifier == (long)sqlReader["id"]);
                                    if (itchIOApp != null)
                                    {
                                        itchIOApp.Title = (string)sqlReader["title"];
                                    }
                                }
                                catch { }
                            }
                        }
                    }
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

                //Add application to check list
                vLauncherAppAvailableCheck.Add(itchIOApp.ExecutablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == itchIOApp.ExecutablePath.ToLower());
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Launcher app already in list: " + itchIOApp.Title);
                    return;
                }

                //Check if application name is ignored
                string appNameLower = itchIOApp.Title.ToLower();
                if (vCtrlIgnoreLauncherName.Any(x => x.String1.ToLower() == appNameLower))
                {
                    //Debug.WriteLine("Launcher app is on the blacklist: " + itchIOApp.Title);
                    return;
                }

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { itchIOApp.Title, itchIOApp.ExecutablePath, "ItchIO" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

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