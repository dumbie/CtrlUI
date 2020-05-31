using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static ArnoldVinkCode.AVDisplayMonitor;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;
using static DirectXInput.AppVariables;
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
        protected override async void OnSourceInitialized(EventArgs e)
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

                //Disable hardware capslock
                await DisableHardwareCapsLock();
            }
            catch { }
        }

        //Hide the keyboard window
        public new void Hide()
        {
            try
            {
                if (this.Opacity != 0)
                {
                    //Play window close sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Update the keyboard opacity
                    this.Opacity = 0;
                    vWindowVisible = false;
                    Debug.WriteLine("Hiding the Keypad window.");
                }
            }
            catch { }
        }

        //Show the keyboard window
        public new void Show()
        {
            try
            {
                //Delay keyboard input
                vControllerDelay_Keyboard = Environment.TickCount + vControllerDelayMediumTicks;

                //Play window open sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Update the keyboard opacity
                this.Opacity = Convert.ToDouble(ConfigurationManager.AppSettings["KeypadOpacity"]);
                this.Visibility = Visibility.Visible;
                vWindowVisible = true;
                Debug.WriteLine("Showing the Keypad window.");
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

        //Update the key names
        public void UpdateKeypadNames()
        {
            try
            {
                textblock_ButtonBack.Text = GetVirtualKeyName((KeysVirtual)vDirectKeypadMapping.ButtonBack, true);
                textblock_ButtonStart.Text = GetVirtualKeyName((KeysVirtual)vDirectKeypadMapping.ButtonStart, true);

                textblock_ButtonX.Text = GetVirtualKeyName((KeysVirtual)vDirectKeypadMapping.ButtonX, true);
                textblock_ButtonY.Text = "Ctrl\nAlt"; //GetVirtualKeyName((KeysVirtual)vDirectKeypadMapping.ButtonY, true);

                textblock_ButtonA.Text = GetVirtualKeyName((KeysVirtual)vDirectKeypadMapping.ButtonA, true);
                textblock_ButtonB.Text = GetVirtualKeyName((KeysVirtual)vDirectKeypadMapping.ButtonB, true);

                textblock_ButtonShoulderLeft.Text = GetVirtualKeyName((KeysVirtual)vDirectKeypadMapping.ButtonShoulderLeft, true);
                textblock_ButtonShoulderRight.Text = GetVirtualKeyName((KeysVirtual)vDirectKeypadMapping.ButtonShoulderRight, true);
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