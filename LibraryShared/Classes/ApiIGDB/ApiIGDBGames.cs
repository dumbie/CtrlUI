namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBGames
        {
            public int id { get; set; }
            public int[] age_ratings { get; set; }
            public double aggregated_rating { get; set; }
            public int aggregated_rating_count { get; set; }
            public ApiIGDBGamesAlternativeNames[] alternative_names { get; set; }
            public ApiIGDBImages[] artworks { get; set; }
            public int[] bundles { get; set; }
            public int category { get; set; }
            public int collection { get; set; }
            public ApiIGDBImages cover { get; set; }
            public int created_at { get; set; }
            public int[] dlcs { get; set; }
            public int[] expansions { get; set; }
            public int[] external_games { get; set; }
            public long first_release_date { get; set; }
            public int follows { get; set; }
            public int franchise { get; set; }
            public int[] franchises { get; set; }
            public ApiIGDBGamesEngines[] game_engines { get; set; }
            public ApiIGDBGamesModes[] game_modes { get; set; }
            public ApiIGDBGamesGenres[] genres { get; set; }
            public int hypes { get; set; }
            public ApiIGDBGamesInvolvedCompanies[] involved_companies { get; set; }
            public int[] keywords { get; set; }
            public ApiIGDBGamesMultiplayerModes[] multiplayer_modes { get; set; }
            public string name { get; set; }
            public int parent_game { get; set; }
            public ApiIGDBPlatforms[] platforms { get; set; }
            public ApiIGDBGamesPlayerPerspectives[] player_perspectives { get; set; }
            public double rating { get; set; }
            public int rating_count { get; set; }
            public ApiIGDBReleaseDates[] release_dates { get; set; }
            public ApiIGDBImages[] screenshots { get; set; }
            public int[] similar_games { get; set; }
            public string slug { get; set; }
            public int[] standalone_expansions { get; set; }
            public int status { get; set; }
            public string storyline { get; set; }
            public string summary { get; set; }
            public int[] tags { get; set; }
            public ApiIGDBGamesThemes[] themes { get; set; }
            public double total_rating { get; set; }
            public int total_rating_count { get; set; }
            public int updated_at { get; set; }
            public string url { get; set; }
            public int version_parent { get; set; }
            public string version_title { get; set; }
            public ApiIGDBGamesVideos[] videos { get; set; }
            public ApiIGDBWebsites[] websites { get; set; }
        }
    }
}