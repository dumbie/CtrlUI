using System.Collections.Generic;
using System.Threading.Tasks;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task LoadLauncherApplications()
        {
            try
            {
                //Scan and add library from Steam
                await SteamScanAddLibrary(SteamLibraryPaths());
            }
            catch { }
        }
    }
}