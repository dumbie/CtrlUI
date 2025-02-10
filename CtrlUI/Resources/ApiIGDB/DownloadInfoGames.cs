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
        //Download igdb games search
        public async Task<ApiIGDBGames[]> ApiIGDB_DownloadGames_Search(string searchName)
        {
            try
            {
                Debug.WriteLine("Downloading IGDB games for: " + searchName);

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
                Uri requestUri = new Uri("https://api.igdb.com/v4/games");

                //Generate fields string
                string fieldString = GenerateIgdbFieldString(typeof(ApiIGDBGames));

                //Create request body
                string requestBodyString = "fields " + fieldString + "; limit 100; search \"" + igdbSearchName + "\";";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download igdb content
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);

                //Check if string is empty
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

                //Return content
                return JsonConvert.DeserializeObject<ApiIGDBGames[]>(resultSearch).OrderBy(x => x.name).ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading IGDB games: " + ex.Message);
                return null;
            }
        }
    }
}