using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Search game information
        public async Task<ApiIGDBGames> Popup_SearchInfoGame(string searchTerm, string searchSubtitle, string searchDescription)
        {
            try
            {
                //Show the text input popup
                string nameDownload = await Popup_ShowHide_TextInput("Game search", searchTerm, "Search", true);
                if (string.IsNullOrWhiteSpace(nameDownload))
                {
                    Debug.WriteLine("No search term entered.");
                    return null;
                }
                nameDownload = nameDownload.ToLower();

                Notification_Show_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading information for: " + searchTerm);

                //Download available games
                ApiIGDBGames[] iGDBGames = await ApiIGDB_DownloadGames_Search(nameDownload);
                if (iGDBGames == null || !iGDBGames.Any())
                {
                    Debug.WriteLine("No games found for: " + searchTerm);
                    Notification_Show_Status("Close", "No games found");
                    return null;
                }

                //Return only result
                if (iGDBGames.Count() == 1)
                {
                    Debug.WriteLine("Only one games found.");
                    return iGDBGames.FirstOrDefault();
                }

                //Ask user which game to download
                List<DataBindString> Answers = new List<DataBindString>();
                foreach (ApiIGDBGames infoGames in iGDBGames)
                {
                    //Check if cover and summary is available
                    if (infoGames.cover == null && string.IsNullOrWhiteSpace(infoGames.summary)) { continue; }

                    //Release date
                    ApiIGDB_ReleaseDateToString(infoGames, out string gameReleaseDate, out string gameReleaseYear);

                    //Game platforms
                    ApiIGDB_PlatformsToString(infoGames, out string gamePlatforms);

                    DataBindString answerDownload = new DataBindString();
                    answerDownload.ImageBitmap = vImagePreloadGame;
                    answerDownload.Name = infoGames.name;
                    answerDownload.NameSub = gamePlatforms;
                    answerDownload.NameDetail = gameReleaseYear;
                    answerDownload.Data1 = infoGames;
                    Answers.Add(answerDownload);
                }

                //Get selected result
                DataBindString messageResult = await Popup_Show_MessageBox("Select a found game (" + Answers.Count() + ")", searchSubtitle, searchDescription, Answers);
                if (messageResult == null)
                {
                    Debug.WriteLine("No game selected");
                    return null;
                }

                //Convert result back to json
                return (ApiIGDBGames)messageResult.Data1;
            }
            catch
            {
                return null;
            }
        }
    }
}