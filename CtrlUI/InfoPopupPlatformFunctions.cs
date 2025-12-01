using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using static ArnoldVinkStyles.AVFocus;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show and close information popup
        public async Task Popup_Show_PlatformInformation(string searchTerm, object dataBindObject)
        {
            try
            {
                //Update databind object
                vContentInformationDataBind = dataBindObject;

                //Show the text input popup
                ApiIGDBPlatforms igdbPlatforms = await Popup_SearchInfoPlatform(searchTerm, string.Empty, string.Empty);
                if (igdbPlatforms == null)
                {
                    Debug.WriteLine("No search term entered.");
                    return;
                }

                //Check if the popup is already open
                if (!vContentInformationOpen)
                {
                    //Play the opening sound
                    PlayInterfaceSound(vSettings, "PromptOpen", false, false);

                    //Save the previous focus element
                    AVFocusDetailsSave(vContentInformationElementFocus, null);
                }

                //Update interface with information
                grid_ContentInformation_Header.Text = igdbPlatforms.name;

                //Top image
                BitmapImage topImage = await GenerateIgdbImage(igdbPlatforms);
                if (topImage != null)
                {
                    image_ContentInfo_Top.ImageSource = topImage;
                    image_ContentInfo_Top.Visibility = Visibility.Visible;
                }
                else
                {
                    image_ContentInfo_Top.Visibility = Visibility.Collapsed;
                }

                //Set ratings
                ellipse_ContentInfo_Rating_Users.Visibility = Visibility.Collapsed;
                border_ContentInfo_Rating_Critics.Visibility = Visibility.Collapsed;

                //Set gallery images
                listView_ContentInfo_Gallery.Visibility = Visibility.Collapsed;

                //Set description
                textblock_ContentInfo_Description.Text = ApiIGDB_PlatformSummaryString(igdbPlatforms);

                //Update popup variables
                vContentInformationOpen = true;

                //Show the popup
                Popup_Show_Element(grid_Popup_ContentInformation);

                //Focus on button
                await FocusFrameworkElement(btn_ContentInfo_Focus);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Information show error: " + ex.Message);
            }
        }
    }
}