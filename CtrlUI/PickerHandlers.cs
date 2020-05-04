using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

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
                await FilePicker_Actions();
            }
            catch { }
        }

        //File and folder actions
        private async Task FilePicker_Actions()
        {
            try
            {
                //Check if actions are enabled
                if (grid_Popup_FilePicker_button_ControllerLeft.Visibility != Visibility.Visible)
                {
                    Debug.WriteLine("File and folders action cancelled, no actions available.");
                    return;
                }

                //Get the selected list item
                DataBindFile selectedItem = (DataBindFile)lb_FilePicker.SelectedItem;

                //Check the selected file type
                if (selectedItem.FileType == FileType.UwpApp)
                {
                    //Add answers for messagebox
                    List<DataBindString> Answers = new List<DataBindString>();

                    DataBindString answerUpdate = new DataBindString();
                    answerUpdate.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Refresh.png" }, IntPtr.Zero, -1, 0);
                    answerUpdate.Name = "Check application update";
                    Answers.Add(answerUpdate);

                    DataBindString answerRemove = new DataBindString();
                    answerRemove.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/RemoveCross.png" }, IntPtr.Zero, -1, 0);
                    answerRemove.Name = "Remove the application";
                    Answers.Add(answerRemove);

                    //Show the messagebox prompt
                    DataBindString messageResult = await Popup_Show_MessageBox("Application actions", "", "Please select an action that you want to use on: " + selectedItem.Name, Answers);
                    if (messageResult != null)
                    {
                        //Update application
                        if (messageResult == answerUpdate)
                        {
                            UwpListUpdateApplication(selectedItem);
                        }
                        //Remove application
                        else if (messageResult == answerRemove)
                        {
                            await UwpListRemoveApplication(selectedItem);
                        }
                    }
                }
                else
                {
                    //Add answers for messagebox
                    List<DataBindString> Answers = new List<DataBindString>();

                    //Check the file type
                    bool preFile = selectedItem.FileType == FileType.PreFolder || selectedItem.FileType == FileType.PreFile || selectedItem.FileType == FileType.GoUp;

                    //Check the sorting type
                    string sortType = string.Empty;
                    if (vFilePickerSortByName)
                    {
                        sortType = "Sort files and folders by date";
                    }
                    else
                    {
                        sortType = "Sort files and folders by name";
                    }

                    DataBindString answerSort = new DataBindString();
                    answerSort.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Sorting.png" }, IntPtr.Zero, -1, 0);
                    answerSort.Name = sortType;
                    Answers.Add(answerSort);

                    DataBindString answerCopy = new DataBindString();
                    if (!preFile)
                    {
                        answerCopy.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Copy.png" }, IntPtr.Zero, -1, 0);
                        answerCopy.Name = "Copy the file or folder";
                        Answers.Add(answerCopy);
                    }

                    DataBindString answerCut = new DataBindString();
                    if (!preFile)
                    {
                        answerCut.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Cut.png" }, IntPtr.Zero, -1, 0);
                        answerCut.Name = "Cut the file or folder";
                        Answers.Add(answerCut);
                    }

                    DataBindString answerPaste = new DataBindString();
                    if (vClipboardFile != null)
                    {
                        answerPaste.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Paste.png" }, IntPtr.Zero, -1, 0);
                        answerPaste.Name = "Paste (" + vClipboardFile.FileType.ToString() + " " + vClipboardFile.ClipboardType.ToString() + ") " + vClipboardFile.Name;
                        Answers.Add(answerPaste);
                    }

                    DataBindString answerRename = new DataBindString();
                    if (!preFile)
                    {
                        answerRename.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Rename.png" }, IntPtr.Zero, -1, 0);
                        answerRename.Name = "Rename the file or folder";
                        Answers.Add(answerRename);
                    }

                    DataBindString answerCreateFolder = new DataBindString();
                    answerCreateFolder.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/FolderAdd.png" }, IntPtr.Zero, -1, 0);
                    answerCreateFolder.Name = "Create a new folder here";
                    Answers.Add(answerCreateFolder);

                    DataBindString answerCreateTextFile = new DataBindString();
                    answerCreateTextFile.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/FileTxt.png" }, IntPtr.Zero, -1, 0);
                    answerCreateTextFile.Name = "Create a new text file here";
                    Answers.Add(answerCreateTextFile);

                    DataBindString answerRemove = new DataBindString();
                    if (!preFile)
                    {
                        answerRemove.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Remove.png" }, IntPtr.Zero, -1, 0);
                        answerRemove.Name = "Move file or folder to recycle bin*";
                        Answers.Add(answerRemove);
                    }

                    DataBindString answerDownloadRomInfo = new DataBindString();
                    if (!preFile && vFilePickerShowRoms)
                    {
                        answerDownloadRomInfo.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Download.png" }, IntPtr.Zero, -1, 0);
                        answerDownloadRomInfo.Name = "Download rom game information";
                        Answers.Add(answerDownloadRomInfo);
                    }

                    DataBindString answerDownloadConsoleInfo = new DataBindString();
                    if (!preFile && vFilePickerShowRoms)
                    {
                        answerDownloadConsoleInfo.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Icons/Download.png" }, IntPtr.Zero, -1, 0);
                        answerDownloadConsoleInfo.Name = "Download console information";
                        Answers.Add(answerDownloadConsoleInfo);
                    }

                    //Show the messagebox prompt
                    DataBindString messageResult = await Popup_Show_MessageBox("File and folder actions", "* Files and folders on a network drive get permanently deleted.", "Please select an action that you want to use on: " + selectedItem.Name, Answers);
                    if (messageResult != null)
                    {
                        //Sort files and folders
                        if (messageResult == answerSort)
                        {
                            await FilePicker_SortFilesFolders(false);
                        }
                        //Copy file or folder
                        else if (messageResult == answerCopy)
                        {
                            FilePicker_FileCopy(selectedItem);
                        }
                        //Cut file or folder
                        else if (messageResult == answerCut)
                        {
                            FilePicker_FileCut(selectedItem);
                        }
                        //Paste file or folder
                        else if (messageResult == answerPaste)
                        {
                            async void TaskAction()
                            {
                                try
                                {
                                    await FilePicker_FilePaste();
                                }
                                catch { }
                            }
                            await AVActions.TaskStart(TaskAction, null);
                        }
                        //Rename file or folder
                        else if (messageResult == answerRename)
                        {
                            await FilePicker_FileRename(selectedItem);
                        }
                        //Create a new folder
                        else if (messageResult == answerCreateFolder)
                        {
                            await FilePicker_CreateFolder();
                        }
                        //Create a new text file
                        else if (messageResult == answerCreateTextFile)
                        {
                            await FilePicker_CreateTextFile();
                        }
                        //Remove file or folder
                        else if (messageResult == answerRemove)
                        {
                            await FilePicker_FileRemove(selectedItem, true);
                        }
                        //Download rom game information
                        else if (messageResult == answerDownloadRomInfo)
                        {
                            RomInformation informationDownloaded = await RomDownloadInformation(selectedItem.Name, 210);
                            if (informationDownloaded != null)
                            {
                                selectedItem.Description = informationDownloaded.RomDescription;
                                if (informationDownloaded.RomImageBitmap != null)
                                {
                                    selectedItem.ImageBitmap = informationDownloaded.RomImageBitmap;
                                }
                            }
                        }
                        //Download console information
                        else if (messageResult == answerDownloadConsoleInfo)
                        {
                            RomInformation informationDownloaded = await ConsoleDownloadInformation(selectedItem.Name, 210);
                            if (informationDownloaded != null)
                            {
                                selectedItem.Description = informationDownloaded.RomDescription;
                                if (informationDownloaded.RomImageBitmap != null)
                                {
                                    selectedItem.ImageBitmap = informationDownloaded.RomImageBitmap;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        //Sort files and folders in picker
        async Task FilePicker_SortFilesFolders(bool silent)
        {
            try
            {
                if (vFilePickerSortByName)
                {
                    if (!silent)
                    {
                        Debug.WriteLine("Sorting files and folders by date");
                        Popup_Show_Status("Sorting", "Sorting by date");
                    }

                    vFilePickerSortByName = false;
                    await Popup_Show_FilePicker(vFilePickerCurrentPath, lb_FilePicker.SelectedIndex, false, null);
                }
                else
                {
                    if (!silent)
                    {
                        Debug.WriteLine("Sorting files and folders by name");
                        Popup_Show_Status("Sorting", "Sorting by name");
                    }

                    vFilePickerSortByName = true;
                    await Popup_Show_FilePicker(vFilePickerCurrentPath, lb_FilePicker.SelectedIndex, false, null);
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
                    if (dataBindFile.FileType == FileType.GoUp)
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

                    //Show the rom information
                    grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Visible;

                    //Set image binding
                    Binding imageBinding = new Binding();
                    imageBinding.Path = new PropertyPath("ImageBitmap");
                    imageBinding.Source = SelectedItem;
                    grid_Popup_FilePicker_image_Description.SetBinding(Image.SourceProperty, imageBinding);

                    //Set text binding
                    Binding textBinding = new Binding();
                    textBinding.Path = new PropertyPath("Description");
                    textBinding.Source = SelectedItem;
                    grid_Popup_FilePicker_textblock_Description.SetBinding(TextBlock.TextProperty, textBinding);
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
                    else if (vMousePressDownRightClick)
                    {
                        await lb_FilePicker_RightClick();
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
                    await FilePicker_Actions();
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
                    if (SelectedItem.FileType == FileType.Folder)
                    {
                        await Popup_Show_FilePicker(SelectedItem.PathFile, -1, true, null);
                    }
                    else if (SelectedItem.FileType == FileType.PreFolder)
                    {
                        await Popup_Show_FilePicker(SelectedItem.PathFile, -1, false, null);
                    }
                    else if (SelectedItem.FileType == FileType.GoUp)
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

        //Handle file picker right click
        async Task lb_FilePicker_RightClick()
        {
            try
            {
                if (lb_FilePicker.SelectedItems.Count > 0 && lb_FilePicker.SelectedIndex != -1)
                {
                    await FilePicker_Actions();
                }
            }
            catch { }
        }
    }
}