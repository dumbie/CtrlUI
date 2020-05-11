using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                    btn_Settings_AppQuickLaunch_TextBlock.Text = "Change quick launch app: " + QuickLaunchApp.Name;
                }
                catch
                {
                    btn_Settings_AppQuickLaunch_TextBlock.Text = "Change the quick launch app";
                }

                //Set the available monitor amount
                try
                {
                    slider_SettingsDisplayMonitor.Maximum = Screen.AllScreens.Count();
                }
                catch { }

                //Show the settings popup
                await Popup_Show(grid_Popup_Settings, null);

                //Focus on settings tab
                await Listbox_Settings_SingleTap();
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

        //Change the settings tab
        async Task SettingsChangeTab(bool changeLeft)
        {
            try
            {
                if (changeLeft)
                {
                    int selectedIndex = Listbox_SettingsMenu.SelectedIndex;
                    if (selectedIndex > 0)
                    {
                        Listbox_SettingsMenu.SelectedIndex = Listbox_SettingsMenu.SelectedIndex - 1;
                        await Listbox_Settings_SingleTap();
                    }
                }
                else
                {
                    Listbox_SettingsMenu.SelectedIndex = Listbox_SettingsMenu.SelectedIndex + 1;
                    await Listbox_Settings_SingleTap();
                }
            }
            catch { }
        }
    }
}