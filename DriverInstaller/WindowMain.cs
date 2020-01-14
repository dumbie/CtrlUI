using ArnoldVinkCode;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;

namespace DriverInstaller
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

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
        public void TextBoxAppend(string Text)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    textbox_Status.AppendText(Text + "\r\n");
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
                    progressbar_Status.IsIndeterminate = Indeterminate;
                    progressbar_Status.Value = Progress;
                });
            }
            catch { }
        }

        //Close DirectXInput if running
        void CloseDirectXInput()
        {
            try
            {
                CloseProcessesByNameOrTitle("DirectXInput", false);
            }
            catch { }
        }

        //Check if DirectXInput is still running
        async Task CheckDirectXInputRunning()
        {
            try
            {
                while (Process.GetProcessesByName("DirectXInput").Any())
                {
                    TextBoxAppend("Waiting for DirectXInput to have closed.");
                    Debug.WriteLine("Waiting for DirectXInput to have closed.");
                    await Task.Delay(500);
                }
                ProgressBarUpdate(10, false);
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

        //Close the application
        async Task Application_Exit(string ExitMessage, bool runDirectXInput)
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Run DirectXInput after the drivers installed
                if (runDirectXInput)
                {
                    TextBoxAppend("Running the DirectXInput application.");
                    ProcessLauncherWin32("DirectXInput-Admin.exe", "", "", true, false);
                }

                //Set the exit reason text message
                TextBoxAppend(ExitMessage);
                ProgressBarUpdate(100, false);

                //Close the application after x seconds
                await Task.Delay(2000);
                Environment.Exit(0);
            }
            catch { }
        }
    }
}