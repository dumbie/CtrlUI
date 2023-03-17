using ArnoldVinkCode;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVImage;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Profile variables
        private ProfileShared shortcutCtrlUILaunch = new ProfileShared();
        private ProfileShared shortcutFpsOverlayerLaunch = new ProfileShared();
        private ProfileShared shortcutFpsOverlayerPosition = new ProfileShared();
        private ProfileShared shortcutFpsBrowser = new ProfileShared();
        private ProfileShared shortcutFpsCrosshair = new ProfileShared();
        private ProfileShared shortcutSwitchKeyboard = new ProfileShared();
        private ProfileShared shortcutSwitchKeypad = new ProfileShared();
        private ProfileShared shortcutCloseKeyboard = new ProfileShared();

        //Add tool functions to the list
        void ToolList_AddItems()
        {
            try
            {
                if (!vDirectKeyboardToolList.Any())
                {
                    shortcutCtrlUILaunch.String1 = "Launch or show CtrlUI";
                    shortcutCtrlUILaunch.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutCtrlUILaunch);

                    shortcutFpsOverlayerLaunch.String1 = "Launch, show or hide Fps Overlayer";
                    shortcutFpsOverlayerLaunch.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Fps.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutFpsOverlayerLaunch);

                    shortcutFpsOverlayerPosition.String1 = "Change Fps Overlayer stats position";
                    shortcutFpsOverlayerPosition.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Move.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutFpsOverlayerPosition);

                    shortcutFpsBrowser.String1 = "Show or hide browser overlay";
                    shortcutFpsBrowser.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Browser.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutFpsBrowser);

                    shortcutFpsCrosshair.String1 = "Show or hide crosshair overlay";
                    shortcutFpsCrosshair.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Crosshair.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutFpsCrosshair);

                    shortcutSwitchKeyboard.String1 = "Switch to keyboard";
                    shortcutSwitchKeyboard.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Keyboard.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutSwitchKeyboard);

                    shortcutSwitchKeypad.String1 = "Switch to keypad";
                    shortcutSwitchKeypad.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Keypad.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutSwitchKeypad);

                    shortcutCloseKeyboard.String1 = "Close keyboard overlay";
                    shortcutCloseKeyboard.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Close.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutCloseKeyboard);
                }
            }
            catch { }
        }

        //Update the current tool text
        void listbox_ToolList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBox ListboxSender = (ListBox)sender;
                if (ListboxSender.SelectedItems.Count > 0 && ListboxSender.SelectedIndex != -1)
                {
                    ProfileShared selectedItem = (ProfileShared)ListboxSender.SelectedItem;
                    textblock_ToolSelect.Text = selectedItem.String1;
                }
            }
            catch { }
        }

        //Handle key and mouse presses
        async void listbox_ToolList_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await Tool_ExecuteAction(sender);
                }
            }
            catch { }
        }

        async void listbox_ToolList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                await Tool_ExecuteAction(sender);
            }
            catch { }
        }

        //Execute tool action
        async Task Tool_ExecuteAction(object sender)
        {
            try
            {
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);
                ListBox ListboxSender = (ListBox)sender;
                if (ListboxSender.SelectedItems.Count > 0 && ListboxSender.SelectedIndex != -1)
                {
                    ProfileShared selectedItem = (ProfileShared)ListboxSender.SelectedItem;
                    if (selectedItem == shortcutCtrlUILaunch)
                    {
                        await ToolFunctions.CtrlUI_LaunchShow();
                    }
                    else if (selectedItem == shortcutFpsOverlayerLaunch)
                    {
                        await ToolFunctions.FpsOverlayer_LaunchShowHide();
                    }
                    else if (selectedItem == shortcutFpsOverlayerPosition)
                    {
                        await ToolFunctions.FpsOverlayer_ChangePosition();
                    }
                    else if (selectedItem == shortcutFpsBrowser)
                    {
                        await ToolFunctions.FpsOverlayer_ShowHideBrowser();
                    }
                    else if (selectedItem == shortcutFpsCrosshair)
                    {
                        await ToolFunctions.FpsOverlayer_ShowHideCrosshair();
                    }
                    else if (selectedItem == shortcutSwitchKeyboard)
                    {
                        await SetModeKeyboard();
                    }
                    else if (selectedItem == shortcutSwitchKeypad)
                    {
                        await App.vWindowMain.KeypadPopupHideShow(true);
                    }
                    else if (selectedItem == shortcutCloseKeyboard)
                    {
                        await this.Hide();
                    }
                }
            }
            catch { }
        }
    }
}