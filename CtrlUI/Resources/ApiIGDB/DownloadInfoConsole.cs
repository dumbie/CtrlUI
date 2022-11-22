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
        //Download console information
        public async Task<DownloadInfoConsole> DownloadInfoConsole(string nameConsole, int imageWidth)
        {
            try
            {
                //Filter the name
                string nameConsoleSave = FilterNameGame(nameConsole, true, false, false, 0);

                //Show the text input popup
                string nameConsoleDownload = await Popup_ShowHide_TextInput("Console search", nameConsoleSave, "Search information for the console", true);
                if (string.IsNullOrWhiteSpace(nameConsoleDownload))
                {
                    Debug.WriteLine("No search term entered.");
                    return null;
                }
                nameConsoleDownload = FilterNameGame(nameConsoleDownload, false, true, false, 0);

                //Search for consoles
                IEnumerable<ApiIGDBPlatforms> iGDBPlatforms = vApiIGDBPlatforms.Where(x => FilterNameGame(x.name, false, true, false, 0).Contains(nameConsoleDownload) || (x.alternative_name != null && FilterNameGame(x.alternative_name, false, true, false, 0).Contains(nameConsoleDownload)));
                if (iGDBPlatforms == null || !iGDBPlatforms.Any())
                {
                    Debug.WriteLine("No consoles found");
                    await Notification_Send_Status("Close", "No consoles found");
                    return null;
                }

                //Ask user which console to download
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
                DataBindString messageResult = await Popup_Show_MessageBox("Select a found console (" + Answers.Count() + ")", "* Information will be saved in the \"Assets\\User\\Games\\Downloaded\" folder as:\n" + nameConsoleSave, "Download image and description for the console:", Answers);
                if (messageResult == null)
                {
                    Debug.WriteLine("No console selected");
                    return null;
                }

                //Create downloaded directory
                AVFiles.Directory_Create("Assets/User/Games/Downloaded", false);

                //Convert result back to json
                ApiIGDBPlatforms selectedConsole = (ApiIGDBPlatforms)messageResult.Data1;

                await Notification_Send_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading information for: " + nameConsole);

                //Get the platform versions id
                string firstPlatformId = selectedConsole.versions.FirstOrDefault().ToString();
                ApiIGDBPlatformVersions[] iGDBPlatformVersions = await ApiIGDBDownloadPlatformVersions(firstPlatformId);
                if (iGDBPlatformVersions == null || !iGDBPlatformVersions.Any())
                {
                    Debug.WriteLine("No information found");
                    await Notification_Send_Status("Close", "No information found");
                    return null;
                }

                ApiIGDBPlatformVersions targetPlatformVersions = iGDBPlatformVersions.FirstOrDefault();

                await Notification_Send_Status("Download", "Downloading image");
                Debug.WriteLine("Downloading image for: " + nameConsole);

                //Get the image url
                BitmapImage downloadedBitmapImage = null;
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
                        Debug.WriteLine("No console images found.");
                        await Notification_Send_Status("Close", "No console images found");
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
                            File.WriteAllBytes("Assets/User/Games/Downloaded/" + nameConsoleSave + ".png", imageBytes);
                            Debug.WriteLine("Saved image: " + imageBytes.Length + "bytes/" + imageUri);
                        }
                        catch { }
                    }
                }

                //Json settings
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                //Json serialize
                string serializedObject = JsonConvert.SerializeObject(targetPlatformVersions, jsonSettings);

                //Save json information
                File.WriteAllText("Assets/User/Games/Downloaded/" + nameConsoleSave + ".json", serializedObject);

                await Notification_Send_Status("Download", "Downloaded information");
                Debug.WriteLine("Downloaded and saved information for: " + nameConsole);

                //Return the information
                DownloadInfoConsole downloadInfo = new DownloadInfoConsole();
                downloadInfo.ImageBitmap = downloadedBitmapImage;
                downloadInfo.Details = targetPlatformVersions;
                return downloadInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading console information: " + ex.Message);
                await Notification_Send_Status("Close", "Failed downloading");
                return null;
            }
        }

        //Download platform version id information
        public async Task<ApiIGDBPlatformVersions[]> ApiIGDBDownloadPlatformVersions(string platformId)
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