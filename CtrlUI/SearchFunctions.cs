using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async void Grid_Popup_Search_button_ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reset the popup to defaults
                await Popup_Reset_Search(true);
            }
            catch { }
        }

        //Hide or show the search
        async Task Popup_ShowHide_Search(bool ForceShow)
        {
            try
            {
                if (vSearchOpen)
                {
                    await Popup_Close_Top();
                    return;
                }

                if (ForceShow)
                {
                    await Popup_Close_All();
                }

                if (Popup_Any_Open())
                {
                    return;
                }

                //Reset the popup to defaults
                await Popup_Reset_Search(false);

                //Show the search popup
                PlayInterfaceSound("PopupOpen", false);

                //Save previous focused element
                if (Keyboard.FocusedElement != null) { vSearchPreviousFocus = (FrameworkElement)Keyboard.FocusedElement; }

                //Show the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_Search, true, true, 0.10);
                AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupTargetElement, 0.02, true, false, 0.10); }
                if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 0.02, true, false, 0.10); }
                //if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                vSearchOpen = true;

                //Focus on the textbox
                await FocusOnElement(grid_Popup_Search_textbox_Search, 0, 0);
            }
            catch { }
        }

        //Reset the popup to defaults
        async Task Popup_Reset_Search(bool FocusTextbox)
        {
            try
            {
                //Clear the current popup list
                List_Search.Clear();
                GC.Collect();

                //Reset the search text
                grid_Popup_Search_Count_TextBlock.Text = string.Empty;
                grid_Popup_Search_textbox_Search.Text = string.Empty;
                grid_Popup_Search_textblock_Result.Text = "Please enter a search term above.";
                grid_Popup_Search_textblock_Result.Visibility = Visibility.Visible;

                //Show or hide the keyboard icon
                if (CheckKeyboardEnabled()) { grid_Popup_Search_button_KeyboardController.Visibility = Visibility.Visible; }
                else { grid_Popup_Search_button_KeyboardController.Visibility = Visibility.Collapsed; }

                //Focus on the textbox
                if (FocusTextbox)
                {
                    await FocusOnElement(grid_Popup_Search_textbox_Search, 0, 0);
                }
            }
            catch { }
        }

        async Task Popup_Close_Search(FrameworkElement FocusElement)
        {
            try
            {
                if (vSearchOpen)
                {
                    PlayInterfaceSound("PopupClose", false);

                    //Reset popup variables
                    vSearchOpen = false;

                    //Clear the current popup list
                    List_Search.Clear();
                    GC.Collect();

                    //Hide the popup with animation
                    AVAnimations.Ani_Visibility(grid_Popup_Search, false, false, 0.10);

                    if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                    else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                    else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                    else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupTargetElement, 1, true, true, 0.10); }
                    else if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 1, true, true, 0.10); }
                    else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                    else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                    while (grid_Popup_Search.Visibility == Visibility.Visible) { await Task.Delay(10); }

                    //Focus on the desired element
                    if (FocusElement != null) { await FocusOnElement(FocusElement, 0, 0); }
                }
            }
            catch { }
        }

        //Update the search results count
        void UpdateSearchResults()
        {
            try
            {
                if (List_Search.Count == 0)
                {
                    grid_Popup_Search_Count_TextBlock.Text = string.Empty;
                    grid_Popup_Search_textblock_Result.Text = "No search results found.";
                    grid_Popup_Search_textblock_Result.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_Popup_Search_Count_TextBlock.Text = " " + List_Search.Count.ToString();
                    grid_Popup_Search_textblock_Result.Visibility = Visibility.Collapsed;
                    lb_Search.SelectedIndex = 0;
                }
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
                    foreach (DataBindApp result in searchResult)
                    {
                        try
                        {
                            List_Search.Add(result);
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
                }
            }
            catch { }
        }
    }
}