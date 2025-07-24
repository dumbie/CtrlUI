using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show and close information popup
        public async Task Popup_Show_GameInformation(string searchTerm, object dataBindObject)
        {
            try
            {
                //Update databind object
                vContentInformationDataBind = dataBindObject;

                //Filter game name
                string searchTermFiltered = FilterNameGame(searchTerm, true, false, 0);

                //Show the text input popup
                ApiIGDBGames igdbGames = await Popup_SearchInfoGame(searchTermFiltered, string.Empty, string.Empty);
                if (igdbGames == null)
                {
                    Debug.WriteLine("No search term entered.");
                    return;
                }

                //Check if the popup is already open
                if (!vContentInformationOpen)
                {
                    //Play the opening sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PromptOpen", false, false);

                    //Save the previous focus element
                    AVFocusDetailsSave(vContentInformationElementFocus, null);
                }

                //Update interface with information
                grid_ContentInformation_Header.Text = igdbGames.name;

                //Top image
                BitmapImage topImage = await GenerateIgdbImage(igdbGames);
                if (topImage != null)
                {
                    image_ContentInfo_Top.Source = topImage;
                    image_ContentInfo_Top.Visibility = Visibility.Visible;
                }
                else
                {
                    image_ContentInfo_Top.Visibility = Visibility.Collapsed;
                }

                //Set ratings
                if (igdbGames.rating_count > 0)
                {
                    int gameRating = Convert.ToInt32(igdbGames.rating);
                    textblock_ContentInfo_Rating_Users.Text = gameRating.ToString();
                    textblock_ContentInfo_Rating_Users_Count.Text = igdbGames.rating_count + "x";
                    ellipse_ContentInfo_Rating_Users.Background = GameRatingToSolidColorBrush(gameRating);
                    ellipse_ContentInfo_Rating_Users.Visibility = Visibility.Visible;
                }
                else
                {
                    ellipse_ContentInfo_Rating_Users.Visibility = Visibility.Collapsed;
                }

                if (igdbGames.aggregated_rating_count > 0)
                {
                    int gameRating = Convert.ToInt32(igdbGames.aggregated_rating);
                    textblock_ContentInfo_Rating_Critics.Text = gameRating.ToString();
                    textblock_ContentInfo_Rating_Critics_Count.Text = igdbGames.aggregated_rating_count + "x";
                    border_ContentInfo_Rating_Critics.Background = GameRatingToSolidColorBrush(gameRating);
                    border_ContentInfo_Rating_Critics.Visibility = Visibility.Visible;
                }
                else
                {
                    border_ContentInfo_Rating_Critics.Visibility = Visibility.Collapsed;
                }

                //Set gallery images
                if (igdbGames.screenshots != null)
                {
                    listbox_ContentInfo_Gallery.Items.Clear();
                    listbox_ContentInfo_Gallery.Visibility = Visibility.Visible;
                    foreach (ApiIGDBImages infoImages in igdbGames.screenshots)
                    {
                        BitmapImage screenshotImage = FileToBitmapImage(["https://images.igdb.com/igdb/image/upload/t_720p/" + infoImages.image_id + ".png"], null, null, vImageLoadSize, 0, IntPtr.Zero, 0);
                        listbox_ContentInfo_Gallery.Items.Add(screenshotImage);
                    }
                }
                else
                {
                    listbox_ContentInfo_Gallery.Visibility = Visibility.Collapsed;
                }

                //Set description
                textblock_ContentInfo_Description.Text = ApiIGDB_GameSummaryString(igdbGames, true, true, true, true, true, true);

                //Update popup variables
                vContentInformationOpen = true;

                //Show the popup
                Popup_Show_Element(grid_Popup_ContentInformation);

                //Focus on button
                await FocusElement(btn_ContentInfo_Focus, vProcessCurrent.WindowHandleMain);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Information show error: " + ex.Message);
            }
        }

        //Get rating solid color brush
        private SolidColorBrush GameRatingToSolidColorBrush(int gameRating)
        {
            try
            {
                if (gameRating >= 75)
                {
                    return (SolidColorBrush)Application.Current.Resources["ApplicationValidBrush"];
                }
                else if (gameRating >= 50)
                {
                    return (SolidColorBrush)Application.Current.Resources["ApplicationIgnoredBrush"];
                }
                else
                {
                    return (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                }
            }
            catch { }
            return new SolidColorBrush(Colors.Gray);
        }
    }
}