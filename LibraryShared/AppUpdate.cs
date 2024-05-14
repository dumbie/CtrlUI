using ArnoldVinkCode;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVFiles;

namespace LibraryShared
{
    public partial class AppUpdate
    {
        public static async Task UpdateCheck()
        {
            try
            {
                Debug.WriteLine("Checking application update.");

                //Close running application updater
                if (AVProcess.Close_ProcessesByName("Updater.exe", true))
                {
                    await Task.Delay(1000);
                }

                //Check if the updater has been updated
                File_Move("Resources/UpdaterReplace.exe", "Updater.exe", true);
                File_Move("Updater/UpdaterReplace.exe", "Updater.exe", true);
            }
            catch { }
        }
    }
}