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
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
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

                //Fix roms starting with number / manual search
            }
            catch { }
            return nameRom;
        }

        //Download rom information
        public async Task<RomInformation> RomDownloadInformation(string nameRom, int wordsSearch, int imageWidth)
        {
            try
            {
                Popup_Show_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading rom information for: " + nameRom);

                //Filter the rom name
                string nameRomSave = RomFilterName(nameRom, true, false, 0);
                string nameRomDownload = RomFilterName(nameRom, true, false, wordsSearch);

                //Download available games
                IEnumerable<ApiIGDBGames> iGDBGames = await ApiIGDBDownloadGames(nameRomDownload);
                if (iGDBGames == null || !iGDBGames.Any())
                {
                    //Reduce search words by one
                    if (wordsSearch > 1)
                    {
                        return await RomDownloadInformation(nameRom, wordsSearch - 1, imageWidth);
                    }

                    //No game found notification
                    Debug.WriteLine("No games found");
                    Popup_Show_Status("Close", "No games found");
                    return null;
                }

                //Ask user which game to download
                CultureInfo cultureInfo = new CultureInfo("en-US");
                List<DataBindString> Answers = new List<DataBindString>();
                BitmapImage gameImage = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Game.png" }, IntPtr.Zero, -1, 0);
                foreach (ApiIGDBGames infoGames in iGDBGames)
                {
                    if (infoGames.cover == 0) { continue; }
                    if (infoGames.summary == string.Empty) { continue; }

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

                    DataBindString answerGame = new DataBindString();
                    answerGame.ImageBitmap = gameImage;
                    answerGame.Name = infoGames.name;
                    answerGame.NameSub = gamePlatforms;
                    answerGame.NameDetail = gameReleaseYear;
                    answerGame.Data1 = gameDescription;
                    answerGame.Data2 = infoGames.cover;
                    Answers.Add(answerGame);
                }

                //Get selected result
                DataBindString messageResult = await Popup_Show_MessageBox("Select a found game (" + iGDBGames.Count() + ")", "* Information will be saved in the \"Assets\\Roms\\Downloaded\" folder as:\n" + nameRomSave, "Download image and description for the game:", Answers);
                if (messageResult == null)
                {
                    Debug.WriteLine("No game selected");
                    return null;
                }

                Popup_Show_Status("Download", "Downloading cover");
                Debug.WriteLine("Downloading rom cover for: " + nameRom);

                //Get the image url
                ApiIGDBCovers[] iGDBCovers = await ApiIGDBDownloadCovers(messageResult.Data2.ToString());
                if (iGDBCovers == null || !iGDBCovers.Any())
                {
                    Debug.WriteLine("No covers found");
                    Popup_Show_Status("Close", "No covers found");
                    return null;
                }

                //Create downloaded directory
                AVFiles.Directory_Create("Assets\\Roms\\Downloaded\\", false);

                //Download and save rom cover
                ApiIGDBCovers infoCovers = iGDBCovers.FirstOrDefault();
                Uri imageUri = new Uri("https://images.igdb.com/igdb/image/upload/t_720p/" + infoCovers.image_id + ".jpg");
                byte[] imageBytes = await AVDownloader.DownloadByteAsync(5000, "CtrlUI", null, imageUri);
                BitmapImage romBitmapImage = null;
                if (imageBytes != null && imageBytes.Length > 256)
                {
                    try
                    {
                        //Save bytes to image file
                        File.WriteAllBytes("Assets\\Roms\\Downloaded\\" + nameRomSave + ".jpg", imageBytes);

                        //Convert bytes to a BitmapImage
                        romBitmapImage = BytesToBitmapImage(imageBytes, imageWidth);

                        Debug.WriteLine("Saved rom cover: " + imageBytes.Length + "bytes/" + imageUri);
                    }
                    catch { }
                }

                //Save rom description
                string romDescription = messageResult.Data1.ToString();
                try
                {
                    File.WriteAllText("Assets\\Roms\\Downloaded\\" + nameRomSave + ".txt", romDescription);
                    Debug.WriteLine("Saved rom description.");
                }
                catch { }

                Popup_Show_Status("Download", "Downloaded information");
                Debug.WriteLine("Downloaded rom information for: " + nameRom);

                //Return the rom information
                RomInformation romInformationDownloaded = new RomInformation();
                romInformationDownloaded.RomImageBitmap = romBitmapImage;
                romInformationDownloaded.RomDescription = romDescription;
                return romInformationDownloaded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading rom information: " + ex.Message);
                Popup_Show_Status("Close", "Failed downloading");
                return null;
            }
        }

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
                Uri requestUri = new Uri("https://api-v3.igdb.com/games/");

                //Create request body
                string requestBodyString = "fields *; limit 80; search \"" + gameName + "\";";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download games
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading games.");
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

        public async Task<ApiIGDBCovers[]> ApiIGDBDownloadCovers(string coverId)
        {
            try
            {
                Debug.WriteLine("Downloading covers for: " + coverId);

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestUserKey = new[] { "User-Key", vApiIGDBUserKey };
                string[][] requestHeaders = new string[][] { requestAccept, requestUserKey };

                //Create request uri
                Uri requestUri = new Uri("https://api-v3.igdb.com/covers/");

                //Create request body
                string requestBodyString = "fields *; limit 80; where id = " + coverId + ";";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download covers
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading covers.");
                    return null;
                }

                //Return covers
                return JsonConvert.DeserializeObject<ApiIGDBCovers[]>(resultSearch);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading covers: " + ex.Message);
                return null;
            }
        }
    }
}