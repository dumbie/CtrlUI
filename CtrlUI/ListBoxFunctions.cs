using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static LibraryShared.Enums;
using static LibraryShared.FocusFunctions;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Bind the lists to the listbox elements
        void ListBoxBindLists()
        {
            try
            {
                lb_Games.IsTextSearchEnabled = true;
                lb_Games.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Games, "Name");
                lb_Games.ItemsSource = List_Games;

                lb_Apps.IsTextSearchEnabled = true;
                lb_Apps.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Apps, "Name");
                lb_Apps.ItemsSource = List_Apps;

                lb_Emulators.IsTextSearchEnabled = true;
                lb_Emulators.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Emulators, "Name");
                lb_Emulators.ItemsSource = List_Emulators;

                lb_Launchers.IsTextSearchEnabled = true;
                lb_Launchers.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Launchers, "Name");
                lb_Launchers.ItemsSource = List_Launchers;

                lb_Shortcuts.IsTextSearchEnabled = true;
                lb_Shortcuts.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Shortcuts, "Name");
                lb_Shortcuts.ItemsSource = List_Shortcuts;

                lb_Processes.IsTextSearchEnabled = true;
                lb_Processes.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Processes, "Name");
                lb_Processes.ItemsSource = List_Processes;

                lb_ColorPicker.ItemsSource = List_ColorPicker;

                lb_Search.IsTextSearchEnabled = true;
                lb_Search.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_Search, "Name");
                lb_Search.ItemsSource = List_Search;

                lb_FilePicker.IsTextSearchEnabled = true;
                lb_FilePicker.IsTextSearchCaseSensitive = false;
                TextSearch.SetTextPath(lb_FilePicker, "Name");
                lb_FilePicker.ItemsSource = List_FilePicker;

                lb_ProfileManager.IsTextSearchEnabled = true;
                lb_ProfileManager.IsTextSearchCaseSensitive = false;
            }
            catch { }
        }

        //Select the first listbox item
        void ListBoxResetIndexes()
        {
            try
            {
                lb_Games.SelectedIndex = 0;
                lb_Apps.SelectedIndex = 0;
                lb_Emulators.SelectedIndex = 0;
                lb_Launchers.SelectedIndex = 0;
                lb_Shortcuts.SelectedIndex = 0;
                lb_Processes.SelectedIndex = 0;
                lb_FilePicker.SelectedIndex = 0;
                lb_ColorPicker.SelectedIndex = 0;
                lb_MessageBox.SelectedIndex = 0;
                lb_Search.SelectedIndex = 0;
            }
            catch { }
        }

        //Listbox move to near character
        async Task ListBoxSelectNearCharacter(bool selectNextCharacter)
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                    if (frameworkElement != null && (frameworkElement.GetType() == typeof(ListBox) || frameworkElement.GetType() == typeof(ListBoxItem)))
                    {
                        ListBox parentListbox = null;
                        if (frameworkElement.GetType() == typeof(ListBoxItem))
                        {
                            parentListbox = AVFunctions.FindVisualParent<ListBox>(frameworkElement);
                        }
                        else
                        {
                            parentListbox = (ListBox)frameworkElement;
                        }

                        if (vSelectNearCharacterLists.Contains(parentListbox.Name))
                        {
                            if (parentListbox.Name == "lb_FilePicker")
                            {
                                await SelectNearCharacterFiles(selectNextCharacter, parentListbox);
                            }
                            else
                            {
                                await SelectNearCharacterApps(selectNextCharacter, parentListbox);
                            }
                        }
                        else
                        {
                            if (selectNextCharacter)
                            {
                                KeySendSingle(KeysVirtual.Next, vProcessCurrent.MainWindowHandle);
                            }
                            else
                            {
                                KeySendSingle(KeysVirtual.Prior, vProcessCurrent.MainWindowHandle);
                            }
                        }
                    }
                });
            }
            catch { }
        }

        async Task SelectNearCharacterFiles(bool selectNextCharacter, ListBox parentListbox)
        {
            try
            {
                //Make sure the list is sorted by name
                await FilePicker_SortFilesFoldersByName(true);

                //Get the current character
                DataBindFile dataBindApp = (DataBindFile)parentListbox.SelectedItem;
                ObservableCollection<DataBindFile> dataBindApplist = (ObservableCollection<DataBindFile>)parentListbox.ItemsSource;
                char currentCharacter = dataBindApp.Name.ToUpper()[0];

                //Set the character filter
                Func<DataBindFile, bool> filterCharacterNoMatch = x => x.Name.ToUpper()[0] != currentCharacter && x.FileType != FileType.FolderPre && x.FileType != FileType.FilePre && x.FileType != FileType.GoUpPre;
                Func<DataBindFile, bool> filterCharacterMatch = x => x.Name.ToUpper()[0] == currentCharacter && x.FileType != FileType.FolderPre && x.FileType != FileType.FilePre && x.FileType != FileType.GoUpPre;

                //Get the target application
                DataBindFile selectAppCurrent = null;
                if (selectNextCharacter)
                {
                    int currentIndex = parentListbox.SelectedIndex;
                    selectAppCurrent = dataBindApplist.Skip(currentIndex).Where(filterCharacterNoMatch).FirstOrDefault();
                }
                else
                {
                    int currentIndex = dataBindApplist.Count() - parentListbox.SelectedIndex;
                    selectAppCurrent = dataBindApplist.Reverse().Skip(currentIndex).Where(filterCharacterNoMatch).FirstOrDefault();
                    if (selectAppCurrent != null)
                    {
                        currentCharacter = selectAppCurrent.Name.ToUpper()[0];
                        selectAppCurrent = dataBindApplist.OrderByDescending(x => x.FileType == selectAppCurrent.FileType).Where(filterCharacterMatch).FirstOrDefault();
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
                    await ListBoxFocusItem(parentListbox, selectAppCurrent, vProcessCurrent.MainWindowHandle);

                    Debug.WriteLine("Selected list character: " + selectCharacterCurrent + "/" + selectAppCurrent.Name);
                }

                //Play interface sound
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
            }
            catch { }
        }

        async Task SelectNearCharacterApps(bool selectNextCharacter, ListBox parentListbox)
        {
            try
            {
                //Make sure the list is sorted by name
                await SortAppListsByName(true);

                //Get the current character
                DataBindApp dataBindApp = (DataBindApp)parentListbox.SelectedItem;
                ObservableCollection<DataBindApp> dataBindApplist = (ObservableCollection<DataBindApp>)parentListbox.ItemsSource;
                char currentCharacter = dataBindApp.Name.ToUpper()[0];

                //Set the character filter
                Func<DataBindApp, bool> filterCharacterNoMatch = x => x.Name.ToUpper()[0] != currentCharacter;
                Func<DataBindApp, bool> filterCharacterMatch = x => x.Name.ToUpper()[0] == currentCharacter;

                //Get the target application
                DataBindApp selectAppCurrent = null;
                if (selectNextCharacter)
                {
                    int currentIndex = parentListbox.SelectedIndex;
                    selectAppCurrent = dataBindApplist.Skip(currentIndex).Where(filterCharacterNoMatch).FirstOrDefault();
                }
                else
                {
                    int currentIndex = dataBindApplist.Count() - parentListbox.SelectedIndex;
                    selectAppCurrent = dataBindApplist.Reverse().Skip(currentIndex).Where(filterCharacterNoMatch).FirstOrDefault();
                    if (selectAppCurrent != null)
                    {
                        currentCharacter = selectAppCurrent.Name.ToUpper()[0];
                        selectAppCurrent = dataBindApplist.Where(filterCharacterMatch).FirstOrDefault();
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
                    await ListBoxFocusItem(parentListbox, selectAppCurrent, vProcessCurrent.MainWindowHandle);

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
                AVActions.ActionDispatcherInvoke(delegate
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
                vDispatcherTimerOverlay.Interval = TimeSpan.FromMilliseconds(2000);
                vDispatcherTimerOverlay.Tick += delegate
                {
                    try
                    {
                        //Hide the overlay
                        grid_Popup_SelectCharacter.Visibility = Visibility.Collapsed;

                        //Renew the timer
                        AVFunctions.TimerRenew(ref vDispatcherTimerOverlay);
                    }
                    catch { }
                };
                AVFunctions.TimerReset(vDispatcherTimerOverlay);
            }
            catch { }
        }

        //Add listbox item to a list
        async Task ListBoxAddItem<T>(ListBox listBox, Collection<T> listCollection, T addItem, bool insertItem, bool selectItem)
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
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
                            await ListBoxFocusOrSelectIndex(listBox, true, false, -1, vProcessCurrent.MainWindowHandle);
                        }
                        else
                        {
                            await ListBoxFocusOrSelectIndex(listBox, false, true, -1, vProcessCurrent.MainWindowHandle);
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
        async Task ListBoxRemoveItem<T>(ListBox listBox, Collection<T> listCollection, T removeItem, bool selectItem)
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
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
                            await ListBoxFocusOrSelectIndex(listBox, false, false, listBoxSelectedIndex, vProcessCurrent.MainWindowHandle);
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
        async Task ListBoxRemoveAll<T>(ListBox listBox, Collection<T> listCollection, Func<T, bool> removeCondition)
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
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
                        await ListBoxFocusOrSelectIndex(listBox, false, false, listBoxSelectedIndex, vProcessCurrent.MainWindowHandle);
                    }
                });
            }
            catch
            {
                Debug.WriteLine("Failed removing all from the listbox.");
            }
        }

        //Check listbox item column position
        bool ListBoxItemColumnPosition(ListBox targetListBox, ListBoxItem targetListBoxItem, bool firstColumn)
        {
            try
            {
                ListBoxCountColumns(targetListBox, out int totalCount, out List<double> offsetPoints);
                double translatePoint = targetListBoxItem.TranslatePoint(new Point(), targetListBox).Y;
                if (firstColumn)
                {
                    if (translatePoint == offsetPoints.FirstOrDefault())
                    {
                        //Debug.WriteLine("ListBoxItem is in first column.");
                        return true;
                    }
                }
                else
                {
                    if (translatePoint == offsetPoints.LastOrDefault())
                    {
                        //Debug.WriteLine("ListBoxItem is in last column.");
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        //Check listbox item row position
        bool ListBoxItemRowPosition(ListBox targetListBox, ListBoxItem targetListBoxItem, bool firstRow)
        {
            try
            {
                ListBoxCountRows(targetListBox, out int totalCount, out List<double> offsetPoints);
                double translatePoint = targetListBoxItem.TranslatePoint(new Point(), targetListBox).X;
                if (firstRow)
                {
                    if (translatePoint == offsetPoints.FirstOrDefault())
                    {
                        //Debug.WriteLine("ListBoxItem is in first row.");
                        return true;
                    }
                }
                else
                {
                    if (translatePoint == offsetPoints.LastOrDefault())
                    {
                        //Debug.WriteLine("ListBoxItem is in last row.");
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        //Count columns in listbox
        void ListBoxCountColumns(ListBox targetListBox, out int totalCount, out List<double> offsetPoints)
        {
            totalCount = 0;
            offsetPoints = new List<double>();
            try
            {
                foreach (object listItem in targetListBox.Items)
                {
                    FrameworkElement containerItem = (FrameworkElement)targetListBox.ItemContainerGenerator.ContainerFromItem(listItem);
                    double translatePoint = containerItem.TranslatePoint(new Point(), targetListBox).Y;
                    if (!offsetPoints.Any(x => x == translatePoint))
                    {
                        totalCount++;
                        offsetPoints.Add(translatePoint);
                    }
                }
                //Debug.WriteLine("Columns: " + totalCount);
            }
            catch
            {
                Debug.WriteLine("Failed to count columns from the listbox.");
            }
        }

        //Count rows in listbox
        void ListBoxCountRows(ListBox targetListBox, out int totalCount, out List<double> offsetPoints)
        {
            totalCount = 0;
            offsetPoints = new List<double>();
            try
            {
                foreach (object listItem in targetListBox.Items)
                {
                    FrameworkElement containerItem = (FrameworkElement)targetListBox.ItemContainerGenerator.ContainerFromItem(listItem);
                    double translatePoint = containerItem.TranslatePoint(new Point(), targetListBox).X;
                    if (!offsetPoints.Any(x => x == translatePoint))
                    {
                        totalCount++;
                        offsetPoints.Add(translatePoint);
                    }
                }
                //Debug.WriteLine("Rows: " + totalCount);
            }
            catch
            {
                Debug.WriteLine("Failed to count rows from the listbox.");
            }
        }
    }
}