using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Open Windows Game controller settings
        async void Button_Settings_CheckControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                Answer1.Name = "Manage controllers";
                Answers.Add(Answer1);

                DataBindString messageResult = await Popup_Show_MessageBox("This will open a window you can't controller", "", "You can always return back to CtrlUI using the 'Guide' button on your controller or you can use the Keyboard Controller's mouse function.", Answers);
                if (messageResult != null && messageResult == Answer1)
                {
                    ProcessLauncherWin32(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\control.exe", "", "joy.cpl", false, false);
                }
            }
            catch { }
        }

        //Change the quick launch app
        async void Button_Settings_AppQuickLaunch(object sender, RoutedEventArgs e)
        {
            try
            {
                //Add all apps to the string list
                List<DataBindString> Answers = new List<DataBindString>();
                foreach (DataBindApp dataBindApp in CombineAppLists(false, false))
                {
                    DataBindString stringApp = new DataBindString() { Name = dataBindApp.Name, ImageBitmap = dataBindApp.ImageBitmap };
                    Answers.Add(stringApp);
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Quick Launch Application", "", "Please select a new quick launch application:", Answers);
                if (messageResult != null)
                {
                    btn_Settings_AppQuickLaunch.Content = "Change quick launch app: " + messageResult.Name;

                    //Set previous quick launch application to false
                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.QuickLaunch))
                    {
                        dataBindApp.QuickLaunch = false;
                    }

                    //Set new quick launch application to true
                    foreach (DataBindApp dataBindApp in CombineAppLists(false, false).Where(x => x.Name.ToLower() == messageResult.Name.ToLower()))
                    {
                        dataBindApp.QuickLaunch = true;
                    }

                    //Show changed message
                    Popup_Show_Status("Play", "Quick launch changed");

                    //Save changes to Json file
                    JsonSaveApplications();
                }
            }
            catch { }
        }

        //Change the interface sound pack
        async void Button_Settings_InterfaceSoundPackName(object sender, RoutedEventArgs e)
        {
            try
            {
                //Add sound packs to string list
                List<DataBindString> Answers = new List<DataBindString>();
                DirectoryInfo directoryInfo = new DirectoryInfo("Assets\\Sounds\\");
                DirectoryInfo[] soundPacks = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                BitmapImage imagePacks = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/VolumeUp.png" }, IntPtr.Zero, -1, 0);

                foreach (DirectoryInfo soundPack in soundPacks)
                {
                    DataBindString AnswerCustom = new DataBindString();
                    AnswerCustom.ImageBitmap = imagePacks;
                    AnswerCustom.Name = soundPack.Name;
                    Answers.Add(AnswerCustom);
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("Interface Sounds", "", "Please select a sound pack to use:", Answers);
                if (messageResult != null)
                {
                    //Show changed message
                    Popup_Show_Status("VolumeUp", "Sound pack changed");

                    //Update the setting
                    SettingSave("InterfaceSoundPackName", messageResult.Name);
                }
            }
            catch { }
        }

        //Launch DirectXInput application
        void Button_LaunchDirectXInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LaunchDirectXInput();
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

        //Show the color picker popup
        async void Button_Settings_ColorPickerAccent(object sender, RoutedEventArgs args)
        {
            try
            {
                await Popup_ShowHide_ColorPicker(false);
            }
            catch { }
        }

        //Create geforce experience shortcut
        async void Button_Settings_AddGeforceExperience_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                //Set application shortcut paths
                string TargetFilePath = Assembly.GetEntryAssembly().CodeBase.Replace(".exe", "-Admin.exe");
                string TargetName = Assembly.GetEntryAssembly().GetName().Name;
                string TargetFileShortcut = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\" + TargetName + ".url";
                string TargetFileBoxArtFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\StreamingAssets\\" + TargetName + "\\box-art.png";
                string TargetFileBoxArtDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\StreamingAssets\\" + TargetName;
                string TargetDirectoryStreamingAssets = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NVIDIA Corporation\\Shield Apps\\StreamingAssets\\";

                //Check if the Streaming Assets folder exists
                Directory_Create(TargetDirectoryStreamingAssets, false);
                Directory_Create(TargetFileBoxArtDirectory, false);

                //Check if the shortcut already exists
                if (!File.Exists(TargetFileShortcut))
                {
                    Debug.WriteLine("Adding application to GeForce Experience");

                    using (StreamWriter StreamWriter = new StreamWriter(TargetFileShortcut))
                    {
                        StreamWriter.WriteLine("[InternetShortcut]");
                        StreamWriter.WriteLine("URL=" + TargetFilePath);
                        StreamWriter.WriteLine("IconFile=" + TargetFilePath.Replace("file:///", ""));
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }

                    //Copy art box to the Streaming Assets directory
                    File_Copy("Assets\\BoxArt.png", TargetFileBoxArtFile, true);

                    btn_Settings_AddGeforceExperience.Content = "Remove CtrlUI from GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("CtrlUI has been added to GeForce Experience", "", "You can now remotely launch CtrlUI from your devices.", Answers);
                }
                else
                {
                    Debug.WriteLine("Removing application from GeForce Experience");

                    File_Delete(TargetFileShortcut);
                    Directory_Delete(TargetFileBoxArtDirectory);

                    btn_Settings_AddGeforceExperience.Content = "Add CtrlUI to GeForce Experience";

                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("CtrlUI has been removed from GeForce Experience", "", "", Answers);
                }
            }
            catch (Exception ex)
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                Answer1.Name = "Alright";
                Answers.Add(Answer1);

                Debug.WriteLine("Failed add GeForce Experience: " + ex.Message);
                await Popup_Show_MessageBox("Failed to add CtrlUI to GeForce Experience", "", "Please make sure that GeForce experience is installed.", Answers);
            }
        }
    }
}