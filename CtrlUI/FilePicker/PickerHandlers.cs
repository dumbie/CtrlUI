using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle the folder select button
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

        async void Grid_Popup_FilePicker_button_ControllerBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FilePicker_SortFilesFoldersSwitch(false);
            }
            catch { }
        }

        async void Button_FilePicker_button_ControllerStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vFilePickerFolderSelectMode)
                {
                    await Popup_Close_FilePicker(true, true);
                }
                else
                {
                    FilePicker_CheckItem();
                }
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
                //Get the selected list item
                DataBindFile selectedItem = (DataBindFile)lb_FilePicker.SelectedItem;

                //Check if actions are available
                if (vFilePickerCurrentPath == "PC" && selectedItem.FileType != FileType.FolderDisc)
                {
                    Debug.WriteLine("File and folders action cancelled, no actions available.");
                    await Notification_Send_Status("Close", "No actions available");
                    return;
                }

                //Check the selected file type
                if (selectedItem.FileType == FileType.UwpApp)
                {
                    //Add answers for messagebox
                    List<DataBindString> Answers = new List<DataBindString>();

                    DataBindString answerUpdate = new DataBindString();
                    answerUpdate.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Refresh.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    answerUpdate.Name = "Check application update";
                    Answers.Add(answerUpdate);

                    DataBindString answerRemove = new DataBindString();
                    answerRemove.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/RemoveCross.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    answerRemove.Name = "Remove the application";
                    Answers.Add(answerRemove);

                    //Show the messagebox prompt
                    DataBindString messageResult = await Popup_Show_MessageBox("Application actions", "", "Please select an action that you want to use on: " + selectedItem.Name, Answers);
                    if (messageResult != null)
                    {
                        //Update application
                        if (messageResult == answerUpdate)
                        {
                            await UwpListUpdateApplication(selectedItem);
                        }
                        //Remove application
                        else if (messageResult == answerRemove)
                        {
                            await UwpListRemoveApplication(selectedItem);
                        }
                    }
                }
                else if (selectedItem.FileType == FileType.FolderDisc)
                {
                    //Add answers for messagebox
                    List<DataBindString> Answers = new List<DataBindString>();

                    DataBindString answerEjectDisc = new DataBindString();
                    answerEjectDisc.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Eject.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    answerEjectDisc.Name = "Eject disc or unmount the image";
                    Answers.Add(answerEjectDisc);

                    //Show the messagebox prompt
                    DataBindString messageResult = await Popup_Show_MessageBox("Application actions", "", "Please select an action that you want to use on: " + selectedItem.Name, Answers);
                    if (messageResult != null && messageResult == answerEjectDisc)
                    {
                        await FilePicker_EjectDrive(selectedItem, selectedItem.PathFile);
                    }
                }
                else
                {
                    //Add answers for messagebox
                    List<DataBindString> Answers = new List<DataBindString>();

                    //Check the file type
                    bool preFile = selectedItem.FileType == FileType.FolderPre || selectedItem.FileType == FileType.FilePre || selectedItem.FileType == FileType.GoUpPre || selectedItem.FileType == FileType.PlatformDesc;

                    //Count checked items
                    int checkedItems = List_FilePicker.Count(x => x.Checked == Visibility.Visible);

                    DataBindString answerHowLongToBeat = new DataBindString();
                    if (!preFile && vFilePickerSettings.ShowEmulatorInterface)
                    {
                        answerHowLongToBeat.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Timer.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerHowLongToBeat.Name = "How long to beat information";
                        Answers.Add(answerHowLongToBeat);
                    }

                    DataBindString answerDownloadGameInfo = new DataBindString();
                    if (!preFile && vFilePickerSettings.ShowEmulatorInterface)
                    {
                        answerDownloadGameInfo.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Download.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerDownloadGameInfo.Name = "Download game information";
                        Answers.Add(answerDownloadGameInfo);
                    }

                    //Check the sorting type
                    string sortType = string.Empty;
                    if (vFilePickerSortingType == SortingType.Name)
                    {
                        sortType = "Sort files and folders by date";
                    }
                    else
                    {
                        sortType = "Sort files and folders by name";
                    }
                    DataBindString answerSort = new DataBindString();
                    answerSort.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Sorting.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    answerSort.Name = sortType;
                    Answers.Add(answerSort);

                    DataBindString answerCopySingle = new DataBindString();
                    if (!preFile)
                    {
                        answerCopySingle.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Copy.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerCopySingle.Name = "Copy the file or folder";
                        Answers.Add(answerCopySingle);
                    }
                    DataBindString answerCopyChecked = new DataBindString();
                    if (checkedItems > 0)
                    {
                        answerCopyChecked.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Copy.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerCopyChecked.Name = "Copy selected files and folders";
                        Answers.Add(answerCopyChecked);
                    }

                    DataBindString answerCutSingle = new DataBindString();
                    if (!preFile)
                    {
                        answerCutSingle.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Cut.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerCutSingle.Name = "Cut the file or folder";
                        Answers.Add(answerCutSingle);
                    }
                    DataBindString answerCutChecked = new DataBindString();
                    if (checkedItems > 0)
                    {
                        answerCutChecked.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Cut.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerCutChecked.Name = "Cut selected files and folders";
                        Answers.Add(answerCutChecked);
                    }

                    DataBindString answerPaste = new DataBindString();
                    if (vClipboardFiles.Count == 1)
                    {
                        DataBindFile clipboardFile = vClipboardFiles.FirstOrDefault();
                        answerPaste.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Paste.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerPaste.Name = "Paste (" + clipboardFile.FileType.ToString() + " " + clipboardFile.ClipboardType.ToString() + ") " + clipboardFile.Name;
                        Answers.Add(answerPaste);
                    }
                    else if (vClipboardFiles.Count > 1)
                    {
                        int copyCount = vClipboardFiles.Count(x => x.ClipboardType == ClipboardType.Copy);
                        int cutCount = vClipboardFiles.Count(x => x.ClipboardType == ClipboardType.Cut);
                        string statusCount = string.Empty;
                        if (copyCount > cutCount)
                        {
                            statusCount = "(" + copyCount + "x copy)";
                        }
                        else
                        {
                            statusCount = "(" + cutCount + "x cut)";
                        }

                        answerPaste.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Paste.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerPaste.Name = "Paste " + statusCount + " files or folders";
                        Answers.Add(answerPaste);
                    }

                    DataBindString answerRename = new DataBindString();
                    if (!preFile)
                    {
                        answerRename.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Rename.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerRename.Name = "Rename the file or folder";
                        Answers.Add(answerRename);
                    }

                    DataBindString answerRemoveSingle = new DataBindString();
                    if (!preFile)
                    {
                        answerRemoveSingle.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerRemoveSingle.Name = "Remove the file or folder";
                        Answers.Add(answerRemoveSingle);
                    }
                    DataBindString answerRemoveChecked = new DataBindString();
                    if (checkedItems > 0)
                    {
                        answerRemoveChecked.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Remove.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                        answerRemoveChecked.Name = "Remove selected files and folders";
                        Answers.Add(answerRemoveChecked);
                    }

                    DataBindString answerCreateFolder = new DataBindString();
                    answerCreateFolder.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/FolderAdd.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    answerCreateFolder.Name = "Create new folder here";
                    Answers.Add(answerCreateFolder);

                    DataBindString answerCreateTextFile = new DataBindString();
                    answerCreateTextFile.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Extensions/Txt.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    answerCreateTextFile.Name = "Create new text file here";
                    Answers.Add(answerCreateTextFile);

                    //Show the messagebox prompt
                    DataBindString messageResult = await Popup_Show_MessageBox("File and folder actions", "", "Please select an action that you want to use on: " + selectedItem.Name, Answers);
                    if (messageResult != null)
                    {
                        //Sort files and folders
                        if (messageResult == answerSort)
                        {
                            await FilePicker_SortFilesFoldersSwitch(false);
                        }
                        //Copy file or folder
                        else if (messageResult == answerCopySingle)
                        {
                            await FilePicker_FileCopy_Single(selectedItem);
                        }
                        else if (messageResult == answerCopyChecked)
                        {
                            await FilePicker_FileCopy_Checked();
                        }
                        //Cut file or folder
                        else if (messageResult == answerCutSingle)
                        {
                            await FilePicker_FileCut_Single(selectedItem);
                        }
                        else if (messageResult == answerCutChecked)
                        {
                            await FilePicker_FileCut_Checked();
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
                            await AVActions.TaskStart(TaskAction);
                        }
                        //Rename file or folder
                        else if (messageResult == answerRename)
                        {
                            await FilePicker_FileRename(selectedItem);
                        }
                        //Create new folder
                        else if (messageResult == answerCreateFolder)
                        {
                            await FilePicker_CreateFolder();
                        }
                        //Create new text file
                        else if (messageResult == answerCreateTextFile)
                        {
                            await FilePicker_CreateTextFile();
                        }
                        //Remove file or folder
                        else if (messageResult == answerRemoveSingle)
                        {
                            await FilePicker_FileRemove_Single(selectedItem);
                        }
                        else if (messageResult == answerRemoveChecked)
                        {
                            await FilePicker_FileRemove_Checked();
                        }
                        //How long to beat information
                        else if (messageResult == answerHowLongToBeat)
                        {
                            await Popup_Show_HowLongToBeat(selectedItem.Name);
                        }
                        //Download game information
                        else if (messageResult == answerDownloadGameInfo)
                        {
                            DownloadInfoGame informationDownloaded = await DownloadInfoGame(selectedItem.Name, vFilePickerSettings.SourceDataBindApp.Name, 210, true, false);
                            if (informationDownloaded != null)
                            {
                                selectedItem.Description = informationDownloaded.Summary;
                                if (informationDownloaded.ImageBitmap != null)
                                {
                                    selectedItem.ImageBitmap = informationDownloaded.ImageBitmap;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        //Sort files and folders in picker
        async Task FilePicker_SortFilesFoldersSwitch(bool silent)
        {
            try
            {
                if (vFilePickerSortingType == SortingType.Name)
                {
                    await FilePicker_SortFilesFoldersByDate(silent);
                }
                else
                {
                    await FilePicker_SortFilesFoldersByName(silent);
                }
            }
            catch { }
        }

        async Task FilePicker_SortFilesFoldersByName(bool silent)
        {
            try
            {
                if (vFilePickerCurrentPath == "PC")
                {
                    Debug.WriteLine("Invalid sorting path, returning.");
                    await Notification_Send_Status("Sorting", "Invalid sort path");
                    return;
                }

                if (!silent)
                {
                    Debug.WriteLine("Sorting files and folders by name");
                    await Notification_Send_Status("Sorting", "Sorting by name");
                }

                vFilePickerSortingType = SortingType.Name;

                SortFunction<DataBindFile> sortFuncFileType = new SortFunction<DataBindFile>();
                sortFuncFileType.function = x => x.FileType;

                SortFunction<DataBindFile> sortFuncName = new SortFunction<DataBindFile>();
                sortFuncName.function = x => x.Name;

                List<SortFunction<DataBindFile>> orderList = new List<SortFunction<DataBindFile>>();
                orderList.Add(sortFuncFileType);
                orderList.Add(sortFuncName);

                SortObservableCollection(lb_FilePicker, List_FilePicker, orderList, null);
            }
            catch { }
        }

        async Task FilePicker_SortFilesFoldersByDate(bool silent)
        {
            try
            {
                if (vFilePickerCurrentPath == "PC")
                {
                    Debug.WriteLine("Invalid sorting path, returning.");
                    await Notification_Send_Status("Sorting", "Invalid sort path");
                    return;
                }

                if (!silent)
                {
                    Debug.WriteLine("Sorting files and folders by date");
                    await Notification_Send_Status("Sorting", "Sorting by date");
                }

                vFilePickerSortingType = SortingType.Date;

                SortFunction<DataBindFile> sortFuncFileType = new SortFunction<DataBindFile>();
                sortFuncFileType.function = x => x.FileType;

                SortFunction<DataBindFile> sortFuncDateModified = new SortFunction<DataBindFile>();
                sortFuncDateModified.function = x => x.DateModified;
                sortFuncDateModified.ascending = false;

                List<SortFunction<DataBindFile>> orderList = new List<SortFunction<DataBindFile>>();
                orderList.Add(sortFuncFileType);
                orderList.Add(sortFuncDateModified);

                SortObservableCollection(lb_FilePicker, List_FilePicker, orderList, null);
            }
            catch { }
        }

        //Go up a folder in the file picker
        async Task FilePicker_GoFolderUp()
        {
            try
            {
                if (grid_Popup_FilePicker_button_ControllerUp.Visibility == Visibility.Visible)
                {
                    //Read the root path
                    DataBindFile dataBindFile = List_FilePicker.Where(x => x.FileType == FileType.GoUpPre).FirstOrDefault();
                    if (dataBindFile != null)
                    {
                        Debug.WriteLine("Folder up: " + dataBindFile.PathFile);
                        await Popup_Show_FilePicker(dataBindFile.PathFile, -1, true, null);
                    }
                    else
                    {
                        Debug.WriteLine("No folder to navigate go up / no up.");
                        await Notification_Send_Status("Up", "No folder to go up");
                    }
                }
            }
            catch
            {
                Debug.WriteLine("No folder to navigate go up / catch.");
                await Notification_Send_Status("Up", "No folder to go up");
            }
        }

        //Load rom information when selection has changed
        void Lb_FilePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (vFilePickerSettings.ShowEmulatorInterface)
                {
                    ListBox ListboxSender = (ListBox)sender;
                    DataBindFile SelectedItem = (DataBindFile)ListboxSender.SelectedItem;
                    //Debug.WriteLine("File picker selection has changed to: " + selectedItem.Name);

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
                        await Listbox_FilePicker_LeftClick();
                    }
                    else if (vMousePressDownRightClick)
                    {
                        await Listbox_FilePicker_RightClick();
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
                    await Listbox_FilePicker_LeftClick();
                }
                else if (e.Key == Key.Back)
                {
                    await FilePicker_GoFolderUp();
                }
                else if (e.Key == Key.Delete)
                {
                    await FilePicker_Actions();
                }
                else if (e.Key == Key.LeftCtrl)
                {
                    FilePicker_CheckItem();
                }
            }
            catch { }
        }

        //Handle file picker left click
        async Task Listbox_FilePicker_LeftClick()
        {
            try
            {
                if (lb_FilePicker.SelectedItems.Count > 0 && lb_FilePicker.SelectedIndex != -1)
                {
                    DataBindFile selectedItem = (DataBindFile)lb_FilePicker.SelectedItem;
                    if (selectedItem.FileType == FileType.Folder || selectedItem.FileType == FileType.FolderDisc || selectedItem.FileType == FileType.FolderPre)
                    {
                        await Popup_Show_FilePicker(selectedItem.PathFile, -1, true, null);
                    }
                    else if (selectedItem.FileType == FileType.GoUpPre)
                    {
                        await FilePicker_GoFolderUp();
                    }
                    else if (selectedItem.FileType == FileType.Link)
                    {
                        ShortcutDetails shortcutDetails = ReadShortcutFile(selectedItem.PathFile);
                        if (Directory.Exists(shortcutDetails.TargetPath))
                        {
                            await Popup_Show_FilePicker(shortcutDetails.TargetPath, -1, true, null);
                        }
                        else if (File.Exists(shortcutDetails.TargetPath))
                        {
                            await Popup_Close_FilePicker(true, false);
                        }
                        else
                        {
                            await Notification_Send_Status("Close", "Link target does not exist");
                            Debug.WriteLine("Link target does not exist");
                        }
                    }
                    else if (selectedItem.FileType == FileType.PlatformDesc)
                    {
                        DownloadInfoPlatform informationDownloaded = await DownloadInfoPlatform(selectedItem.PathFile, 210, false, false);
                        if (informationDownloaded != null)
                        {
                            selectedItem.Description = ApiIGDB_PlatformSummaryString(informationDownloaded.Details);
                            if (informationDownloaded.ImageBitmap != null)
                            {
                                selectedItem.ImageBitmap = informationDownloaded.ImageBitmap;
                            }
                        }
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
        async Task Listbox_FilePicker_RightClick()
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