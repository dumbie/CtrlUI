using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static ArnoldVinkCode.AVImage;
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
                //Show the popup
                AVActions.ElementSetValue(elementTarget, VisibilityProperty, Visibility.Visible);
                AVActions.ElementSetValue(elementTarget, IsEnabledProperty, true);

                //Hide the background
                AVActions.ElementSetValue(grid_Video_Background, OpacityProperty, 0.08);
                AVActions.ElementSetValue(grid_Main, OpacityProperty, 0.08);
                AVActions.ElementSetValue(grid_Main, IsEnabledProperty, false);

                //Hide other popups
                if (elementTarget != grid_Popup_TextInput && vTextInputOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_TextInput, OpacityProperty, 0.02);
                    AVActions.ElementSetValue(grid_Popup_TextInput, IsEnabledProperty, false);
                }
                if (elementTarget != grid_Popup_MessageBox && vMessageBoxOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_MessageBox, OpacityProperty, 0.02);
                    AVActions.ElementSetValue(grid_Popup_MessageBox, IsEnabledProperty, false);
                }
                if (elementTarget != grid_Popup_FilePicker && vFilePickerOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_FilePicker, OpacityProperty, 0.02);
                    AVActions.ElementSetValue(grid_Popup_FilePicker, IsEnabledProperty, false);
                }
                if (elementTarget != vPopupElementTarget && vPopupOpen)
                {
                    AVActions.ElementSetValue(vPopupElementTarget, OpacityProperty, 0.02);
                    AVActions.ElementSetValue(vPopupElementTarget, IsEnabledProperty, false);
                }
                if (elementTarget != grid_Popup_ColorPicker && vColorPickerOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_ColorPicker, OpacityProperty, 0.02);
                    AVActions.ElementSetValue(grid_Popup_ColorPicker, IsEnabledProperty, false);
                }
                if (elementTarget != grid_Popup_Search && vSearchOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_Search, OpacityProperty, 0.02);
                    AVActions.ElementSetValue(grid_Popup_Search, IsEnabledProperty, false);
                }
                if (elementTarget != grid_Popup_MainMenu && vMainMenuOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_MainMenu, OpacityProperty, 0.02);
                    AVActions.ElementSetValue(grid_Popup_MainMenu, IsEnabledProperty, false);
                }
            }
            catch { }
        }

        //Hide the popup
        void Popup_Hide_Element(FrameworkElement elementTarget)
        {
            try
            {
                //Hide the popup
                AVActions.ElementSetValue(elementTarget, VisibilityProperty, Visibility.Collapsed);
                AVActions.ElementSetValue(elementTarget, IsEnabledProperty, false);

                //Show the background
                if (!Popup_Any_Open())
                {
                    double backgroundBrightness = (double)Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundBrightness"]) / 100;
                    AVActions.ElementSetValue(grid_Video_Background, OpacityProperty, backgroundBrightness);
                    AVActions.ElementSetValue(grid_Main, OpacityProperty, 1.00);
                    AVActions.ElementSetValue(grid_Main, IsEnabledProperty, true);
                    return;
                }

                //Show other popups
                if (vTextInputOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_TextInput, OpacityProperty, 1.00);
                    AVActions.ElementSetValue(grid_Popup_TextInput, IsEnabledProperty, true);
                }
                else if (vMessageBoxOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_MessageBox, OpacityProperty, 1.00);
                    AVActions.ElementSetValue(grid_Popup_MessageBox, IsEnabledProperty, true);
                }
                else if (vFilePickerOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_FilePicker, OpacityProperty, 1.00);
                    AVActions.ElementSetValue(grid_Popup_FilePicker, IsEnabledProperty, true);
                }
                else if (vPopupOpen)
                {
                    AVActions.ElementSetValue(vPopupElementTarget, OpacityProperty, 1.00);
                    AVActions.ElementSetValue(vPopupElementTarget, IsEnabledProperty, true);
                }
                else if (vColorPickerOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_ColorPicker, OpacityProperty, 1.00);
                    AVActions.ElementSetValue(grid_Popup_ColorPicker, IsEnabledProperty, true);
                }
                else if (vSearchOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_Search, OpacityProperty, 1.00);
                    AVActions.ElementSetValue(grid_Popup_Search, IsEnabledProperty, true);
                }
                else if (vMainMenuOpen)
                {
                    AVActions.ElementSetValue(grid_Popup_MainMenu, OpacityProperty, 1.00);
                    AVActions.ElementSetValue(grid_Popup_MainMenu, IsEnabledProperty, true);
                }
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

                PlayInterfaceSound("PopupOpen", false);

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
                    PlayInterfaceSound("PopupClose", false);

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
                    PlayInterfaceSound("PopupOpen", false);

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
                    PlayInterfaceSound("PopupClose", false);

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

        //Show the status popup
        public void Popup_Show_Status(string IconName, string Message)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    grid_Message_Status_Image.Source = FileToBitmapImage(new string[] { "Assets/Icons/" + IconName + ".png" }, IntPtr.Zero, -1, 0);
                    grid_Message_Status_Text.Text = Message;
                    grid_Message_Status.Visibility = Visibility.Visible;
                });

                vDispatcherTimerOverlay.Stop();
                vDispatcherTimerOverlay.Interval = TimeSpan.FromSeconds(3);
                vDispatcherTimerOverlay.Tick += delegate
                {
                    grid_Message_Status.Visibility = Visibility.Collapsed;
                    vDispatcherTimerOverlay.Stop();
                };
                vDispatcherTimerOverlay.Start();
            }
            catch { }
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
                        if (frameworkElementFocus.FocusElement.GetType() == typeof(ListBoxItem))
                        {
                            frameworkElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(frameworkElementFocus.FocusElement);
                            frameworkElementFocus.FocusIndex = frameworkElementFocus.FocusListBox.SelectedIndex;
                        }
                    }
                    else if (Keyboard.FocusedElement != null && Keyboard.FocusedElement != App.vWindowMain)
                    {
                        Debug.WriteLine("Saved previous focus keyboard: " + Keyboard.FocusedElement);
                        frameworkElementFocus.FocusElement = (FrameworkElement)Keyboard.FocusedElement;
                        if (frameworkElementFocus.FocusElement.GetType() == typeof(ListBoxItem))
                        {
                            frameworkElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(frameworkElementFocus.FocusElement);
                            frameworkElementFocus.FocusIndex = frameworkElementFocus.FocusListBox.SelectedIndex;
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
                        KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                    }

                    //Reset the previous focus
                    frameworkElementFocus.Reset();
                });
            }
            catch { }
        }
    }
}