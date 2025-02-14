using ArnoldVinkCode;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Download igdb platforms search
        public async Task<ApiIGDBPlatforms[]> ApiIGDB_DownloadPlatforms_Search(string searchName)
        {
            try
            {
                Debug.WriteLine("Downloading IGDB platforms for: " + searchName);

                //Generate where search string
                string[] searchSplitted = searchName.Split(' ');
                string whereString = "where " + AVFunctions.StringJoin(searchSplitted, " | ", "name ~ *\"", "\"*") + ";";

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

                //Generate fields string
                string fieldString = GenerateIgdbFieldString(typeof(ApiIGDBPlatforms));

                //Create request body
                string requestBodyString = "fields " + fieldString + "; limit 100; " + whereString;
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download igdb content
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);

                //Check if string is empty
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

                //Return content
                return JsonConvert.DeserializeObject<ApiIGDBPlatforms[]>(resultSearch).OrderBy(x => x.name).ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading IGDB platforms: " + ex.Message);
                return null;
            }
        }
    }
}