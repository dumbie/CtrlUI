using ArnoldVinkCode;
using ArnoldVinkCode.Styles;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Hide or show the search
        async Task Popup_ShowHide_Search(bool ForceShow)
        {
            try
            {
                if (vSearchOpen)
                {
                    await Popup_Close_Top();
                    return;
                }

                if (ForceShow)
                {
                    await Popup_Close_All();
                }

                if (Popup_Any_Open())
                {
                    return;
                }

                //Reset the popup to defaults
                await Popup_Reset_Search(false);

                //Show the search popup
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Save the previous focus element
                Popup_PreviousElementFocus_Save(vSearchElementFocus, null);

                //Show the popup
                Popup_Show_Element(grid_Popup_Search);

                vSearchOpen = true;

                //Force focus on element
                await FocusOnElement(grid_Popup_Search_textbox, false, vProcessCurrent.MainWindowHandle);

                //Launch the keyboard controller
                if (vAppActivated && vControllerAnyConnected())
                {
                    await KeyboardControllerHideShow(true);
                }
            }
            catch { }
        }

        //Reset the popup to defaults
        async Task Popup_Reset_Search(bool FocusTextbox)
        {
            try
            {
                //Clear the current popup list
                List_Search.Clear();

                //Reset the search text
                grid_Popup_Search_Count_TextBlock.Text = string.Empty;

                if (FocusTextbox)
                {
                    //Empty the textbox text
                    grid_Popup_Search_textbox.Text = string.Empty;

                    //Force focus on element
                    await FocusOnElement(grid_Popup_Search_textbox, false, vProcessCurrent.MainWindowHandle);
                }
                else
                {
                    string placeholderString = (string)grid_Popup_Search_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                    grid_Popup_Search_textbox.Text = placeholderString;
                }

                grid_Popup_Search_textblock_Result.Text = "Please enter a search term above.";
                grid_Popup_Search_textblock_Result.Visibility = Visibility.Visible;

                //Show or hide the keyboard icon
                if (CheckKeyboardEnabled())
                {
                    grid_Popup_Search_button_KeyboardControllerIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_Popup_Search_button_KeyboardControllerIcon.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        async Task Popup_Close_Search()
        {
            try
            {
                if (vSearchOpen)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Reset popup variables
                    vSearchOpen = false;

                    //Clear the current popup list
                    List_Search.Clear();

                    //Hide the popup
                    Popup_Hide_Element(grid_Popup_Search);

                    //Focus on the previous focus element
                    await Popup_PreviousElementFocus_Focus(vSearchElementFocus);
                }
            }
            catch { }
        }

        //Update the search results count
        void UpdateSearchResults()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    string searchString = grid_Popup_Search_textbox.Text;
                    string placeholderString = (string)grid_Popup_Search_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                    if (string.IsNullOrWhiteSpace(searchString) || searchString == placeholderString)
                    {
                        grid_Popup_Search_Count_TextBlock.Text = string.Empty;
                        grid_Popup_Search_textblock_Result.Text = "Please enter a search term above.";
                        grid_Popup_Search_textblock_Result.Visibility = Visibility.Visible;
                    }
                    else if (List_Search.Count == 0)
                    {
                        grid_Popup_Search_Count_TextBlock.Text = string.Empty;
                        grid_Popup_Search_textblock_Result.Text = "No search results found.";
                        grid_Popup_Search_textblock_Result.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grid_Popup_Search_Count_TextBlock.Text = " " + List_Search.Count.ToString();
                        grid_Popup_Search_textblock_Result.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch { }
        }
    }
}