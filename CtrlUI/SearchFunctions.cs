using ArnoldVinkCode;
using ArnoldVinkCode.Styles;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.FocusFunctions;

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
                    await FrameworkElementFocus(grid_Search_textbox, false, vProcessCurrent.MainWindowHandle);
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
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    string searchString = grid_Search_textbox.Text;
                    string placeholderString = (string)grid_Search_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                    if (!string.IsNullOrWhiteSpace(searchString) && searchString != placeholderString && dataBindApp.Name.ToLower().Contains(searchString.ToLower()))
                    {
                        //Add search process
                        await ListBoxAddItem(lb_Search, List_Search, dataBindApp, false, false);

                        //Update the search results count
                        UpdateSearchResults();

                        Debug.WriteLine("Added search process: " + searchString);
                    }
                });
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