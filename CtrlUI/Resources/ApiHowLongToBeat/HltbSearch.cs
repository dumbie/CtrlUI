using ArnoldVinkCode;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Search and download game information
        public async Task<ApiHltbSearchResult> ApiHowLongToBeat_Search(string gameName)
        {
            try
            {
                Debug.WriteLine("Searching how long to beat game: " + gameName);

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestReferer = new[] { "Referer", "https://howlongtobeat.com" };
                //Xbox string[] requestAuthorization = new[] { "Authorization", "Basic bWljcm9zb2Z0X3N0b3JlZnJvbnQ6MmZhMDcxNDU2ZDJhZDNiNmI2M2Y2N2Q4NGVhYzcwNTY=" };
                string[][] requestHeaders = new string[][] { requestAccept, requestReferer };

                //Set download uri
                //Xbox string apiUrl = "https://howlongtobeat.com/___api/games?xbox_id=9WZDNCRFHWD2";
                string apiUrl = "https://www.howlongtobeat.com/api/search";

                //Split name and remove characters
                string[] searchFilterTerms = gameName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //Create json request
                ApiHltbSearchQuery jsonSearchQuery = new ApiHltbSearchQuery();
                jsonSearchQuery.searchTerms = searchFilterTerms;
                string jsonRequest = JsonConvert.SerializeObject(jsonSearchQuery);

                //Create request body
                StringContent requestBodyStringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                //Download howlongtobeat results
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, new Uri(apiUrl), requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading how long to beat, no connection.");
                    return null;
                }

                //Deserialize json string
                return JsonConvert.DeserializeObject<ApiHltbSearchResult>(resultSearch);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed searching how long to beat: " + ex.Message);
                return null;
            }
        }
    }
}