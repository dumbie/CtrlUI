using ArnoldVinkCode.Styles;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
                await lb_AppList_RightClick(lb_Search);
            }
            catch { }
        }

        async void grid_Search_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchString = grid_Search_textbox.Text;
                string placeholderString = (string)grid_Search_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                if (!string.IsNullOrWhiteSpace(searchString) && searchString != placeholderString)
                {
                    //Clear the current popup list
                    List_Search.Clear();

                    //Search and add applications
                    IEnumerable<DataBindApp> searchResult = CombineAppLists(true, true, true).Where(x => x.Name.ToLower().Contains(searchString.ToLower()));
                    foreach (DataBindApp dataBindApp in searchResult)
                    {
                        try
                        {
                            await ListBoxAddItem(lb_Search, List_Search, dataBindApp, false, false);
                        }
                        catch { }
                    }

                    //Update the search results count
                    UpdateSearchResults();

                    //Select the first search index
                    lb_Search.SelectedIndex = 0;

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