using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.AVColors;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Handle Mode Switch
        async void ButtonModeTool_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await SetModeTool();
                }
            }
            catch { }
        }
        async void ButtonModeTool_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await SetModeTool();
            }
            catch { }
        }

        async void ButtonModeMedia_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await SetModeMedia();
                }
            }
            catch { }
        }
        async void ButtonModeMedia_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await SetModeMedia();
            }
            catch { }
        }

        async void ButtonModeKeyboard_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await SetModeKeyboard();
                }
            }
            catch { }
        }
        async void ButtonModeKeyboard_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await SetModeKeyboard();
            }
            catch { }
        }

        //Switch between keyboard modes
        async Task SwitchKeyboardMode()
        {
            try
            {
                KeyboardMode keyboardMode = (KeyboardMode)SettingLoad(vConfigurationDirectXInput, "KeyboardMode", typeof(int));
                if (keyboardMode == KeyboardMode.Keyboard)
                {
                    await SetModeMedia();
                }
                else if (keyboardMode == KeyboardMode.Media)
                {
                    await SetModeTool();
                }
                else if (keyboardMode == KeyboardMode.Tool)
                {
                    await SetModeKeyboard();
                }
            }
            catch { }
        }

        //Set current keyboard mode
        async Task SetKeyboardMode()
        {
            try
            {
                KeyboardMode keyboardMode = (KeyboardMode)SettingLoad(vConfigurationDirectXInput, "KeyboardMode", typeof(int));
                if (keyboardMode == KeyboardMode.Media)
                {
                    await SetModeMedia();
                }
                else if (keyboardMode == KeyboardMode.Keyboard)
                {
                    await SetModeKeyboard();
                }
                else if (keyboardMode == KeyboardMode.Tool)
                {
                    await SetModeTool();
                }
            }
            catch { }
        }

        public async Task SetModeKeyboard()
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Update help bar
                    stackpanel_DPad.Visibility = Visibility.Collapsed;
                    textblock_DPad.Text = string.Empty;
                    stackpanel_ButtonLeft.Visibility = Visibility.Visible;
                    textblock_ButtonLeft.Text = "Backspace";
                    stackpanel_ButtonUp.Visibility = Visibility.Visible;
                    textblock_ButtonUp.Text = "Space";
                    stackpanel_ButtonRight.Visibility = Visibility.Visible;
                    textblock_ButtonRight.Text = "Enter";
                    stackpanel_ButtonLbRb.Visibility = Visibility.Visible;
                    textblock_ButtonLbRb.Text = "Mouse click";
                    stackpanel_LeftTriggerOff.Visibility = Visibility.Visible;
                    textblock_LeftTriggerOff.Text = "Caps";
                    stackpanel_RightTriggerOff.Visibility = Visibility.Visible;
                    textblock_RightTriggerOff.Text = "Tab";
                    stackpanel_ThumbPress.Visibility = Visibility.Visible;
                    textblock_ThumbPress.Text = "Arrows";
                    stackpanel_ThumbLeftOff.Visibility = Visibility.Visible;
                    textblock_ThumbLeftOff.Text = "Mouse";
                    stackpanel_ThumbRightOff.Visibility = Visibility.Visible;
                    textblock_ThumbRightOff.Text = "Scroll";
                    stackpanel_BackOff.Visibility = Visibility.Visible;
                    textblock_BackOff.Text = "Emoji/Text";
                    stackpanel_StartOff.Visibility = Visibility.Visible;
                    textblock_StartOff.Text = "Media";
                    stackpanel_Guide.Visibility = Visibility.Visible;
                    textblock_Guide.Text = "Close";

                    //Show keyboard interface
                    grid_Keyboard.Visibility = Visibility.Visible;
                    grid_Media.Visibility = Visibility.Collapsed;
                    grid_Tool.Visibility = Visibility.Collapsed;

                    //Update border color
                    SolidColorBrush backgroundColor = new SolidColorBrush(Colors.Transparent);
                    border_ControllerHelp_Accent.Background = backgroundColor;
                    border_Header_Accent.Background = backgroundColor;
                    border_Media_Accent.Background = backgroundColor;

                    //Focus on keyboard button
                    if (vFocusedButtonKeyboard.FocusElement == null)
                    {
                        await FocusElement(key_h, vInteropWindowHandle);
                    }
                    else
                    {
                        await AVFocusDetailsFocus(vFocusedButtonKeyboard, vInteropWindowHandle);
                    }

                    //Play sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Keyboard";
                    notificationDetails.Text = "Switched to keyboard mode";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Update settings
                    SettingSave(vConfigurationDirectXInput, "KeyboardMode", Convert.ToInt32(KeyboardMode.Keyboard).ToString());
                });
            }
            catch { }
        }

        public async Task SetModeMedia()
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Update help bar
                    stackpanel_DPad.Visibility = Visibility.Visible;
                    textblock_DPad.Text = "Arrows";
                    stackpanel_ButtonLeft.Visibility = Visibility.Visible;
                    textblock_ButtonLeft.Text = "Media Prev";
                    stackpanel_ButtonUp.Visibility = Visibility.Visible;
                    textblock_ButtonUp.Text = "Play/Pause";
                    stackpanel_ButtonRight.Visibility = Visibility.Visible;
                    textblock_ButtonRight.Text = "Media Next";
                    stackpanel_ButtonLbRb.Visibility = Visibility.Visible;
                    textblock_ButtonLbRb.Text = "Mouse click";
                    stackpanel_LeftTriggerOff.Visibility = Visibility.Visible;
                    textblock_LeftTriggerOff.Text = string.Empty;
                    stackpanel_RightTriggerOff.Visibility = Visibility.Visible;
                    textblock_RightTriggerOff.Text = "Volume";
                    stackpanel_ThumbPress.Visibility = Visibility.Visible;
                    textblock_ThumbPress.Text = "Mute";
                    stackpanel_ThumbLeftOff.Visibility = Visibility.Visible;
                    textblock_ThumbLeftOff.Text = "Mouse";
                    stackpanel_ThumbRightOff.Visibility = Visibility.Visible;
                    textblock_ThumbRightOff.Text = "Move";
                    stackpanel_BackOff.Visibility = Visibility.Visible;
                    textblock_BackOff.Text = "Fullscreen";
                    stackpanel_StartOff.Visibility = Visibility.Visible;
                    textblock_StartOff.Text = "Tool";
                    stackpanel_Guide.Visibility = Visibility.Visible;
                    textblock_Guide.Text = "Close";

                    //Save keyboard focus element
                    KeyboardMode keyboardMode = (KeyboardMode)SettingLoad(vConfigurationDirectXInput, "KeyboardMode", typeof(int));
                    if (keyboardMode == KeyboardMode.Keyboard)
                    {
                        AVFocusDetailsSave(vFocusedButtonKeyboard, null);
                    }

                    //Show media interface
                    grid_Keyboard.Visibility = Visibility.Collapsed;
                    grid_Media.Visibility = Visibility.Visible;
                    grid_Tool.Visibility = Visibility.Collapsed;

                    //Update border color
                    SolidColorBrush backgroundBrush = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];
                    SolidColorBrush backgroundBrushOpacity = AdjustColorOpacity(backgroundBrush, 0.20);
                    border_ControllerHelp_Accent.Background = backgroundBrushOpacity;
                    border_Header_Accent.Background = backgroundBrushOpacity;
                    border_Media_Accent.Background = backgroundBrushOpacity;

                    //Focus on keyboard button
                    await FocusElement(key_ModeKeyboard, vInteropWindowHandle);

                    //Play sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Keyboard";
                    notificationDetails.Text = "Switched to media mode";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Update settings
                    SettingSave(vConfigurationDirectXInput, "KeyboardMode", Convert.ToInt32(KeyboardMode.Media).ToString());
                });
            }
            catch { }
        }

        public async Task SetModeTool()
        {
            try
            {
                await AVActions.DispatcherInvoke(async delegate
                {
                    //Update help bar
                    stackpanel_DPad.Visibility = Visibility.Collapsed;
                    textblock_DPad.Text = string.Empty;
                    stackpanel_ButtonLeft.Visibility = Visibility.Collapsed;
                    textblock_ButtonLeft.Text = string.Empty;
                    stackpanel_ButtonUp.Visibility = Visibility.Collapsed;
                    textblock_ButtonUp.Text = string.Empty;
                    stackpanel_ButtonRight.Visibility = Visibility.Collapsed;
                    textblock_ButtonRight.Text = string.Empty;
                    stackpanel_ButtonLbRb.Visibility = Visibility.Visible;
                    textblock_ButtonLbRb.Text = "Mouse click";
                    stackpanel_LeftTriggerOff.Visibility = Visibility.Collapsed;
                    textblock_LeftTriggerOff.Text = string.Empty;
                    stackpanel_RightTriggerOff.Visibility = Visibility.Collapsed;
                    textblock_RightTriggerOff.Text = string.Empty;
                    stackpanel_ThumbPress.Visibility = Visibility.Collapsed;
                    textblock_ThumbPress.Text = string.Empty;
                    stackpanel_ThumbLeftOff.Visibility = Visibility.Visible;
                    textblock_ThumbLeftOff.Text = "Mouse";
                    stackpanel_ThumbRightOff.Visibility = Visibility.Visible;
                    textblock_ThumbRightOff.Text = "Move";
                    stackpanel_BackOff.Visibility = Visibility.Collapsed;
                    textblock_BackOff.Text = string.Empty;
                    stackpanel_StartOff.Visibility = Visibility.Visible;
                    textblock_StartOff.Text = "Keyboard";
                    stackpanel_Guide.Visibility = Visibility.Visible;
                    textblock_Guide.Text = "Close";

                    //Save keyboard focus element
                    KeyboardMode keyboardMode = (KeyboardMode)SettingLoad(vConfigurationDirectXInput, "KeyboardMode", typeof(int));
                    if (keyboardMode == KeyboardMode.Keyboard)
                    {
                        AVFocusDetailsSave(vFocusedButtonKeyboard, null);
                    }

                    //Show tool interface
                    grid_Keyboard.Visibility = Visibility.Collapsed;
                    grid_Media.Visibility = Visibility.Collapsed;
                    grid_Tool.Visibility = Visibility.Visible;

                    //Update border color
                    SolidColorBrush backgroundColor = new SolidColorBrush(Colors.Transparent);
                    border_ControllerHelp_Accent.Background = backgroundColor;
                    border_Header_Accent.Background = backgroundColor;
                    border_Media_Accent.Background = backgroundColor;

                    //Focus on tool list
                    await ListBoxFocusOrSelectIndex(listbox_ToolList, false, listbox_ToolList.SelectedIndex, vInteropWindowHandle);
                    await ListBoxFocusIndex(listbox_ToolList, false, listbox_ToolList.SelectedIndex, vInteropWindowHandle);

                    //Play sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Keyboard";
                    notificationDetails.Text = "Switched to tool mode";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Update settings
                    SettingSave(vConfigurationDirectXInput, "KeyboardMode", Convert.ToInt32(KeyboardMode.Tool).ToString());
                });
            }
            catch { }
        }
    }
}