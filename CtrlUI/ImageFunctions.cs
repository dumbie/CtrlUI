using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSearch;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Reset application image
        async Task Image_Application_Reset()
        {
            try
            {
                //Set application and executable name
                string imageFileName = vEditAppDataBind.Name;
                string imageFileExeName = string.Empty;
                string imageFileExePath = string.Empty;
                if (vEditAppDataBind != null)
                {
                    imageFileName = vEditAppDataBind.Name;
                    imageFileExeName = Path.GetFileNameWithoutExtension(vEditAppDataBind.PathExe);
                    imageFileExePath = vEditAppDataBind.PathExe;
                }
                else
                {
                    imageFileName = tb_AddAppName.Text;
                    imageFileExeName = Path.GetFileNameWithoutExtension(tb_AddAppExePath.Text);
                    imageFileExePath = tb_AddAppExePath.Text;
                }

                //Search application image files
                string[] foundImages = Search_Files(new string[] { imageFileName, imageFileExeName }, vImageSourceFoldersUser, false);

                //Remove application image files
                foreach (string foundImage in foundImages)
                {
                    File_Delete(foundImage);
                }

                //Reload the application image
                BitmapImage applicationImage = FileToBitmapImage(new string[] { imageFileName, imageFileExeName, imageFileExePath }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                img_AddAppLogo.Source = applicationImage;
                if (vEditAppDataBind != null)
                {
                    vEditAppDataBind.ImageBitmap = applicationImage;
                }

                await Notification_Send_Status("Restart", "App image reset");
                Debug.WriteLine("App image reset: " + imageFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to reset application image: " + ex.Message);
            }
        }
    }
}