using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSearch;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Remove application image
        public void Image_Application_Remove(DataBindApp dataBindApp)
        {
            try
            {
                //Set application and executable name
                string imageFileName = dataBindApp.Name;
                string imageFileExeName = Path.GetFileNameWithoutExtension(dataBindApp.PathExe);

                //Search application image files
                string[] foundImages = Search_Files(new string[] { imageFileName, imageFileExeName }, vImageSourceFoldersUser, false);

                //Remove application image files
                foreach (string foundImage in foundImages)
                {
                    File_Delete(foundImage);
                }

                Debug.WriteLine("Application image removed: " + imageFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove application image: " + ex.Message);
            }
        }

        //Reset application image
        public async Task Image_Application_Reset(DataBindApp dataBindApp)
        {
            try
            {
                //Set application and executable name
                string imageFileName = string.Empty;
                string imageFileExeName = string.Empty;
                string imageFileExePath = string.Empty;
                if (dataBindApp != null)
                {
                    imageFileName = dataBindApp.Name;
                    imageFileExeName = Path.GetFileNameWithoutExtension(dataBindApp.PathExe);
                    imageFileExePath = dataBindApp.PathExe;
                }
                else
                {
                    imageFileName = tb_AddAppName.Text;
                    imageFileExeName = Path.GetFileNameWithoutExtension(tb_AddAppPathExe.Text);
                    imageFileExePath = tb_AddAppPathExe.Text;
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
                if (dataBindApp != null)
                {
                    dataBindApp.ImageBitmap = applicationImage;
                }

                await Notification_Send_Status("Restart", "App image reset");
                Debug.WriteLine("Application image reset: " + imageFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to reset application image: " + ex.Message);
            }
        }
    }
}