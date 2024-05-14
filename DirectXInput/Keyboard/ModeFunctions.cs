using ArnoldVinkCode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVFocus;
using static ArnoldVinkCode.Styles.AVColors;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Switch between keyboard modes
        async Task SwitchKeyboardMode()
        {
            try
            {
                if (vKeyboardCurrentMode == KeyboardMode.Keyboard)
                {
                    await SetModeMedia();
                }
                else if (vKeyboardCurrentMode == KeyboardMode.Media)
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
                if (vKeyboardCurrentMode == KeyboardMode.Media)
                {
                    await SetModeMedia();
                }
                else if (vKeyboardCurrentMode == KeyboardMode.Keyboard)
                {
                    await SetModeKeyboard();
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

                    //Update tool bar
                    image_Tool_SwitchMode.Source = vImagePreloadIconMusic;

                    //Show keyboard interface
                    grid_Keyboard.Visibility = Visibility.Visible;
                    grid_Media.Visibility = Visibility.Collapsed;

                    //Update border color
                    SolidColorBrush backgroundColor = new SolidColorBrush(Colors.Transparent);
                    border_ControllerHelp_Accent.Background = backgroundColor;
                    border_Header_Accent.Background = backgroundColor;
                    border_Tool_Accent.Background = backgroundColor;
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
                    vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Update variables
                    vKeyboardCurrentMode = KeyboardMode.Keyboard;
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

                    //Update tool bar
                    image_Tool_SwitchMode.Source = vImagePreloadIconKeyboard;

                    //Save keyboard focus element
                    if (vKeyboardCurrentMode == KeyboardMode.Keyboard)
                    {
                        AVFocusDetailsSave(vFocusedButtonKeyboard, null);
                    }

                    //Show media interface
                    grid_Keyboard.Visibility = Visibility.Collapsed;
                    grid_Media.Visibility = Visibility.Visible;

                    //Update border color
                    SolidColorBrush backgroundBrush = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];
                    SolidColorBrush backgroundBrushOpacity = AdjustColorOpacity(backgroundBrush, 0.20);
                    border_ControllerHelp_Accent.Background = backgroundBrushOpacity;
                    border_Header_Accent.Background = backgroundBrushOpacity;
                    border_Tool_Accent.Background = backgroundBrushOpacity;
                    border_Media_Accent.Background = backgroundBrushOpacity;

                    //Focus on keyboard button
                    await FocusElement(key_Tool_SwitchMode, vInteropWindowHandle);

                    //Play sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                    //Show notification
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Keyboard";
                    notificationDetails.Text = "Switched to media mode";
                    vWindowOverlay.Notification_Show_Status(notificationDetails);

                    //Update variables
                    vKeyboardCurrentMode = KeyboardMode.Media;
                });
            }
            catch { }
        }
    }
}