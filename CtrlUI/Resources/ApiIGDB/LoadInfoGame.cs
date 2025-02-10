using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Generate BitmapImage for ApiIGDBGames
        async Task<BitmapImage> ApiIGDB_GameBitmapImage(ApiIGDBGames infoGames)
        {
            try
            {
                //Get download uri
                Uri downloadUri = null;
                if (infoGames.cover != null)
                {
                    downloadUri = new Uri("https://images.igdb.com/igdb/image/upload/t_720p/" + infoGames.cover.image_id + ".png");
                }

                //Download image to bytes
                byte[] imageBytes = await AVDownloader.DownloadByteAsync(5000, "CtrlUI", null, downloadUri);

                //Check downloaded image bytes
                if (imageBytes != null && imageBytes.Length > 256)
                {
                    try
                    {
                        //Cache bytes to variable
                        vContentInformationImageBytes = imageBytes;

                        //Convert bytes to BitmapImage
                        return BytesToBitmapImage(imageBytes, 0, 0);
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }

        //Generate summary string for ApiIGDBGames
        string ApiIGDB_GameSummaryString(ApiIGDBGames infoGames, bool incReleaseDate, bool incGenres, bool incPlatforms, bool incEngines, bool incCompanies)
        {
            string summaryString = string.Empty;
            try
            {
                //Fix add features... single player, multiplayer, coop etc.

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