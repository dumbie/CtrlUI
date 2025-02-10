using ArnoldVinkCode;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Generate BitmapImage for ApiIGDBPlatforms
        async Task<BitmapImage> ApiIGDB_PlatformBitmapImage(ApiIGDBPlatforms infoPlatforms)
        {
            try
            {
                //Get first versions
                ApiIGDBPlatformsVersions infoVersions = infoPlatforms.versions.FirstOrDefault();

                //Get download uri
                Uri downloadUri = null;
                if (infoVersions != null)
                {
                    downloadUri = new Uri("https://images.igdb.com/igdb/image/upload/t_720p/" + infoVersions.platform_logo.image_id + ".png");
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

        //Generate summary string for ApiIGDBPlatforms
        string ApiIGDB_PlatformSummaryString(ApiIGDBPlatforms infoPlatforms)
        {
            string summaryString = string.Empty;
            try
            {
                //Get first versions
                ApiIGDBPlatformsVersions infoVersions = infoPlatforms.versions.FirstOrDefault();

                //Cpu
                if (!string.IsNullOrWhiteSpace(infoVersions.cpu))
                {
                    summaryString += "\nCpu: " + infoVersions.cpu;
                }

                //Memory
                if (!string.IsNullOrWhiteSpace(infoVersions.memory))
                {
                    summaryString += "\nMemory: " + infoVersions.memory;
                }

                //Graphics
                if (!string.IsNullOrWhiteSpace(infoVersions.graphics))
                {
                    summaryString += "\nGraphics: " + infoVersions.graphics;
                }

                //Output
                if (!string.IsNullOrWhiteSpace(infoVersions.output))
                {
                    summaryString += "\nOutput: " + infoVersions.output;
                }

                //Extras
                if (!string.IsNullOrWhiteSpace(infoVersions.media))
                {
                    summaryString += "\nExtras: " + infoVersions.media;
                }

                //Online
                if (!string.IsNullOrWhiteSpace(infoVersions.online))
                {
                    summaryString += "\nOnline: " + infoVersions.online;
                }

                //Operating System
                if (!string.IsNullOrWhiteSpace(infoVersions.os))
                {
                    summaryString += "\nOS: " + infoVersions.os;
                }

                //Summary
                if (!string.IsNullOrWhiteSpace(infoPlatforms.summary))
                {
                    summaryString += "\n\n" + infoPlatforms.summary;
                }
                else if (!string.IsNullOrWhiteSpace(infoVersions.summary))
                {
                    summaryString += "\n\n" + infoVersions.summary;
                }
                else
                {
                    summaryString += "\n\nThere is no description available.";
                }

                //Remove first line break
                summaryString = AVFunctions.StringRemoveStart(summaryString, "\n");
            }
            catch { }
            if (string.IsNullOrWhiteSpace(summaryString)) { summaryString = "There is no description available."; }
            return summaryString;
        }
    }
}