namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiHltbSearchQuery
        {
            public bool useCache = false;
            public string searchType { get; set; } = "games";
            public string[] searchTerms { get; set; }
            public int searchPage { get; set; } = 1;
            public int size { get; set; } = 25;
            public SearchOptions searchOptions { get; set; } = new SearchOptions();

            public class SearchOptions
            {
                public Games games { get; set; } = new Games();
                public Users users { get; set; } = new Users();
                public string filter { get; set; } = string.Empty;
                public int sort { get; set; }
                public int randomizer { get; set; }
            }

            public class Games
            {
                public int userId { get; set; }
                public string platform { get; set; } = string.Empty;
                public string sortCategory { get; set; } = "popular";
                public string rangeCategory { get; set; } = "main";
                public RangeTime rangeTime { get; set; } = new RangeTime();
                public Gameplay gameplay { get; set; } = new Gameplay();
                public RangeYear rangeYear { get; set; } = new RangeYear();
                public string modifier { get; set; } = string.Empty;
            }

            public class RangeTime
            {
                public int? min { get; set; }
                public int? max { get; set; }
            }

            public class RangeYear
            {
                public int? min { get; set; }
                public int? max { get; set; }
            }

            public class Gameplay
            {
                public string perspective { get; set; } = string.Empty;
                public string flow { get; set; } = string.Empty;
                public string genre { get; set; } = string.Empty;
                public string subGenre { get; set; } = string.Empty;
                public string difficulty { get; set; } = string.Empty;
            }

            public class Users
            {
                public string sortCategory { get; set; } = "postcount";
            }
        }

        public class ApiHltbSearchResult
        {
            public string color { get; set; }
            public string title { get; set; }
            public string category { get; set; }
            public int count { get; set; }
            public int pageCurrent { get; set; }
            public int? pageTotal { get; set; }
            public int pageSize { get; set; }
            public Data[] data { get; set; }

            public class Data
            {
                public int count { get; set; }
                public int game_id { get; set; }
                public string game_name { get; set; }
                public int game_name_date { get; set; }
                public string game_alias { get; set; }
                public string game_type { get; set; }
                public string game_image { get; set; }
                public int comp_lvl_combine { get; set; }
                public int comp_lvl_sp { get; set; }
                public int comp_lvl_co { get; set; }
                public int comp_lvl_mp { get; set; }
                public int comp_lvl_spd { get; set; }
                public int comp_main { get; set; }
                public int comp_plus { get; set; }
                public int comp_100 { get; set; }
                public int comp_all { get; set; }
                public int comp_main_count { get; set; }
                public int comp_plus_count { get; set; }
                public int comp_100_count { get; set; }
                public int comp_all_count { get; set; }
                public int invested_co { get; set; }
                public int invested_mp { get; set; }
                public int invested_co_count { get; set; }
                public int invested_mp_count { get; set; }
                public int count_comp { get; set; }
                public int count_speedrun { get; set; }
                public int count_backlog { get; set; }
                public int count_review { get; set; }
                public int review_score { get; set; }
                public int count_playing { get; set; }
                public int count_retired { get; set; }
                public string profile_dev { get; set; }
                public int profile_popular { get; set; }
                public int profile_steam { get; set; }
                public string profile_platform { get; set; }
                public int release_world { get; set; }
            }
        }
    }
}