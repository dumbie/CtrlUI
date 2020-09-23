using ArnoldVinkCode;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Download image id information
        public async Task<string> Api_Twitch_Authenticate()
        {
            try
            {
                //Check if auth token is cached
                if (vApiIGDBTokenExpire != null && DateTime.Now < vApiIGDBTokenExpire)
                {
                    Debug.WriteLine("Returning auth cache from Twitch.");
                    return vApiIGDBTokenCache;
                }

                Debug.WriteLine("Authenticating with Twitch.");

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[][] requestHeaders = new string[][] { requestAccept };

                //Create request uri
                Uri requestUri = new Uri("https://id.twitch.tv/oauth2/token?client_id=" + vApiIGDBClientID + "&client_secret=" + vApiIGDBAuthorization + "&grant_type=client_credentials");

                //Authenticate with Twitch
                string resultAuth = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, null);
                if (string.IsNullOrWhiteSpace(resultAuth))
                {
                    Debug.WriteLine("Failed authenticating with Twitch, no connection.");
                    return string.Empty;
                }

                //Deserialize json string
                ApiTwitchOauth2 jsonAuth = JsonConvert.DeserializeObject<ApiTwitchOauth2>(resultAuth);

                //Check if authenticated
                if (jsonAuth.access_token != null && !string.IsNullOrWhiteSpace(jsonAuth.access_token))
                {
                    vApiIGDBTokenCache = jsonAuth.access_token;
                    vApiIGDBTokenExpire = DateTime.Now.AddSeconds(jsonAuth.expires_in).AddSeconds(-30);
                    return vApiIGDBTokenCache;
                }
                else
                {
                    Debug.WriteLine("Failed authenticating with Twitch: " + jsonAuth.message);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed authenticating with Twitch: " + ex.Message);
                return string.Empty;
            }
        }
    }
}