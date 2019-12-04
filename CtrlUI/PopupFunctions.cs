using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.SoundPlayer;

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

                PlayInterfaceSound(vInterfaceSoundVolume, "PopupOpen", false);

                //Save previous focused element
                if (Keyboard.FocusedElement != null)
                {
                    vMainMenuElementFocus.FocusPrevious = (FrameworkElement)Keyboard.FocusedElement;
                    if (vMainMenuElementFocus.FocusPrevious.GetType() == typeof(ListBoxItem))
                    {
                        vMainMenuElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(vMainMenuElementFocus.FocusPrevious);
                        vMainMenuElementFocus.FocusIndex = vMainMenuElementFocus.FocusListBox.SelectedIndex;
                    }
                }

                //Show the popup with animation
                AVAnimations.Ani_Visibility(grid_Popup_MainMenu, true, true, 0.10);
                AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 0.02, true, false, 0.10); }
                if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 0.02, true, false, 0.10); }
                if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 0.02, true, false, 0.10); }
                if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                //if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                vMainMenuOpen = true;

                //Focus on the menu listbox
                await FocusOnListbox(listbox_MainMenu, false, false, -1);

                //Update the clock with date
                UpdateClock();

                //Update the controller help
                UpdateControllerHelp();
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
                    PlayInterfaceSound(vInterfaceSoundVolume, "PopupClose", false);

                    //Reset popup variables
                    vMainMenuOpen = false;

                    //Hide the popup with animation
                    AVAnimations.Ani_Visibility(grid_Popup_MainMenu, false, false, 0.10);

                    if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                    else if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 1, true, true, 0.10); }
                    else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                    else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                    else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 1, true, true, 0.10); }
                    else if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 1, true, true, 0.10); }
                    else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                    else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                    while (grid_Popup_MainMenu.Visibility == Visibility.Visible) { await Task.Delay(10); }

                    //Update the clock without date
                    UpdateClock();

                    //Force focus on an element
                    if (vMainMenuElementFocus.FocusTarget != null)
                    {
                        await FocusOnElement(vMainMenuElementFocus.FocusTarget, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (vMainMenuElementFocus.FocusListBox != null)
                    {
                        await FocusOnListbox(vMainMenuElementFocus.FocusListBox, false, false, vMainMenuElementFocus.FocusIndex);
                    }
                    else
                    {
                        await FocusOnElement(vMainMenuElementFocus.FocusPrevious, false, vProcessCurrent.MainWindowHandle);
                    }

                    //Reset previous focus
                    vMainMenuElementFocus.Reset();
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
                    PlayInterfaceSound(vInterfaceSoundVolume, "PopupOpen", false);

                    //Update popup variables
                    vPopupElementTarget = ShowPopup;

                    //Save previous focused element
                    if (Keyboard.FocusedElement != null)
                    {
                        vPopupElementFocus.FocusPrevious = (FrameworkElement)Keyboard.FocusedElement;
                        if (vPopupElementFocus.FocusPrevious.GetType() == typeof(ListBoxItem))
                        {
                            vPopupElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(vPopupElementFocus.FocusPrevious);
                            vPopupElementFocus.FocusIndex = vPopupElementFocus.FocusListBox.SelectedIndex;
                        }
                    }

                    //Show the popup with animation
                    AVAnimations.Ani_Visibility(ShowPopup, true, true, 0.10);
                    AVAnimations.Ani_Opacity(grid_Main, 0.08, true, false, 0.10);

                    if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 0.02, true, false, 0.10); }
                    if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 0.02, true, false, 0.10); }
                    if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 0.02, true, false, 0.10); }
                    //if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupTargetElement, 0.02, true, false, 0.10); }
                    if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 0.02, true, false, 0.10); }
                    if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 0.02, true, false, 0.10); }
                    if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 0.02, true, false, 0.10); }

                    vPopupOpen = true;

                    //Force focus on an element
                    if (FocusElement != null)
                    {
                        await FocusOnElement(FocusElement, false, vProcessCurrent.MainWindowHandle);
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
                    PlayInterfaceSound(vInterfaceSoundVolume, "PopupClose", false);

                    //Reset popup variables
                    vPopupOpen = false;

                    //Hide the popup with animation
                    AVAnimations.Ani_Visibility(vPopupElementTarget, false, false, 0.10);

                    if (!Popup_Any_Open()) { AVAnimations.Ani_Opacity(grid_Main, 1, true, true, 0.10); }
                    else if (vTextInputOpen) { AVAnimations.Ani_Opacity(grid_Popup_TextInput, 1, true, true, 0.10); }
                    else if (vMessageBoxOpen) { AVAnimations.Ani_Opacity(grid_Popup_MessageBox, 1, true, true, 0.10); }
                    else if (vFilePickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_FilePicker, 1, true, true, 0.10); }
                    else if (vPopupOpen) { AVAnimations.Ani_Opacity(vPopupElementTarget, 1, true, true, 0.10); }
                    else if (vColorPickerOpen) { AVAnimations.Ani_Opacity(grid_Popup_ColorPicker, 1, true, true, 0.10); }
                    else if (vSearchOpen) { AVAnimations.Ani_Opacity(grid_Popup_Search, 1, true, true, 0.10); }
                    else if (vMainMenuOpen) { AVAnimations.Ani_Opacity(grid_Popup_MainMenu, 1, true, true, 0.10); }

                    while (vPopupElementTarget.Visibility == Visibility.Visible) { await Task.Delay(10); }

                    //Force focus on an element
                    if (vPopupElementFocus.FocusTarget != null)
                    {
                        await FocusOnElement(vPopupElementFocus.FocusTarget, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (vPopupElementFocus.FocusListBox != null)
                    {
                        await FocusOnListbox(vPopupElementFocus.FocusListBox, false, false, vPopupElementFocus.FocusIndex);
                    }
                    else
                    {
                        await FocusOnElement(vPopupElementFocus.FocusPrevious, false, vProcessCurrent.MainWindowHandle);
                    }

                    //Reset previous focus
                    vPopupElementFocus.Reset();
                }
            }
            catch { }
        }

        //Close open top popup (xaml order)
        async Task Popup_Close_Top()
        {
            try
            {
                if (vTextInputOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_TextInput(); }); }
                else if (vMessageBoxOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MessageBox(); }); }
                else if (vFilePickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_FilePicker(false, false); }); }
                else if (vPopupOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close(); }); }
                else if (vColorPickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_ColorPicker(); }); }
                else if (vSearchOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_Search(); }); }
                else if (vMainMenuOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MainMenu(); }); }
            }
            catch { }
        }

        //Close all open popups (xaml order)
        async Task Popup_Close_All()
        {
            try
            {
                if (vTextInputOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_TextInput(); }); }
                if (vMessageBoxOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MessageBox(); }); }
                if (vFilePickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_FilePicker(false, false); }); }
                if (vPopupOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close(); }); }
                if (vColorPickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_ColorPicker(); }); }
                if (vSearchOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_Search(); }); }
                if (vMainMenuOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MainMenu(); }); }
            }
            catch { }
        }

        //Check if there is any popup open
        bool Popup_Any_Open()
        {
            try
            {
                if (vPopupOpen || vColorPickerOpen || vSearchOpen || vMainMenuOpen || vFilePickerOpen || vMessageBoxOpen) { return true; }
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