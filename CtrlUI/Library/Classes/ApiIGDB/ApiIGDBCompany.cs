namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBCompany
        {
            public int id { get; set; }
            public long change_date { get; set; }
            public int change_date_category { get; set; }
            public int changed_company_id { get; set; }
            public int country { get; set; }
            public int created_at { get; set; }
            public string description { get; set; }
            public int[] developed { get; set; }
            public int[] published { get; set; }
            public ApiIGDBImages logo { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public long start_date { get; set; }
            public int start_date_category { get; set; }
            public int updated_at { get; set; }
            public ApiIGDBWebsites[] websites { get; set; }
            public string url { get; set; }
            public int? parent { get; set; }
        }
    }
}