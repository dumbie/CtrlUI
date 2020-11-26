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
                    string gameReleaseDate = string.Empty;
                    string gameReleaseYear = string.Empty;
                    ApiIGDB_ReleaseDateToString(infoGames, out gameReleaseDate, out gameReleaseYear);
                    summaryString += "\nReleased: " + gameReleaseDate;
                }

                //Genres
                string gameGenres = string.Empty;
                if (infoGames.genres != null)
                {
                    foreach (int genreId in infoGames.genres)
                    {
                        ApiIGDBGenres apiIGDBGenres = vApiIGDBGenres.Where(x => x.id == genreId).FirstOrDefault();
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
            gameReleaseDate = string.Empty;
            gameReleaseYear = string.Empty;
            try
            {
                if (infoGames.first_release_date != 0)
                {
                    DateTime epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(infoGames.first_release_date);
                    gameReleaseDate = epochDateTime.ToString("dd MMMM yyyy", vAppCultureInfo);
                    gameReleaseYear = epochDateTime.ToString("yyyy", vAppCultureInfo);
                    return;
                }
            }
            catch { }
        }
    }
}