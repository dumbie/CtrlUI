namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBGamesInvolvedCompanies
        {
            public int id { get; set; }
            public ApiIGDBCompany company { get; set; }
            public int created_at { get; set; }
            public bool developer { get; set; }
            public int game { get; set; }
            public bool porting { get; set; }
            public bool publisher { get; set; }
            public bool supporting { get; set; }
            public int updated_at { get; set; }
        }
    }
}