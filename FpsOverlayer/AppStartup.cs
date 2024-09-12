using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkCode.Styles.MainColors;
using static FpsOverlayer.AppBackup;
using static FpsOverlayer.AppHotkeys;
using static FpsOverlayer.AppVariables;
using static FpsOverlayer.SocketHandlers;
using static LibraryShared.AppUpdate;

namespace FpsOverlayer
{
    public class AppStartup
    {
        public async static Task Startup()
        {
            try
            {
                Debug.WriteLine("Welcome to application.");

                //Setup application defaults
                AVStartup.SetupDefaults(ProcessPriority.High, true);

                //Application update checks
                await UpdateCheck();

                //Check application settings
                vWindowSettings.Settings_Check();

                //Check application shortcuts
                vWindowSettings.Shortcuts_Check();

                //Change application accent color
                string colorLightHex = SettingLoad(vConfigurationCtrlUI, "ColorAccentLight", typeof(string));
                ChangeApplicationAccentColor(colorLightHex);

                //Backup Notes
                BackupNotes();

                //Show windows
                vWindowStats.Show();
                vWindowTools.Show();
                vWindowCrosshair.Show();

                //Register keyboard hotkeys
                AVInputOutputHotkey.Start();
                AVInputOutputHotkey.EventHotkeyPressedList += EventHotkeyPressed;

                //Enable the socket server
                await EnableSocketServer();
            }
            catch { }
        }

        //Enable the socket server
        private static async Task EnableSocketServer()
        {
            try
            {
                int socketServerPort = SettingLoad(vConfigurationCtrlUI, "ServerPort", typeof(int)) + 2;

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", socketServerPort, false, true);
                vArnoldVinkSockets.vSocketTimeout = 250;
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
                await vArnoldVinkSockets.SocketServerEnable();
            }
            catch { }
        }
    }
}