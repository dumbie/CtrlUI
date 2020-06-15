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
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Download game information
        public async Task<DownloadInfoGame> DownloadInfoGame(string nameRom, int imageWidth)
        {
            try
            {
                //Filter the name
                string nameRomSave = FileFilterName(nameRom, true, false, 0);

                //Show the text input popup
                string nameRomDownload = await Popup_ShowHide_TextInput("Game search", nameRomSave, "Search information for the game", true);
                if (string.IsNullOrWhiteSpace(nameRomDownload))
                {
                    Debug.WriteLine("No search term entered.");
                    return null;
                }
                nameRomDownload = nameRomDownload.ToLower();

                await Notification_Send_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading information for: " + nameRom);

                //Download available games
                IEnumerable<ApiIGDBGames> iGDBGames = await ApiIGDB_DownloadGames(nameRomDownload);
                if (iGDBGames == null || !iGDBGames.Any())
                {
                    Debug.WriteLine("No games found");
                    await Notification_Send_Status("Close", "No games found");
                    return null;
                }

                //Ask user which game to download
                List<DataBindString> Answers = new List<DataBindString>();
                BitmapImage imageAnswer = FileToBitmapImage(new string[] { "Assets/Icons/Game.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                foreach (ApiIGDBGames infoGames in iGDBGames)
                {
                    //Check if information is available
                    if (infoGames.cover == 0 && string.IsNullOrWhiteSpace(infoGames.summary)) { continue; }

                    //Release date
                    string gameReleaseDate = string.Empty;
                    string gameReleaseYear = string.Empty;
                    ApiIGDB_ReleaseDateToString(infoGames, out gameReleaseDate, out gameReleaseYear);

                    //Game platforms
                    string gamePlatforms = string.Empty;
                    if (infoGames.platforms != null)
                    {
                        foreach (int platformId in infoGames.platforms)
                        {
                            ApiIGDBPlatforms apiIGDBPlatforms = vApiIGDBPlatforms.Where(x => x.id == platformId).FirstOrDefault();
                            gamePlatforms = AVFunctions.StringAdd(gamePlatforms, apiIGDBPlatforms.name, ",");
                        }
                    }

                    DataBindString answerDownload = new DataBindString();
                    answerDownload.ImageBitmap = imageAnswer;
                    answerDownload.Name = infoGames.name;
                    answerDownload.NameSub = gamePlatforms;
                    answerDownload.NameDetail = gameReleaseYear;
                    answerDownload.Data1 = infoGames;
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

                //Convert result back to json
                ApiIGDBGames selectedGame = (ApiIGDBGames)messageResult.Data1;

                await Notification_Send_Status("Download", "Downloading image");
                Debug.WriteLine("Downloading image for: " + nameRom);

                //Get the image url
                BitmapImage downloadedBitmapImage = null;
                string downloadImageId = selectedGame.cover.ToString();
                if (downloadImageId != "0")
                {
                    ApiIGDBImage[] iGDBImages = await ApiIGDB_DownloadImage(downloadImageId, "covers");
                    if (iGDBImages == null || !iGDBImages.Any())
                    {
                        Debug.WriteLine("No images found");
                        await Notification_Send_Status("Close", "No images found");
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
                            File.WriteAllBytes("Assets/Roms/Downloaded/" + nameRomSave + ".png", imageBytes);
                            Debug.WriteLine("Saved image: " + imageBytes.Length + "bytes/" + imageUri);
                        }
                        catch { }
                    }
                }

                //Json settings
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                //Json serialize
                string serializedObject = JsonConvert.SerializeObject(selectedGame, jsonSettings);

                //Save json information
                File.WriteAllText("Assets/Roms/Downloaded/" + nameRomSave + ".json", serializedObject);

                await Notification_Send_Status("Download", "Downloaded information");
                Debug.WriteLine("Downloaded and saved information for: " + nameRom);

                //Return the information
                DownloadInfoGame downloadInfo = new DownloadInfoGame();
                downloadInfo.ImageBitmap = downloadedBitmapImage;
                downloadInfo.Details = selectedGame;
                return downloadInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading game information: " + ex.Message);
                await Notification_Send_Status("Close", "Failed downloading");
                return null;
            }
        }

        //Download all available games
        public async Task<IEnumerable<ApiIGDBGames>> ApiIGDB_DownloadGames(string gameName)
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
    }
}