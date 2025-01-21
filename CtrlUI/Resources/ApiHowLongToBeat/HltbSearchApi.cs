using ArnoldVinkCode;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Search and download game information
        public async Task<ApiHltbSearchResultApi> ApiHowLongToBeat_Search_Api(string gameIdentifier)
        {
            try
            {
                Debug.WriteLine("Searching how long to beat game: " + gameIdentifier);

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestReferer = new[] { "Referer", "https://howlongtobeat.com" };
                string[] requestAuthorization = new[] { "Authorization", "Basic bWljcm9zb2Z0X3N0b3JlZnJvbnQ6MmZhMDcxNDU2ZDJhZDNiNmI2M2Y2N2Q4NGVhYzcwNTY=" };
                string[][] requestHeaders = new string[][] { requestAccept, requestReferer, requestAuthorization };

                //Set download url
                //string apiUrl = "https://howlongtobeat.com/___api/games?all";
                //string apiUrl = "https://howlongtobeat.com/___api/games?popular";
                string apiUrl = "https://howlongtobeat.com/___api/games?id=4247";
                //string apiUrl = "https://howlongtobeat.com/___api/games?steam_id=70";
                //string apiUrl = "https://howlongtobeat.com/___api/games?xbox_id=9WZDNCRFHWD2";
                //string apiUrl = "https://howlongtobeat.com/___api/games?ign_id=bf67619f-8604-4be9-a7b2-deff8821cdb0";

                //Download how long to beat results
                string resultSearch = await AVDownloader.DownloadStringAsync(5000, "CtrlUI", requestHeaders, new Uri(apiUrl));
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading how long to beat, no connection.");
                    return null;
                }

                //Deserialize json string
                return JsonConvert.DeserializeObject<ApiHltbSearchResultApi>(resultSearch);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed searching how long to beat: " + ex.Message);
                return null;
            }
        }
    }
}