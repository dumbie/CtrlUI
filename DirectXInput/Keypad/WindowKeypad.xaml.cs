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
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.Keypad
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

                //Update the key names
                UpdateKeypadNames();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Hide the keypad window
        public new void Hide()
        {
            try
            {
                //Play window close sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                //Update the keypad visibility
                this.Visibility = Visibility.Collapsed;
                vWindowVisible = false;
                Debug.WriteLine("Hiding the Keypad window.");
            }
            catch { }
        }

        //Show the keypad window
        public new async Task Show()
        {
            try
            {
                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Disable hardware capslock
                await DisableHardwareCapsLock();

                //Set the keypad mapping profile
                SetKeypadMappingProfile();

                //Update the key names
                UpdateKeypadNames();

                //Update the keypad opacity
                UpdateKeypadOpacity();

                //Update the keypad style
                UpdateKeypadStyle();

                //Update the keypad visibility
                this.Visibility = Visibility.Visible;
                vWindowVisible = true;
                Debug.WriteLine("Showing the Keypad window.");
            }
            catch { }
        }

        //Update the keypad opacity
        public void UpdateKeypadOpacity()
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

                        List<Image> allImages = AVFunctions.FindVisualChildren<Image>(grid_Application);
                        foreach (Image image in allImages)
                        {
                            if (vKeypadMappingProfile.KeypadDisplayStyle == 0)
                            {
                                image.Style = (Style)Application.Current.Resources["KeypadImageLight"];
                            }
                            else
                            {
                                image.Style = (Style)Application.Current.Resources["KeypadImageDark"];
                            }
                        }
                    }
                    catch { }
                });
            }
            catch { }
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
                vKeypadMappingProfile = directKeypadMappingProfile;
            }
            catch { }
        }

        //Update the window position on resolution change
        public void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateWindowPosition();
            }
            catch { }
        }

        //Update the window style
        void UpdateWindowStyle()
        {
            try
            {
                //Set the window style
                IntPtr UpdatedStyle = new IntPtr((uint)WindowStyles.WS_VISIBLE);
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_STYLE, UpdatedStyle);

                //Set the window style ex
                IntPtr UpdatedExStyle = new IntPtr((uint)(WindowStylesEx.WS_EX_TOPMOST | WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TRANSPARENT));
                SetWindowLongAuto(vInteropWindowHandle, (int)WindowLongFlags.GWL_EXSTYLE, UpdatedExStyle);

                //Set the window as top most
                SetWindowPos(vInteropWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)(WindowSWP.NOMOVE | WindowSWP.NOSIZE));

                Debug.WriteLine("The window style has been updated.");
            }
            catch { }
        }

        //Update the window position
        public void UpdateWindowPosition()
        {
            try
            {
                //Get the current active screen
                int monitorNumber = Convert.ToInt32(vConfigurationCtrlUI.AppSettings.Settings["DisplayMonitor"].Value);
                DisplayMonitorSettings displayMonitorSettings = GetScreenSettings(monitorNumber);

                //Move and resize the window
                WindowMove(vInteropWindowHandle, displayMonitorSettings.BoundsLeft, displayMonitorSettings.BoundsTop);
                WindowResize(vInteropWindowHandle, displayMonitorSettings.WidthNative, displayMonitorSettings.HeightNative);
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
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftLeftMod, vKeypadMappingProfile.ThumbLeftLeft, grid_ThumbLeftLeft, textblock_ThumbLeftLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftUpMod, vKeypadMappingProfile.ThumbLeftUp, grid_ThumbLeftUp, textblock_ThumbLeftUp);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftRightMod, vKeypadMappingProfile.ThumbLeftRight, grid_ThumbLeftRight, textblock_ThumbLeftRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbLeftDownMod, vKeypadMappingProfile.ThumbLeftDown, grid_ThumbLeftDown, textblock_ThumbLeftDown);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightLeftMod, vKeypadMappingProfile.ThumbRightLeft, grid_ThumbRightLeft, textblock_ThumbRightLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightUpMod, vKeypadMappingProfile.ThumbRightUp, grid_ThumbRightUp, textblock_ThumbRightUp);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightRightMod, vKeypadMappingProfile.ThumbRightRight, grid_ThumbRightRight, textblock_ThumbRightRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ThumbRightDownMod, vKeypadMappingProfile.ThumbRightDown, grid_ThumbRightDown, textblock_ThumbRightDown);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonBackMod, vKeypadMappingProfile.ButtonBack, grid_ButtonBack, textblock_ButtonBack);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonStartMod, vKeypadMappingProfile.ButtonStart, grid_ButtonStart, textblock_ButtonStart);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonXMod, vKeypadMappingProfile.ButtonX, grid_ButtonX, textblock_ButtonX);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonYMod, vKeypadMappingProfile.ButtonY, grid_ButtonY, textblock_ButtonY);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonAMod, vKeypadMappingProfile.ButtonA, grid_ButtonA, textblock_ButtonA);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonBMod, vKeypadMappingProfile.ButtonB, grid_ButtonB, textblock_ButtonB);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonShoulderLeftMod, vKeypadMappingProfile.ButtonShoulderLeft, grid_ButtonShoulderLeft, textblock_ButtonShoulderLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonTriggerLeftMod, vKeypadMappingProfile.ButtonTriggerLeft, grid_ButtonTriggerLeft, textblock_ButtonTriggerLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonThumbLeftMod, vKeypadMappingProfile.ButtonThumbLeft, grid_ButtonThumbLeft, textblock_ButtonThumbLeft);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonShoulderRightMod, vKeypadMappingProfile.ButtonShoulderRight, grid_ButtonShoulderRight, textblock_ButtonShoulderRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonTriggerRightMod, vKeypadMappingProfile.ButtonTriggerRight, grid_ButtonTriggerRight, textblock_ButtonTriggerRight);
                        UpdateKeypadKeyDetails(vKeypadMappingProfile.ButtonThumbRightMod, vKeypadMappingProfile.ButtonThumbRight, grid_ButtonThumbRight, textblock_ButtonThumbRight);
                    }
                    catch { }
                });
            }
            catch { }
        }

        //Update key details
        void UpdateKeypadKeyDetails(KeysVirtual? keyMod, KeysVirtual? key, Grid keyGrid, TextBlock keyTextBlock)
        {
            try
            {
                if (keyMod != null)
                {
                    keyTextBlock.Text = GetVirtualKeyName((KeysVirtual)keyMod, true) + "\n" + GetVirtualKeyName((KeysVirtual)key, true);
                    keyGrid.Opacity = 1;
                }
                else if (key != null)
                {
                    keyTextBlock.Text = GetVirtualKeyName((KeysVirtual)key, true);
                    keyGrid.Opacity = 1;
                }
                else
                {
                    keyTextBlock.Text = "?";
                    keyGrid.Opacity = 0;
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
                        await KeyPressSingle((byte)KeysVirtual.CapsLock, false);
                    }
                });
            }
            catch { }
        }
    }
}