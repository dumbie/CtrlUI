using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //ApiIGDB Generate expended field string
        private static string GenerateIgdbFieldString(Type targetType)
        {
            try
            {
                //List property names
                List<string> typeProperties = targetType.GetProperties().Select(x => x.Name).ToList();

                //Replace expendable properties
                for (int i = 0; i < typeProperties.Count(); i++)
                {
                    try
                    {
                        if (targetType == typeof(ApiIGDBPlatforms))
                        {
                            //Platforms
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "platform_logo", "platform_logo.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "platform_family", "platform_family.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "websites", "websites.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "versions", "versions.*,versions.platform_logo.*,versions.platform_version_release_dates.*,versions.companies.*,versions.companies.company.*,versions.companies.company.logo.*,versions.companies.company.websites.*");
                        }
                        else if (targetType == typeof(ApiIGDBGames))
                        {
                            //Games
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "alternative_names", "alternative_names.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "cover", "cover.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "game_engines", "game_engines.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "game_modes", "game_modes.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "multiplayer_modes", "multiplayer_modes.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "genres", "genres.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "release_dates", "release_dates.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "player_perspectives", "player_perspectives.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "screenshots", "screenshots.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "artworks", "artworks.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "themes", "themes.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "videos", "videos.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "websites", "websites.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "platforms", "platforms.*,platforms.platform_logo.*,platforms.platform_family.*,platforms.versions.*,platforms.versions.platform_logo.*,platforms.versions.platform_version_release_dates.*,platforms.versions.companies.*,platforms.versions.companies.company.*,platforms.versions.companies.company.logo.*,platforms.versions.companies.company.websites.*,platforms.websites.*");
                            typeProperties[i] = StringReplaceWholeWord(typeProperties[i], "involved_companies", "involved_companies.*,involved_companies.company.*,involved_companies.company.logo.*,involved_companies.company.websites.*");
                        }
                    }
                    catch { }
                }

                //Join field properties
                string fieldString = string.Join(",", typeProperties);
                Debug.WriteLine("IGDB fields: " + fieldString);
                return fieldString;
            }
            catch { }
            return "*";
        }

        //ApiIGDB ReleaseDate to string
        public void ApiIGDB_ReleaseDateToString(ApiIGDBGames infoGames, out string gameReleaseDate, out string gameReleaseYear)
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
        public void ApiIGDB_PlatformsToString(ApiIGDBGames infoGames, out string gameInfo)
        {
            gameInfo = string.Empty;
            try
            {
                foreach (ApiIGDBPlatforms igdbInfo in infoGames.platforms)
                {
                    gameInfo = AVFunctions.StringAdd(gameInfo, igdbInfo.name, ",");
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(gameInfo)) { gameInfo = "Unknown"; }
        }

        //ApiIGDB Genres to string
        public void ApiIGDB_GenresToString(ApiIGDBGames infoGames, out string gameInfo)
        {
            gameInfo = string.Empty;
            try
            {
                foreach (ApiIGDBGamesGenres igdbInfo in infoGames.genres)
                {
                    gameInfo = AVFunctions.StringAdd(gameInfo, igdbInfo.name, ",");
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(gameInfo)) { gameInfo = "Unknown"; }
        }

        //ApiIGDB Companies to string
        public void ApiIGDB_CompaniesToString(ApiIGDBGames infoGames, out string gameInfo)
        {
            gameInfo = string.Empty;
            try
            {
                foreach (ApiIGDBGamesInvolvedCompanies igdbInfo in infoGames.involved_companies)
                {
                    gameInfo = AVFunctions.StringAdd(gameInfo, igdbInfo.company.name, ",");
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(gameInfo)) { gameInfo = "Unknown"; }
        }

        //ApiIGDB Engines to string
        public void ApiIGDB_EnginesToString(ApiIGDBGames infoGames, out string gameInfo)
        {
            gameInfo = string.Empty;
            try
            {
                foreach (ApiIGDBGamesEngines infoId in infoGames.game_engines)
                {
                    gameInfo = AVFunctions.StringAdd(gameInfo, infoId.name, ",");
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(gameInfo)) { gameInfo = "Unknown"; }
        }

        //ApiIGDB Game Modes to string
        public void ApiIGDB_GameModesToString(ApiIGDBGames infoGames, out string gameInfo)
        {
            gameInfo = string.Empty;
            try
            {
                foreach (ApiIGDBGamesModes infoId in infoGames.game_modes)
                {
                    gameInfo = AVFunctions.StringAdd(gameInfo, infoId.name, ",");
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(gameInfo)) { gameInfo = "Unknown"; }
        }

        //ApiIGDB Themes to string
        public void ApiIGDB_ThemesToString(ApiIGDBGames infoGames, out string gameInfo)
        {
            gameInfo = string.Empty;
            try
            {
                foreach (ApiIGDBGamesThemes infoId in infoGames.themes)
                {
                    gameInfo = AVFunctions.StringAdd(gameInfo, infoId.name, ",");
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(gameInfo)) { gameInfo = "Unknown"; }
        }

        //ApiIGDB Perspectives to string
        public void ApiIGDB_PerspectivesToString(ApiIGDBGames infoGames, out string gameInfo)
        {
            gameInfo = string.Empty;
            try
            {
                foreach (ApiIGDBGamesPlayerPerspectives infoId in infoGames.player_perspectives)
                {
                    gameInfo = AVFunctions.StringAdd(gameInfo, infoId.name, ",");
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(gameInfo)) { gameInfo = "Unknown"; }
        }
    }
}