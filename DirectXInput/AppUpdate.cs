using ArnoldVinkCode;
using System;
using System.Reflection;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessWin32Functions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check for available application update
        public async Task CheckForAppUpdate(bool Silent)
        {
            try
            {
                string ResCurrentVersion = await AVDownloader.DownloadStringAsync(5000, "DirectXInput", null, new Uri("http://download.arnoldvink.com/CtrlUI.zip-version.txt" + "?nc=" + Environment.TickCount));
                if (!string.IsNullOrWhiteSpace(ResCurrentVersion) && ResCurrentVersion != Assembly.GetEntryAssembly().FullName.Split('=')[1].Split(',')[0])
                {
                    int Result = await MessageBoxPopup("A newer version has been found: v" + ResCurrentVersion, "Do you want to update the application to the newest version now?", "Update now", "Cancel", "", "");
                    if (Result == 1)
                    {
                        await ProcessLauncherWin32Async("Updater.exe", "", "", false, false);
                        await Application_Exit(true);
                    }
                }
                else
                {
                    if (!Silent)
                    {
                        await MessageBoxPopup("No new application update has been found.", "", "Ok", "", "", "");
                    }
                }
            }
            catch
            {
                if (!Silent)
                {
                    await MessageBoxPopup("Failed to check for the latest application version", "Please check your internet connection and try again.", "Ok", "", "", "");
                }
            }
        }
    }
}