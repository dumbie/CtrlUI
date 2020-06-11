using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the popup
        void Popup_Show_Element(FrameworkElement elementTarget)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Show the popup
                    elementTarget.Visibility = Visibility.Visible;
                    elementTarget.IsEnabled = true;

                    //Hide the background
                    grid_Video_Background.Opacity = 0.08;
                    grid_Main.Opacity = 0.08;
                    grid_Main.IsEnabled = false;

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
                    if (elementTarget != grid_Popup_Search && vSearchOpen)
                    {
                        grid_Popup_Search.Opacity = 0.02;
                        grid_Popup_Search.IsEnabled = false;
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
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Hide the popup
                    elementTarget.Visibility = Visibility.Collapsed;
                    elementTarget.IsEnabled = false;

                    //Show the background
                    if (!Popup_Any_Open())
                    {
                        double backgroundBrightness = (double)Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundBrightness"]) / 100;
                        grid_Video_Background.Opacity = backgroundBrightness;
                        grid_Main.Opacity = 1.00;
                        grid_Main.IsEnabled = true;
                        return;
                    }

                    //Show other popups
                    if (vTextInputOpen)
                    {
                        grid_Popup_TextInput.Opacity = 1.00;
                        grid_Popup_TextInput.IsEnabled = true;
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
                    else if (vSearchOpen)
                    {
                        grid_Popup_Search.Opacity = 1.00;
                        grid_Popup_Search.IsEnabled = true;
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

                PlayInterfaceSound(vConfigurationApplication, "PopupOpen", false);

                //Save the previous focus element
                Popup_PreviousElementFocus_Save(vMainMenuElementFocus, null);

                //Show the popup
                Popup_Show_Element(grid_Popup_MainMenu);

                vMainMenuOpen = true;

                //Focus on the menu listbox
                await ListboxFocusIndex(listbox_MainMenu, false, false, -1);

                //Update the clock with date
                UpdateClockTime();

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
                    PlayInterfaceSound(vConfigurationApplication, "PopupClose", false);

                    //Reset popup variables
                    vMainMenuOpen = false;

                    //Hide the popup
                    Popup_Hide_Element(grid_Popup_MainMenu);

                    //Update the clock without date
                    UpdateClockTime();

                    //Focus on the previous focus element
                    await Popup_PreviousElementFocus_Focus(vMainMenuElementFocus);
                }
            }
            catch { }
        }

        //Show specific popup
        async Task Popup_Show(FrameworkElement ShowPopup, FrameworkElement FocusElement)
        {
            try
            {
                if (!vPopupOpen)
                {
                    PlayInterfaceSound(vConfigurationApplication, "PopupOpen", false);

                    //Update popup variables
                    vPopupElementTarget = ShowPopup;

                    //Save the previous focus element
                    Popup_PreviousElementFocus_Save(vPopupElementFocus, null);

                    //Show the popup
                    Popup_Show_Element(ShowPopup);

                    vPopupOpen = true;

                    //Force focus on element
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
                    PlayInterfaceSound(vConfigurationApplication, "PopupClose", false);

                    //Reset popup variables
                    vPopupOpen = false;

                    //Hide the popup
                    Popup_Hide_Element(vPopupElementTarget);

                    //Focus on the previous focus element
                    await Popup_PreviousElementFocus_Focus(vPopupElementFocus);
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
                if (vPopupOpen || vColorPickerOpen || vSearchOpen || vMainMenuOpen || vFilePickerOpen || vMessageBoxOpen || vTextInputOpen)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Save the previous focus element
        static void Popup_PreviousElementFocus_Save(FrameworkElementFocus frameworkElementFocus, FrameworkElement previousFocus)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (previousFocus != null)
                    {
                        Debug.WriteLine("Saved previous focus element: " + previousFocus);
                        frameworkElementFocus.FocusElement = previousFocus;
                        if (frameworkElementFocus.FocusElement != null && frameworkElementFocus.FocusElement.GetType() == typeof(ListBoxItem))
                        {
                            frameworkElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(frameworkElementFocus.FocusElement);
                            frameworkElementFocus.FocusIndex = frameworkElementFocus.FocusListBox.SelectedIndex;
                        }
                    }
                    else
                    {
                        //Get the currently focused element
                        FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;

                        //Check the currently focused element
                        if (frameworkElement != null && frameworkElement != App.vWindowMain)
                        {
                            Debug.WriteLine("Saved previous focus keyboard: " + frameworkElement);
                            frameworkElementFocus.FocusElement = frameworkElement;
                            if (frameworkElementFocus.FocusElement != null && frameworkElementFocus.FocusElement.GetType() == typeof(ListBoxItem))
                            {
                                frameworkElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(frameworkElementFocus.FocusElement);
                                frameworkElementFocus.FocusIndex = frameworkElementFocus.FocusListBox.SelectedIndex;
                            }
                        }
                    }
                });
            }
            catch { }
        }

        //Focus on the previous focus element
        async Task Popup_PreviousElementFocus_Focus(FrameworkElementFocus frameworkElementFocus)
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    //Check if focus element is disconnected
                    bool disconnectedSource = false;
                    if (frameworkElementFocus.FocusElement != null)
                    {
                        disconnectedSource = frameworkElementFocus.FocusElement.DataContext == BindingOperations.DisconnectedSource;
                    }

                    //Force focus on element
                    if (frameworkElementFocus.FocusElement != null && !disconnectedSource)
                    {
                        Debug.WriteLine("Focusing on previous element: " + frameworkElementFocus.FocusElement);
                        await FocusOnElement(frameworkElementFocus.FocusElement, false, vProcessCurrent.MainWindowHandle);
                    }
                    else if (frameworkElementFocus.FocusListBox != null)
                    {
                        Debug.WriteLine("Focusing on previous listbox: " + frameworkElementFocus.FocusListBox);
                        await ListboxFocusIndex(frameworkElementFocus.FocusListBox, false, false, frameworkElementFocus.FocusIndex);
                    }
                    else
                    {
                        Debug.WriteLine("No previous focus element, pressing tab key.");
                        await KeySendSingle(KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                    }

                    //Reset the previous focus
                    frameworkElementFocus.Reset();
                });
            }
            catch { }
        }
    }
}