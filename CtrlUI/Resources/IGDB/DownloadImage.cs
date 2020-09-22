using ArnoldVinkCode;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Download image id information
        public async Task<ApiIGDBImage[]> ApiIGDB_DownloadImage(string imageId, string imageCategory)
        {
            try
            {
                Debug.WriteLine("Downloading image for: " + imageId);

                //Authenticate with Twitch
                string authAccessToken = await Api_Twitch_Authenticate();
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
                Uri requestUri = new Uri("https://api.igdb.com/v4/" + imageCategory);

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