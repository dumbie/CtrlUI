using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        void RemoveStartupShortcut(string shortcutName)
        {
            try
            {
                string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                DirectoryInfo shortcutDirectory = new DirectoryInfo(shortcutPath);
                FileInfo[] shortcutFiles = shortcutDirectory.GetFiles("*.url", SearchOption.AllDirectories);
                foreach (FileInfo shortcutFile in shortcutFiles)
                {
                    try
                    {
                        if (shortcutFile.Name.ToLower() == shortcutName.ToLower())
                        {
                            AVFiles.File_Delete(shortcutFile.FullName);
                            Debug.WriteLine("Removed startup shortcut: " + shortcutFile.FullName);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove startup shortcut: " + ex.Message);
            }
        }
    }
}