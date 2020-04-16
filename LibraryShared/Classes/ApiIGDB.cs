namespace LibraryShared
{
    public partial class Classes
    {
        public static string ApiIGDBGenres(int genreId)
        {
            try
            {
                if (genreId == 2) { return "Point Click"; }
                else if (genreId == 4) { return "Fighting"; }
                else if (genreId == 5) { return "Shooter"; }
                else if (genreId == 7) { return "Music"; }
                else if (genreId == 8) { return "Platform"; }
                else if (genreId == 9) { return "Puzzle"; }
                else if (genreId == 10) { return "Racing"; }
                else if (genreId == 11) { return "Realtime strategy"; }
                else if (genreId == 12) { return "Roleplaying"; }
                else if (genreId == 13) { return "Simulator"; }
                else if (genreId == 14) { return "Sport"; }
                else if (genreId == 15) { return "Strategy"; }
                else if (genreId == 16) { return "Turnbased strategy"; }
                else if (genreId == 24) { return "Tactical"; }
                else if (genreId == 25) { return "Hack Slash"; }
                else if (genreId == 26) { return "Quiz"; }
                else if (genreId == 30) { return "Pinball"; }
                else if (genreId == 31) { return "Adventure"; }
                else if (genreId == 32) { return "Indie"; }
                else if (genreId == 33) { return "Arcade"; }
                else if (genreId == 34) { return "Visual Novel"; }
            }
            catch { }
            return "Unknown";
        }

        public static string ApiIGDBPlatforms(int platformId)
        {
            try
            {
                if (platformId == 3) { return "Linux"; }
                else if (platformId == 4) { return "Nintendo 64"; }
                else if (platformId == 5) { return "Nintendo Wii"; }
                else if (platformId == 6) { return "Windows"; }
                else if (platformId == 7) { return "PlayStation 1"; }
                else if (platformId == 167) { return "PlayStation 5"; }
                else if (platformId == 169) { return "Xbox Series X"; }
            }
            catch { }
            return "Unknown";
        }

        public class ApiIGDBGames
        {
            public int id { get; set; }
            public int[] age_ratings { get; set; }
            public decimal aggregated_rating { get; set; }
            public int aggregated_rating_count { get; set; }
            public int[] alternative_names { get; set; }
            public int category { get; set; }
            public int collection { get; set; }
            public int cover { get; set; }
            public long created_at { get; set; }
            public int[] expansions { get; set; }
            public int[] external_games { get; set; }
            public long first_release_date { get; set; }
            public int[] game_modes { get; set; }
            public int[] genres { get; set; }
            public int[] involved_companies { get; set; }
            public int[] keywords { get; set; }
            public string name { get; set; }
            public int[] platforms { get; set; }
            public int[] player_perspectives { get; set; }
            public decimal popularity { get; set; }
            public decimal rating { get; set; }
            public int rating_count { get; set; }
            public long[] release_dates { get; set; }
            public int[] screenshots { get; set; }
            public int[] similar_games { get; set; }
            public string slug { get; set; }
            public string summary { get; set; }
            public long[] tags { get; set; }
            public int[] themes { get; set; }
            public decimal total_rating { get; set; }
            public int total_rating_count { get; set; }
            public long updated_at { get; set; }
            public string url { get; set; }
            public int[] videos { get; set; }
            public int[] websites { get; set; }
        }

        public class ApiIGDBCovers
        {
            public int id { get; set; }
            public int game { get; set; }
            public int height { get; set; }
            public string image_id { get; set; }
            public string url { get; set; }
            public int width { get; set; }
        }
    }
}