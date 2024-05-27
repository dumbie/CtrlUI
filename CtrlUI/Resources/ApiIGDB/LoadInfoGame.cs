using ArnoldVinkCode;
using System;
using System.Linq;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Generate summary string from ApiIGDB
        string ApiIGDB_GameSummaryString(ApiIGDBGames infoGames)
        {
            string summaryString = string.Empty;
            try
            {
                //Release date
                if (infoGames.first_release_date != 0)
                {
                    ApiIGDB_ReleaseDateToString(infoGames, out string gameReleaseDate, out string gameReleaseYear);
                    summaryString += "\nReleased: " + gameReleaseDate;
                }

                //Genres
                string gameGenres = string.Empty;
                if (infoGames.genres != null)
                {
                    foreach (int genreId in infoGames.genres)
                    {
                        ApiIGDBGenres apiIGDBGenres = vApiIGDBGenres.FirstOrDefault(x => x.id == genreId);
                        gameGenres = AVFunctions.StringAdd(gameGenres, apiIGDBGenres.name, ",");
                    }
                    if (!string.IsNullOrWhiteSpace(gameGenres))
                    {
                        summaryString += "\nGenres: " + gameGenres;
                    }
                }

                //Summary
                if (string.IsNullOrWhiteSpace(infoGames.summary))
                {
                    summaryString += "\n\nThere is no description available.";
                }
                else
                {
                    summaryString += "\n\n" + infoGames.summary;
                }

                //Remove first linebreak
                summaryString = AVFunctions.StringRemoveStart(summaryString, "\n");
            }
            catch { }
            if (string.IsNullOrWhiteSpace(summaryString)) { summaryString = "There is no description available."; }
            return summaryString;
        }

        //ApiIGDB ReleaseDate to string
        void ApiIGDB_ReleaseDateToString(ApiIGDBGames infoGames, out string gameReleaseDate, out string gameReleaseYear)
        {
            gameReleaseDate = "Unknown";
            gameReleaseYear = "N/A";
            try
            {
                if (infoGames.first_release_date != 0)
                {
                    DateTime epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(infoGames.first_release_date);
                    gameReleaseDate = epochDateTime.ToString("d MMMM yyyy", vAppCultureInfo);
                    gameReleaseYear = epochDateTime.ToString("yyyy", vAppCultureInfo);
                }
            }
            catch { }
        }

        //ApiIGDB Platforms to string
        void ApiIGDB_PlatformsToString(ApiIGDBGames infoGames, out string gamePlatforms)
        {
            gamePlatforms = string.Empty;
            try
            {
                foreach (int platformId in infoGames.platforms)
                {
                    ApiIGDBPlatforms apiIGDBPlatforms = vApiIGDBPlatforms.FirstOrDefault(x => x.id == platformId);
                    if (apiIGDBPlatforms != null)
                    {
                        gamePlatforms = AVFunctions.StringAdd(gamePlatforms, apiIGDBPlatforms.name, ",");
                    }
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(gamePlatforms)) { gamePlatforms = "Unknown"; }
        }
    }
}