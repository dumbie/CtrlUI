using ArnoldVinkCode;
using ArnoldVinkStyles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using static ArnoldVinkCode.AVShortcut;
using static ArnoldVinkStyles.AVImage;
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
                await Popup_Show_Sorting();
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
                    //Get clicked item
                    DataBindFile clickedObject = (DataBindFile)listView_FilePicker.SelectedItem;

                    FilePicker_CheckItem(clickedObject);
                }
            }
            catch { }
        }

        async void Button_FilePicker_button_ControllerLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get clicked item
                DataBindFile clickedObject = (DataBindFile)listView_FilePicker.SelectedItem;

                await FilePicker_RightClick(clickedObject);
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
                    DataBindFile dataBindFile = List_FilePicker.FirstOrDefault(x => x.FileType == FileType.GoUpPre);
                    if (dataBindFile != null)
                    {
                        Debug.WriteLine("Folder up: " + dataBindFile.PathFile);
                        await Popup_Show_FilePicker(dataBindFile.PathFile, -1, true, null);
                    }
                    else
                    {
                        Debug.WriteLine("No folder to navigate go up / no up.");
                        await Notification_Show_Status("Up", "No folder to go up");
                    }
                }
            }
            catch
            {
                Debug.WriteLine("No folder to navigate go up / catch.");
                await Notification_Show_Status("Up", "No folder to go up");
            }
        }

        //Handle file picker mouse/touch tapped
        async void ListView_FilePicker_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check which mouse button is pressed
                if (vMousePressDownXButton1)
                {
                    await FilePicker_GoFolderUp();
                }
                else if (vMousePressDownLeft)
                {
                    //Get clicked item
                    DataBindFile clickedObject = AVListView.GetRoutedListViewItemObject<DataBindFile>(e);

                    await FilePicker_LeftClick(clickedObject);
                }
                else if (vMousePressDownRight)
                {
                    //Get clicked item
                    DataBindFile clickedObject = AVListView.GetRoutedListViewItemObject<DataBindFile>(e);

                    await FilePicker_RightClick(clickedObject);
                }
                else if (vMousePressDownMiddle)
                {
                    //Get clicked item
                    DataBindFile clickedObject = AVListView.GetRoutedListViewItemObject<DataBindFile>(e);

                    FilePicker_CheckItem(clickedObject);
                }
            }
            catch { }
        }

        //Handle file picker keyboard/controller tapped
        async void ListView_FilePicker_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //Check which key is pressed
                if (e.Key == VirtualKey.Back)
                {
                    await FilePicker_GoFolderUp();
                }
                else if (e.Key == VirtualKey.Space)
                {
                    //Get clicked item
                    DataBindFile clickedObject = AVListView.GetRoutedListViewItemObject<DataBindFile>(e);

                    await FilePicker_LeftClick(clickedObject);
                }
                else if (e.Key == VirtualKey.Delete)
                {
                    //Get clicked item
                    DataBindFile clickedObject = AVListView.GetRoutedListViewItemObject<DataBindFile>(e);

                    await FilePicker_RightClick(clickedObject);
                }
                else if (e.Key == VirtualKey.LeftControl)
                {
                    //Get clicked item
                    DataBindFile clickedObject = AVListView.GetRoutedListViewItemObject<DataBindFile>(e);

                    FilePicker_CheckItem(clickedObject);
                }
                else if (e.Key == VirtualKey.F2)
                {
                    //Get clicked item
                    DataBindFile clickedObject = AVListView.GetRoutedListViewItemObject<DataBindFile>(e);

                    await FilePicker_FileRename(clickedObject);
                }
            }
            catch { }
        }

        //Handle file picker left click
        async Task FilePicker_LeftClick(DataBindFile dataBindFile)
        {
            try
            {
                //Check clicked object
                if (dataBindFile == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                if (dataBindFile.FileType == FileType.Folder || dataBindFile.FileType == FileType.FolderDisc || dataBindFile.FileType == FileType.FolderPre)
                {
                    await Popup_Show_FilePicker(dataBindFile.PathFile, -1, true, null);
                }
                else if (dataBindFile.FileType == FileType.GoUpPre)
                {
                    await FilePicker_GoFolderUp();
                }
                else if (dataBindFile.IsShortcut)
                {
                    ShortcutDetails shortcutDetails = ReadShortcutFile(dataBindFile.PathFile);
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
                        await Notification_Show_Status("Close", "Link target does not exist");
                        Debug.WriteLine("Link target does not exist");
                    }
                }
                else
                {
                    await Popup_Close_FilePicker(true, false);
                }
            }
            catch { }
        }

        //File and folder actions
        private async Task FilePicker_RightClick(DataBindFile dataBindFile)
        {
            try
            {
                //Check clicked object
                if (dataBindFile == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                //Check if actions are available
                if (vFilePickerCurrentPath == "PC" && dataBindFile.FileType != FileType.FolderDisc)
                {
                    Debug.WriteLine("File and folders action cancelled, no actions available.");
                    await Notification_Show_Status("Close", "No actions available");
                    return;
                }

                //Check the selected file type
                if (dataBindFile.FileType == FileType.UwpApp)
                {
                    //Add answers for messagebox
                    List<DataBindString> Answers = new List<DataBindString>();

                    DataBindString answerUninstall = new DataBindString();
                    answerUninstall.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/RemoveCross.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    answerUninstall.Name = "Uninstall this application";
                    Answers.Add(answerUninstall);

                    //Show the messagebox prompt
                    DataBindString messageResult = await Popup_Show_MessageBox("Application actions", "", "Please select an action that you want to use on: " + dataBindFile.Name, Answers);
                    if (messageResult != null)
                    {
                        if (messageResult == answerUninstall)
                        {
                            await UwpListUninstallApplication(dataBindFile);
                        }
                    }
                }
                else if (dataBindFile.FileType == FileType.FolderDisc)
                {
                    //Add answers for messagebox
                    List<DataBindString> Answers = new List<DataBindString>();

                    DataBindString answerEjectDisc = new DataBindString();
                    answerEjectDisc.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/Eject.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    answerEjectDisc.Name = "Eject disc or unmount the image";
                    Answers.Add(answerEjectDisc);

                    //Show the messagebox prompt
                    DataBindString messageResult = await Popup_Show_MessageBox("Application actions", "", "Please select an action that you want to use on: " + dataBindFile.Name, Answers);
                    if (messageResult != null)
                    {
                        if (messageResult == answerEjectDisc)
                        {
                            await FilePicker_EjectDrive(dataBindFile, dataBindFile.PathFile);
                        }
                    }
                }
                else
                {
                    //Add answers for messagebox
                    List<DataBindString> Answers = new List<DataBindString>();

                    //Check the file type
                    bool preFile = dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUpPre;

                    //Count checked items
                    int checkedItems = List_FilePicker.Count(x => x.Checked == Visibility.Visible);

                    DataBindString answerShowGameInfo = new DataBindString();
                    if (!preFile && vFilePickerSettings.ShowEmulatorInterface)
                    {
                        answerShowGameInfo.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Information.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerShowGameInfo.Name = "Show game information";
                        Answers.Add(answerShowGameInfo);
                    }

                    DataBindString answerHowLongToBeat = new DataBindString();
                    if (!preFile && vFilePickerSettings.ShowEmulatorInterface)
                    {
                        answerHowLongToBeat.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Timer.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerHowLongToBeat.Name = "How long to beat information";
                        Answers.Add(answerHowLongToBeat);
                    }

                    DataBindString answerCopySingle = new DataBindString();
                    if (!preFile)
                    {
                        answerCopySingle.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Copy.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerCopySingle.Name = "Copy the file or folder";
                        Answers.Add(answerCopySingle);
                    }
                    DataBindString answerCopyChecked = new DataBindString();
                    if (checkedItems > 0)
                    {
                        answerCopyChecked.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Copy.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerCopyChecked.Name = "Copy selected files and folders";
                        Answers.Add(answerCopyChecked);
                    }

                    DataBindString answerCutSingle = new DataBindString();
                    if (!preFile)
                    {
                        answerCutSingle.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Cut.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerCutSingle.Name = "Cut the file or folder";
                        Answers.Add(answerCutSingle);
                    }
                    DataBindString answerCutChecked = new DataBindString();
                    if (checkedItems > 0)
                    {
                        answerCutChecked.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Cut.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerCutChecked.Name = "Cut selected files and folders";
                        Answers.Add(answerCutChecked);
                    }

                    DataBindString answerPaste = new DataBindString();
                    if (vClipboardFiles.Count == 1)
                    {
                        DataBindFile clipboardFile = vClipboardFiles.FirstOrDefault();
                        answerPaste.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Paste.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
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

                        answerPaste.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Paste.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerPaste.Name = "Paste " + statusCount + " files or folders";
                        Answers.Add(answerPaste);
                    }

                    DataBindString answerRename = new DataBindString();
                    if (!preFile)
                    {
                        answerRename.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Rename.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerRename.Name = "Rename the file or folder";
                        Answers.Add(answerRename);
                    }

                    DataBindString answerRemoveSingle = new DataBindString();
                    if (!preFile)
                    {
                        answerRemoveSingle.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Remove.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerRemoveSingle.Name = "Remove the file or folder";
                        Answers.Add(answerRemoveSingle);
                    }
                    DataBindString answerRemoveChecked = new DataBindString();
                    if (checkedItems > 0)
                    {
                        answerRemoveChecked.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                        {
                            FilePaths = ["Assets/Default/Icons/Remove.png"],
                            BackupPath = vImageBackupSource,
                            Dispatcher = this.Dispatcher
                        });
                        answerRemoveChecked.Name = "Remove selected files and folders";
                        Answers.Add(answerRemoveChecked);
                    }

                    DataBindString answerCreateFolder = new DataBindString();
                    answerCreateFolder.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/FolderAdd.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    answerCreateFolder.Name = "Create new folder here";
                    Answers.Add(answerCreateFolder);

                    DataBindString answerCreateTextFile = new DataBindString();
                    answerCreateTextFile.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                    {
                        FilePaths = ["Assets/Default/Icons/TextFile.png"],
                        BackupPath = vImageBackupSource,
                        Dispatcher = this.Dispatcher
                    });
                    answerCreateTextFile.Name = "Create new text file here";
                    Answers.Add(answerCreateTextFile);

                    //Show the messagebox prompt
                    DataBindString messageResult = await Popup_Show_MessageBox("File and folder actions", "", "Please select an action that you want to use on: " + dataBindFile.Name, Answers);
                    if (messageResult != null)
                    {
                        //Copy file or folder
                        if (messageResult == answerCopySingle)
                        {
                            await FilePicker_FileCopy_Single(dataBindFile);
                        }
                        else if (messageResult == answerCopyChecked)
                        {
                            await FilePicker_FileCopy_Checked();
                        }
                        //Cut file or folder
                        else if (messageResult == answerCutSingle)
                        {
                            await FilePicker_FileCut_Single(dataBindFile);
                        }
                        else if (messageResult == answerCutChecked)
                        {
                            await FilePicker_FileCut_Checked();
                        }
                        //Paste file or folder
                        else if (messageResult == answerPaste)
                        {
                            async Task TaskAction()
                            {
                                try
                                {
                                    await FilePicker_FilePaste();
                                }
                                catch { }
                            }
                            AVActions.TaskStartBackground(TaskAction);
                        }
                        //Rename file or folder
                        else if (messageResult == answerRename)
                        {
                            await FilePicker_FileRename(dataBindFile);
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
                            await FilePicker_FileRemove_Single(dataBindFile);
                        }
                        else if (messageResult == answerRemoveChecked)
                        {
                            await FilePicker_FileRemove_Checked();
                        }
                        //How long to beat information
                        else if (messageResult == answerHowLongToBeat)
                        {
                            await Popup_Show_HowLongToBeat(dataBindFile.Name);
                        }
                        //Show game information
                        else if (messageResult == answerShowGameInfo)
                        {
                            await Popup_Show_GameInformation(dataBindFile.Name, dataBindFile);
                        }
                    }
                }
            }
            catch { }
        }
    }
}