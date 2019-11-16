using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
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
                if (Keyboard.FocusedElement != null)
                {
                    vSearchElementFocus.FocusPrevious = (FrameworkElement)Keyboard.FocusedElement;
                    if (vSearchElementFocus.FocusPrevious.GetType() == typeof(ListBoxItem))
                    {
                        vSearchElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(vSearchElementFocus.FocusPrevious);
                        vSearchElementFocus.FocusIndex = vSearchElementFocus.FocusListBox.SelectedIndex;
                    }
                }

                //Show the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_Search, true, true, 0.10);
                AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 0.02, true, false, 0.10); }
                if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 0.02, true, false, 0.10); }
                //if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                vSearchOpen = true;

                //Force focus on an element
                if (vAppActivated && vControllerAnyConnected())
                {
                    await FocusOnElement(grid_Popup_Search_button_KeyboardControllerButton, false, vProcessCurrent.MainWindowHandle);
                }
                else
                {
                    await FocusOnElement(grid_Popup_Search_textbox_Search, false, vProcessCurrent.MainWindowHandle);
                }
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

                if (FocusTextbox)
                {
                    grid_Popup_Search_textbox_Search.Text = string.Empty;
                }
                else
                {
                    grid_Popup_Search_textbox_Search.Text = "Search application...";
                }

                grid_Popup_Search_textblock_Result.Text = "Please enter a search term above.";
                grid_Popup_Search_textblock_Result.Visibility = Visibility.Visible;
                grid_Popup_Search_button_KeyboardControllerButton.Visibility = Visibility.Visible;

                //Show or hide the keyboard icon
                if (CheckKeyboardEnabled())
                {
                    grid_Popup_Search_button_KeyboardControllerIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_Popup_Search_button_KeyboardControllerIcon.Visibility = Visibility.Collapsed;
                }

                //Force focus on an element
                if (FocusTextbox)
                {
                    await FocusOnElement(grid_Popup_Search_textbox_Search, false, vProcessCurrent.MainWindowHandle);
                }
            }
            catch { }
        }

        async Task Popup_Close_Search()
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
                    else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 1, true, true, 0.10); }
                    else if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 1, true, true, 0.10); }
                    else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                    else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                    while (grid_Popup_Search.Visibility == Visibility.Visible)
                    {
                        await Task.Delay(10);
                    }

                    //Force focus on an element
                    if (vSearchElementFocus.FocusTarget != null)
                    {
                        await FocusOnElement(vSearchElementFocus.FocusTarget, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (vSearchElementFocus.FocusListBox != null)
                    {
                        await FocusOnListbox(vSearchElementFocus.FocusListBox, false, false, vSearchElementFocus.FocusIndex);
                    }
                    else
                    {
                        await FocusOnElement(vSearchElementFocus.FocusPrevious, false, vProcessCurrent.MainWindowHandle);
                    }

                    //Reset previous focus
                    vSearchElementFocus.Reset();
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
                    grid_Popup_Search_textbox_Search.Text = string.Empty;
                    grid_Popup_Search_textblock_Result.Text = "No search results found.";
                    grid_Popup_Search_textblock_Result.Visibility = Visibility.Visible;
                    grid_Popup_Search_button_KeyboardControllerButton.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_Popup_Search_Count_TextBlock.Text = " " + List_Search.Count.ToString();
                    grid_Popup_Search_textblock_Result.Visibility = Visibility.Collapsed;
                    grid_Popup_Search_button_KeyboardControllerButton.Visibility = Visibility.Collapsed;
                    lb_Search.SelectedIndex = 0;
                }
            }
            catch { }
        }
    }
}