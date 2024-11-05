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
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVJsonFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Download platform information
        public async Task<DownloadInfoPlatform> DownloadInfoPlatform(string namePlatform, int imageWidth, int imageHeight, bool downloadImage, bool useCache)
        {
            try
            {
                //Filter the name
                string namePlatformSave = AVFiles.FileNameReplaceInvalidChars(namePlatform, string.Empty);
                string userSaveDirectory = "Assets/User/Emulators/" + namePlatformSave + "/";
                string defaultDirectory = "Assets/Default/Emulators/" + namePlatformSave + "/";

                //Load and return cached
                if (useCache)
                {
                    //Create return object
                    DownloadInfoPlatform cacheInfo = new DownloadInfoPlatform();

                    //Load details and summary
                    string jsonFile = FileToString(new string[] { userSaveDirectory + "Platform.json", defaultDirectory + "Platform.json" });
                    if (!string.IsNullOrWhiteSpace(jsonFile))
                    {
                        cacheInfo.Details = JsonConvert.DeserializeObject<ApiIGDBPlatformVersions>(jsonFile);
                        cacheInfo.Summary = ApiIGDB_PlatformSummaryString(cacheInfo.Details);
                    }
                    else
                    {
                        cacheInfo.Summary = "There is no description available.";
                    }

                    //Load bitmap image
                    cacheInfo.ImageBitmap = FileToBitmapImage(new string[] { userSaveDirectory + "Platform.png", defaultDirectory + "Platform.png" }, null, null, imageWidth, imageHeight, IntPtr.Zero, 0);

                    //Return the information
                    return cacheInfo;
                }

                //Show the text input popup
                string nameDownload = await Popup_ShowHide_TextInput("Platform search", namePlatformSave, "Search information for the platform", true);
                if (string.IsNullOrWhiteSpace(nameDownload))
                {
                    Debug.WriteLine("No search term entered.");
                    return null;
                }
                nameDownload = nameDownload.ToLower();

                //Search for platforms
                IEnumerable<ApiIGDBPlatforms> iGDBPlatforms = vApiIGDBPlatforms.Where(x => StringMatch(x.name, nameDownload, true) || StringMatch(x.alternative_name, nameDownload, true));
                if (iGDBPlatforms == null || !iGDBPlatforms.Any())
                {
                    Debug.WriteLine("No platforms found for: " + namePlatform);
                    await Notification_Send_Status("Close", "No platforms found");
                    return null;
                }

                //Ask user which platform to download
                List<DataBindString> Answers = new List<DataBindString>();
                foreach (ApiIGDBPlatforms infoPlatforms in iGDBPlatforms)
                {
                    DataBindString answerDownload = new DataBindString();
                    answerDownload.ImageBitmap = vImagePreloadEmulator;
                    answerDownload.Name = infoPlatforms.name;
                    answerDownload.NameSub = infoPlatforms.alternative_name;
                    answerDownload.Data1 = infoPlatforms;
                    Answers.Add(answerDownload);
                }

                //Get selected result
                DataBindString messageResult = await Popup_Show_MessageBox("Select a found platform (" + Answers.Count() + ")", "* Information will be saved in the '" + userSaveDirectory + "' folder", "Download image and description for the platform:", Answers);
                if (messageResult == null)
                {
                    Debug.WriteLine("No platform selected");
                    return null;
                }

                //Create downloaded directory
                AVFiles.Directory_Create(userSaveDirectory, false);

                //Convert result back to json
                ApiIGDBPlatforms selectedPlatform = (ApiIGDBPlatforms)messageResult.Data1;

                await Notification_Send_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading information for: " + namePlatform);

                //Get the platform versions id
                string firstPlatformId = selectedPlatform.versions.FirstOrDefault().ToString();
                ApiIGDBPlatformVersions[] iGDBPlatformVersions = await ApiIGDB_DownloadPlatformVersions(firstPlatformId);
                if (iGDBPlatformVersions == null || !iGDBPlatformVersions.Any())
                {
                    Debug.WriteLine("No information found");
                    await Notification_Send_Status("Close", "No information found");
                    return null;
                }
                ApiIGDBPlatformVersions targetPlatformVersions = iGDBPlatformVersions.FirstOrDefault();

                //Download and save image
                BitmapImage downloadedBitmapImage = null;
                if (downloadImage)
                {
                    await Notification_Send_Status("Download", "Downloading image");
                    Debug.WriteLine("Downloading image for: " + namePlatform);

                    //Get the image url
                    string downloadImageId = targetPlatformVersions.platform_logo.ToString();
                    if (downloadImageId != "0")
                    {
                        ApiIGDBImage[] iGDBImages = await ApiIGDB_DownloadImage(downloadImageId, "platform_logos");
                        if (iGDBImages == null)
                        {
                            Debug.WriteLine("Failed to download images.");
                            await Notification_Send_Status("Close", "Failed downloading images");
                            return null;
                        }
                        else if (!iGDBImages.Any())
                        {
                            Debug.WriteLine("No platform images found.");
                            await Notification_Send_Status("Close", "No platform images found");
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
                                downloadedBitmapImage = BytesToBitmapImage(imageBytes, imageWidth, imageHeight);

                                //Save bytes to image file
                                File.WriteAllBytes(userSaveDirectory + "Platform.png", imageBytes);

                                Debug.WriteLine("Saved platform image: " + imageBytes.Length + "bytes/" + imageUri);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No platform images found.");
                        await Notification_Send_Status("Close", "No platform images found");
                        return null;
                    }
                }

                //Save object to json
                JsonSaveObject(targetPlatformVersions, userSaveDirectory + "Platform.json");

                await Notification_Send_Status("Download", "Downloaded information");
                Debug.WriteLine("Downloaded and saved information for: " + namePlatform);

                //Return the information
                DownloadInfoPlatform downloadInfo = new DownloadInfoPlatform();
                downloadInfo.ImageBitmap = downloadedBitmapImage;
                downloadInfo.Details = targetPlatformVersions;
                downloadInfo.Summary = ApiIGDB_PlatformSummaryString(downloadInfo.Details);
                return downloadInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading platform information: " + ex.Message);
                await Notification_Send_Status("Close", "Failed downloading platform");
                return null;
            }
        }

        //Download platforms by search
        public async Task<ApiIGDBPlatforms[]> ApiIGDB_DownloadPlatformsSearch(string searchName)
        {
            try
            {
                Debug.WriteLine("Downloading platforms for: " + searchName);

                //Replace spaces with asterisk
                string igdbSearchName = string.Empty;
                if (searchName.Count(char.IsWhiteSpace) > 1)
                {
                    igdbSearchName = searchName.Replace(" ", "*");
                }
                else
                {
                    igdbSearchName = searchName;
                }

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
                Uri requestUri = new Uri("https://api.igdb.com/v4/platforms");

                //Create request body
                string requestBodyString = "fields *; limit 100; search \"" + igdbSearchName + "\";";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download available platforms
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading platforms.");
                    return null;
                }

                //Check if status is set
                if (resultSearch.Contains("\"status\"") && resultSearch.Contains("\"type\""))
                {
                    Debug.WriteLine("Received invalid platforms data.");
                    return null;
                }

                //Return platforms sorted
                return JsonConvert.DeserializeObject<ApiIGDBPlatforms[]>(resultSearch).OrderBy(x => x.name).ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading platforms: " + ex.Message);
                return null;
            }
        }

        //Download platform version id information
        public async Task<ApiIGDBPlatformVersions[]> ApiIGDB_DownloadPlatformVersions(string platformId)
        {
            try
            {
                Debug.WriteLine("Downloading platform versions for: " + platformId);

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
                Uri requestUri = new Uri("https://api.igdb.com/v4/platform_versions");

                //Create request body
                string requestBodyString = "fields *; limit 100; where id = " + platformId + ";";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download available platform versions
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
    }
}