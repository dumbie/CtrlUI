using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVImage;
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
                    grid_Search_textbox.Text = grid_Search_textbox.PlaceholderText;
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
                await DispatcherInvoke(this.Dispatcher, async delegate
                {
                    string searchString = grid_Search_textbox.Text;
                    string placeholderString = grid_Search_textbox.PlaceholderText;
                    if (!string.IsNullOrWhiteSpace(searchString) && searchString != placeholderString && dataBindApp.Name.ToLower().Contains(searchString.ToLower()))
                    {
                        //Set search category image to databind app
                        await SearchAppSetCategoryImage(dataBindApp);

                        //Add search result to listbox
                        await ListViewAddItem(listView_Search, List_Search, dataBindApp, false, false);

                        //Update the search results count
                        UpdateSearchResults();

                        Debug.WriteLine("Added search process: " + searchString);
                    }
                });
            }
            catch { }
        }

        //Set search category image to databind app
        private async Task SearchAppSetCategoryImage(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.Category == AppCategory.App)
                {
                    dataBindApp.StatusSearchCategoryImage = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/App.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                }
                else if (dataBindApp.Category == AppCategory.Game)
                {
                    dataBindApp.StatusSearchCategoryImage = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Game.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                }
                else if (dataBindApp.Category == AppCategory.Emulator)
                {
                    dataBindApp.StatusSearchCategoryImage = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Emulator.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                }
                else if (dataBindApp.Category == AppCategory.Launcher)
                {
                    dataBindApp.StatusSearchCategoryImage = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Launcher.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                }
                else if (dataBindApp.Category == AppCategory.Process)
                {
                    dataBindApp.StatusSearchCategoryImage = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Process.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                }
                else if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    dataBindApp.StatusSearchCategoryImage = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Shortcut.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                }
                else if (dataBindApp.Category == AppCategory.Gallery)
                {
                    dataBindApp.StatusSearchCategoryImage = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Image.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                }
            }
            catch { }
        }

        //Update the search results count
        void UpdateSearchResults()
        {
            try
            {
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    string searchString = grid_Search_textbox.Text;
                    string placeholderString = grid_Search_textbox.PlaceholderText;
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