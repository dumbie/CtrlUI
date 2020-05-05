using ArnoldVinkCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Api variables
        public static string vApiIGDBUserKey = "b25eb31b7612c7158867a3cd7849dbee"; //Yes, I know I didn't remove the api key.

        //Filter the rom name
        public string RomFilterName(string nameRom, bool removeExtension, bool removeSpaces, int takeWords)
        {
            try
            {
                //Remove file extension
                if (removeExtension)
                {
                    nameRom = Path.GetFileNameWithoutExtension(nameRom);
                }

                //Lowercase the rom name
                nameRom = nameRom.ToLower();

                //Remove symbols with text
                nameRom = Regex.Replace(nameRom, @"\((.*?)\)", string.Empty);
                nameRom = Regex.Replace(nameRom, @"\{(.*?)\}", string.Empty);
                nameRom = Regex.Replace(nameRom, @"\[(.*?)\]", string.Empty);

                //Replace characters
                nameRom = nameRom.Replace("'", " ").Replace(".", " ").Replace(",", " ").Replace("-", " ").Replace("_", " ");

                //Replace double spaces
                nameRom = Regex.Replace(nameRom, @"\s+", " ");

                //Remove words
                string[] nameFilterRemoveContains = new string[] { "usa", "eur", "pal", "ntsc", "repack", "proper" };
                string[] nameRomSplit = nameRom.Split(' ').Where(x => !nameFilterRemoveContains.Any(x.Contains)).ToArray();

                //Take words
                if (takeWords <= 0)
                {
                    nameRom = string.Join(" ", nameRomSplit);
                }
                else
                {
                    nameRom = string.Join(" ", nameRomSplit.Take(takeWords));
                }

                //Remove spaces
                if (removeSpaces)
                {
                    nameRom = nameRom.Replace(" ", string.Empty);
                }
                else
                {
                    nameRom = AVFunctions.StringRemoveStart(nameRom, " ");
                    nameRom = AVFunctions.StringRemoveEnd(nameRom, " ");
                }
            }
            catch { }
            return nameRom;
        }

        //Download rom information
        public async Task<RomInformation> RomDownloadInformation(string nameRom, int imageWidth)
        {
            try
            {
                //Filter the rom name
                string nameRomSave = RomFilterName(nameRom, true, false, 0);

                //Show the text input popup
                string nameRomDownload = await Popup_ShowHide_TextInput("Rom search", nameRomSave, "Search information for the rom", true);
                if (string.IsNullOrWhiteSpace(nameRomDownload))
                {
                    Debug.WriteLine("No search term entered.");
                    return null;
                }
                nameRomDownload = nameRomDownload.ToLower();

                Popup_Show_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading information for: " + nameRom);

                //Download available games
                IEnumerable<ApiIGDBGames> iGDBGames = await ApiIGDBDownloadGames(nameRomDownload);
                if (iGDBGames == null || !iGDBGames.Any())
                {
                    Debug.WriteLine("No games found");
                    Popup_Show_Status("Close", "No games found");
                    return null;
                }

                //Ask user which game to download
                CultureInfo cultureInfo = new CultureInfo("en-US");
                List<DataBindString> Answers = new List<DataBindString>();
                BitmapImage imageAnswer = FileToBitmapImage(new string[] { "Assets/Icons/Game.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                foreach (ApiIGDBGames infoGames in iGDBGames)
                {
                    //Check if information is available
                    if (infoGames.cover == 0 && infoGames.summary == string.Empty) { continue; }

                    //Get the release date
                    string gameReleaseDate = string.Empty;
                    string gameReleaseYear = string.Empty;
                    if (infoGames.first_release_date != 0)
                    {
                        DateTime epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(infoGames.first_release_date);
                        gameReleaseDate = "Released: " + epochDateTime.ToString("dd MMMM yyyy", cultureInfo) + "\n\n";
                        gameReleaseYear = epochDateTime.ToString("yyyy", cultureInfo);
                    }

                    //Get the game genres
                    string gameGenres = string.Empty;
                    if (infoGames.genres != null)
                    {
                        foreach (int genreId in infoGames.genres)
                        {
                            ApiIGDBGenres apiIGDBGenres = vApiIGDBGenres.Where(x => x.id == genreId).FirstOrDefault();
                            gameGenres = AVFunctions.StringAdd(gameGenres, apiIGDBGenres.name, ",");
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(gameGenres))
                    {
                        if (string.IsNullOrWhiteSpace(gameReleaseDate))
                        {
                            gameGenres = "Genres: " + gameGenres + "\n\n";
                        }
                        else
                        {
                            gameGenres = "Genres: " + gameGenres + "\n";
                        }
                    }

                    //Get the game platforms
                    string gamePlatforms = string.Empty;
                    if (infoGames.platforms != null)
                    {
                        foreach (int platformId in infoGames.platforms)
                        {
                            ApiIGDBPlatforms apiIGDBPlatforms = vApiIGDBPlatforms.Where(x => x.id == platformId).FirstOrDefault();
                            gamePlatforms = AVFunctions.StringAdd(gamePlatforms, apiIGDBPlatforms.name, ",");
                        }
                    }

                    //Check if the summary is empty
                    if (string.IsNullOrWhiteSpace(infoGames.summary))
                    {
                        infoGames.summary = "There is no description available.";
                    }

                    //Add information to summary
                    string gameDescription = gameGenres + gameReleaseDate + infoGames.summary;

                    DataBindString answerDownload = new DataBindString();
                    answerDownload.ImageBitmap = imageAnswer;
                    answerDownload.Name = infoGames.name;
                    answerDownload.NameSub = gamePlatforms;
                    answerDownload.NameDetail = gameReleaseYear;
                    answerDownload.Data1 = gameDescription;
                    answerDownload.Data2 = infoGames.cover;
                    Answers.Add(answerDownload);
                }

                //Get selected result
                DataBindString messageResult = await Popup_Show_MessageBox("Select a found game (" + Answers.Count() + ")", "* Information will be saved in the \"Assets\\Roms\\Downloaded\" folder as:\n" + nameRomSave, "Download image and description for the game:", Answers);
                if (messageResult == null)
                {
                    Debug.WriteLine("No game selected");
                    return null;
                }

                //Create downloaded directory
                AVFiles.Directory_Create("Assets/Roms/Downloaded", false);

                Popup_Show_Status("Download", "Downloading image");
                Debug.WriteLine("Downloading image for: " + nameRom);

                //Get the image url
                BitmapImage downloadedBitmapImage = null;
                string downloadImageId = messageResult.Data2.ToString();
                if (downloadImageId != "0")
                {
                    ApiIGDBImage[] iGDBImages = await ApiIGDBDownloadImage(downloadImageId, "covers");
                    if (iGDBImages == null || !iGDBImages.Any())
                    {
                        Debug.WriteLine("No images found");
                        Popup_Show_Status("Close", "No images found");
                        return null;
                    }

                    //Download and save rom image
                    ApiIGDBImage infoImages = iGDBImages.FirstOrDefault();
                    Uri imageUri = new Uri("https://images.igdb.com/igdb/image/upload/t_720p/" + infoImages.image_id + ".jpg");
                    byte[] imageBytes = await AVDownloader.DownloadByteAsync(5000, "CtrlUI", null, imageUri);
                    if (imageBytes != null && imageBytes.Length > 256)
                    {
                        try
                        {
                            //Save bytes to image file
                            File.WriteAllBytes("Assets/Roms/Downloaded/" + nameRomSave + ".jpg", imageBytes);

                            //Convert bytes to a BitmapImage
                            downloadedBitmapImage = BytesToBitmapImage(imageBytes, imageWidth);

                            Debug.WriteLine("Saved image: " + imageBytes.Length + "bytes/" + imageUri);
                        }
                        catch { }
                    }
                }

                //Save description
                string downloadedDescription = messageResult.Data1.ToString();
                try
                {
                    File.WriteAllText("Assets/Roms/Downloaded/" + nameRomSave + ".txt", downloadedDescription);
                    Debug.WriteLine("Saved description.");
                }
                catch { }

                Popup_Show_Status("Download", "Downloaded information");
                Debug.WriteLine("Downloaded information for: " + nameRom);

                //Return the rom information
                RomInformation romInformationDownloaded = new RomInformation();
                romInformationDownloaded.RomImageBitmap = downloadedBitmapImage;
                romInformationDownloaded.RomDescription = downloadedDescription;
                return romInformationDownloaded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading rom information: " + ex.Message);
                Popup_Show_Status("Close", "Failed downloading");
                return null;
            }
        }

        //Download console information
        public async Task<RomInformation> ConsoleDownloadInformation(string nameConsole, int imageWidth)
        {
            try
            {
                //Filter the console name
                string nameConsoleSave = RomFilterName(nameConsole, true, false, 0);

                //Show the text input popup
                string nameConsoleDownload = await Popup_ShowHide_TextInput("Console search", nameConsoleSave, "Search information for the console", true);
                if (string.IsNullOrWhiteSpace(nameConsoleDownload))
                {
                    Debug.WriteLine("No search term entered.");
                    return null;
                }
                nameConsoleDownload = RomFilterName(nameConsoleDownload, false, true, 0);

                //Search for consoles
                IEnumerable<ApiIGDBPlatforms> iGDBPlatforms = vApiIGDBPlatforms.Where(x => RomFilterName(x.name, false, true, 0).Contains(nameConsoleDownload) || (x.alternative_name != null && RomFilterName(x.alternative_name, false, true, 0).Contains(nameConsoleDownload)));
                if (iGDBPlatforms == null || !iGDBPlatforms.Any())
                {
                    Debug.WriteLine("No consoles found");
                    Popup_Show_Status("Close", "No consoles found");
                    return null;
                }

                //Ask user which console to download
                List<DataBindString> Answers = new List<DataBindString>();
                BitmapImage imageAnswer = FileToBitmapImage(new string[] { "Assets/Icons/Emulator.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                foreach (ApiIGDBPlatforms infoPlatforms in iGDBPlatforms)
                {
                    DataBindString answerDownload = new DataBindString();
                    answerDownload.ImageBitmap = imageAnswer;
                    answerDownload.Name = infoPlatforms.name;
                    answerDownload.NameSub = infoPlatforms.alternative_name;
                    answerDownload.Data1 = infoPlatforms.versions.FirstOrDefault();
                    Answers.Add(answerDownload);
                }

                //Get selected result
                DataBindString messageResult = await Popup_Show_MessageBox("Select a found console (" + Answers.Count() + ")", "* Information will be saved in the \"Assets\\Roms\\Downloaded\" folder as:\n" + nameConsoleSave, "Download image and description for the console:", Answers);
                if (messageResult == null)
                {
                    Debug.WriteLine("No console selected");
                    return null;
                }

                //Create downloaded directory
                AVFiles.Directory_Create("Assets/Roms/Downloaded", false);

                Popup_Show_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading information for: " + nameConsole);

                //Get the platform versions id
                ApiIGDBPlatformVersions[] iGDBPlatformVersions = await ApiIGDBDownloadPlatformVersions(messageResult.Data1.ToString());
                if (iGDBPlatformVersions == null || !iGDBPlatformVersions.Any())
                {
                    Debug.WriteLine("No information found");
                    Popup_Show_Status("Close", "No information found");
                    return null;
                }

                ApiIGDBPlatformVersions targetPlatformVersions = iGDBPlatformVersions.FirstOrDefault();

                Popup_Show_Status("Download", "Downloading image");
                Debug.WriteLine("Downloading image for: " + nameConsole);

                //Get the image url
                BitmapImage downloadedBitmapImage = null;
                string downloadImageId = targetPlatformVersions.platform_logo.ToString();
                if (downloadImageId != "0")
                {
                    ApiIGDBImage[] iGDBImages = await ApiIGDBDownloadImage(downloadImageId, "platform_logos");
                    if (iGDBImages == null || !iGDBImages.Any())
                    {
                        Debug.WriteLine("No images found");
                        Popup_Show_Status("Close", "No images found");
                        return null;
                    }

                    //Download and save image
                    ApiIGDBImage infoImages = iGDBImages.FirstOrDefault();
                    Uri imageUri = new Uri("https://images.igdb.com/igdb/image/upload/t_720p/" + infoImages.image_id + ".png");
                    byte[] imageBytes = await AVDownloader.DownloadByteAsync(5000, "CtrlUI", null, imageUri);
                    if (imageBytes != null && imageBytes.Length > 256)
                    {
                        try
                        {
                            //Save bytes to image file
                            File.WriteAllBytes("Assets/Roms/Downloaded/" + nameConsoleSave + ".png", imageBytes);

                            //Convert bytes to a BitmapImage
                            downloadedBitmapImage = BytesToBitmapImage(imageBytes, imageWidth);

                            Debug.WriteLine("Saved image: " + imageBytes.Length + "bytes/" + imageUri);
                        }
                        catch { }
                    }
                }

                //Check if the summary is empty
                if (string.IsNullOrWhiteSpace(targetPlatformVersions.summary))
                {
                    targetPlatformVersions.summary = "There is no description available.";
                }

                //Save description
                string downloadedDescription = targetPlatformVersions.summary;
                try
                {
                    File.WriteAllText("Assets/Roms/Downloaded/" + nameConsoleSave + ".txt", downloadedDescription);
                    Debug.WriteLine("Saved description.");
                }
                catch { }

                Popup_Show_Status("Download", "Downloaded information");
                Debug.WriteLine("Downloaded information for: " + nameConsole);

                //Return the rom information
                RomInformation romInformationDownloaded = new RomInformation();
                romInformationDownloaded.RomImageBitmap = downloadedBitmapImage;
                romInformationDownloaded.RomDescription = downloadedDescription;
                return romInformationDownloaded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading console information: " + ex.Message);
                Popup_Show_Status("Close", "Failed downloading");
                return null;
            }
        }

        //Download all avaliable games
        public async Task<IEnumerable<ApiIGDBGames>> ApiIGDBDownloadGames(string gameName)
        {
            try
            {
                Debug.WriteLine("Downloading games for: " + gameName);

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestUserKey = new[] { "User-Key", vApiIGDBUserKey };
                string[][] requestHeaders = new string[][] { requestAccept, requestUserKey };

                //Create request uri
                Uri requestUri = new Uri("https://api-v3.igdb.com/games");

                //Create request body
                string requestBodyString = "fields *; limit 100; search \"" + gameName + "\";";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download available games
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading games.");
                    return null;
                }

                //Check if status is set
                if (resultSearch.Contains("\"status\"") && resultSearch.Contains("\"type\""))
                {
                    Debug.WriteLine("Received invalid games data.");
                    return null;
                }

                //Return games sorted
                return JsonConvert.DeserializeObject<IEnumerable<ApiIGDBGames>>(resultSearch).OrderBy(x => x.name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading games: " + ex.Message);
                return null;
            }
        }

        //Download platform version id information
        public async Task<ApiIGDBPlatformVersions[]> ApiIGDBDownloadPlatformVersions(string platformId)
        {
            try
            {
                Debug.WriteLine("Downloading platform versions for: " + platformId);

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestUserKey = new[] { "User-Key", vApiIGDBUserKey };
                string[][] requestHeaders = new string[][] { requestAccept, requestUserKey };

                //Create request uri
                Uri requestUri = new Uri("https://api-v3.igdb.com/platform_versions");

                //Create request body
                string requestBodyString = "fields *; limit 100; where id = " + platformId + ";";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download available platforms
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading platform versions.");
                    return null;
                }

                //Check if status is set
                if (resultSearch.Contains("\"status\"") && resultSearch.Contains("\"type\""))
                {
                    Debug.WriteLine("Received invalid platform versions data.");
                    return null;
                }

                //Return platform versions
                return JsonConvert.DeserializeObject<ApiIGDBPlatformVersions[]>(resultSearch);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading platform versions: " + ex.Message);
                return null;
            }
        }

        //Download image id information
        public async Task<ApiIGDBImage[]> ApiIGDBDownloadImage(string imageId, string imageCategory)
        {
            try
            {
                Debug.WriteLine("Downloading image for: " + imageId);

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestUserKey = new[] { "User-Key", vApiIGDBUserKey };
                string[][] requestHeaders = new string[][] { requestAccept, requestUserKey };

                //Create request uri
                Uri requestUri = new Uri("https://api-v3.igdb.com/" + imageCategory);

                //Create request body
                string requestBodyString = "fields *; limit 100; where id = " + imageId + ";";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download available image
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading image.");
                    return null;
                }

                //Check if status is set
                if (resultSearch.Contains("\"status\"") && resultSearch.Contains("\"type\""))
                {
                    Debug.WriteLine("Received invalid image data.");
                    return null;
                }

                //Return covers
                return JsonConvert.DeserializeObject<ApiIGDBImage[]>(resultSearch);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading image: " + ex.Message);
                return null;
            }
        }
    }
}