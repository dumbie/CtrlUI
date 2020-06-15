using System.Windows.Media.Imaging;

namespace LibraryShared
{
    public partial class Classes
    {
        public class DownloadInfoGame
        {
            public BitmapImage ImageBitmap { get; set; }
            public ApiIGDBGames Details { get; set; }
        }

        public class DownloadInfoConsole
        {
            public BitmapImage ImageBitmap { get; set; }
            public ApiIGDBPlatformVersions Details { get; set; }
        }
    }
}