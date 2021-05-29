using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static DirectXInput.SettingsNotify;
using static LibraryShared.Classes;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

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

                //Update the window style
                UpdateWindowStyle();

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
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Update the window visibility
                    UpdateWindowVisibility(false);

                    //Notify - Fps Overlayer keypad size changed
                    await NotifyFpsOverlayerKeypadSizeChanged(0);

                    //Update last active status
                    vKeyboardKeypadLastActive = "Keypad";

                    //Keypad release keyboard buttons
                    ControllerInteractionKeypadRelease();

                    //Delay CtrlUI output
                    vController0.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController1.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController2.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
                    vController3.Delay_CtrlUIOutput = GetSystemTicksMs() + vControllerDelayMediumTicks;
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
                App.vWindowKeyboard.Hide();
                await App.vWindowMedia.Hide();

                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Disable hardware capslock
                await DisableHardwareCapsLock();

                //Set the keypad mapping profile
                SetKeypadMappingProfile();

                //Update the key names
                UpdateKeypadNames();

                //Update the keypad opacity
                UpdatePopupOpacity();

                //Update the keypad style
                UpdateKeypadStyle();

                //Update the keypad size
                double keypadHeight = UpdateKeypadSize();

                //Notify - Fps Overlayer keypad size changed
                await NotifyFpsOverlayerKeypadSizeChanged(Convert.ToInt32(keypadHeight));

                //Update the window visibility
                UpdateWindowVisibility(true);
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
                            image_DPad.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadDpad.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ThumbLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumb.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ThumbRight.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumb.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_TriggersLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadTriggers.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_TriggersRight.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadTriggers.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ButtonBack.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadButton.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ButtonStart.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadButton.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_Action.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadAction.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        }
                        else
                        {
                            image_DPad.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadDpadDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ThumbLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumbDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0); image_ThumbLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumbDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ThumbRight.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumbDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0); image_ThumbLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadThumbDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_TriggersLeft.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadTriggersDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_TriggersRight.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadTriggersDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ButtonBack.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadButtonDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_ButtonStart.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadButtonDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            image_Action.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Buttons/KeypadActionDark.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
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
            double KeypadImageHeight = 0;
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    try
                    {
                        int targetPercentage = vKeypadMappingProfile.KeypadDisplaySize;
                        Debug.WriteLine("Changing keypad size to: " + targetPercentage);

                        double keypadTextSize = ((double)30 / 100) * targetPercentage;
                        KeypadImageHeight = ((double)240 / 100) * targetPercentage;
                        double KeypadPaddingTwo = KeypadImageHeight / 2;
                        double KeypadPaddingThree = KeypadImageHeight / 3;

                        Application.Current.Resources["KeypadTextSize"] = keypadTextSize;
                        Application.Current.Resources["KeypadImageHeight"] = KeypadImageHeight;
                        Application.Current.Resources["KeypadPaddingTwo"] = KeypadPaddingTwo;
                        Application.Current.Resources["KeypadPaddingThree"] = KeypadPaddingThree;
                    }
                    catch { }
                });
            }
            catch { }
            return KeypadImageHeight;
        }

        //Set the keypad mapping profile
        void SetKeypadMappingProfile()
        {
            try
            {
                string processNameLower = vProcessForeground.Name.ToLower();
                string processTitleLower = vProcessForeground.Title.ToLower();
                KeypadMapping directKeypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name.ToLower() == processNameLower || processTitleLower.Contains(x.Name.ToLower())).FirstOrDefault();
                if (directKeypadMappingProfile == null)
                {
                    directKeypadMappingProfile = vDirectKeypadMapping.Where(x => x.Name == "Default").FirstOrDefault();
                }

                //Show keypad mapping profile notification
                if (vKeypadMappingProfile != directKeypadMappingProfile)
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Keypad";
                    notificationDetails.Text = "Profile set to " + directKeypadMappingProfile.Name;
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }

                //Update the keypad mapping profile
                vKeypadMappingProfile = directKeypadMappingProfile;
            }
            catch { }
        }

        //Update the window position on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(500);

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
                    //Create and show the window
                    base.Show();

                    //Update the window style (focus workaround)
                    UpdateWindowStyle();

                    this.Title = "DirectXInput Keypad (Visible)";
                    vWindowVisible = true;
                    Debug.WriteLine("Showing the window.");
                }
                else
                {
                    //Update the window style
                    IntPtr updatedStyle = IntPtr.Zero;
                    SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                    this.Title = "DirectXInput Keypad (Hidden)";
                    vWindowVisible = false;
                    Debug.WriteLine("Hiding the window.");
                }
            }
            catch { }
        }

        //Update the window style (focus workaround)
        void UpdateWindowStyle()
        {
            try
            {
                //Set the window style
                IntPtr updatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, updatedStyle);

                //Set the window style ex
                IntPtr updatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, updatedExStyle);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));
            }
            catch { }
        }

        //Update the window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "DisplayMonitor"));
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Move and resize the window
                WindowMove(vInteropWindowHandle, displayMonitorSettings.BoundsLeft, displayMonitorSettings.BoundsTop);
                WindowResize(vInteropWindowHandle, displayMonitorSettings.WidthNative, displayMonitorSettings.HeightNative);
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
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.DPadLeftMod, vKeypadMappingProfile.DPadLeft, textblock_DPadLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.DPadUpMod, vKeypadMappingProfile.DPadUp, textblock_DPadUp);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.DPadRightMod, vKeypadMappingProfile.DPadRight, textblock_DPadRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.DPadDownMod, vKeypadMappingProfile.DPadDown, textblock_DPadDown);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftLeftMod, vKeypadMappingProfile.ThumbLeftLeft, textblock_ThumbLeftLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftUpMod, vKeypadMappingProfile.ThumbLeftUp, textblock_ThumbLeftUp);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftRightMod, vKeypadMappingProfile.ThumbLeftRight, textblock_ThumbLeftRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftDownMod, vKeypadMappingProfile.ThumbLeftDown, textblock_ThumbLeftDown);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightLeftMod, vKeypadMappingProfile.ThumbRightLeft, textblock_ThumbRightLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightUpMod, vKeypadMappingProfile.ThumbRightUp, textblock_ThumbRightUp);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightRightMod, vKeypadMappingProfile.ThumbRightRight, textblock_ThumbRightRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightDownMod, vKeypadMappingProfile.ThumbRightDown, textblock_ThumbRightDown);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonBackMod, vKeypadMappingProfile.ButtonBack, textblock_ButtonBack);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonStartMod, vKeypadMappingProfile.ButtonStart, textblock_ButtonStart);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonXMod, vKeypadMappingProfile.ButtonX, textblock_ButtonX);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonYMod, vKeypadMappingProfile.ButtonY, textblock_ButtonY);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonAMod, vKeypadMappingProfile.ButtonA, textblock_ButtonA);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonBMod, vKeypadMappingProfile.ButtonB, textblock_ButtonB);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonShoulderLeftMod, vKeypadMappingProfile.ButtonShoulderLeft, textblock_ButtonShoulderLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonTriggerLeftMod, vKeypadMappingProfile.ButtonTriggerLeft, textblock_ButtonTriggerLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonThumbLeftMod, vKeypadMappingProfile.ButtonThumbLeft, textblock_ThumbLeftButton);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonShoulderRightMod, vKeypadMappingProfile.ButtonShoulderRight, textblock_ButtonShoulderRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonTriggerRightMod, vKeypadMappingProfile.ButtonTriggerRight, textblock_ButtonTriggerRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonThumbRightMod, vKeypadMappingProfile.ButtonThumbRight, textblock_ThumbRightButton);
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Update key details
        void UpdateKeypadKeyDetails(KeysVirtual? modifierKey, KeysVirtual? virtualKey, TextBlock keyTextLabel)
        {
            try
            {
                if (modifierKey != null)
                {
                    keyTextLabel.Text = GetVirtualKeyName((KeysVirtual)modifierKey, true) + "\n" + GetVirtualKeyName((KeysVirtual)virtualKey, true);
                    keyTextLabel.Opacity = 1;
                }
                else if (virtualKey != null)
                {
                    keyTextLabel.Text = GetVirtualKeyName((KeysVirtual)virtualKey, true);
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

        //Disable hardware capslock
        public async Task DisableHardwareCapsLock()
        {
            try
            {
                await AVActions.ActionDispatcherInvokeAsync(async delegate
                {
                    if (System.Windows.Input.Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                    {
                        await KeyPressSingleAuto(KeysVirtual.CapsLock);
                    }
                });
            }
            catch { }
        }
    }
}