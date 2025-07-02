using System;

namespace CtrlUI
{
    public partial class Classes
    {
        public class DLsiteProductAdditionalInfo
        {
            public int Id { get; set; }
            public string WorkId { get; set; }
            public string WorkName { get; set; }
            public bool? Favorite { get; set; }
            public DateTime? FavoriteAt { get; set; }
            public bool? Hidden { get; set; }
            public DateTime? HiddenAt { get; set; }
            public string DownloadPath { get; set; }
            public DateTime? DownloadedAt { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public int? PlaySec { get; set; }
            public string ExeFilePath { get; set; }
            public bool? Downloaded { get; set; }
            public bool? HasExe { get; set; }
        }
    }
}