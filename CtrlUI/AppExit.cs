using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInteropDll;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    public partial class AppExit
    {
        //Application close prompt
        public static async Task Exit_Prompt()
        {
            try
            {
                //Show the closing messagebox
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerCloseCtrlUI = new DataBindString();
                AnswerCloseCtrlUI.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerCloseCtrlUI.Name = "Close CtrlUI";
                Answers.Add(AnswerCloseCtrlUI);

                DataBindString AnswerRestartCtrlUI = new DataBindString();
                AnswerRestartCtrlUI.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerRestartCtrlUI.Name = "Restart CtrlUI";
                Answers.Add(AnswerRestartCtrlUI);

                DataBindString AnswerShutdownPC = new DataBindString();
                AnswerShutdownPC.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Shutdown.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerShutdownPC.Name = "Shutdown my PC";
                Answers.Add(AnswerShutdownPC);

                DataBindString AnswerRestartPC = new DataBindString();
                AnswerRestartPC.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Restart.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerRestartPC.Name = "Restart my PC";
                Answers.Add(AnswerRestartPC);

                DataBindString AnswerLockPC = new DataBindString();
                AnswerLockPC.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Lock.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                AnswerLockPC.Name = "Lock my PC";
                Answers.Add(AnswerLockPC);

                DataBindString messageResult = await vWindowMain.Popup_Show_MessageBox("Would you like to close CtrlUI or shutdown your PC?", "If you have DirectXInput running and a controller connected you can launch CtrlUI by pressing on the 'Guide' button.", "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerCloseCtrlUI)
                    {
                        vWindowMain.Notification_Show_Status("AppClose", "Closing CtrlUI");
                        await Exit();
                    }
                    else if (messageResult == AnswerRestartCtrlUI)
                    {
                        vWindowMain.Notification_Show_Status("AppRestart", "Restarting CtrlUI");
                        await Restart();
                    }
                    else if (messageResult == AnswerRestartPC)
                    {
                        vWindowMain.Notification_Show_Status("Restart", "Restarting your PC");

                        //Restart the PC
                        AVProcess.Launch_ShellExecute(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/r /f /t 0", true);

                        //Close CtrlUI
                        await Exit();
                    }
                    else if (messageResult == AnswerShutdownPC)
                    {
                        vWindowMain.Notification_Show_Status("Shutdown", "Shutting down your PC");

                        //Shutdown the PC
                        AVProcess.Launch_ShellExecute(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/s /f /t 0", true);

                        //Close CtrlUI
                        await Exit();
                    }
                    else if (messageResult == AnswerLockPC)
                    {
                        vWindowMain.Notification_Show_Status("Lock", "Locking your PC");

                        //Lock the PC
                        LockWorkStation();
                    }
                }
            }
            catch { }
        }

        //Restart application
        public static async Task Restart()
        {
            try
            {
                AVProcess.Launch_ShellExecute("CtrlUI.exe", "", "-restart", true);
                await Exit();
            }
            catch { }
        }

        //Close application
        public static async Task Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Disable application window
                vWindowMain.AppWindowDisable("Closing CtrlUI, please wait.");

                //Stop the background tasks
                await WindowMain.TasksBackgroundStop();

                //Disable the socket server
                if (vArnoldVinkSockets != null)
                {
                    await vArnoldVinkSockets.SocketServerDisable();
                }

                //Close the application
                Environment.Exit(0);
            }
            catch { }
        }
    }
}