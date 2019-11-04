using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show and load Settings window information
        async Task ShowLoadSettingsPopup()
        {
            try
            {
                //Set and load the quick launch application name
                try
                {
                    DataBindApp QuickLaunchApp = CombineAppLists(false, false).Where(x => x.QuickLaunch).FirstOrDefault();
                    btn_Settings_AppQuickLaunch.Content = "Change quick launch app: " + QuickLaunchApp.Name;
                }
                catch
                {
                    btn_Settings_AppQuickLaunch.Content = "Change the quick launch app";
                }

                await Popup_Show(grid_Popup_Settings, cb_SettingsLaunchFullscreen, true);
            }
            catch { }
        }

        //Open Windows Game controller settings
        async void Button_Settings_CheckControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Manage controllers";
                Answers.Add(Answer1);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                DataBindString Result = await Popup_Show_MessageBox("This will open a window you can't controller", "", "You can always return back to CtrlUI using the 'Guide' button on your controller or you can use the Keyboard Controller's mouse function.", Answers);
                if (Result != null && Result == Answer1)
                {
                    await ProcessLauncherWin32Prepare(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\control.exe", "", "joy.cpl", true, true, false);
                }
            }
            catch { }
        }

        //Launch DirectXInput application
        async void Button_LaunchDirectXInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckRunningProcessByName("DirectXInput", false))
                {
                    await ProcessLauncherWin32Prepare("DirectXInput-Admin.exe", "", "", true, true, false);
                }
            }
            catch { }
        }

        //Check for available application update
        async void Button_Settings_CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await CheckForAppUpdate(false);
            }
            catch { }
        }

        //Check for available application update
        bool CheckingForUpdate = false;
        async Task CheckForAppUpdate(bool Popup)
        {
            try
            {
                if (!CheckingForUpdate)
                {
                    CheckingForUpdate = true;

                    string ResCurrentVersion = await AVDownloader.DownloadStringAsync(5000, "CtrlUI", null, new Uri("http://download.arnoldvink.com/CtrlUI.zip-version.txt" + "?nc=" + Environment.TickCount));
                    if (ResCurrentVersion != Assembly.GetEntryAssembly().FullName.Split('=')[1].Split(',')[0])
                    {
                        if (Popup) { Popup_Show_Status("Refresh", "Update available"); }
                        else
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Refresh.png" }, IntPtr.Zero, -1);
                            Answer1.Name = "Update now";
                            Answers.Add(Answer1);

                            DataBindString cancelString = new DataBindString();
                            cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                            cancelString.Name = "Cancel";
                            Answers.Add(cancelString);

                            DataBindString Result = await Popup_Show_MessageBox("A newer version has been found: v" + ResCurrentVersion, "", "Do you want to update the application to the newest version now?", Answers);
                            if (Result != null && Result == Answer1)
                            {
                                await ProcessLauncherWin32Prepare("Updater.exe", "", "", false, true, false);
                                await Application_Exit(true);
                            }
                        }
                    }
                    else
                    {
                        if (!Popup)
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                            Answer1.Name = "Alright";
                            Answers.Add(Answer1);

                            await Popup_Show_MessageBox("No new application update has been found", "", "", Answers);
                        }
                    }

                    //Set the last application update check date
                    SettingSave("AppUpdateCheck", DateTime.Now.ToString(vAppCultureInfo));
                    CheckingForUpdate = false;
                }
            }
            catch
            {
                CheckingForUpdate = false;
                if (!Popup)
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Failed to check for application update", "", "Please check your internet connection and try again.", Answers);
                }
            }
        }

        //Create startup shortcut
        void ManageShortcutStartup()
        {
            try
            {
                //Set application shortcut paths
                string TargetFilePath_Admin = Assembly.GetEntryAssembly().CodeBase.Replace(".exe", "-Admin.exe");
                string TargetName_Admin = Assembly.GetEntryAssembly().GetName().Name;
                string TargetFileShortcut_Admin = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + TargetName_Admin + ".url";

                //Check if the shortcut already exists
                if (!File.Exists(TargetFileShortcut_Admin))
                {
                    Debug.WriteLine("Adding application to Windows startup");
                    using (StreamWriter StreamWriter = new StreamWriter(TargetFileShortcut_Admin))
                    {
                        StreamWriter.WriteLine("[InternetShortcut]");
                        StreamWriter.WriteLine("URL=" + TargetFilePath_Admin);
                        StreamWriter.WriteLine("IconFile=" + TargetFilePath_Admin.Replace("file:///", ""));
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }
                }
                else
                {
                    Debug.WriteLine("Removing application from Windows startup");
                    if (File.Exists(TargetFileShortcut_Admin)) { File.Delete(TargetFileShortcut_Admin); }
                }
            }
            catch { Debug.WriteLine("Failed creating startup shortcut."); }
        }

        //Create geforce experience shortcut
        async void Button_Settings_AddGeforceExperience_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                //Set application shortcut paths
                string TargetFilePath_Admin = Assembly.GetEntryAssembly().CodeBase.Replace(".exe", "-Admin.exe");
                string TargetName_Admin = Assembly.GetEntryAssembly().GetName().Name;
                string TargetFileShortcut_Admin = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\" + TargetName_Admin + ".url";
                string TargetFileBoxArtFile_Admin = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\StreamingAssets\\" + TargetName_Admin + "\\box-art.png";
                string TargetFileBoxArtDirectory_Admin = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\StreamingAssets\\" + TargetName_Admin;

                //Check if the shortcut folder exists
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\StreamingAssets\\"))
                {
                    Debug.WriteLine("GeForce experience shortcut folder not found, creating it.");
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\StreamingAssets\\");
                }

                //Check if the shortcut already exists
                if (!File.Exists(TargetFileShortcut_Admin))
                {
                    Debug.WriteLine("Adding application to GeForce Experience");

                    using (StreamWriter StreamWriter = new StreamWriter(TargetFileShortcut_Admin))
                    {
                        StreamWriter.WriteLine("[InternetShortcut]");
                        StreamWriter.WriteLine("URL=" + TargetFilePath_Admin);
                        StreamWriter.WriteLine("IconFile=" + TargetFilePath_Admin.Replace("file:///", ""));
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }

                    //Copy art box to the Streaming Assets directory
                    Directory.CreateDirectory(TargetFileBoxArtFile_Admin.Replace("\\box-art.png", ""));
                    File.Copy("Assets\\BoxArt.png", TargetFileBoxArtFile_Admin, true);

                    btn_Settings_AddGeforceExperience.Content = "Remove CtrlUI from GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("CtrlUI has been added to GeForce Experience", "", "You can now remotely launch the Controller User Interface from your devices.", Answers);
                }
                else
                {
                    Debug.WriteLine("Removing application from GeForce Experience");
                    if (File.Exists(TargetFileShortcut_Admin)) { File.Delete(TargetFileShortcut_Admin); }
                    if (Directory.Exists(TargetFileBoxArtDirectory_Admin)) { Directory.Delete(TargetFileBoxArtDirectory_Admin, true); }

                    btn_Settings_AddGeforceExperience.Content = "Add CtrlUI to GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("CtrlUI has been removed from GeForce Experience", "", "", Answers);
                }
            }
            catch
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Alright";
                Answers.Add(Answer1);

                await Popup_Show_MessageBox("Failed to add CtrlUI to GeForce Experience", "", "Please make sure that GeForce experience is installed.", Answers);
            }
        }
    }
}