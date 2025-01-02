using ArnoldVinkCode;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update api authentication key
        public async Task<bool> ApiHowLongToBeat_UpdateAuthKey()
        {
            try
            {
                string[][] requestHeaders = new string[][] { };

                //Download site html
                string apiUrl = "https://howlongtobeat.com";
                string resultHtml = await AVDownloader.DownloadStringAsync(5000, "CtrlUI", requestHeaders, new Uri(apiUrl));

                //Extract app json url
                Match regExAppJson = Regex.Match(resultHtml, "/_next/static/chunks/pages/_app-(.*?).js");
                string urlAppJson = regExAppJson.Groups[0].Value;

                //Download app json
                apiUrl += urlAppJson;
                string resultAppJson = await AVDownloader.DownloadStringAsync(5000, "CtrlUI", requestHeaders, new Uri(apiUrl));

                //Extract api auth key
                string[] apiSearchNames = ["search", "find", "lookup"];
                foreach (string searchName in apiSearchNames)
                {
                    Match regExApiAuthKey = Regex.Match(resultAppJson, "fetch\\(\"/api/" + searchName + "/\".concat\\(\"(.*?)\"\\).concat\\(\"(.*?)\"\\)");
                    if (regExApiAuthKey.Success)
                    {
                        vApiHltbSearchName = searchName;
                        vApiHltbAuthKey = regExApiAuthKey.Groups[1].Value + regExApiAuthKey.Groups[2].Value;
                        Debug.WriteLine("Updated how long to beat api key: " + vApiHltbAuthKey);
                        return true;
                    }
                }

                Debug.WriteLine("Failed to update how long to beat api key: empty key.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update how long to beat api key: " + ex.Message);
                return false;
            }
        }

        //Search and download game information
        public async Task<ApiHltbSearchResult> ApiHowLongToBeat_Search(string gameName)
        {
            try
            {
                Debug.WriteLine("Searching how long to beat game: " + gameName);

                //Check authentication key
                //Fix check if auth key expired
                if (string.IsNullOrWhiteSpace(vApiHltbAuthKey))
                {
                    if (!await ApiHowLongToBeat_UpdateAuthKey())
                    {
                        return null;
                    }
                }

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestReferer = new[] { "Referer", "https://howlongtobeat.com" };
                //Xbox string[] requestAuthorization = new[] { "Authorization", "Basic bWljcm9zb2Z0X3N0b3JlZnJvbnQ6MmZhMDcxNDU2ZDJhZDNiNmI2M2Y2N2Q4NGVhYzcwNTY=" };
                string[][] requestHeaders = new string[][] { requestAccept, requestReferer };

                //Set download url
                //Xbox string apiUrl = "https://howlongtobeat.com/___api/games?xbox_id=9WZDNCRFHWD2";
                string apiUrl = "https://howlongtobeat.com/api/" + vApiHltbSearchName + "/" + vApiHltbAuthKey;

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