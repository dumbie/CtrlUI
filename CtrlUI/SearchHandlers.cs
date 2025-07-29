using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Reset the popup to defaults
        async void grid_Search_button_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Search_Reset(true);
            }
            catch { }
        }

        async void Button_SearchInteractItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ListView_Apps_RightClick(listView_Search);
            }
            catch { }
        }

        async void grid_Search_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchString = grid_Search_textbox.Text;
                string placeholderString = grid_Search_textbox.PlaceholderText;
                if (!string.IsNullOrWhiteSpace(searchString) && searchString != placeholderString)
                {
                    //Clear the current popup list
                    List_Search.Clear();

                    //Search and add applications
                    IEnumerable<DataBindApp> searchResult = CombineAppLists(true, true, true, true, true, true, true).Where(x => x.Name.ToLower().Contains(searchString.ToLower()));
                    foreach (DataBindApp dataBindApp in searchResult)
                    {
                        try
                        {
                            //Set search category image to databind app
                            await SearchAppSetCategoryImage(dataBindApp);

                            //Add search result to listbox
                            await ListViewAddItem(listView_Search, List_Search, dataBindApp, false, false);
                        }
                        catch { }
                    }

                    //Update search result gallery images
                    UpdateGalleryMediaImages(true);

                    //Update search results count
                    UpdateSearchResults();

                    //Select first search index
                    listView_Search.SelectedIndex = 0;

                    Debug.WriteLine("Added search application: " + searchString);
                }
                else
                {
                    //Clear the current popup list
                    List_Search.Clear();

                    //Reset the search text
                    grid_Search_textblock_Result.Text = "Please enter a search term.";
                    grid_Search_textblock_Result.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }
    }
}