using System.Windows.Media.Imaging;

namespace LibraryShared
{
    public partial class Classes
    {
        public class DownloadInfoGame
        {
            public BitmapImage ImageBitmap { get; set; }
            public ApiIGDBGames Details { get; set; }
            public string Summary { get; set; }
        }

        public class DownloadInfoPlatform
        {
            public BitmapImage ImageBitmap { get; set; }
            public ApiIGDBPlatforms Details { get; set; }
            public string Summary { get; set; }
        }
    }
}