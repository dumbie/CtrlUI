using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.SettingsNotify;
using static DirectXInput.WindowMain;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;
using static LibraryUsb.FakerInputDevice;

namespace DirectXInput.KeypadCode
{
    public partial class WindowKeypad : Window
    {
        //Window Initialize
        public WindowKeypad() { InitializeComponent(); }

        //Window Variables
        private IntPtr vInteropWindowHandle = IntPtr.Zero;
        public bool vWindowVisible = false;

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Get interop window handle
                vInteropWindowHandle = new WindowInteropHelper(this).EnsureHandle();

                //Set render mode to software
                HwndSource hwndSource = HwndSource.FromHwnd(vInteropWindowHandle);
                HwndTarget hwndTarget = hwndSource.CompositionTarget;
                hwndTarget.RenderMode = RenderMode.SoftwareOnly;

                //Update the window style
                WindowUpdateStyleVisible(vInteropWindowHandle, true, true, true);

                //Update the window position
                UpdateWindowPosition();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Hide the window
        public new async Task Hide()
        {
            try
            {
                if (vWindowVisible)
                {
                    //Play window close sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false, false);

                    //Stop the update tasks
                    await TasksBackgroundStop();

                    //Update the window visibility
                    UpdateWindowVisibility(false);

                    //Notify - Fps Overlayer keypad size changed
                    await NotifyFpsOverlayerKeypadSizeChanged(0);

                    //Update last active status
                    vKeyboardKeypadLastActive = "Keypad";

                    //Release keyboard and mouse
                    vFakerInputDevice.KeyboardReset();
                    vFakerInputDevice.MouseResetAbsolute();
                    vFakerInputDevice.MouseResetRelative();

                    //Delay CtrlUI output
                    vController0.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks250;
                    vController1.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks250;
                    vController2.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks250;
                    vController3.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayTicks250;
                }
            }
            catch { }
        }

        //Show the window
        public new async Task Show()
        {
            try
            {
                //Close other popups
                await App.vWindowKeyboard.Hide();

                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false, false);

                //Start the update tasks
                TasksBackgroundStart();

                //Disable hardware capslock
                DisableHardwareCapsLock();

                //Enable hardware numlock
                EnableHardwareNumLock();

                //Update the window visibility
                UpdateWindowVisibility(true);

                //Notify - Fps Overlayer keypad size changed
                await NotifyFpsOverlayerKeypadSizeChanged(Convert.ToInt32(vKeypadImageHeight));
            }
            catch { }
        }

        //Update the keypad style
        public void UpdateKeypadStyle()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        Debug.WriteLine("Setting keypad style to: " + vKeypadMappingProfile.KeypadDisplayStyle);

                        List<TextBlock> allTextBlocks = AVFunctions.FindVisualChildren<TextBlock>(grid_Application);
                        foreach (TextBlock textBlock in allTextBlocks)
                        {
                            if (vKeypadMappingProfile.KeypadDisplayStyle == 0)
                            {
                                textBlock.Style = (Style)Application.Current.Resources["KeypadTextLight"];
                            }
                            else
                            {
                                textBlock.Style = (Style)Application.Current.Resources["KeypadTextDark"];
                            }
                        }

                        if (vKeypadMappingProfile.KeypadDisplayStyle == 0)
                        {
                            image_DPad.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadDpad.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ThumbLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumb.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ThumbRight.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumb.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_TriggersLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadTriggers.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_TriggersRight.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadTriggers.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ButtonBack.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadButton.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ButtonStart.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadButton.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_Action.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadAction.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            vKeypadNormalBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF");
                        }
                        else
                        {
                            image_DPad.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadDpadDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ThumbLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumbDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0); image_ThumbLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumbDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ThumbRight.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumbDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0); image_ThumbLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumbDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_TriggersLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadTriggersDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_TriggersRight.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadTriggersDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ButtonBack.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadButtonDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ButtonStart.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadButtonDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_Action.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadActionDark.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                            vKeypadNormalBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#000000");
                        }
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Update the keypad size
        public double UpdateKeypadSize()
        {
            double keypadImageHeight = 0;
            try
            {
                ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        int targetPercentage = vKeypadMappingProfile.KeypadDisplaySize;
                        Debug.WriteLine("Changing keypad size to: " + targetPercentage);

                        double keypadTextSize = ((double)30 / 100) * targetPercentage;
                        keypadImageHeight = ((double)240 / 100) * targetPercentage;
                        double KeypadPaddingTwo = keypadImageHeight / 2;
                        double KeypadPaddingThree = keypadImageHeight / 3;

                        Application.Current.Resources["KeypadTextSize"] = keypadTextSize;
                        Application.Current.Resources["KeypadImageHeight"] = keypadImageHeight;
                        Application.Current.Resources["KeypadPaddingTwo"] = KeypadPaddingTwo;
                        Application.Current.Resources["KeypadPaddingThree"] = KeypadPaddingThree;
                    }
                    catch { }
                });
            }
            catch { }
            vKeypadImageHeight = keypadImageHeight;
            return keypadImageHeight;
        }

        //Set the keypad mapping profile
        void SetKeypadMappingProfile()
        {
            try
            {
                string processNameLower = vProcessForeground.ExeNameNoExt.ToLower();
                string processTitleLower = vProcessForeground.WindowTitle.ToLower().Replace(" ", string.Empty);
                KeypadMapping keypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name.ToLower() == processNameLower || processTitleLower.Contains(x.Name.ToLower().Replace(" ", string.Empty))).FirstOrDefault();
                if (keypadMappingProfile == null)
                {
                    keypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name == "Default").FirstOrDefault();
                }

                //Show keypad mapping profile notification
                if (vKeypadMappingProfile != keypadMappingProfile)
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Keypad";
                    notificationDetails.Text = "Profile set to " + keypadMappingProfile.Name;
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }

                //Update the keypad mapping profile
                vKeypadMappingProfile = keypadMappingProfile;
            }
            catch { }
        }

        //Update the window position on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(1000);

                //Update the window position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Update the window visibility
        void UpdateWindowVisibility(bool visible)
        {
            try
            {
                if (visible)
                {
                    if (!vWindowVisible)
                    {
                        //Create and show the window
                        base.Show();

                        //Update the window style
                        WindowUpdateStyleVisible(vInteropWindowHandle, true, true, true);

                        this.Title = "DirectXInput Keypad (Visible)";
                        vWindowVisible = true;
                        Debug.WriteLine("Showing the window.");
                    }
                }
                else
                {
                    if (vWindowVisible)
                    {
                        //Update the window style
                        WindowUpdateStyleHidden(vInteropWindowHandle);

                        this.Title = "DirectXInput Keypad (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                }
            }
            catch { }
        }

        //Update the window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = SettingLoad(vConfigurationCtrlUI, "DisplayMonitor", typeof(int));

                //Move the window position
                WindowUpdatePosition(monitorNumber, vInteropWindowHandle, AVWindowPosition.FullScreen);
            }
            catch { }
        }

        //Update the popup opacity
        public void UpdatePopupOpacity()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        this.Opacity = vKeypadMappingProfile.KeypadOpacity;
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Update all keypad key names
        public void UpdateKeypadNames()
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.DPadLeftMod0, vKeypadMappingProfile.DPadLeftMod1, vKeypadMappingProfile.DPadLeft, textblock_DPadLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.DPadUpMod0, vKeypadMappingProfile.DPadUpMod1, vKeypadMappingProfile.DPadUp, textblock_DPadUp);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.DPadRightMod0, vKeypadMappingProfile.DPadRightMod1, vKeypadMappingProfile.DPadRight, textblock_DPadRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.DPadDownMod0, vKeypadMappingProfile.DPadDownMod1, vKeypadMappingProfile.DPadDown, textblock_DPadDown);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftLeftMod0, vKeypadMappingProfile.ThumbLeftLeftMod1, vKeypadMappingProfile.ThumbLeftLeft, textblock_ThumbLeftLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftUpMod0, vKeypadMappingProfile.ThumbLeftUpMod1, vKeypadMappingProfile.ThumbLeftUp, textblock_ThumbLeftUp);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftRightMod0, vKeypadMappingProfile.ThumbLeftRightMod1, vKeypadMappingProfile.ThumbLeftRight, textblock_ThumbLeftRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftDownMod0, vKeypadMappingProfile.ThumbLeftDownMod1, vKeypadMappingProfile.ThumbLeftDown, textblock_ThumbLeftDown);

                        if (vKeypadMappingProfile.KeypadMouseMoveEnabled)
                        {
                            textblock_ThumbRightLeft.Text = "🖱";
                            textblock_ThumbRightLeft.Opacity = 1;
                            textblock_ThumbRightUp.Text = "🖱";
                            textblock_ThumbRightUp.Opacity = 1;
                            textblock_ThumbRightRight.Text = "🖱";
                            textblock_ThumbRightRight.Opacity = 1;
                            textblock_ThumbRightDown.Text = "🖱";
                            textblock_ThumbRightDown.Opacity = 1;
                        }
                        else
                        {
                            UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightLeftMod0, vKeypadMappingProfile.ThumbRightLeftMod1, vKeypadMappingProfile.ThumbRightLeft, textblock_ThumbRightLeft);
                            UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightUpMod0, vKeypadMappingProfile.ThumbRightUpMod1, vKeypadMappingProfile.ThumbRightUp, textblock_ThumbRightUp);
                            UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightRightMod0, vKeypadMappingProfile.ThumbRightRightMod1, vKeypadMappingProfile.ThumbRightRight, textblock_ThumbRightRight);
                            UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightDownMod0, vKeypadMappingProfile.ThumbRightDownMod1, vKeypadMappingProfile.ThumbRightDown, textblock_ThumbRightDown);
                        }

                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonBackMod0, vKeypadMappingProfile.ButtonBackMod1, vKeypadMappingProfile.ButtonBack, textblock_ButtonBack);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonStartMod0, vKeypadMappingProfile.ButtonStartMod1, vKeypadMappingProfile.ButtonStart, textblock_ButtonStart);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonXMod0, vKeypadMappingProfile.ButtonXMod1, vKeypadMappingProfile.ButtonX, textblock_ButtonX);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonYMod0, vKeypadMappingProfile.ButtonYMod1, vKeypadMappingProfile.ButtonY, textblock_ButtonY);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonAMod0, vKeypadMappingProfile.ButtonAMod1, vKeypadMappingProfile.ButtonA, textblock_ButtonA);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonBMod0, vKeypadMappingProfile.ButtonBMod1, vKeypadMappingProfile.ButtonB, textblock_ButtonB);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonShoulderLeftMod0, vKeypadMappingProfile.ButtonShoulderLeftMod1, vKeypadMappingProfile.ButtonShoulderLeft, textblock_ButtonShoulderLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonTriggerLeftMod0, vKeypadMappingProfile.ButtonTriggerLeftMod1, vKeypadMappingProfile.ButtonTriggerLeft, textblock_ButtonTriggerLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonThumbLeftMod0, vKeypadMappingProfile.ButtonThumbLeftMod1, vKeypadMappingProfile.ButtonThumbLeft, textblock_ThumbLeftButton);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonShoulderRightMod0, vKeypadMappingProfile.ButtonShoulderRightMod1, vKeypadMappingProfile.ButtonShoulderRight, textblock_ButtonShoulderRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonTriggerRightMod0, vKeypadMappingProfile.ButtonTriggerRightMod1, vKeypadMappingProfile.ButtonTriggerRight, textblock_ButtonTriggerRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonThumbRightMod0, vKeypadMappingProfile.ButtonThumbRightMod1, vKeypadMappingProfile.ButtonThumbRight, textblock_ThumbRightButton);
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Update key details
        void UpdateKeypadKeyDetails(KeyboardModifiers modifierKey0, KeyboardModifiers modifierKey1, KeyboardKeys virtualKey, TextBlock keyTextLabel)
        {
            try
            {
                if (modifierKey0 != KeyboardModifiers.None && modifierKey1 != KeyboardModifiers.None && virtualKey != KeyboardKeys.None)
                {
                    keyTextLabel.Text = vFakerInputDevice.GetKeyboardModifiersName(modifierKey0, true) + "\n" + vFakerInputDevice.GetKeyboardModifiersName(modifierKey1, true) + "\n" + vFakerInputDevice.GetKeyboardKeysName(virtualKey, true);
                    keyTextLabel.Opacity = 1;
                }
                else if (modifierKey0 != KeyboardModifiers.None && modifierKey1 != KeyboardModifiers.None)
                {
                    keyTextLabel.Text = vFakerInputDevice.GetKeyboardModifiersName(modifierKey0, true) + "\n" + vFakerInputDevice.GetKeyboardModifiersName(modifierKey1, true);
                    keyTextLabel.Opacity = 1;
                }
                else if (modifierKey0 != KeyboardModifiers.None && virtualKey != KeyboardKeys.None)
                {
                    keyTextLabel.Text = vFakerInputDevice.GetKeyboardModifiersName(modifierKey0, true) + "\n" + vFakerInputDevice.GetKeyboardKeysName(virtualKey, true);
                    keyTextLabel.Opacity = 1;
                }
                else if (modifierKey1 != KeyboardModifiers.None && virtualKey != KeyboardKeys.None)
                {
                    keyTextLabel.Text = vFakerInputDevice.GetKeyboardModifiersName(modifierKey1, true) + "\n" + vFakerInputDevice.GetKeyboardKeysName(virtualKey, true);
                    keyTextLabel.Opacity = 1;
                }
                else if (modifierKey0 != KeyboardModifiers.None)
                {
                    keyTextLabel.Text = vFakerInputDevice.GetKeyboardModifiersName(modifierKey0, true);
                    keyTextLabel.Opacity = 1;
                }
                else if (modifierKey1 != KeyboardModifiers.None)
                {
                    keyTextLabel.Text = vFakerInputDevice.GetKeyboardModifiersName(modifierKey1, true);
                    keyTextLabel.Opacity = 1;
                }
                else if (virtualKey != KeyboardKeys.None)
                {
                    keyTextLabel.Text = vFakerInputDevice.GetKeyboardKeysName(virtualKey, true);
                    keyTextLabel.Opacity = 1;
                }
                else
                {
                    keyTextLabel.Text = "?";
                    keyTextLabel.Opacity = 0;
                }
            }
            catch { }
        }
    }
}