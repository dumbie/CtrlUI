using ArnoldVinkCode;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle the selection button
        async void Grid_Popup_FilePicker_button_SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Popup_Close_FilePicker(true, true);
            }
            catch { }
        }

        async void Button_FilePicker_button_ControllerRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Popup_Close_FilePicker(false, false);
            }
            catch { }
        }

        async void Button_FilePicker_button_ControllerUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FilePicker_GoFolderUp();
            }
            catch { }
        }

        async void Button_FilePicker_button_ControllerLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FilePicker_SortFilesFolders(false);
            }
            catch { }
        }

        //Sort files and folders in picker
        async Task FilePicker_SortFilesFolders(bool Silent)
        {
            try
            {
                if (grid_Popup_FilePicker_button_ControllerLeft.Visibility == Visibility.Visible)
                {
                    if (vFilePickerSortByName)
                    {
                        if (!Silent)
                        {
                            Debug.WriteLine("Sorting files and folders by date");
                            Popup_Show_Status("Sorting", "Sorting by date");
                        }

                        vFilePickerSortByName = false;
                        await Popup_Show_FilePicker(vFilePickerCurrentPath, lb_FilePicker.SelectedIndex, false, null);
                    }
                    else
                    {
                        if (!Silent)
                        {
                            Debug.WriteLine("Sorting files and folders by name");
                            Popup_Show_Status("Sorting", "Sorting by name");
                        }

                        vFilePickerSortByName = true;
                        await Popup_Show_FilePicker(vFilePickerCurrentPath, lb_FilePicker.SelectedIndex, false, null);
                    }
                }
            }
            catch { }
        }

        //Go a folder up in the file picker
        async Task FilePicker_GoFolderUp()
        {
            try
            {
                if (grid_Popup_FilePicker_button_ControllerUp.Visibility == Visibility.Visible)
                {
                    //Set the navigate index
                    int navigateIndex = -1;
                    if (vFilePickerNavigateIndexes.Any())
                    {
                        //Read the last index
                        navigateIndex = vFilePickerNavigateIndexes.LastOrDefault();

                        //Remove the last index
                        int navigateCount = vFilePickerNavigateIndexes.Count;
                        vFilePickerNavigateIndexes.RemoveAt(navigateCount - 1);
                    }

                    //Read the root path
                    DataBindFile dataBindFile = List_FilePicker.FirstOrDefault();
                    if (dataBindFile.Type == "GoUp")
                    {
                        Debug.WriteLine("Folder up: " + dataBindFile.PathFile + " / " + navigateIndex);
                        await Popup_Show_FilePicker(dataBindFile.PathFile, navigateIndex, false, null);
                    }
                    else
                    {
                        Debug.WriteLine("No folder to navigate go up / not up.");
                        Popup_Show_Status("Up", "No folder to go up");
                    }
                }
            }
            catch
            {
                Debug.WriteLine("No folder to navigate go up / catch.");
                Popup_Show_Status("Up", "No folder to go up");
            }
        }

        //Load rom information when selection has changed
        void Lb_FilePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (vFilePickerShowRoms)
                {
                    ListBox ListboxSender = (ListBox)sender;
                    DataBindFile SelectedItem = (DataBindFile)ListboxSender.SelectedItem;
                    //Debug.WriteLine("File picker selection has changed to: " + SelectedItem.Name);

                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Visible;
                    grid_Popup_FilePicker_image_Description.Source = SelectedItem.ImageBitmap;
                    if (string.IsNullOrWhiteSpace(SelectedItem.Description))
                    {
                        grid_Popup_FilePicker_textblock_Description.Text = "There is no description available.";
                    }
                    else
                    {
                        grid_Popup_FilePicker_textblock_Description.Text = SelectedItem.Description;
                    }
                }
            }
            catch { }
        }

        //Handle file picker mouse/touch tapped
        async void ListBox_FilePicker_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (vMousePressDownXButton1)
                    {
                        await FilePicker_GoFolderUp();
                    }
                    else if (vMousePressDownLeftClick)
                    {
                        await lb_FilePicker_LeftClick();
                    }
                }
            }
            catch { }
        }

        //Handle file picker keyboard/controller tapped
        async void ListBox_FilePicker_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await lb_FilePicker_LeftClick();
                }
                else if (e.Key == Key.Back)
                {
                    await FilePicker_GoFolderUp();
                }
                else if (e.Key == Key.Delete)
                {
                    await FilePicker_SortFilesFolders(false);
                }
            }
            catch { }
        }
        void ListBox_FilePicker_KeyPressDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Up && lb_FilePicker.SelectedIndex == 0)
                {
                    //Improve: KeySendCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                    KeyPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);
                }
                else if (e.Key == Key.Down && (lb_FilePicker.Items.Count - 1) == lb_FilePicker.SelectedIndex)
                {
                    KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                }
            }
            catch { }
        }

        //Handle file picker left click
        async Task lb_FilePicker_LeftClick()
        {
            try
            {
                if (lb_FilePicker.SelectedItems.Count > 0 && lb_FilePicker.SelectedIndex != -1)
                {
                    DataBindFile SelectedItem = (DataBindFile)lb_FilePicker.SelectedItem;
                    if (SelectedItem.Type == "Directory")
                    {
                        await Popup_Show_FilePicker(SelectedItem.PathFile, -1, true, null);
                    }
                    else if (SelectedItem.Type == "PreDirectory")
                    {
                        await Popup_Show_FilePicker(SelectedItem.PathFile, -1, false, null);
                    }
                    else if (SelectedItem.Type == "GoUp")
                    {
                        await FilePicker_GoFolderUp();
                    }
                    else
                    {
                        await Popup_Close_FilePicker(true, false);
                    }
                }
            }
            catch { }
        }
    }
}