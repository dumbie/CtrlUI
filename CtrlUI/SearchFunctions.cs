using ArnoldVinkCode;
using ArnoldVinkCode.Styles;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVFocus;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Reset search to defaults
        async Task Search_Reset(bool focusTextbox)
        {
            try
            {
                Debug.WriteLine("Resetting search to defaults.");

                //Clear the current popup list
                List_Search.Clear();

                if (focusTextbox)
                {
                    //Empty textbox text
                    grid_Search_textbox.Text = string.Empty;

                    //Force focus on element
                    await FocusElement(grid_Search_textbox, vProcessCurrent.WindowHandleMain);
                }
                else
                {
                    //Set placeholder text
                    string placeholderString = (string)grid_Search_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                    grid_Search_textbox.Text = placeholderString;
                }

                grid_Search_textblock_Result.Text = "Please enter a search term.";
                grid_Search_textblock_Result.Visibility = Visibility.Visible;
            }
            catch { }
        }

        //Add process to the search list
        async Task AddSearchProcess(DataBindApp dataBindApp)
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    string searchString = grid_Search_textbox.Text;
                    string placeholderString = (string)grid_Search_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                    if (!string.IsNullOrWhiteSpace(searchString) && searchString != placeholderString && dataBindApp.Name.ToLower().Contains(searchString.ToLower()))
                    {
                        //Set search category image to databind app
                        SearchAppSetCategoryImage(dataBindApp);

                        //Add search result to listbox
                        await ListBoxAddItem(lb_Search, List_Search, dataBindApp, false, false);

                        //Update the search results count
                        UpdateSearchResults();

                        Debug.WriteLine("Added search process: " + searchString);
                    }
                });
            }
            catch { }
        }

        //Set search category image to databind app
        private static void SearchAppSetCategoryImage(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.Category == AppCategory.App)
                {
                    dataBindApp.StatusSearchCategoryImage = vImagePreloadApp;
                }
                else if (dataBindApp.Category == AppCategory.Game)
                {
                    dataBindApp.StatusSearchCategoryImage = vImagePreloadGame;
                }
                else if (dataBindApp.Category == AppCategory.Emulator)
                {
                    dataBindApp.StatusSearchCategoryImage = vImagePreloadEmulator;
                }
                else if (dataBindApp.Category == AppCategory.Process)
                {
                    dataBindApp.StatusSearchCategoryImage = vImagePreloadProcess;
                }
                else if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    dataBindApp.StatusSearchCategoryImage = vImagePreloadShortcut;
                }
            }
            catch { }
        }

        //Update the search results count
        void UpdateSearchResults()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    string searchString = grid_Search_textbox.Text;
                    string placeholderString = (string)grid_Search_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                    if (string.IsNullOrWhiteSpace(searchString) || searchString == placeholderString)
                    {
                        grid_Search_textblock_Result.Text = "Please enter a search term.";
                        grid_Search_textblock_Result.Visibility = Visibility.Visible;
                    }
                    else if (List_Search.Count == 0)
                    {
                        grid_Search_textblock_Result.Text = "No search results found.";
                        grid_Search_textblock_Result.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grid_Search_textblock_Result.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch { }
        }
    }
}