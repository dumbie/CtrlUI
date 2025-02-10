namespace LibraryShared
{
    public partial class Classes
    {
        public class ApiIGDBCompanies
        {
            public int id { get; set; }
            public ApiIGDBCompany company { get; set; }
            public bool developer { get; set; }
            public bool manufacturer { get; set; }
        }
    }
}