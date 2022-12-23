using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.FocusFunctions;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the File Picker Popup Task
        async Task Popup_Show_FilePicker(string targetPath, int targetIndex, bool storeIndex, FrameworkElement previousFocus)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        await Popup_Show_FilePicker_Task(targetPath, targetIndex, storeIndex, previousFocus);
                    }
                    catch { }
                }
                await AVActions.TaskStartReturn(TaskAction);
            }
            catch { }
        }

        //Show the File Picker Popup
        async Task Popup_Show_FilePicker_Task(string targetPath, int targetIndex, bool storeIndex, FrameworkElement previousFocus)
        {
            try
            {
                //Check if the popup is already open
                if (!vFilePickerOpen)
                {
                    //Play the popup opening sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                    //Save the previous focus element
                    FrameworkElementFocusSave(vFilePickerElementFocus, previousFocus);
                }

                //Reset file picker variables
                vFilePickerCompleted = false;
                vFilePickerCancelled = false;
                vFilePickerResult = null;
                vFilePickerOpen = true;

                //Check file picker busy status
                while (vFilePickerLoadBusy)
                {
                    vFilePickerLoadCancel = true;
                    await Task.Delay(100);
                }
                vFilePickerLoadBusy = true;

                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Show file picker loading animation
                    gif_FilePicker_Loading.Show();

                    //Set file picker header texts
                    grid_Popup_FilePicker_txt_Title.Text = vFilePickerSettings.Title;
                    grid_Popup_FilePicker_txt_Description.Text = vFilePickerSettings.Description;

                    //Change the file picker interface
                    if (vFilePickerSettings.ShowEmulatorInterface)
                    {
                        //Change list picker style
                        lb_FilePicker.Style = Application.Current.Resources["ListBoxWrapPanelVertical"] as Style;
                        lb_FilePicker.ItemTemplate = Application.Current.Resources["ListBoxItemRom"] as DataTemplate;

                        //Show rom information
                        grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //Change list picker style
                        lb_FilePicker.Style = Application.Current.Resources["ListBoxVertical"] as Style;
                        lb_FilePicker.ItemTemplate = Application.Current.Resources["ListBoxItemFile"] as DataTemplate;

                        //Hide rom information
                        grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                    }

                    //Update the navigation history index
                    if (storeIndex)
                    {
                        FilePicker_NavigationHistoryAddUpdate(vFilePickerCurrentPath, lb_FilePicker.SelectedIndex);
                    }

                    //Clear the current file picker list
                    List_FilePicker.Clear();
                });

                //Show the popup
                Popup_Show_Element(grid_Popup_FilePicker);

                //Update the current picker path
                vFilePickerSourcePath = vFilePickerCurrentPath;
                vFilePickerCurrentPath = targetPath;

                //Check target type
                if (targetPath == "PC")
                {
                    //Get and list all the disk drives
                    await FilePicker_LoadPC();

                    //File Picker focus on item
                    await FilePicker_Focus(targetIndex, targetPath);
                }
                else if (targetPath == "UWP")
                {
                    //Get and list all uwp applications
                    await FilePicker_LoadUwpApps();

                    //File Picker focus on item
                    await FilePicker_Focus(targetIndex, targetPath);
                }
                else
                {
                    //Get and list all files and folders
                    await FilePicker_LoadFilesFolders(targetPath);

                    //File Picker focus on item
                    await FilePicker_Focus(targetIndex, targetPath);

                    //Load image and descriptions
                    await FilePicker_LoadDetails();
                }

                //Hide file picker loading animation
                AVActions.ActionDispatcherInvoke(delegate
                {
                    gif_FilePicker_Loading.Hide();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading filepicker: " + ex.Message);
                await Notification_Send_Status("Close", "Picker loading failed");
                await FilePicker_GoFolderUp();
            }
            finally
            {
                //Update file picker loading status
                vFilePickerLoadCancel = false;
                vFilePickerLoadBusy = false;
            }
        }

        //File Picker focus on item
        async Task<bool> FilePicker_Focus(int targetIndex, string targetPath)
        {
            try
            {
                //Get navigation history index
                if (targetIndex == -1)
                {
                    targetIndex = FilePicker_NavigationHistoryGetIndex(targetPath);
                }

                //Check the navigation index
                if (targetIndex == -1 && !string.IsNullOrWhiteSpace(vFilePickerSourcePath))
                {
                    DataBindFile sourceFileItem = List_FilePicker.Where(x => x.PathFile == vFilePickerSourcePath).FirstOrDefault();
                    if (sourceFileItem != null)
                    {
                        Debug.WriteLine("Source file path found: " + vFilePickerSourcePath);

                        //Focus on the file picker listbox item
                        await ListBoxFocusItem(lb_FilePicker, sourceFileItem, vProcessCurrent.MainWindowHandle);
                        return true;
                    }
                }

                //Focus on the file picker listbox index
                await ListboxFocusIndex(lb_FilePicker, false, false, targetIndex, vProcessCurrent.MainWindowHandle);
                return true;
            }
            catch { }
            return false;
        }

        //File Picker change select mode
        void FilePicker_ChangeSelectMode(bool folderMode)
        {
            try
            {
                if (folderMode)
                {
                    vFilePickerFolderSelectMode = true;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Visible;
                        grid_Popup_FilePicker_button_ControllerStart.ToolTip = new ToolTip() { Content = "Use the currently opened folder" };
                    });
                }
                else
                {
                    vFilePickerFolderSelectMode = false;
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;
                        grid_Popup_FilePicker_button_ControllerStart.ToolTip = new ToolTip() { Content = "Select file or folder" };
                    });
                }
            }
            catch { }
        }

        //File Picker check item
        void FilePicker_CheckItem()
        {
            try
            {
                DataBindFile dataBindFile = (DataBindFile)lb_FilePicker.SelectedItem;

                //Check the file or folder
                if (vFilePickerCurrentPath == "PC" || dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUpPre)
                {
                    Debug.WriteLine("Invalid file type, cannot be selected.");
                    return;
                }

                //Check or uncheck item
                if (dataBindFile.Checked != Visibility.Visible)
                {
                    dataBindFile.Checked = Visibility.Visible;
                }
                else
                {
                    dataBindFile.Checked = Visibility.Collapsed;
                }
            }
            catch { }
        }

        //File Picker add or update navigation history
        public static void FilePicker_NavigationHistoryAddUpdate(string targetPath, int targetIndex)
        {
            try
            {
                PickerNavigation navigateHistory = vFilePickerNavigationHistory.Where(x => x.Path.ToLower() == targetPath.ToLower()).FirstOrDefault();
                if (navigateHistory != null)
                {
                    //Update navigation history
                    navigateHistory.Index = targetIndex;
                    Debug.WriteLine("Updated picker history: " + targetIndex + " / " + targetPath);
                }
                else
                {
                    //Add navigation history
                    PickerNavigation navigateHistoryNew = new PickerNavigation();
                    navigateHistoryNew.Path = targetPath;
                    navigateHistoryNew.Index = targetIndex;
                    vFilePickerNavigationHistory.Add(navigateHistoryNew);
                    Debug.WriteLine("Added picker history: " + targetIndex + " / " + targetPath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add or update picker history: " + ex.Message);
            }
        }

        //File Picker get index from navigation history
        public static int FilePicker_NavigationHistoryGetIndex(string targetPath)
        {
            try
            {
                Debug.WriteLine("Looking for navigation index: " + targetPath);
                PickerNavigation navigateHistory = vFilePickerNavigationHistory.Where(x => x.Path.ToLower() == targetPath.ToLower()).FirstOrDefault();
                if (navigateHistory != null)
                {
                    return navigateHistory.Index;
                }
            }
            catch { }
            return -1;
        }

        //Check if there are files or folders
        void FilePicker_CheckFilesAndFoldersCount()
        {
            try
            {
                int totalFileCount = List_FilePicker.Count - 1; //Filter out GoUp
                if (totalFileCount > 0)
                {
                    //Enable or disable file and folder availability
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;
                        grid_Popup_FilePicker_textblock_CurrentPath.Text = "Current path: " + vFilePickerCurrentPath + " (" + totalFileCount + " items)";
                    });
                    Debug.WriteLine("There are files and folders in the list.");
                }
                else
                {
                    //Enable or disable file and folder availability
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Visible;
                    });
                    Debug.WriteLine("No files and folders in the list.");
                }
            }
            catch { }
        }

        //Close file picker popup
        async Task Popup_Close_FilePicker(bool IsCompleted, bool CurrentFolder)
        {
            try
            {
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                //Cancel file picker load
                while (vFilePickerLoadBusy)
                {
                    vFilePickerLoadCancel = true;
                    await Task.Delay(100);
                }

                //Reset file picker variables
                vFilePickerOpen = false;
                if (IsCompleted)
                {
                    vFilePickerCompleted = true;
                    if (CurrentFolder)
                    {
                        DataBindFile targetPath = new DataBindFile();
                        targetPath.PathFile = vFilePickerCurrentPath;
                        vFilePickerResult = targetPath;
                    }
                    else
                    {
                        vFilePickerResult = (DataBindFile)lb_FilePicker.SelectedItem;
                    }
                }
                else
                {
                    vFilePickerCancelled = true;
                }

                //Update the navigation history index
                FilePicker_NavigationHistoryAddUpdate(vFilePickerCurrentPath, lb_FilePicker.SelectedIndex);

                //Update the previous picker path
                if (vFilePickerCurrentPath[1] == ':')
                {
                    vFilePickerPreviousPath = vFilePickerCurrentPath;
                }

                //Clear the current file picker list
                List_FilePicker.Clear();

                //Hide the popup
                Popup_Hide_Element(grid_Popup_FilePicker);

                //Focus on the previous focus element
                await FrameworkElementFocusFocus(vFilePickerElementFocus, vProcessCurrent.MainWindowHandle);
            }
            catch { }
        }
    }
}