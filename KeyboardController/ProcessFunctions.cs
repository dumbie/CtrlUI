using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessFunctions;
using static ArnoldVinkCode.ProcessWin32Functions;

namespace KeyboardController
{
    partial class WindowMain
    {
        //Launch DirectXInput application
        async Task LaunchDirectXInput()
        {
            try
            {
                if (!CheckRunningProcessByNameOrTitle("DirectXInput", false))
                {
                    await Notification_Send_Status("DirectXInput", "Launching DirectXInput");
                    Debug.WriteLine("Launching DirectXInput");

                    await ProcessLauncherWin32Async("DirectXInput-Admin.exe", "", "", true, false);
                }
            }
            catch { }
        }
    }
}