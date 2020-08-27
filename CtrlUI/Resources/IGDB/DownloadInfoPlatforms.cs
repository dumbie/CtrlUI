using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Download igdb platforms
        public async Task ApiIGDBDownloadPlatforms()
        {
            try
            {
                Debug.WriteLine("Downloading IGDB platforms.");

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestUserKey = new[] { "User-Key", vApiIGDBUserKey };
                string[][] requestHeaders = new string[][] { requestAccept, requestUserKey };

                //Create request uri
                Uri requestUri = new Uri("https://api-v3.igdb.com/platforms");

                //Create request body
                string requestBodyString = "fields *; limit 500; sort id asc;";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download igdb genres
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading IGDB platforms.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading IGDB platforms: " + ex.Message);
                return;
            }
        }
    }
}