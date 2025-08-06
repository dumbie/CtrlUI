using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVSortObservableCollection;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Bind the lists to the listbox elements
        void ListViewBindLists()
        {
            try
            {
                //FixStyle
                //lb_Apps.IsTextSearchEnabled = true;
                //lb_Apps.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(lb_Apps, "Name");
                listView_Apps.ItemsSource = List_Apps;

                //listView_Games.IsTextSearchEnabled = true;
                //listView_Games.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(listView_Games, "Name");
                listView_Games.ItemsSource = List_Games;

                //lb_Emulators.IsTextSearchEnabled = true;
                //lb_Emulators.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(lb_Emulators, "Name");
                listView_Emulators.ItemsSource = List_Emulators;

                //lb_Launchers.IsTextSearchEnabled = true;
                //lb_Launchers.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(lb_Launchers, "Name");
                listView_Launchers.ItemsSource = List_Launchers;

                //lb_Shortcuts.IsTextSearchEnabled = true;
                //lb_Shortcuts.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(lb_Shortcuts, "Name");
                listView_Shortcuts.ItemsSource = List_Shortcuts;

                //lb_Processes.IsTextSearchEnabled = true;
                //lb_Processes.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(lb_Processes, "Name");
                listView_Processes.ItemsSource = List_Processes;

                //lb_Gallery.IsTextSearchEnabled = true;
                //lb_Gallery.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(lb_Gallery, "Name");
                listView_Gallery.ItemsSource = List_Gallery;

                listView_ColorPicker.ItemsSource = List_ColorPicker;

                //lb_Search.IsTextSearchEnabled = true;
                //lb_Search.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(lb_Search, "Name");
                listView_Search.ItemsSource = List_Search;

                //lb_FilePicker.IsTextSearchEnabled = true;
                //lb_FilePicker.IsTextSearchCaseSensitive = false;
                //TextSearch.SetTextPath(lb_FilePicker, "Name");
                listView_FilePicker.ItemsSource = List_FilePicker;

                //lb_ProfileManager.IsTextSearchEnabled = true;
                //lb_ProfileManager.IsTextSearchCaseSensitive = false;
            }
            catch { }
        }

        //Select first ListView items
        void ListViewResetIndexes()
        {
            try
            {
                listView_Apps.SelectedIndex = 0;
                listView_Games.SelectedIndex = 0;
                listView_Emulators.SelectedIndex = 0;
                listView_Launchers.SelectedIndex = 0;
                listView_Shortcuts.SelectedIndex = 0;
                listView_Processes.SelectedIndex = 0;
                listView_Gallery.SelectedIndex = 0;
                listView_Search.SelectedIndex = 0;
                listView_Manage_AddAppCategory.SelectedIndex = 0;
                listView_Manage_AddEmulatorCategory.SelectedIndex = 0;
                listView_LauncherSetting.SelectedIndex = 0;
                listView_ColorPicker.SelectedIndex = 0;
                listView_FilePicker.SelectedIndex = 0;
                listView_ProfileManager.SelectedIndex = 0;
                listView_ContentInfo_Gallery.SelectedIndex = 0;
                listView_HowLongToBeat.SelectedIndex = 0;
                listView_MessageBox.SelectedIndex = 0;
                listView_SettingsMenu.SelectedIndex = 0;
                listView_MainMenu.SelectedIndex = 0;
                listView_Sorting.SelectedIndex = 0;

                Debug.WriteLine("Selected first ListView items.");
            }
            catch { }
        }

        //Listbox move to near character
        async Task ListViewSelectNearCharacter(bool selectNextCharacter)
        {
            try
            {
                await DispatcherInvoke(this.Dispatcher, async delegate
                {
                    ListView focusedListView = GetFocusedListView();
                    if (focusedListView != null && vSelectNearCharacterLists.Contains(focusedListView.Name))
                    {
                        if (focusedListView.Name == "listView_FilePicker")
                        {
                            await SelectNearCharacterFiles(selectNextCharacter, focusedListView);
                        }
                        else
                        {
                            await SelectNearCharacterApps(selectNextCharacter, focusedListView);
                        }
                    }
                    else
                    {
                        if (selectNextCharacter)
                        {
                            KeySendSingle(KeysVirtual.PageDown, vProcessCurrent.WindowHandleMain);
                        }
                        else
                        {
                            KeySendSingle(KeysVirtual.PageUp, vProcessCurrent.WindowHandleMain);
                        }
                    }
                });
            }
            catch { }
        }

        async Task SelectNearCharacterFiles(bool selectNextCharacter, ListView targetListView)
        {
            try
            {
                //Check file picker folder
                if (vFilePickerCurrentPath == "PC")
                {
                    Debug.WriteLine("Invalid folder, cannot be sorted.");
                    return;
                }

                //Sort list by name
                SortFunction<DataBindFile> sortFuncFileType = new SortFunction<DataBindFile>()
                {
                    Function = x => x.FileType
                };
                SortFunction<DataBindFile> sortFuncName = new SortFunction<DataBindFile>()
                {
                    Function = x => x.Name
                };
                var sortFunction = (List<SortFunction<DataBindFile>>)[sortFuncFileType, sortFuncName];
                SortObservableCollection(targetListView, sortFunction, null);

                //Get the current character
                DataBindFile dataBindApp = (DataBindFile)targetListView.SelectedItem;
                ObservableCollection<DataBindFile> dataBindApplist = (ObservableCollection<DataBindFile>)targetListView.ItemsSource;
                char currentCharacter = dataBindApp.Name.ToUpper()[0];

                //Set the character filter
                Func<DataBindFile, bool> filterCharacterNoMatch = x => x.Name.ToUpper()[0] != currentCharacter && x.FileType != FileType.FolderPre && x.FileType != FileType.FilePre && x.FileType != FileType.GoUpPre;
                Func<DataBindFile, bool> filterCharacterMatch = x => x.Name.ToUpper()[0] == currentCharacter && x.FileType != FileType.FolderPre && x.FileType != FileType.FilePre && x.FileType != FileType.GoUpPre;

                //Get the target application
                DataBindFile selectAppCurrent = null;
                if (selectNextCharacter)
                {
                    int currentIndex = targetListView.SelectedIndex;
                    selectAppCurrent = dataBindApplist.Skip(currentIndex).FirstOrDefault(filterCharacterNoMatch);
                }
                else
                {
                    int currentIndex = dataBindApplist.Count() - targetListView.SelectedIndex;
                    selectAppCurrent = dataBindApplist.Reverse().Skip(currentIndex).FirstOrDefault(filterCharacterNoMatch);
                    if (selectAppCurrent != null)
                    {
                        currentCharacter = selectAppCurrent.Name.ToUpper()[0];
                        selectAppCurrent = dataBindApplist.OrderByDescending(x => x.FileType == selectAppCurrent.FileType).FirstOrDefault(filterCharacterMatch);
                    }
                }

                //Select the target application
                if (selectAppCurrent != null)
                {
                    char selectCharacterCurrent = selectAppCurrent.Name.ToUpper()[0];
                    char selectCharacterNext1 = (char)(selectCharacterCurrent + 1);
                    char selectCharacterNext2 = (char)(selectCharacterCurrent + 2);
                    char selectCharacterNext3 = (char)(selectCharacterCurrent + 3);
                    char selectCharacterPrev1 = (char)(selectCharacterCurrent - 1);
                    char selectCharacterPrev2 = (char)(selectCharacterCurrent - 2);
                    char selectCharacterPrev3 = (char)(selectCharacterCurrent - 3);
                    string selectStringCurrent = selectCharacterCurrent.ToString();
                    string selectStringNext = selectCharacterNext1.ToString() + selectCharacterNext2.ToString() + selectCharacterNext3.ToString();
                    string selectStringPrev = selectCharacterPrev3.ToString() + selectCharacterPrev2.ToString() + selectCharacterPrev1.ToString();

                    //Show character overlay
                    ShowCharacterOverlay(selectStringCurrent, selectStringNext, selectStringPrev);

                    //Listbox focus and select the item
                    await ListViewFocusItem(targetListView, selectAppCurrent, vProcessCurrent.WindowHandleMain);

                    Debug.WriteLine("Selected list character: " + selectCharacterCurrent + "/" + selectAppCurrent.Name);
                }

                //Play interface sound
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
            }
            catch { }
        }

        async Task SelectNearCharacterApps(bool selectNextCharacter, ListView targetListView)
        {
            try
            {
                //Sort list by name
                SortFunction<DataBindApp> sortFuncName = new SortFunction<DataBindApp>();
                sortFuncName.Function = x => x.Name;
                SortObservableCollection(targetListView, sortFuncName, null);

                //Get the current character
                DataBindApp dataBindApp = (DataBindApp)targetListView.SelectedItem;
                ObservableCollection<DataBindApp> dataBindApplist = (ObservableCollection<DataBindApp>)targetListView.ItemsSource;
                char currentCharacter = dataBindApp.Name.ToUpper()[0];

                //Set the character filter
                Func<DataBindApp, bool> filterCharacterNoMatch = x => x.Name.ToUpper()[0] != currentCharacter;
                Func<DataBindApp, bool> filterCharacterMatch = x => x.Name.ToUpper()[0] == currentCharacter;

                //Get the target application
                DataBindApp selectAppCurrent = null;
                if (selectNextCharacter)
                {
                    int currentIndex = targetListView.SelectedIndex;
                    selectAppCurrent = dataBindApplist.Skip(currentIndex).FirstOrDefault(filterCharacterNoMatch);
                }
                else
                {
                    int currentIndex = dataBindApplist.Count() - targetListView.SelectedIndex;
                    selectAppCurrent = dataBindApplist.Reverse().Skip(currentIndex).FirstOrDefault(filterCharacterNoMatch);
                    if (selectAppCurrent != null)
                    {
                        currentCharacter = selectAppCurrent.Name.ToUpper()[0];
                        selectAppCurrent = dataBindApplist.FirstOrDefault(filterCharacterMatch);
                    }
                }

                //Select the target application
                if (selectAppCurrent != null)
                {
                    char selectCharacterCurrent = selectAppCurrent.Name.ToUpper()[0];
                    char selectCharacterNext1 = (char)(selectCharacterCurrent + 1);
                    char selectCharacterNext2 = (char)(selectCharacterCurrent + 2);
                    char selectCharacterNext3 = (char)(selectCharacterCurrent + 3);
                    char selectCharacterPrev1 = (char)(selectCharacterCurrent - 1);
                    char selectCharacterPrev2 = (char)(selectCharacterCurrent - 2);
                    char selectCharacterPrev3 = (char)(selectCharacterCurrent - 3);
                    string selectStringCurrent = selectCharacterCurrent.ToString();
                    string selectStringNext = selectCharacterNext1.ToString() + selectCharacterNext2.ToString() + selectCharacterNext3.ToString();
                    string selectStringPrev = selectCharacterPrev3.ToString() + selectCharacterPrev2.ToString() + selectCharacterPrev1.ToString();

                    //Show character overlay
                    ShowCharacterOverlay(selectStringCurrent, selectStringNext, selectStringPrev);

                    //Listbox focus and select the item
                    await ListViewFocusItem(targetListView, selectAppCurrent, vProcessCurrent.WindowHandleMain);

                    Debug.WriteLine("Selected list character: " + selectCharacterCurrent + "/" + selectAppCurrent.Name);
                }

                //Play interface sound
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
            }
            catch { }
        }

        //Show the character overlay
        public void ShowCharacterOverlay(string currentChar, string nextChar, string prevChar)
        {
            try
            {
                //Show the overlay
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    try
                    {
                        grid_SelectCharacterNextText.Text = nextChar;
                        grid_SelectCharacterCurrentText.Text = currentChar;
                        grid_SelectCharacterPreviousText.Text = prevChar;
                        grid_Popup_SelectCharacter.Visibility = Visibility.Visible;
                    }
                    catch { }
                });

                //Start overlay timer
                vAVTimerOverlayCharacter.Interval = 2000;
                vAVTimerOverlayCharacter.TickSet = delegate
                {
                    try
                    {
                        DispatcherInvoke(this.Dispatcher, delegate
                        {
                            //Stop overlay timer
                            vAVTimerOverlayCharacter.Stop();

                            //Hide overlay
                            grid_Popup_SelectCharacter.Visibility = Visibility.Collapsed;
                        });
                    }
                    catch { }
                };
                vAVTimerOverlayCharacter.Start();
            }
            catch { }
        }

        //Add listbox item to a list
        async Task ListViewAddItem<T>(ListView listBox, Collection<T> listCollection, T addItem, bool insertItem, bool selectItem)
        {
            try
            {
                await DispatcherInvoke(this.Dispatcher, async delegate
                {
                    //Debug.WriteLine("Adding item to list collection: " + listCollection);

                    //Add or insert the item to the list
                    if (insertItem)
                    {
                        //Debug.WriteLine(listCollection + " listbox item has been inserted.");
                        listCollection.Insert(0, addItem);
                    }
                    else
                    {
                        //Debug.WriteLine(listCollection + " listbox item has been added.");
                        listCollection.Add(addItem);
                    }

                    //Select the item in the listbox
                    if (listBox != null && selectItem)
                    {
                        if (insertItem)
                        {
                            await ListViewFocusIndex(listBox, false, 0, vProcessCurrent.WindowHandleMain);
                        }
                        else
                        {
                            await ListViewFocusIndex(listBox, true, 0, vProcessCurrent.WindowHandleMain);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding listbox item: " + ex.Message);
            }
        }

        //Remove listbox item from a listbox
        async Task ListViewRemoveItem<T>(ListView listBox, Collection<T> listCollection, T removeItem, bool selectItem)
        {
            try
            {
                await DispatcherInvoke(this.Dispatcher, async delegate
                {
                    //Store the current listbox items count
                    int listBoxItemCount = listBox.Items.Count;

                    //Store the currently selected index
                    int listBoxSelectedIndex = listBox.SelectedIndex;

                    //Remove the listbox item from list
                    listCollection.Remove(removeItem);

                    //Check if there is a listbox item removed
                    if (listBoxItemCount != listBox.Items.Count)
                    {
                        Debug.WriteLine(listBox.Name + " listbox item has been removed.");
                        if (selectItem)
                        {
                            await ListViewFocusIndex(listBox, false, listBoxSelectedIndex, vProcessCurrent.WindowHandleMain);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed removing item from the listbox: " + ex.Message);
            }
        }

        //Remove all matching items from a listbox
        async Task ListViewRemoveAll<T>(ListView listBox, Collection<T> listCollection, Func<T, bool> removeCondition)
        {
            try
            {
                await DispatcherInvoke(this.Dispatcher, async delegate
                {
                    //Store the current listbox items count
                    int listBoxItemCount = listBox.Items.Count;

                    //Store the currently selected index
                    int listBoxSelectedIndex = listBox.SelectedIndex;

                    //Remove the listbox items from list
                    listCollection.ListRemoveAll(removeCondition);

                    //Check if there is a listbox item removed
                    if (listBoxItemCount != listBox.Items.Count)
                    {
                        Debug.WriteLine(listBox.Name + " " + (listBoxItemCount - listBox.Items.Count) + " items have been removed.");
                        await ListViewFocusIndex(listBox, false, listBoxSelectedIndex, vProcessCurrent.WindowHandleMain);
                    }
                });
            }
            catch
            {
                Debug.WriteLine("Failed removing all from the listbox.");
            }
        }
    }
}