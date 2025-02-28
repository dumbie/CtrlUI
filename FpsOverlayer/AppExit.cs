using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static FpsOverlayer.AppTasks;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer
{
    public class AppExit
    {
        public async static Task Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Stop monitoring hardware
                vHardwareComputer.Close();

                //Stop background tasks
                await TasksBackgroundStop();

                //Disable socket server
                if (vArnoldVinkSockets != null)
                {
                    await vArnoldVinkSockets.SocketServerDisable();
                }

                //Hide visible tray icon
                AppTray.TrayNotifyIcon.Visible = false;

                //Exit application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}