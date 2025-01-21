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
        private async Task<bool> ApiHowLongToBeat_UpdateAuthKey_Html()
        {
            try
            {
                string[][] requestHeaders = new string[][] { };

                //Download site html
                string urlSite = "https://howlongtobeat.com";
                string resultHtml = await AVDownloader.DownloadStringAsync(5000, "CtrlUI", requestHeaders, new Uri(urlSite));

                //Extract app json url
                Match regExAppJson = Regex.Match(resultHtml, "/_next/static/chunks/pages/_app-(.*?).js");
                string urlAppJson = regExAppJson.Groups[0].Value;

                //Download app json
                urlSite += urlAppJson;
                string resultAppJson = await AVDownloader.DownloadStringAsync(5000, "CtrlUI", requestHeaders, new Uri(urlSite));

                //Extract api auth fetch
                string authFetch = string.Empty;
                Match regExApiAuthFetch = Regex.Match(resultAppJson, ".*(fetch\\()(.*?)(stringify\\({searchType:)");
                authFetch = regExApiAuthFetch.Groups[2].Value;

                //Extract api auth name
                string authName = string.Empty;
                Match regExApiAuthName = Regex.Match(authFetch, "(/api/)(.*?)(/)");
                authName = regExApiAuthName.Groups[2].Value;
                vApiHltbSearchName = authName;

                //Extract api auth key
                string authKey = string.Empty;
                MatchCollection regExApiAuthKey = Regex.Matches(authFetch, ".concat\\(\"(.*?)\"\\)");
                foreach (Match match in regExApiAuthKey)
                {
                    authKey += match.Groups[1].Value;
                }

                //Check api auth key
                if (string.IsNullOrWhiteSpace(authKey))
                {
                    Debug.WriteLine("Failed to update how long to beat api key: empty key.");
                    return false;
                }
                else
                {
                    vApiHltbAuthDateTime = DateTime.Now;
                    vApiHltbAuthKey = authKey;
                    Debug.WriteLine("Updated how long to beat api key: " + vApiHltbSearchName + " / " + vApiHltbAuthKey);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update how long to beat api key: " + ex.Message);
                return false;
            }
        }

        //Search and download game information
        public async Task<ApiHltbSearchResultHtml> ApiHowLongToBeat_Search_Html(string gameName)
        {
            try
            {
                Debug.WriteLine("Searching how long to beat game: " + gameName);

                //Check authentication key
                double authExpiredMinutes = 0;
                if (vApiHltbAuthDateTime != null)
                {
                    authExpiredMinutes = DateTime.Now.Subtract((DateTime)vApiHltbAuthDateTime).TotalMinutes;
                }

                if (authExpiredMinutes > 15 || string.IsNullOrWhiteSpace(vApiHltbAuthKey))
                {
                    if (!await ApiHowLongToBeat_UpdateAuthKey_Html())
                    {
                        return null;
                    }
                }

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestReferer = new[] { "Referer", "https://howlongtobeat.com" };
                string[][] requestHeaders = new string[][] { requestAccept, requestReferer };

                //Set download url
                string apiUrl = "https://howlongtobeat.com/api/" + vApiHltbSearchName + "/" + vApiHltbAuthKey;

                //Split name and remove characters
                string[] searchFilterTerms = gameName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //Create json request
                ApiHltbSearchQueryHtml jsonSearchQuery = new ApiHltbSearchQueryHtml();
                jsonSearchQuery.searchTerms = searchFilterTerms;
                string jsonRequest = JsonConvert.SerializeObject(jsonSearchQuery);

                //Create request body
                StringContent requestBodyStringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                //Download how long to beat results
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, new Uri(apiUrl), requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading how long to beat, no connection.");
                    return null;
                }

                //Deserialize json string
                return JsonConvert.DeserializeObject<ApiHltbSearchResultHtml>(resultSearch);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed searching how long to beat: " + ex.Message);
                return null;
            }
        }
    }
}