﻿using ArnoldVinkCode;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVFocus;
using static CtrlUI.AppVariables;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the popup
        void Popup_Show_Element(FrameworkElement elementTarget, double mainOpacity = 0.10)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Show the popup
                    elementTarget.Visibility = Visibility.Visible;
                    elementTarget.IsEnabled = true;

                    //Hide the background
                    grid_Main.Opacity = mainOpacity;
                    grid_Main.IsEnabled = false;
                    grid_ControllerHelp_Content.Opacity = mainOpacity;

                    //Hide other popups
                    if (elementTarget != grid_Popup_TextInput && vTextInputOpen)
                    {
                        grid_Popup_TextInput.Opacity = 0.02;
                        grid_Popup_TextInput.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_MessageBox && vMessageBoxOpen)
                    {
                        grid_Popup_MessageBox.Opacity = 0.02;
                        grid_Popup_MessageBox.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_Sorting && vSortingOpen)
                    {
                        grid_Popup_Sorting.Opacity = 0.02;
                        grid_Popup_Sorting.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_HowLongToBeat && vHowLongToBeatOpen)
                    {
                        grid_Popup_HowLongToBeat.Opacity = 0.02;
                        grid_Popup_HowLongToBeat.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_ContentInformation && vContentInformationOpen)
                    {
                        grid_Popup_ContentInformation.Opacity = 0.02;
                        grid_Popup_ContentInformation.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_FilePicker && vFilePickerOpen)
                    {
                        grid_Popup_FilePicker.Opacity = 0.02;
                        grid_Popup_FilePicker.IsEnabled = false;
                    }
                    if (elementTarget != vPopupElementTarget && vPopupOpen)
                    {
                        vPopupElementTarget.Opacity = 0.02;
                        vPopupElementTarget.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_ColorPicker && vColorPickerOpen)
                    {
                        grid_Popup_ColorPicker.Opacity = 0.02;
                        grid_Popup_ColorPicker.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_MainMenu && vMainMenuOpen)
                    {
                        grid_Popup_MainMenu.Opacity = 0.02;
                        grid_Popup_MainMenu.IsEnabled = false;
                    }
                });
            }
            catch { }
        }

        //Hide the popup
        void Popup_Hide_Element(FrameworkElement elementTarget)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Hide the popup
                    elementTarget.Visibility = Visibility.Collapsed;
                    elementTarget.IsEnabled = false;

                    //Show the background
                    if (!Popup_Open_Any())
                    {
                        grid_Main.Opacity = 1.00;
                        grid_Main.IsEnabled = true;
                        grid_ControllerHelp_Content.Opacity = 1.00;
                        return;
                    }

                    //Show other popups
                    if (vTextInputOpen)
                    {
                        grid_Popup_TextInput.Opacity = 1.00;
                        grid_Popup_TextInput.IsEnabled = true;
                    }
                    else if (vSortingOpen)
                    {
                        grid_Popup_Sorting.Opacity = 1.00;
                        grid_Popup_Sorting.IsEnabled = true;
                    }
                    else if (vHowLongToBeatOpen)
                    {
                        grid_Popup_HowLongToBeat.Opacity = 1.00;
                        grid_Popup_HowLongToBeat.IsEnabled = true;
                    }
                    else if (vContentInformationOpen)
                    {
                        grid_Popup_ContentInformation.Opacity = 1.00;
                        grid_Popup_ContentInformation.IsEnabled = true;
                    }
                    else if (vMessageBoxOpen)
                    {
                        grid_Popup_MessageBox.Opacity = 1.00;
                        grid_Popup_MessageBox.IsEnabled = true;
                    }
                    else if (vFilePickerOpen)
                    {
                        grid_Popup_FilePicker.Opacity = 1.00;
                        grid_Popup_FilePicker.IsEnabled = true;
                    }
                    else if (vPopupOpen)
                    {
                        vPopupElementTarget.Opacity = 1.00;
                        vPopupElementTarget.IsEnabled = true;
                    }
                    else if (vColorPickerOpen)
                    {
                        grid_Popup_ColorPicker.Opacity = 1.00;
                        grid_Popup_ColorPicker.IsEnabled = true;
                    }
                    else if (vMainMenuOpen)
                    {
                        grid_Popup_MainMenu.Opacity = 1.00;
                        grid_Popup_MainMenu.IsEnabled = true;
                    }
                });
            }
            catch { }
        }

        //Hide or show the main menu
        async Task Popup_ShowHide_MainMenu(bool ForceShow)
        {
            try
            {
                if (vMainMenuOpen)
                {
                    await Popup_Close_Top(false);
                    return;
                }

                if (ForceShow)
                {
                    await Popup_Close_All();
                }

                if (Popup_Open_Any())
                {
                    return;
                }

                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                //Save the previous focus element
                AVFocusDetailsSave(vMainMenuElementFocus, null);

                //Show the popup
                Popup_Show_Element(grid_Popup_MainMenu);

                vMainMenuOpen = true;

                //Focus on the menu listbox
                await ListBoxFocusIndex(listbox_MainMenu, false, -1, vProcessCurrent.WindowHandleMain);

                //Update the clock with date
                UpdateClockTime();

                //Update controller help
                UpdateControllerHelp();

                //Update controller colors
                UpdateControllerColor();
            }
            catch { }
        }

        //Close all the open popups
        async Task Popup_Close_MainMenu()
        {
            try
            {
                if (vMainMenuOpen)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                    //Reset popup variables
                    vMainMenuOpen = false;

                    //Hide the popup
                    Popup_Hide_Element(grid_Popup_MainMenu);

                    //Update the clock without date
                    UpdateClockTime();

                    //Focus on the previous focus element
                    await AVFocusDetailsFocus(vMainMenuElementFocus, vProcessCurrent.WindowHandleMain);
                }
            }
            catch { }
        }

        //Show specific popup
        async Task Popup_Show(FrameworkElement ShowPopup, FrameworkElement FocusElement, double mainOpacity = 0.08)
        {
            try
            {
                if (!vPopupOpen)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                    //Update popup variables
                    vPopupElementTarget = ShowPopup;

                    //Save the previous focus element
                    AVFocusDetailsSave(vPopupElementFocus, null);

                    //Show the popup
                    Popup_Show_Element(ShowPopup, mainOpacity);

                    //Update popup variables
                    vPopupOpen = true;

                    //Force focus on element
                    if (FocusElement != null)
                    {
                        await AVFocus.FocusElement(FocusElement, vProcessCurrent.WindowHandleMain);
                    }
                }
            }
            catch { }
        }

        //Close all the open popups
        async Task Popup_Close()
        {
            try
            {
                if (vPopupOpen)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                    //Reset popup variables
                    vPopupOpen = false;

                    //Hide the popup
                    Popup_Hide_Element(vPopupElementTarget);

                    //Focus on the previous focus element
                    await AVFocusDetailsFocus(vPopupElementFocus, vProcessCurrent.WindowHandleMain);
                }
            }
            catch { }
        }

        //Close open top popup (xaml order)
        async Task Popup_Close_Top(bool waitClose, bool selectEditItem = false)
        {
            try
            {
                //Close open top popup
                if (vTextInputOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_TextInput(); }); }
                else if (vMessageBoxOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_MessageBox(); }); }
                else if (vHowLongToBeatOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_HowLongToBeat(); }); }
                else if (vSortingOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_Sorting(); }); }
                else if (vContentInformationOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_ContentInformation(); }); }
                else if (vFilePickerOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_FilePicker(false, false); }); }
                else if (vColorPickerOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_ColorPicker(); }); }
                else if (vPopupOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close(); }); }
                else if (vMainMenuOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_MainMenu(); }); }

                //Wait for popups to have closed
                if (waitClose)
                {
                    await Task.Delay(250);
                }

                //Focus on the edit listbox item
                if (selectEditItem)
                {
                    await FocusEditListboxItem();
                }
            }
            catch { }
        }

        //Close all open popups (xaml order)
        async Task Popup_Close_All()
        {
            try
            {
                if (vTextInputOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_TextInput(); }); }
                if (vMessageBoxOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_MessageBox(); }); }
                if (vHowLongToBeatOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_HowLongToBeat(); }); }
                if (vSortingOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_Sorting(); }); }
                if (vContentInformationOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_ContentInformation(); }); }
                if (vFilePickerOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_FilePicker(false, false); }); }
                if (vColorPickerOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_ColorPicker(); }); }
                if (vPopupOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close(); }); }
                if (vMainMenuOpen) { await AVActions.DispatcherInvoke(async delegate { await Popup_Close_MainMenu(); }); }
            }
            catch { }
        }

        //Check if there is any popup open
        bool Popup_Open_Any()
        {
            try
            {
                if (vPopupOpen || vColorPickerOpen || vMainMenuOpen || vFilePickerOpen || vHowLongToBeatOpen || vContentInformationOpen || vMessageBoxOpen || vTextInputOpen || vSortingOpen)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Check if specific popup is open
        bool Popup_Open_Check(FrameworkElement popupGrid)
        {
            bool popupOpen = false;
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    popupOpen = vPopupOpen && vPopupElementTarget == popupGrid;
                });
            }
            catch { }
            return popupOpen;
        }
    }
}