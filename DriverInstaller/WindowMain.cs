using ArnoldVinkCode;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ArnoldVinkProcesses;

namespace DriverInstaller
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Startup
        public async Task Startup()
        {
            try
            {
                //Check if DirectXInput is still running
                await CheckDirectXInputRunning();

                //Install the required drivers
                async void TaskAction()
                {
                    try
                    {
                        await InstallRequiredDrivers();
                    }
                    catch { }
                }
                await AVActions.TaskStart(TaskAction, null);
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

        //Check if DirectXInput is still running
        async Task CheckDirectXInputRunning()
        {
            try
            {
                TextBoxAppend("Waiting for DirectXInput to have closed.");
                while (Process.GetProcessesByName("DirectXInput").Any())
                {
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
        async Task Application_Exit(string ExitMessage)
        {
            try
            {
                Debug.WriteLine("Exiting Driver Installer.");

                //Run DirectXInput after the drivers installed
                TextBoxAppend("Running the DirectXInput application.");
                ProcessLauncherWin32("DirectXInput-Admin.exe", "", "", false, false);

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