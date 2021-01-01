using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using static ArnoldVinkCode.AVFiles;

namespace FpsOverlayer
{
    public partial class WindowSettings
    {
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