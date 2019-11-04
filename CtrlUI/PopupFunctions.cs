using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Hide or show the main menu
        async Task Popup_ShowHide_MainMenu(bool ForceShow)
        {
            try
            {
                if (vMainMenuOpen)
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

                PlayInterfaceSound("PopupOpen", false);

                //Save previous focused element
                if (Keyboard.FocusedElement != null)
                {
                    vMainMenuPreviousFocus = (FrameworkElement)Keyboard.FocusedElement;
                }

                //Show the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_MainMenu, true, true, 0.10);
                AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupTargetElement, 0.02, true, false, 0.10); }
                if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                //if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                vMainMenuOpen = true;

                //Focus on the menu listbox
                await FocusOnListbox(listbox_MainMenu, false, false, -1, false);

                //Update the clock with date
                UpdateClock();

                //Update the controller help
                UpdateControllerHelp();
            }
            catch { }
        }

        //Close all the open popups
        async Task Popup_Close_MainMenu(FrameworkElement FocusElement)
        {
            try
            {
                if (vMainMenuOpen)
                {
                    PlayInterfaceSound("PopupClose", false);

                    //Reset popup variables
                    vMainMenuOpen = false;

                    //Hide the popup with animation
                    AVAnimations.Ani_Visibility(grid_Popup_MainMenu, false, false, 0.10);

                    if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                    else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                    else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                    else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupTargetElement, 1, true, true, 0.10); }
                    else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                    else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                    while (grid_Popup_MainMenu.Visibility == Visibility.Visible) { await Task.Delay(10); }

                    //Update the clock without date
                    UpdateClock();

                    //Focus on the desired element
                    if (FocusElement != null) { await FocusOnElement(FocusElement, 0, 0); }
                }
            }
            catch { }
        }

        //Show specific popup
        async Task Popup_Show(FrameworkElement ShowPopup, FrameworkElement FocusElement, bool QuickClose)
        {
            try
            {
                if (!vPopupOpen)
                {
                    PlayInterfaceSound("PopupOpen", false);

                    //Update popup variables
                    vPopupTargetElement = ShowPopup;

                    //Save previous focused element
                    if (Keyboard.FocusedElement != null) { vPopupPreviousFocus = (FrameworkElement)Keyboard.FocusedElement; }

                    //Show the popup with animation
                    AVAnimations.Ani_Visibility(ShowPopup, true, true, 0.10);
                    AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                    if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                    if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                    //if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupTargetElement, 0.02, true, false, 0.10); }
                    if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                    if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                    vPopupOpen = true;

                    //Focus on the desired element
                    if (FocusElement != null) { await FocusOnElement(FocusElement, 0, 0); }
                }
            }
            catch { }
        }

        //Close all the open popups
        async Task Popup_Close(FrameworkElement FocusElement)
        {
            try
            {
                if (vPopupOpen)
                {
                    PlayInterfaceSound("PopupClose", false);

                    //Reset popup variables
                    vPopupOpen = false;

                    //Hide the popup with animation
                    AVAnimations.Ani_Visibility(vPopupTargetElement, false, false, 0.10);

                    if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                    else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                    else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                    else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupTargetElement, 1, true, true, 0.10); }
                    else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                    else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                    while (vPopupTargetElement.Visibility == Visibility.Visible) { await Task.Delay(10); }

                    //Focus on the desired element
                    if (FocusElement != null) { await FocusOnElement(FocusElement, 0, 0); }
                }
            }
            catch { }
        }

        //Close open top popup (xaml order)
        async Task Popup_Close_Top()
        {
            try
            {
                if (vMessageBoxOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MessageBox(); }); }
                else if (vFilePickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_FilePicker(false, false); }); }
                else if (vPopupOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close(vPopupPreviousFocus); }); }
                else if (vSearchOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_Search(vSearchPreviousFocus); }); }
                else if (vMainMenuOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MainMenu(vMainMenuPreviousFocus); }); }
            }
            catch { }
        }

        //Close all open popups (xaml order)
        async Task Popup_Close_All()
        {
            try
            {
                if (vMessageBoxOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MessageBox(); }); }
                if (vFilePickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_FilePicker(false, false); }); }
                if (vPopupOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close(vPopupPreviousFocus); }); }
                if (vSearchOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_Search(vSearchPreviousFocus); }); }
                if (vMainMenuOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MainMenu(vMainMenuPreviousFocus); }); }
            }
            catch { }
        }

        //Check if there is any popup open
        bool Popup_Any_Open()
        {
            try
            {
                if (vPopupOpen || vSearchOpen || vMainMenuOpen || vFilePickerOpen || vMessageBoxOpen) { return true; }
                else { return false; }
            }
            catch { return false; }
        }

        //Show the status popup
        public void Popup_Show_Status(string IconName, string Message)
        {
            try
            {
                vDispatcherTimer.Stop();

                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/" + IconName + ".png" }, IntPtr.Zero, -1);
                    grid_Message_Status_Text.Text = Message;
                    grid_Message_Status.Visibility = Visibility.Visible;
                });

                vDispatcherTimer.Interval = TimeSpan.FromSeconds(3);
                vDispatcherTimer.Tick += delegate
                {
                    grid_Message_Status.Visibility = Visibility.Collapsed;
                    vDispatcherTimer.Stop();
                };
                vDispatcherTimer.Start();
            }
            catch { }
        }
    }
}