using ArnoldVinkCode;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        string OculusDatabaseParseBlob(string blobString, string targetStart, string targetEnd)
        {
            string stringBetween = string.Empty;
            try
            {
                //Fix properly decode and convert blob to class

                //Search for start and end strings
                Match regex = Regex.Matches(blobString, targetStart + "(.*?)" + targetEnd, RegexOptions.Singleline | RegexOptions.RightToLeft).FirstOrDefault();
                stringBetween = regex.Groups[1].ToString();

                //Remove unicode from found string
                stringBetween = Regex.Replace(stringBetween, "[^\t\r\n -~]", string.Empty);

                //Remove last character from string
                stringBetween = stringBetween.Remove(stringBetween.Length - 1);

                //Trim string of unwanted spaces
                stringBetween = stringBetween.Trim();
            }
            catch { }
            return stringBetween;
        }

        public async Task<OculusDatabaseApp> OculusDatabaseApplication(SQLiteConnection sqLiteConnection, string targetHashkey)
        {
            OculusDatabaseApp oculusDbApp = new OculusDatabaseApp();
            try
            {
                using (SQLiteCommand sqlCommand = new SQLiteCommand("SELECT value FROM Objects WHERE hashkey='" + targetHashkey + "' AND typename='Application'", sqLiteConnection))
                {
                    using (SQLiteDataReader sqlReader = sqlCommand.ExecuteReader())
                    {
                        while (await sqlReader.ReadAsync())
                        {
                            //Convert blob to string
                            byte[] blobBytes = (byte[])sqlReader["value"];
                            string blobString = Encoding.UTF8.GetString(blobBytes);

                            //Parse information from blob
                            oculusDbApp.canonical_name = OculusDatabaseParseBlob(blobString, "canonical_name", "category");
                            oculusDbApp.display_name = OculusDatabaseParseBlob(blobString, "display_name", "display_short_description");
                            oculusDbApp.display_short_description = OculusDatabaseParseBlob(blobString, "display_short_description", "genres");
                            oculusDbApp.organization_name = OculusDatabaseParseBlob(blobString, "name", "quality_rating_aggregate");
                            oculusDbApp.genres = OculusDatabaseParseBlob(blobString, "genres", "grouping");
                        }
                    }
                }
            }
            catch { }
            return oculusDbApp;
        }

        async Task OculusScanAddLibrary()
        {
            try
            {
                //Fix find way to launch with protocol oculus://link/deeplink/ to check vr hardware
                //Fix load application images from CoreData\Software\StoreAssets folder
                //Fix add right click menu option to launch 2d application

                //Get database paths
                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string databasePathOriginal = Path.Combine(roamingPath, "Oculus\\sessions\\_oaf");
                string databasePathCopy = Path.Combine(roamingPath, "Oculus\\sessions\\_oafcopy");
                string databasePathFile = Path.Combine(roamingPath, "Oculus\\sessions\\_oafcopy\\data.sqlite");

                //Make copy of database file to avoid read issues
                AVFiles.Directory_Copy(databasePathOriginal, databasePathCopy, true);

                //Create and open sql connection
                using SQLiteConnection sqLiteConnection = new SQLiteConnection("Data Source=" + databasePathFile + ";Mode=ReadOnly");
                await sqLiteConnection.OpenAsync();

                //Get all Oculus library paths
                List<string> oculusLibraryPaths = new List<string>();
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyLibraries = registryKeyCurrentUser.OpenSubKey("Software\\Oculus VR, LLC\\Oculus\\Libraries"))
                    {
                        if (regKeyLibraries != null)
                        {
                            foreach (string libraryId in regKeyLibraries.GetSubKeyNames())
                            {
                                try
                                {
                                    using (RegistryKey libraryDetails = regKeyLibraries.OpenSubKey(libraryId))
                                    {
                                        oculusLibraryPaths.Add(libraryDetails.GetValue("OriginalPath").ToString());
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }

                //Search for json files in library
                Dictionary<string, string> jsonFiles = new Dictionary<string, string>();
                foreach (string libraryPath in oculusLibraryPaths)
                {
                    try
                    {
                        //Search json files
                        string pathJson = Path.Combine(libraryPath, "Manifests");
                        string[] foundJson = Directory.GetFiles(pathJson, "*.json.mini", SearchOption.AllDirectories);
                        foreach (string jsonPath in foundJson)
                        {
                            jsonFiles.Add(jsonPath, libraryPath);
                        }
                    }
                    catch { }
                }

                //Add applications from json
                foreach (var jsonPaths in jsonFiles)
                {
                    try
                    {
                        //Get paths from dictionary
                        string jsonPath = jsonPaths.Key;
                        string libraryPath = jsonPaths.Value;

                        //Deserialize json app file
                        string appJson = File.ReadAllText(jsonPath);
                        OculusJsonApp appDeserial = JsonConvert.DeserializeObject<OculusJsonApp>(appJson);

                        //Get app information from database
                        OculusDatabaseApp appDatabase = await OculusDatabaseApplication(sqLiteConnection, appDeserial.appId);

                        //Set launch variables
                        string appName = appDatabase.display_name;
                        string executablePath = Path.Combine(libraryPath, "Software", appDeserial.canonicalName, appDeserial.launchFile);
                        string executableArguments = appDeserial.launchParameters;
                        await OculusAddApplication(appName, executablePath, executableArguments);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding Oculus library: " + ex.Message);
            }
        }

        async Task OculusAddApplication(string appName, string executablePath, string executableArguments)
        {
            try
            {
                //Add application to check list
                vLauncherAppAvailableCheck.Add(executablePath);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.FirstOrDefault(x => x.PathExe.ToLower() == executablePath.ToLower());
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

                //Get application image
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { appName, executablePath, "Oculus" }, vImageSourceFoldersAppsCombined, vImageBackupSource, vImageLoadSize, 0, IntPtr.Zero, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Oculus,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = executablePath,
                    Argument = executableArguments,
                    StatusLauncherImage = vImagePreloadOculus
                };

                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added Oculus app: " + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding Oculus app: " + appName);
            }
        }
    }
}