using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        void ListBox_Search_KeyPressDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Up && lb_Search.SelectedIndex == 0)
                {
                    //Improve: KeySendCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                    KeyPressCombo((byte)KeysVirtual.Shift, (byte)KeysVirtual.Tab, false);
                }
                else if (e.Key == Key.Down && (lb_Search.Items.Count - 1) == lb_Search.SelectedIndex)
                {
                    KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                }
            }
            catch { }
        }

        async void Grid_Popup_Search_button_ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reset the popup to defaults
                await Popup_Reset_Search(true);
            }
            catch { }
        }

        //Open the keyboard controller
        async void Button_SearchKeyboardController_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseShowKeyboardController();
                await FocusOnElement(grid_Popup_Search_textbox_Search, false, vProcessCurrent.MainWindowHandle);
            }
            catch { }
        }

        void Grid_Popup_Search_textbox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string stringSearch = grid_Popup_Search_textbox_Search.Text;
                if (!string.IsNullOrWhiteSpace(stringSearch) && stringSearch != "Search application...")
                {
                    //Clear the current popup list
                    List_Search.Clear();
                    GC.Collect();

                    //Search for applications
                    IEnumerable<DataBindApp> searchResult = CombineAppLists(true, true).Where(x => x.Name.ToLower().Contains(stringSearch.ToLower()));
                    foreach (DataBindApp dataBindApp in searchResult)
                    {
                        try
                        {
                            List_Search.Add(dataBindApp);
                        }
                        catch { }
                    }

                    //Update the search results count
                    UpdateSearchResults();

                    Debug.WriteLine("Search application: " + stringSearch);
                }
                else
                {
                    //Clear the current popup list
                    List_Search.Clear();
                    GC.Collect();

                    //Reset the search text
                    grid_Popup_Search_Count_TextBlock.Text = string.Empty;
                    grid_Popup_Search_textblock_Result.Text = "Please enter a search term above.";
                    grid_Popup_Search_textblock_Result.Visibility = Visibility.Visible;
                    grid_Popup_Search_button_KeyboardControllerButton.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }
    }
}