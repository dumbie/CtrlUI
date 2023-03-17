using System;
using System.Linq;
using System.Threading.Tasks;
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
        //Add tool functions to the list
        void ToolList_AddItems()
        {
            try
            {
                if (!vDirectKeyboardToolList.Any())
                {
                    ProfileShared shortcutCtrlUI = new ProfileShared();
                    shortcutCtrlUI.String1 = "Launch or show CtrlUI";
                    shortcutCtrlUI.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutCtrlUI);

                    ProfileShared shortcutFpsOverlayer = new ProfileShared();
                    shortcutFpsOverlayer.String1 = "Launch or show Fps Overlayer";
                    shortcutFpsOverlayer.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Fps.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutFpsOverlayer);

                    ProfileShared shortcutFpsBrowser = new ProfileShared();
                    shortcutFpsBrowser.String1 = "Show or hide browser overlay";
                    shortcutFpsBrowser.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Browser.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutFpsBrowser);

                    ProfileShared shortcutFpsCrosshair = new ProfileShared();
                    shortcutFpsCrosshair.String1 = "Show or hide crosshair overlay";
                    shortcutFpsCrosshair.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Crosshair.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutFpsCrosshair);

                    ProfileShared shortcutFpsMove = new ProfileShared();
                    shortcutFpsMove.String1 = "Move fps overlay position";
                    shortcutFpsMove.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Move.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutFpsMove);

                    ProfileShared shortcutSwitchKeyboard = new ProfileShared();
                    shortcutSwitchKeyboard.String1 = "Switch to keyboard";
                    shortcutSwitchKeyboard.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Keyboard.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutSwitchKeyboard);

                    ProfileShared shortcutSwitchKeypad = new ProfileShared();
                    shortcutSwitchKeypad.String1 = "Switch to keypad";
                    shortcutSwitchKeypad.Object1 = FileToBitmapImage(new string[] { "Assets/Default/Icons/Keypad.png" }, null, vImageBackupSource, IntPtr.Zero, 100, 0);
                    vDirectKeyboardToolList.Add(shortcutSwitchKeypad);

                    ProfileShared shortcutCloseKeyboard = new ProfileShared();
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
                    string selectedAction = selectedItem.String1;

                    if (selectedAction == "Launch or show CtrlUI")
                    {
                        await ProcessFunctions.LaunchShowCtrlUI();
                    }
                    else if (selectedAction == "Launch or show fps overlay")
                    {
                        await ProcessFunctions.LaunchFpsOverlayer(false);
                    }
                    else if (selectedAction == "Close keyboard overlay")
                    {
                        await this.Hide();
                    }
                }
            }
            catch { }
        }
    }
}