using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVUwpAppx;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check if databind paths are available
        async Task<bool> CheckDatabindPathAuto(DataBindApp dataBindApp)
        {
            try
            {
                if (dataBindApp.Category == AppCategory.Emulator && !dataBindApp.LaunchSkipRom)
                {
                    //Check if the rom folder exists
                    if (!Directory.Exists(dataBindApp.PathRoms))
                    {
                        await Notification_Send_Status("Close", "Rom folder not found");
                        Debug.WriteLine("Rom folder not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }

                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    //Check if the application exists
                    if (GetUwpAppPackageByAppUserModelId(dataBindApp.AppUserModelId) == null)
                    {
                        await Notification_Send_Status("Close", "Application not found");
                        Debug.WriteLine("Launch application not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }
                else
                {
                    //Check if application executable exists
                    if (!File.Exists(dataBindApp.PathExe))
                    {
                        await Notification_Send_Status("Close", "Executable not found");
                        Debug.WriteLine("Launch executable not found.");
                        dataBindApp.StatusAvailable = Visibility.Visible;
                        return false;
                    }
                }

                //Paths are available update status
                dataBindApp.StatusAvailable = Visibility.Collapsed;
            }
            catch { }
            return true;
        }
    }
}