using ArnoldVinkCode;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessWin32Functions;

namespace Updater
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Update the textblock
        public void TextBlockUpdate(string Text)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    textblock_Status.Text = Text;
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

        //Window Startup
        public async Task Startup()
        {
            try
            {
                //Check if previous update files are in the way
                if (File.Exists("UpdaterNew.exe")) { try { File.Delete("UpdaterNew.exe"); } catch { } }
                if (File.Exists("AppUpdate.zip")) { try { File.Delete("AppUpdate.zip"); } catch { } }

                //Check if CtrlUI is running and close it
                bool CtrlUIRunning = false;
                foreach (Process CloseProcess in Process.GetProcessesByName("CtrlUI"))
                {
                    CtrlUIRunning = true;
                    CloseProcess.Kill();
                }

                //Check if DirectXInput is running and close it
                bool DirectXInputRunning = false;
                foreach (Process CloseProcess in Process.GetProcessesByName("DirectXInput"))
                {
                    DirectXInputRunning = true;
                    CloseProcess.Kill();
                }

                //Check if KeyboardController is running and close it
                bool KeyboardControllerRunning = false;
                foreach (Process CloseProcess in Process.GetProcessesByName("KeyboardController"))
                {
                    KeyboardControllerRunning = true;
                    CloseProcess.Kill();
                }

                //Check if Fps Overlayer is running and close it
                bool FpsOverlayerRunning = false;
                foreach (Process CloseProcess in Process.GetProcessesByName("FpsOverlayer"))
                {
                    FpsOverlayerRunning = true;
                    CloseProcess.Kill();
                }

                //Check if Driver Installer is running and close it
                foreach (Process CloseProcess in Process.GetProcessesByName("DriverInstaller"))
                {
                    CloseProcess.Kill();
                }

                //Wait for applications to have closed
                await Task.Delay(1000);

                //Download application update from the website
                try
                {
                    WebClient WebClient = new WebClient();
                    WebClient.Headers[HttpRequestHeader.UserAgent] = "Application Updater";
                    WebClient.DownloadProgressChanged += (object Object, DownloadProgressChangedEventArgs Args) =>
                    {
                        ProgressBarUpdate(Args.ProgressPercentage, false);
                        TextBlockUpdate("Downloading update file: " + Args.ProgressPercentage + "%");
                    };
                    await WebClient.DownloadFileTaskAsync(new Uri("http://download.arnoldvink.com/?dl=CtrlUI.zip"), "AppUpdate.zip");
                    Debug.WriteLine("Update file has been downloaded");
                }
                catch
                {
                    await Application_Exit("Failed to download update, closing in a bit.");
                    return;
                }

                //Delete the old drivers directory
                try
                {
                    if (Directory.Exists("Resources/Drivers"))
                    {
                        Debug.WriteLine("Removing: Drivers directory.");
                        Directory.Delete("Resources/Drivers", true);
                    }
                }
                catch { }

                //Extract the downloaded update archive
                try
                {
                    TextBlockUpdate("Updating the application to the latest version.");
                    using (ZipArchive ZipArchive = ZipFile.OpenRead("AppUpdate.zip"))
                    {
                        foreach (ZipArchiveEntry ZipFile in ZipArchive.Entries)
                        {
                            string ExtractPath = AVFunctions.StringReplaceFirst(ZipFile.FullName, "CtrlUI/", "", false);
                            if (!string.IsNullOrWhiteSpace(ExtractPath))
                            {
                                if (string.IsNullOrWhiteSpace(ZipFile.Name)) { Directory.CreateDirectory(ExtractPath); }
                                else
                                {
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("Apps.json".ToLower())) { Debug.WriteLine("Skipping: Apps.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("AppsBlacklistProcess.json".ToLower())) { Debug.WriteLine("Skipping: AppsBlacklistProcess.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("AppsBlacklistShortcut.json".ToLower())) { Debug.WriteLine("Skipping: AppsBlacklistShortcut.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("AppsBlacklistShortcutUri.json".ToLower())) { Debug.WriteLine("Skipping: AppsBlacklistShortcutUri.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("AppsOtherLaunchers.json".ToLower())) { Debug.WriteLine("Skipping: AppsOtherLaunchers.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("AppsOtherTools.json".ToLower())) { Debug.WriteLine("Skipping: AppsOtherTools.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("FileLocations.json".ToLower())) { Debug.WriteLine("Skipping: FileLocations.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("ShortcutLocations.json".ToLower())) { Debug.WriteLine("Skipping: ShortcutLocations.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("FpsBlacklistProcess.json".ToLower())) { Debug.WriteLine("Skipping: FpsBlacklistProcess.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("FpsPositionProcess.json".ToLower())) { Debug.WriteLine("Skipping: FpsPositionProcess.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("Controllers.json".ToLower())) { Debug.WriteLine("Skipping: Controllers.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("Background.png".ToLower())) { Debug.WriteLine("Skipping: Background.png"); continue; }

                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlUI.exe.Config".ToLower())) { Debug.WriteLine("Skipping: CtrlUI.exe.Config"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("DirectXInput.exe.Config".ToLower())) { Debug.WriteLine("Skipping: DirectXInput.exe.Config"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("KeyboardController.exe.Config".ToLower())) { Debug.WriteLine("Skipping: KeyboardController.exe.Config"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("FpsOverlayer.exe.Config".ToLower())) { Debug.WriteLine("Skipping: FpsOverlayer.exe.Config"); continue; }

                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("Updater.exe".ToLower()))
                                    {
                                        Debug.WriteLine("Renaming: Updater.exe");
                                        ExtractPath = ExtractPath.Replace("Updater.exe", "UpdaterNew.exe");
                                    }

                                    ZipFile.ExtractToFile(ExtractPath, true);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    await Application_Exit("Failed to extract update, closing in a bit.");
                    return;
                }

                //Delete the update installation zip file
                TextBlockUpdate("Cleaning up the update installation files.");
                if (File.Exists("AppUpdate.zip"))
                {
                    Debug.WriteLine("Removing: AppUpdate.zip");
                    File.Delete("AppUpdate.zip");
                }

                //Start CtrlUI after the update has completed.
                if (CtrlUIRunning)
                {
                    TextBlockUpdate("Running the updated version of the application.");
                    ProcessLauncherWin32("CtrlUI-Admin.exe", "", "", true, false);
                }

                //Start DirectXInput after the update has completed.
                if (DirectXInputRunning)
                {
                    TextBlockUpdate("Running the updated version of the application.");
                    ProcessLauncherWin32("DirectXInput-Admin.exe", "", "", true, false);
                }

                //Start KeyboardController after the update has completed.
                if (KeyboardControllerRunning)
                {
                    TextBlockUpdate("Running the updated version of the application.");
                    ProcessLauncherWin32("KeyboardController-Admin.exe", "", "", true, false);
                }

                //Start FpsOverlayer after the update has completed.
                if (FpsOverlayerRunning)
                {
                    TextBlockUpdate("Running the updated version of the application.");
                    ProcessLauncherWin32("FpsOverlayer-Admin.exe", "", "", true, false);
                }

                //Close the application
                await Application_Exit("Application has been updated, closing in a bit.");
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
                Debug.WriteLine("Exiting application.");

                //Delete the update installation zip file
                if (File.Exists("AppUpdate.zip"))
                {
                    Debug.WriteLine("Removing: AppUpdate.zip");
                    File.Delete("AppUpdate.zip");
                }

                //Set the exit reason text message
                TextBlockUpdate(ExitMessage);
                ProgressBarUpdate(100, false);

                //Close the application after x seconds
                await Task.Delay(2000);
                Environment.Exit(0);
            }
            catch { }
        }
    }
}