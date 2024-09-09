using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace CtrlUI
{
    public partial class AppBackup
    {
        //Backup Json profiles
        public static void BackupJsonProfiles()
        {
            try
            {
                Debug.WriteLine("Creating Json profiles backup.");

                //Create backup directory
                AVFiles.Directory_Create("Backups", false);

                //Cleanup profile backups
                FileInfo[] fileInfo = new DirectoryInfo("Backups").GetFiles("*.zip");
                foreach (FileInfo backupFile in fileInfo)
                {
                    try
                    {
                        TimeSpan backupSpan = DateTime.Now - backupFile.CreationTime;
                        if (backupSpan.TotalDays > 5)
                        {
                            backupFile.Delete();
                        }
                    }
                    catch { }
                }

                //Create profile backup
                string backupTime = DateTime.Now.ToString("yyyyMMddHHmmss") + "-Profiles.zip";
                ZipFile.CreateFromDirectory("Profiles\\User", "Backups\\" + backupTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed making profiles backup: " + ex.Message);
            }
        }
    }
}