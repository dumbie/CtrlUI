using ArnoldVinkCode;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DriverInstaller.AppVariables;
using static LibraryShared.Classes;

namespace DriverInstaller
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Make window able to drag from border
                this.MouseDown += WindowMain_MouseDown;

                //Show welcome message in textbox
                TextBoxAppend("Welcome to the Driver Installer.");

                //Check if DirectXInput is running
                vDirectXInputRunning = Process.GetProcessesByName("DirectXInput").Any();

                //Load Json profiles
                JsonLoadProfile(ref vDirectCloseTools, "DirectCloseTools");
            }
            catch { }
        }

        //Drag the window around
        private void WindowMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch { }
        }

        //Enable or disable element
        public void ElementEnableDisable(FrameworkElement frameworkElement, bool enableElement)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (enableElement)
                    {
                        frameworkElement.IsEnabled = true;
                    }
                    else
                    {
                        frameworkElement.IsEnabled = false;
                    }
                });
            }
            catch { }
        }

        //Append text to textbox
        public void TextBoxAppend(string appendText)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    textbox_Status.AppendText(appendText + "\r\n");
                    textbox_Status.ScrollToEnd();
                });
            }
            catch { }
        }

        //Update the progressbar
        public void ProgressBarUpdate(double Progress, bool Indeterminate)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (Indeterminate)
                    {
                        gif_Progress_Status.Hide();
                    }
                    else
                    {
                        gif_Progress_Status.Show();
                    }
                    progressbar_Status.IsIndeterminate = Indeterminate;
                    progressbar_Status.Value = Progress;
                });
            }
            catch { }
        }

        //Close running controller tools
        async Task CloseControllerTools()
        {
            try
            {
                TextBoxAppend("Closing running controller tools.");
                Debug.WriteLine("Closing running controller tools.");

                //Close DirectXInput
                try
                {
                    CloseProcessesByNameOrTitle("DirectXInput", false);
                }
                catch { }

                //Close other tools
                foreach (ProfileShared closeTool in vDirectCloseTools)
                {
                    try
                    {
                        CloseProcessesByNameOrTitle(closeTool.String1, false);
                    }
                    catch { }
                }

                //Wait for tools to close
                await Task.Delay(1000);
            }
            catch { }
        }

        //Application Close Handler
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
            }
            catch { }
        }

        //Application Close Button
        async void Button_Driver_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Application_Exit("Closing the driver installer in a bit.", vDirectXInputRunning);
            }
            catch { }
        }

        //Close the application
        async Task Application_Exit(string exitMessage, bool runDirectXInput)
        {
            try
            {
                Debug.WriteLine("Exiting application.");
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.IsEnabled = false;
                });

                //Disable the buttons
                ElementEnableDisable(button_Driver_Install, false);
                ElementEnableDisable(button_Driver_Uninstall, false);
                ElementEnableDisable(button_Driver_Close, false);

                //Run DirectXInput after the drivers installed
                if (runDirectXInput)
                {
                    TextBoxAppend("Running the DirectXInput application.");
                    await ProcessLauncherWin32Async("DirectXInput-Admin.exe", "", "", true, false);
                }

                //Set the exit reason text message
                TextBoxAppend(exitMessage);
                ProgressBarUpdate(100, false);

                //Close the application after x seconds
                await Task.Delay(3000);
                Environment.Exit(0);
            }
            catch { }
        }
    }
}