using Shell32;
using System.Diagnostics;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task<bool> FilePicker_EjectDrive(DataBindFile dataBindFile, string driveLetter)
        {
            try
            {
                Debug.WriteLine("Ejecting the disc drive: " + driveLetter);
                Notification_Show_Status("FolderDisc", "Ejecting the drive");

                //Get the drive
                int ssfDRIVES = 17;
                Shell shell = new Shell();
                Folder folder = shell.NameSpace(ssfDRIVES);
                FolderItem folderItem = folder.ParseName(driveLetter);

                //Eject the disc or image
                folderItem.InvokeVerb("Eject");

                //Remove drive from the listbox
                await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, dataBindFile, true);

                Notification_Show_Status("FolderDisc", "Ejected the drive");
                return true;
            }
            catch { }
            Notification_Show_Status("Close", "Failed to eject drive");
            return false;
        }
    }
}