using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVFiles;
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

        //Create startup shortcut
        void ManageShortcutStartup()
        {
            try
            {
                //Set application shortcut paths
                string targetFilePath = Assembly.GetEntryAssembly().CodeBase.Replace(".exe", "-Admin.exe");
                string targetName = Assembly.GetEntryAssembly().GetName().Name;
                string targetFileShortcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), targetName + ".url");

                //Check if the shortcut already exists
                if (!File.Exists(targetFileShortcut))
                {
                    Debug.WriteLine("Adding application to Windows startup.");
                    using (StreamWriter StreamWriter = new StreamWriter(targetFileShortcut))
                    {
                        StreamWriter.WriteLine("[InternetShortcut]");
                        StreamWriter.WriteLine("URL=" + targetFilePath);
                        StreamWriter.WriteLine("IconFile=" + targetFilePath.Replace("file:///", ""));
                        StreamWriter.WriteLine("IconIndex=0");
                        StreamWriter.Flush();
                    }
                }
                else
                {
                    Debug.WriteLine("Removing application from Windows startup.");
                    File_Delete(targetFileShortcut);
                }
            }
            catch
            {
                Debug.WriteLine("Failed creating startup shortcut.");
            }
        }
    }
}