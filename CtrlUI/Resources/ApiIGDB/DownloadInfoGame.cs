using ArnoldVinkCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Download game information
        public async Task<DownloadInfoGame> DownloadInfoGame(string nameGame, string nameEmulatorPlatform, int imageWidth, bool downloadImage, bool useCache)
        {
            try
            {
                //Filter the name
                string namePlatformSave = FilterNameFile(nameEmulatorPlatform);
                string nameGameSave = FilterNameFile(nameGame);
                string nameGameSearch = FilterNameGame(nameGame, true, false, true, 0);
                string userSaveDirectory = string.Empty;
                string defaultDirectory = string.Empty;
                if (!string.IsNullOrWhiteSpace(nameEmulatorPlatform))
                {
                    userSaveDirectory = "Assets/User/Emulators/" + namePlatformSave + "/";
                    defaultDirectory = "Assets/Default/Emulators/" + namePlatformSave + "/";
                }
                else
                {
                    userSaveDirectory = "Assets/User/Games/";
                    defaultDirectory = "Assets/Default/Games/";
                }

                //Load and return cached
                if (useCache)
                {
                    //Create return object
                    DownloadInfoGame cacheInfo = new DownloadInfoGame();

                    //Load details and summary
                    string jsonFile = FileToString(new string[] { userSaveDirectory + nameGameSave + ".json", defaultDirectory + nameGameSave + ".json" });
                    if (!string.IsNullOrWhiteSpace(jsonFile))
                    {
                        cacheInfo.Details = JsonConvert.DeserializeObject<ApiIGDBGames>(jsonFile);
                        cacheInfo.Summary = ApiIGDB_GameSummaryString(cacheInfo.Details);
                    }
                    else
                    {
                        cacheInfo.Summary = "There is no description available.";
                    }

                    //Load bitmap image
                    cacheInfo.ImageBitmap = FileToBitmapImage(new string[] { userSaveDirectory + "Platform.png", defaultDirectory + "Platform.png" }, null, null, IntPtr.Zero, imageWidth, 0);

                    //Return the information
                    return cacheInfo;
                }

                //Show the text input popup
                string nameDownload = await Popup_ShowHide_TextInput("Game search", nameGameSearch, "Search information for the game", true);
                if (string.IsNullOrWhiteSpace(nameDownload))
                {
                    Debug.WriteLine("No search term entered.");
                    return null;
                }
                nameDownload = nameDownload.ToLower();

                await Notification_Send_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading information for: " + nameGame);

                //Download available games
                ApiIGDBGames[] iGDBGames = await ApiIGDB_DownloadGamesSearch(nameDownload);
                if (iGDBGames == null || !iGDBGames.Any())
                {
                    Debug.WriteLine("No games found for: " + nameGame);
                    await Notification_Send_Status("Close", "No games found");
                    return null;
                }

                //Ask user which game to download
                List<DataBindString> Answers = new List<DataBindString>();
                foreach (ApiIGDBGames infoGames in iGDBGames)
                {
                    //Check if cover and summary is available
                    if (infoGames.cover == 0 && string.IsNullOrWhiteSpace(infoGames.summary)) { continue; }

                    //Release date
                    ApiIGDB_ReleaseDateToString(infoGames, out string gameReleaseDate, out string gameReleaseYear);

                    //Game platforms
                    ApiIGDB_PlatformsToString(infoGames, out string gamePlatforms);

                    DataBindString answerDownload = new DataBindString();
                    answerDownload.ImageBitmap = vImagePreloadGame;
                    answerDownload.Name = infoGames.name;
                    answerDownload.NameSub = gamePlatforms;
                    answerDownload.NameDetail = gameReleaseYear;
                    answerDownload.Data1 = infoGames;
                    Answers.Add(answerDownload);
                }

                //Get selected result
                DataBindString messageResult = await Popup_Show_MessageBox("Select a found game (" + Answers.Count() + ")", "* Information will be saved in the '" + userSaveDirectory + "' folder as:\n" + nameGameSave, "Download image and description for the game:", Answers);
                if (messageResult == null)
                {
                    Debug.WriteLine("No game selected");
                    return null;
                }

                //Create downloaded directory
                AVFiles.Directory_Create(userSaveDirectory, false);

                //Convert result back to json
                ApiIGDBGames selectedGame = (ApiIGDBGames)messageResult.Data1;

                //Download and save image
                BitmapImage downloadedBitmapImage = null;
                if (downloadImage)
                {
                    await Notification_Send_Status("Download", "Downloading image");
                    Debug.WriteLine("Downloading image for: " + nameGame);

                    //Get the image url
                    string downloadImageId = selectedGame.cover.ToString();
                    if (downloadImageId != "0")
                    {
                        ApiIGDBImage[] iGDBImages = await ApiIGDB_DownloadImage(downloadImageId, "covers");
                        if (iGDBImages == null)
                        {
                            Debug.WriteLine("Failed to download images.");
                            await Notification_Send_Status("Close", "Failed downloading images");
                            return null;
                        }
                        else if (!iGDBImages.Any())
                        {
                            Debug.WriteLine("No game images found.");
                            await Notification_Send_Status("Close", "No game images found");
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
                                //Convert bytes to a BitmapImage
                                downloadedBitmapImage = BytesToBitmapImage(imageBytes, imageWidth);

                                //Save bytes to image file
                                File.WriteAllBytes(userSaveDirectory + nameGameSave + ".png", imageBytes);

                                Debug.WriteLine("Saved image: " + imageBytes.Length + "bytes/" + imageUri);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No game images found.");
                        await Notification_Send_Status("Close", "No game images found");
                        return null;
                    }
                }

                //Json settings
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                //Json serialize
                string serializedObject = JsonConvert.SerializeObject(selectedGame, jsonSettings);

                //Save json information
                File.WriteAllText(userSaveDirectory + nameGameSave + ".json", serializedObject);

                await Notification_Send_Status("Download", "Downloaded information");
                Debug.WriteLine("Downloaded and saved information for: " + nameGame);

                //Return the information
                DownloadInfoGame downloadInfo = new DownloadInfoGame();
                downloadInfo.ImageBitmap = downloadedBitmapImage;
                downloadInfo.Details = selectedGame;
                downloadInfo.Summary = ApiIGDB_GameSummaryString(downloadInfo.Details);
                return downloadInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading game information: " + ex.Message);
                await Notification_Send_Status("Close", "Failed downloading game");
                return null;
            }
        }

        //Download games by search
        public async Task<ApiIGDBGames[]> ApiIGDB_DownloadGamesSearch(string searchName)
        {
            try
            {
                Debug.WriteLine("Downloading games for: " + searchName);

                //Authenticate with Twitch
                string authAccessToken = await ApiTwitch_Authenticate();
                if (string.IsNullOrWhiteSpace(authAccessToken))
                {
                    return null;
                }

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestClientID = new[] { "Client-ID", vApiIGDBClientID };
                string[] requestAuthorization = new[] { "Authorization", "Bearer " + authAccessToken };
                string[][] requestHeaders = new string[][] { requestAccept, requestClientID, requestAuthorization };

                //Create request uri
                Uri requestUri = new Uri("https://api.igdb.com/v4/games");

                //Create request body
                string requestBodyString = "fields *; limit 100; search \"" + searchName + "\";";
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
                return JsonConvert.DeserializeObject<ApiIGDBGames[]>(resultSearch).OrderBy(x => x.name).ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading games: " + ex.Message);
                return null;
            }
        }
    }
}