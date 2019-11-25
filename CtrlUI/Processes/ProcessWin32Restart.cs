using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessWin32Functions;
using static ArnoldVinkCode.ProcessWin32StoreFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RestartPrepareWin32Store(DataBindApp dataBindApp)
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
                Debug.WriteLine("Restarting Win32Store application: " + dataBindApp.Name + " / " + dataBindApp.ProcessMulti.Identifier + " / " + dataBindApp.ProcessMulti.WindowHandle);

                await RestartProcessWin32Store(dataBindApp.ProcessMulti.Identifier, dataBindApp.NameExe, dataBindApp.PathExe, dataBindApp.ProcessMulti.Argument);
            }
            catch { }
        }

        async Task RestartPrepareWin32(DataBindApp dataBindApp)
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
                Debug.WriteLine("Restarting Win32 application: " + dataBindApp.Name + " / " + dataBindApp.ProcessMulti.Identifier + " / " + dataBindApp.ProcessMulti.WindowHandle);


                await RestartProcessWin32(dataBindApp.ProcessMulti.Identifier, dataBindApp.PathExe, dataBindApp.PathLaunch, dataBindApp.ProcessMulti.Argument);
            }
            catch { }
        }
    }
}