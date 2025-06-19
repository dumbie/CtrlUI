using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Close and reset information popup
        async Task Popup_Close_ContentInformation()
        {
            try
            {
                //Play the closing sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PromptClose", false, false);

                //Reset popup variables
                vContentInformationOpen = false;
                vContentInformationDataBind = null;
                vContentInformationImageBytes = [];

                //Hide the popup
                Popup_Hide_Element(grid_Popup_ContentInformation);

                //Focus on the previous focus element
                await AVFocusDetailsFocus(vContentInformationElementFocus, vProcessCurrent.WindowHandleMain);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Information close error: " + ex.Message);
            }
        }

        //Update DataBind and save image to file
        public void ContentInformationSave()
        {
            try
            {
                //Convert bytes to BitmapImage
                BitmapImage bitmapImage = BytesToBitmapImage(vContentInformationImageBytes, 0, 0);
                if (bitmapImage == null)
                {
                    Notification_Show_Status("Save", "No image to save");
                    return;
                }

                //Update DataBind and save image to file
                if (vContentInformationDataBind.GetType() == typeof(DataBindApp))
                {
                    DataBindApp dataBindApp = (DataBindApp)vContentInformationDataBind;

                    //Set BitmapImage to DataBind
                    dataBindApp.ImageBitmap = bitmapImage;

                    //Get save file path
                    string saveFilePath = GetAssetsImageFilePath(dataBindApp, ".png", false);

                    //Save bytes to image file
                    AVFiles.BytesToFile(saveFilePath, vContentInformationImageBytes);
                }
                else
                {
                    DataBindFile dataBindFile = (DataBindFile)vContentInformationDataBind;

                    //Set BitmapImage to DataBind
                    dataBindFile.ImageBitmap = bitmapImage;

                    //Get save file path
                    string saveFilePath = GetAssetsImageFilePath(dataBindFile, ".png", false);

                    //Save bytes to image file
                    AVFiles.BytesToFile(saveFilePath, vContentInformationImageBytes);
                }

                Notification_Show_Status("Save", "Saved and using image");
            }
            catch { }
        }

        //Update DataBind and save image to file
        private void Grid_Popup_ContentInformation_button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentInformationSave();
            }
            catch { }
        }
    }
}