using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.AVWindowFunctions;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer.ToolsOverlay
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

                //Update window position
                UpdateWindowPosition();

                //Bind lists to the listbox elements
                ListBoxBindLists();

                //Reset browser interface to defaults
                Browser_Reset_Interface(string.Empty);

                //Show or hide tools
                Show_Hide_Tools();

                //Check if resolution has changed
                SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            }
            catch { }
        }

        //Show or hide tools
        private void Show_Hide_Tools()
        {
            try
            {
                bool showNotes = SettingLoad(vConfigurationFpsOverlayer, "ToolsShowNotes", typeof(bool));
                bool showBrowser = SettingLoad(vConfigurationFpsOverlayer, "ToolsShowBrowser", typeof(bool));

                //Switch visibility
                if (showNotes)
                {
                    border_Notes.Visibility = Visibility.Visible;
                }
                else
                {
                    border_Notes.Visibility = Visibility.Collapsed;
                }

                if (showBrowser)
                {
                    border_Browser.Visibility = Visibility.Visible;
                }
                else
                {
                    border_Browser.Visibility = Visibility.Collapsed;
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

                //Remove browser from grid
                Browser_Remove_Grid(string.Empty);
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

                        //Update window visibility
                        WindowUpdateVisibility(vInteropWindowHandle, true);

                        //Update window style
                        WindowUpdateStyle(vInteropWindowHandle, true, vToolsBlockInteract, vToolsBlockInteract);

                        this.Title = "ToolsOverlayer (Visible)";
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

                        this.Title = "ToolsOverlayer (Hidden)";
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

        //Close tools overlay
        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
            }
            catch { }
        }

        //Update window position on resolution change
        public async void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                //Wait for change to complete
                await Task.Delay(2000);

                //Update window style
                WindowUpdateStyle(vInteropWindowHandle, true, vToolsBlockInteract, vToolsBlockInteract);

                //Check if window is out of screen
                WindowCheckScreenBounds(null, vInteropWindowHandle, 10);
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

        //Load all notes to list
        private void LoadNotesList(string selectNoteName)
        {
            try
            {
                vNotesFiles.Clear();
                List<string> noteFiles = AVFiles.GetFilesLevel("Notes", "*", 0);
                foreach (string fileName in noteFiles)
                {
                    string noteName = Path.GetFileNameWithoutExtension(fileName);
                    vNotesFiles.Add(noteName);
                }

                //Select note
                if (string.IsNullOrWhiteSpace(selectNoteName))
                {
                    combobox_Notes_Select.SelectedIndex = 0;
                }
                else
                {
                    combobox_Notes_Select.SelectedItem = selectNoteName;
                }
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

        //Switch clickthrough mode
        public void SwitchToolsClickthrough(bool forceVisible)
        {
            try
            {
                if (forceVisible || vToolsBlockInteract)
                {
                    //Show bars
                    border_Menu.Visibility = Visibility.Visible;
                    grid_Browser_Menu.Visibility = Visibility.Visible;
                    grid_Notes_Menu.Visibility = Visibility.Visible;
                    textbox_Notes_Name.Visibility = Visibility.Visible;

                    //Update window style
                    vToolsBlockInteract = false;
                    WindowUpdateStyle(vInteropWindowHandle, true, vToolsBlockInteract, vToolsBlockInteract);
                }
                else
                {
                    //Hide bars
                    border_Menu.Visibility = Visibility.Collapsed;
                    grid_Browser_Menu.Visibility = Visibility.Collapsed;
                    grid_Browser_Link.Visibility = Visibility.Collapsed;
                    grid_Notes_Menu.Visibility = Visibility.Collapsed;
                    textbox_Notes_Name.Visibility = Visibility.Collapsed;

                    //Update window style
                    vToolsBlockInteract = true;
                    WindowUpdateStyle(vInteropWindowHandle, true, vToolsBlockInteract, vToolsBlockInteract);
                }
            }
            catch { }
        }

        //Switch browser visibility
        public void SwitchToolsVisibility()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (vWindowVisible && vToolsBlockInteract)
                    {
                        SwitchToolsClickthrough(false);
                    }
                    else if (vWindowVisible)
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }
                });
            }
            catch { }
        }

        private void button_ShowHide_Browser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (border_Browser.Visibility == Visibility.Visible)
                {
                    //Switch visibility
                    border_Browser.Visibility = Visibility.Collapsed;

                    //Update setting
                    SettingSave(vConfigurationFpsOverlayer, "ToolsShowBrowser", "False");
                }
                else
                {
                    //Switch visibility
                    border_Browser.Visibility = Visibility.Visible;

                    //Update setting
                    SettingSave(vConfigurationFpsOverlayer, "ToolsShowBrowser", "True");
                }
            }
            catch { }
        }

        private void button_ShowHide_Notes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (border_Notes.Visibility == Visibility.Visible)
                {
                    //Switch visibility
                    border_Notes.Visibility = Visibility.Collapsed;

                    //Update setting
                    SettingSave(vConfigurationFpsOverlayer, "ToolsShowNotes", "False");
                }
                else
                {
                    //Switch visibility
                    border_Notes.Visibility = Visibility.Visible;

                    //Update setting
                    SettingSave(vConfigurationFpsOverlayer, "ToolsShowNotes", "True");
                }
            }
            catch { }
        }
    }
}