using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
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
                UpdateElementVisibility(elementTarget, true);
                UpdateElementEnabled(elementTarget, true);

                //Hide the background
                UpdateElementOpacity(grid_Video_Background, 0.08);
                UpdateElementOpacity(grid_Main, 0.08);
                UpdateElementEnabled(grid_Main, false);

                //Hide other popups
                if (elementTarget != grid_Popup_TextInput && vTextInputOpen)
                {
                    UpdateElementOpacity(grid_Popup_TextInput, 0.02);
                    UpdateElementEnabled(grid_Popup_TextInput, false);
                }
                if (elementTarget != grid_Popup_MessageBox && vMessageBoxOpen)
                {
                    UpdateElementOpacity(grid_Popup_MessageBox, 0.02);
                    UpdateElementEnabled(grid_Popup_MessageBox, false);
                }
                if (elementTarget != grid_Popup_FilePicker && vFilePickerOpen)
                {
                    UpdateElementOpacity(grid_Popup_FilePicker, 0.02);
                    UpdateElementEnabled(grid_Popup_FilePicker, false);
                }
                if (elementTarget != vPopupElementTarget && vPopupOpen)
                {
                    UpdateElementOpacity(vPopupElementTarget, 0.02);
                    UpdateElementEnabled(vPopupElementTarget, false);
                }
                if (elementTarget != grid_Popup_ColorPicker && vColorPickerOpen)
                {
                    UpdateElementOpacity(grid_Popup_ColorPicker, 0.02);
                    UpdateElementEnabled(grid_Popup_ColorPicker, false);
                }
                if (elementTarget != grid_Popup_Search && vSearchOpen)
                {
                    UpdateElementOpacity(grid_Popup_Search, 0.02);
                    UpdateElementEnabled(grid_Popup_Search, false);
                }
                if (elementTarget != grid_Popup_MainMenu && vMainMenuOpen)
                {
                    UpdateElementOpacity(grid_Popup_MainMenu, 0.02);
                    UpdateElementEnabled(grid_Popup_MainMenu, false);
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
                UpdateElementVisibility(elementTarget, false);
                UpdateElementEnabled(elementTarget, false);

                //Show the background
                if (!Popup_Any_Open())
                {
                    double backgroundBrightness = (double)Convert.ToInt32(ConfigurationManager.AppSettings["BackgroundBrightness"]) / 100;
                    UpdateElementOpacity(grid_Video_Background, backgroundBrightness);
                    UpdateElementOpacity(grid_Main, 1);
                    UpdateElementEnabled(grid_Main, true);
                    return;
                }

                //Show other popups
                if (vTextInputOpen)
                {
                    UpdateElementOpacity(grid_Popup_TextInput, 1);
                    UpdateElementEnabled(grid_Popup_TextInput, true);
                }
                else if (vMessageBoxOpen)
                {
                    UpdateElementOpacity(grid_Popup_MessageBox, 1);
                    UpdateElementEnabled(grid_Popup_MessageBox, true);
                }
                else if (vFilePickerOpen)
                {
                    UpdateElementOpacity(grid_Popup_FilePicker, 1);
                    UpdateElementEnabled(grid_Popup_FilePicker, true);
                }
                else if (vPopupOpen)
                {
                    UpdateElementOpacity(vPopupElementTarget, 1);
                    UpdateElementEnabled(vPopupElementTarget, true);
                }
                else if (vColorPickerOpen)
                {
                    UpdateElementOpacity(grid_Popup_ColorPicker, 1);
                    UpdateElementEnabled(grid_Popup_ColorPicker, true);
                }
                else if (vSearchOpen)
                {
                    UpdateElementOpacity(grid_Popup_Search, 1);
                    UpdateElementEnabled(grid_Popup_Search, true);
                }
                else if (vMainMenuOpen)
                {
                    UpdateElementOpacity(grid_Popup_MainMenu, 1);
                    UpdateElementEnabled(grid_Popup_MainMenu, true);
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

                PlayInterfaceSound(vInterfaceSoundVolume, "PopupOpen", false);

                //Save the previous focus element
                Popup_PreviousElementFocus_Save(vMainMenuElementFocus, null);

                //Show the popup
                Popup_Show_Element(grid_Popup_MainMenu);

                vMainMenuOpen = true;

                //Focus on the menu listbox
                await ListboxFocus(listbox_MainMenu, false, false, -1);

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

                    //Hide the popup
                    Popup_Hide_Element(grid_Popup_MainMenu);

                    //Update the clock without date
                    UpdateClock();

                    //Focus on the previous focus element
                    await Popup_PreviousElementFocus_Focus(vMainMenuElementFocus);
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
                    PlayInterfaceSound(vInterfaceSoundVolume, "PopupClose", false);

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

        //Save the previous focus element
        static void Popup_PreviousElementFocus_Save(FrameworkElementFocus frameworkElementFocus, FrameworkElement previousFocus)
        {
            try
            {
                if (previousFocus != null)
                {
                    Debug.WriteLine("Set previous focus element: " + previousFocus);
                    frameworkElementFocus.FocusElement = previousFocus;
                    if (frameworkElementFocus.FocusElement.GetType() == typeof(ListBoxItem))
                    {
                        frameworkElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(frameworkElementFocus.FocusElement);
                        frameworkElementFocus.FocusIndex = frameworkElementFocus.FocusListBox.SelectedIndex;
                    }
                }
                else if (Keyboard.FocusedElement != null && Keyboard.FocusedElement != App.vWindowMain)
                {
                    Debug.WriteLine("Set previous focus keyboard: " + Keyboard.FocusedElement);
                    frameworkElementFocus.FocusElement = (FrameworkElement)Keyboard.FocusedElement;
                    if (frameworkElementFocus.FocusElement.GetType() == typeof(ListBoxItem))
                    {
                        frameworkElementFocus.FocusListBox = AVFunctions.FindVisualParent<ListBox>(frameworkElementFocus.FocusElement);
                        frameworkElementFocus.FocusIndex = frameworkElementFocus.FocusListBox.SelectedIndex;
                    }
                }
            }
            catch { }
        }

        //Focus on the previous focus element
        async Task Popup_PreviousElementFocus_Focus(FrameworkElementFocus frameworkElementFocus)
        {
            try
            {
                //Force focus on element
                if (frameworkElementFocus.FocusElement != null)
                {
                    Debug.WriteLine("Focusing on previous element: " + frameworkElementFocus.FocusElement);
                    await FocusOnElement(frameworkElementFocus.FocusElement, false, vProcessCurrent.MainWindowHandle);
                }
                else if (frameworkElementFocus.FocusListBox != null)
                {
                    Debug.WriteLine("Focusing on previous listbox: " + frameworkElementFocus.FocusListBox);
                    await ListboxFocus(frameworkElementFocus.FocusListBox, false, false, frameworkElementFocus.FocusIndex);
                }
                else
                {
                    Debug.WriteLine("No previous focus element, pressing tab key.");
                    KeySendSingle((byte)KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                }

                //Reset the previous focus
                frameworkElementFocus.Reset();
            }
            catch { }
        }
    }
}