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

                //Create json request
                ApiHltbSearchQuery jsonSearchQuery = new ApiHltbSearchQuery();
                jsonSearchQuery.searchTerms = new[] { gameName };
                string jsonRequest = JsonConvert.SerializeObject(jsonSearchQuery);

                //Create request body
                StringContent requestBodyStringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                //Create request uri
                Uri requestUri = new Uri(apiUrl);

                //Authenticate with Twitch
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading how long to beat, no connection.");
                    return null;
                }

                //Deserialize json string
                ApiHltbSearchResult jsonSearchResult = JsonConvert.DeserializeObject<ApiHltbSearchResult>(resultSearch);
                foreach (ApiHltbSearchResult.Data hltbData in jsonSearchResult.data)
                {
                    Debug.WriteLine(hltbData.game_name);
                    if (hltbData.comp_all_count <= 0)
                    {
                        Debug.WriteLine("Unknown gameplay time");
                    }
                    else
                    {
                        if (hltbData.comp_main > 0)
                        {
                            Debug.WriteLine("Maingame" + AVFunctions.SecondsToHms(hltbData.comp_main, true, false));
                        }
                        else
                        {
                            Debug.WriteLine("MaingameUnknown");
                        }
                        Debug.WriteLine("Main + Side" + AVFunctions.SecondsToHms(hltbData.comp_plus, true, false));
                        Debug.WriteLine("100% done" + AVFunctions.SecondsToHms(hltbData.comp_100, true, false));
                        Debug.WriteLine("Completed " + hltbData.comp_all_count + " times");
                    }
                }

                return jsonSearchResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed searching how long to beat: " + ex.Message);
                return null;
            }
        }
    }
}