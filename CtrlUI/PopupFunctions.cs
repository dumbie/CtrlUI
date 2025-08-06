using ArnoldVinkStyles;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVFocus;
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
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    //Show the popup
                    elementTarget.Visibility = Visibility.Visible;
                    elementTarget.AvSetEnabled(true, null);

                    //Hide and disable main
                    grid_Main.AvSetEnabled(false, mainOpacity);
                    grid_ControllerHelp_Content.AvSetEnabled(false, mainOpacity);

                    //Hide and disable popups
                    if (elementTarget != grid_Popup_TextInput && vTextInputOpen)
                    {
                        grid_Popup_TextInput.AvSetEnabled(false, 0.02);
                    }
                    if (elementTarget != grid_Popup_MessageBox && vMessageBoxOpen)
                    {
                        grid_Popup_MessageBox.AvSetEnabled(false, 0.02);
                    }
                    if (elementTarget != grid_Popup_Sorting && vSortingOpen)
                    {
                        grid_Popup_Sorting.AvSetEnabled(false, 0.02);
                    }
                    if (elementTarget != grid_Popup_HowLongToBeat && vHowLongToBeatOpen)
                    {
                        grid_Popup_HowLongToBeat.AvSetEnabled(false, 0.02);
                    }
                    if (elementTarget != grid_Popup_ContentInformation && vContentInformationOpen)
                    {
                        grid_Popup_ContentInformation.AvSetEnabled(false, 0.02);
                    }
                    if (elementTarget != grid_Popup_FilePicker && vFilePickerOpen)
                    {
                        grid_Popup_FilePicker.AvSetEnabled(false, 0.02);
                    }
                    if (elementTarget != vPopupElementTarget && vPopupOpen)
                    {
                        vPopupElementTarget.AvSetEnabled(false, 0.02);
                    }
                    if (elementTarget != grid_Popup_ColorPicker && vColorPickerOpen)
                    {
                        grid_Popup_ColorPicker.AvSetEnabled(false, 0.02);
                    }
                    if (elementTarget != grid_Popup_MainMenu && vMainMenuOpen)
                    {
                        grid_Popup_MainMenu.AvSetEnabled(false, 0.02);
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
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    //Hide the popup
                    elementTarget.Visibility = Visibility.Collapsed;
                    elementTarget.AvSetEnabled(false, null);

                    //Show and enable main
                    if (!Popup_Open_Any())
                    {
                        grid_Main.AvSetEnabled(true, 1.00);
                        grid_ControllerHelp_Content.AvSetEnabled(true, 1.00);
                        return;
                    }

                    //Show and enable popups
                    if (vTextInputOpen)
                    {
                        grid_Popup_TextInput.AvSetEnabled(true, 1.00);
                    }
                    else if (vSortingOpen)
                    {
                        grid_Popup_Sorting.AvSetEnabled(true, 1.00);
                    }
                    else if (vHowLongToBeatOpen)
                    {
                        grid_Popup_HowLongToBeat.AvSetEnabled(true, 1.00);
                    }
                    else if (vContentInformationOpen)
                    {
                        grid_Popup_ContentInformation.AvSetEnabled(true, 1.00);
                    }
                    else if (vMessageBoxOpen)
                    {
                        grid_Popup_MessageBox.AvSetEnabled(true, 1.00);
                    }
                    else if (vFilePickerOpen)
                    {
                        grid_Popup_FilePicker.AvSetEnabled(true, 1.00);
                    }
                    else if (vPopupOpen)
                    {
                        vPopupElementTarget.AvSetEnabled(true, 1.00);
                    }
                    else if (vColorPickerOpen)
                    {
                        grid_Popup_ColorPicker.AvSetEnabled(true, 1.00);
                    }
                    else if (vMainMenuOpen)
                    {
                        grid_Popup_MainMenu.AvSetEnabled(true, 1.00);
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
                await ListViewFocusIndex(listView_MainMenu, false, -1, vProcessCurrent.WindowHandleMain);

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
                        await AVFocus.FocusFrameworkElement(FocusElement, vProcessCurrent.WindowHandleMain);
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
                if (vTextInputOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_TextInput(); }); }
                else if (vMessageBoxOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_MessageBox(); }); }
                else if (vHowLongToBeatOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_HowLongToBeat(); }); }
                else if (vSortingOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_Sorting(); }); }
                else if (vContentInformationOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_ContentInformation(); }); }
                else if (vFilePickerOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_FilePicker(false, false); }); }
                else if (vColorPickerOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_ColorPicker(); }); }
                else if (vPopupOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close(); }); }
                else if (vMainMenuOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_MainMenu(); }); }

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
                if (vTextInputOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_TextInput(); }); }
                if (vMessageBoxOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_MessageBox(); }); }
                if (vHowLongToBeatOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_HowLongToBeat(); }); }
                if (vSortingOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_Sorting(); }); }
                if (vContentInformationOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_ContentInformation(); }); }
                if (vFilePickerOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_FilePicker(false, false); }); }
                if (vColorPickerOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_ColorPicker(); }); }
                if (vPopupOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close(); }); }
                if (vMainMenuOpen) { await DispatcherInvoke(this.Dispatcher, async delegate { await Popup_Close_MainMenu(); }); }
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
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    popupOpen = vPopupOpen && vPopupElementTarget == popupGrid;
                });
            }
            catch { }
            return popupOpen;
        }
    }
}