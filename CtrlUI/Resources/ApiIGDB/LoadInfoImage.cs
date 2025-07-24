using ArnoldVinkCode;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //ApiIGDB Generate image
        public static async Task<BitmapImage> GenerateIgdbImage(object targetInfo)
        {
            try
            {
                //Get download uri
                Uri downloadUri = null;
                if (targetInfo.GetType() == typeof(ApiIGDBGames))
                {
                    //Convert object
                    ApiIGDBGames apiIGDB = (ApiIGDBGames)targetInfo;

                    //Get download uri
                    if (apiIGDB.cover != null)
                    {
                        downloadUri = new Uri("https://images.igdb.com/igdb/image/upload/t_720p/" + apiIGDB.cover.image_id + ".png");
                    }
                }
                else if (targetInfo.GetType() == typeof(ApiIGDBPlatforms))
                {
                    //Convert object
                    ApiIGDBPlatforms apiIGDB = (ApiIGDBPlatforms)targetInfo;

                    //Get first versions
                    ApiIGDBPlatformsVersions infoVersions = apiIGDB.versions.FirstOrDefault();

                    //Get download uri
                    if (infoVersions != null)
                    {
                        downloadUri = new Uri("https://images.igdb.com/igdb/image/upload/t_720p/" + infoVersions.platform_logo.image_id + ".png");
                    }
                }

                //Download image to bytes
                byte[] imageBytes = await AVDownloader.DownloadByteAsync(5000, "CtrlUI", null, downloadUri);

                //Check downloaded image bytes
                if (imageBytes != null && imageBytes.Length > 256)
                {
                    try
                    {
                        //Cache bytes to variable
                        vContentInformationImageBytes = imageBytes;

                        //Convert bytes to BitmapImage
                        return BytesToBitmapImage(imageBytes, 0, 0);
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }
    }
}