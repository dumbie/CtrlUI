using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RightClickShortcut(ListView listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                Debug.WriteLine("Right clicked shortcut: " + dataBindApp.Name + " from: " + listboxSender.Name);

                List<DataBindString> Answers = new List<DataBindString>();

                DataBindString AnswerShowGameInfo = new DataBindString();
                AnswerShowGameInfo.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Information.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerShowGameInfo.Name = "Show game information";
                Answers.Add(AnswerShowGameInfo);

                DataBindString AnswerHowLongToBeat = new DataBindString();
                AnswerHowLongToBeat.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Timer.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerHowLongToBeat.Name = "How long to beat information";
                Answers.Add(AnswerHowLongToBeat);

                DataBindString AnswerRemove = new DataBindString();
                AnswerRemove.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Remove.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerRemove.Name = "Remove the shortcut file";
                Answers.Add(AnswerRemove);

                DataBindString AnswerRename = new DataBindString();
                AnswerRename.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Rename.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerRename.Name = "Rename the shortcut file";
                Answers.Add(AnswerRename);

                DataBindString AnswerHide = new DataBindString();
                AnswerHide.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Hide.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerHide.Name = "Hide shortcut from list";
                Answers.Add(AnswerHide);

                //Get launch information
                string launchInformation = string.Empty;
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    launchInformation = dataBindApp.AppUserModelId + " (" + dataBindApp.NameExe + ")";
                }
                else
                {
                    launchInformation = dataBindApp.PathExe;
                }

                //Add launch argument
                if (!string.IsNullOrWhiteSpace(dataBindApp.Argument))
                {
                    launchInformation += "\nLaunch argument: " + dataBindApp.Argument;
                }

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", launchInformation, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerHowLongToBeat)
                    {
                        await Popup_Show_HowLongToBeat(dataBindApp.Name);
                    }
                    else if (messageResult == AnswerShowGameInfo)
                    {
                        await Popup_Show_GameInformation(dataBindApp.Name, dataBindApp);
                    }
                    else if (messageResult == AnswerRemove)
                    {
                        await List_FileRemove_Prompt(listboxSender, listboxSelectedIndex, dataBindApp);
                    }
                    else if (messageResult == AnswerRename)
                    {
                        await RenameShortcutFile(dataBindApp);
                    }
                    else if (messageResult == AnswerHide)
                    {
                        await HideShortcutFile(listboxSender, listboxSelectedIndex, dataBindApp);
                    }
                }
            }
            catch { }
        }

        //Hide the shortcut file
        async Task HideShortcutFile(ListView listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                await Notification_Show_Status("Hide", "Hiding shortcut " + dataBindApp.Name);
                Debug.WriteLine("Hiding shortcut by name: " + dataBindApp.Name + " path: " + dataBindApp.PathShortcut);

                //Create new profile shared
                ProfileShared profileShared = new ProfileShared();
                profileShared.String1 = dataBindApp.Name;

                //Add shortcut file to the ignore list
                vCtrlIgnoreShortcutName.Add(profileShared);
                JsonSaveObject(vCtrlIgnoreShortcutName, @"Profiles\User\CtrlIgnoreShortcutName.json");

                //Remove application from the list
                await RemoveAppFromList(dataBindApp, false, false, true);

                //Select the previous index
                await ListViewFocusIndex(listboxSender, false, listboxSelectedIndex, vProcessCurrent.WindowHandleMain);
            }
            catch (Exception ex)
            {
                await Notification_Show_Status("Hide", "Failed hiding");
                Debug.WriteLine("Failed hiding shortcut: " + ex.Message);
            }
        }

        //Rename the shortcut file
        async Task RenameShortcutFile(DataBindApp dataBindApp)
        {
            try
            {
                await Notification_Show_Status("Rename", "Renaming shortcut");
                Debug.WriteLine("Renaming shortcut: " + dataBindApp.Name + " path: " + dataBindApp.PathShortcut);

                //Show the text input popup
                string textInputString = await Popup_ShowHide_TextInput("Rename shortcut", dataBindApp.Name, "Rename the shortcut file", false);

                //Check if file name changed
                if (textInputString == dataBindApp.Name)
                {
                    await Notification_Show_Status("Rename", "File name not changed");
                    Debug.WriteLine("The file name did not change.");
                    return;
                }

                //Check the changed file name
                if (!string.IsNullOrWhiteSpace(textInputString))
                {
                    string shortcutDirectory = Path.GetDirectoryName(dataBindApp.PathShortcut);
                    string fileExtension = Path.GetExtension(dataBindApp.PathShortcut);
                    string newFilePath = Path.Combine(shortcutDirectory, textInputString + fileExtension);

                    bool fileRenamed = File_Move(dataBindApp.PathShortcut, newFilePath, true);
                    if (fileRenamed)
                    {
                        dataBindApp.Name = textInputString;
                        dataBindApp.PathShortcut = newFilePath;

                        await Notification_Show_Status("Rename", "Renamed shortcut");
                        Debug.WriteLine("Renamed shortcut file to: " + textInputString);
                    }
                    else
                    {
                        await Notification_Show_Status("Rename", "Failed renaming");
                        Debug.WriteLine("Failed renaming shortcut.");
                    }
                }
            }
            catch (Exception ex)
            {
                await Notification_Show_Status("Rename", "Failed renaming");
                Debug.WriteLine("Failed renaming shortcut: " + ex.Message);
            }
        }
    }
}