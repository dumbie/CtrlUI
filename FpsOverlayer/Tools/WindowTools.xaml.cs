using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public partial class WindowTools : Window
    {
        //Window Initialize
        public WindowTools() { InitializeComponent(); }

        //Window Variables
        public IntPtr vInteropWindowHandle = IntPtr.Zero;
        public bool vWindowVisible = false;
        private Point vWindowMousePoint;
        private Thickness vWindowMargin;
        private double vWindowWidth;
        private double vWindowHeight;

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

                //Update window display affinity
                UpdateWindowAffinity();

                //Update window position
                UpdateWindowPosition();

                //Bind lists to the listbox elements
                ListBoxBindLists();

                //Reset browser interface
                Browser_Reset_Interface(string.Empty, true);

                //Show or hide certain tools
                ShowHide_Tools();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

                //Show tools when enabled
                if (SettingLoad(vConfigurationFpsOverlayer, "ToolsLaunch", typeof(bool)))
                {
                    SwitchToolsVisibility();
                }
            }
            catch { }
        }

        //Hide the window
        public new void Hide()
        {
            try
            {
                //Update the window visibility
                UpdateWindowVisibility(false);
            }
            catch { }
        }

        //Show the window
        public new void Show()
        {
            try
            {
                //Update the window visibility
                UpdateWindowVisibility(true);
            }
            catch { }
        }

        //Update the window visibility
        public void UpdateWindowVisibility(bool visible)
        {
            try
            {
                if (visible)
                {
                    if (!vWindowVisible)
                    {
                        //Create and show the window
                        base.Show();

                        //Update window visibility
                        WindowUpdateVisibility(vInteropWindowHandle, true);

                        //Update window style
                        WindowUpdateStyle(vInteropWindowHandle, true, true, vToolsBlockInteract, vToolsBlockInteract);

                        this.Title = "Tools Overlayer (Visible)";
                        vWindowVisible = true;
                        Debug.WriteLine("Showing the window.");
                    }
                }
                else
                {
                    if (vWindowVisible)
                    {
                        //Update window visibility
                        WindowUpdateVisibility(vInteropWindowHandle, false);

                        this.Title = "Tools Overlayer (Hidden)";
                        vWindowVisible = false;
                        Debug.WriteLine("Hiding the window.");
                    }
                }
            }
            catch { }
        }

        //Update window position
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

        //Update window display affinity
        public void UpdateWindowAffinity()
        {
            try
            {
                if (SettingLoad(vConfigurationFpsOverlayer, "HideScreenCapture", typeof(bool)))
                {
                    SetWindowDisplayAffinity(vInteropWindowHandle, DisplayAffinityFlags.WDA_EXCLUDEFROMCAPTURE);
                }
                else
                {
                    SetWindowDisplayAffinity(vInteropWindowHandle, DisplayAffinityFlags.WDA_NONE);
                }
            }
            catch { }
        }

        //Update windows on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for resolution change
                await Task.Delay(2000);

                //Update window style
                WindowUpdateStyle(vInteropWindowHandle, true, true, vToolsBlockInteract, vToolsBlockInteract);

                //Update window position
                UpdateWindowPosition();
            }
            catch { }
        }

        //Close tools overlay
        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SwitchToolsVisibility();
            }
            catch { }
        }

        //Bind lists to listbox elements
        void ListBoxBindLists()
        {
            try
            {
                listbox_Link.ItemsSource = vFpsBrowserLinks;
                combobox_Notes_Select.ItemsSource = vNotesFiles;

                LoadNotesList(string.Empty);

                Debug.WriteLine("Lists bound to interface.");
            }
            catch { }
        }

        //Switch clickthrough mode
        private void button_Pin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SwitchToolsClickthrough(false);
            }
            catch { }
        }
    }
}