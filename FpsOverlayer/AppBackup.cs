using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace FpsOverlayer
{
    public partial class AppBackup
    {
        //Backup Notes
        public static void BackupNotes()
        {
            try
            {
                Debug.WriteLine("Creating notes backup.");

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
                string backupTime = DateTime.Now.ToString("yyyyMMddHHmmss") + "-Notes.zip";
                ZipFile.CreateFromDirectory("Notes", "Backups\\" + backupTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed making notes backup: " + ex.Message);
            }
        }
    }
}