using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RestartPrepareUwp(ProcessMulti processMulti, DataBindApp dataBindApp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataBindApp.PathExe))
                {
                    Popup_Show_Status("Close", "Failed restarting " + dataBindApp.Name);
                    Debug.WriteLine("Failed to restart process: " + dataBindApp.Name);
                    return;
                }

                Popup_Show_Status("Switch", "Restarting " + dataBindApp.Name);
                Debug.WriteLine("Restarting UWP application: " + dataBindApp.Name + " / " + processMulti.ProcessId + " / " + processMulti.WindowHandle);

                await RestartProcessUwp(dataBindApp.Name, dataBindApp.PathExe, dataBindApp.Argument, dataBindApp.ProcessId, dataBindApp.WindowHandle);
            }
            catch { }
        }
    }
}