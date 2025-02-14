using ArnoldVinkCode;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Generate summary string for ApiIGDBGames
        string ApiIGDB_GameSummaryString(ApiIGDBGames infoGames, bool incReleaseDate, bool incGenres, bool incModes, bool incPlatforms, bool incEngines, bool incCompanies)
        {
            string summaryString = string.Empty;
            try
            {
                //Release date
                if (incReleaseDate)
                {
                    ApiIGDB_ReleaseDateToString(infoGames, out string gameReleaseDate, out string gameReleaseYear);
                    if (gameReleaseDate != "Unknown")
                    {
                        summaryString += "\nReleased: " + gameReleaseDate;
                    }
                }

                //Genres
                if (incGenres)
                {
                    ApiIGDB_GenresToString(infoGames, out string gameGenres);
                    if (gameGenres != "Unknown")
                    {
                        summaryString += "\nGenres: " + gameGenres;
                    }
                }

                //Themes
                if (incModes)
                {
                    ApiIGDB_ThemesToString(infoGames, out string gameThemes);
                    if (gameThemes != "Unknown")
                    {
                        summaryString += "\nThemes: " + gameThemes;
                    }
                }

                //Game Modes
                if (incModes)
                {
                    ApiIGDB_GameModesToString(infoGames, out string gameModes);
                    if (gameModes != "Unknown")
                    {
                        summaryString += "\nGame modes: " + gameModes;
                    }
                }

                //Perspectives
                if (incModes)
                {
                    ApiIGDB_PerspectivesToString(infoGames, out string gamePerspectives);
                    if (gamePerspectives != "Unknown")
                    {
                        summaryString += "\nPerspectives: " + gamePerspectives;
                    }
                }

                //Platforms
                if (incPlatforms)
                {
                    ApiIGDB_PlatformsToString(infoGames, out string gamePlatforms);
                    if (gamePlatforms != "Unknown")
                    {
                        summaryString += "\nPlatforms: " + gamePlatforms;
                    }
                }

                //Companies
                if (incCompanies)
                {
                    ApiIGDB_CompaniesToString(infoGames, out string gameCompanies);
                    if (gameCompanies != "Unknown")
                    {
                        summaryString += "\nCompanies: " + gameCompanies;
                    }
                }

                //Engines
                if (incEngines)
                {
                    ApiIGDB_EnginesToString(infoGames, out string gameEngines);
                    if (gameEngines != "Unknown")
                    {
                        summaryString += "\nGame engine: " + gameEngines;
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

                //Remove first line break
                summaryString = AVFunctions.StringRemoveStart(summaryString, "\n");
            }
            catch { }
            if (string.IsNullOrWhiteSpace(summaryString)) { summaryString = "There is no description available."; }
            return summaryString;
        }
    }
}