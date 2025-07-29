using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    public partial class WindowMain
    {
        //Application close prompt
        public async Task Exit_Prompt()
        {
            try
            {
                //Show the closing messagebox
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerCloseCtrlUI = new DataBindString();
                AnswerCloseCtrlUI.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/AppClose.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerCloseCtrlUI.Name = "Close CtrlUI";
                Answers.Add(AnswerCloseCtrlUI);

                DataBindString AnswerRestartCtrlUI = new DataBindString();
                AnswerRestartCtrlUI.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/AppRestart.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerRestartCtrlUI.Name = "Restart CtrlUI";
                Answers.Add(AnswerRestartCtrlUI);

                DataBindString AnswerShutdownPC = new DataBindString();
                AnswerShutdownPC.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Shutdown.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerShutdownPC.Name = "Shutdown my PC";
                Answers.Add(AnswerShutdownPC);

                DataBindString AnswerRestartPC = new DataBindString();
                AnswerRestartPC.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Restart.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerRestartPC.Name = "Restart my PC";
                Answers.Add(AnswerRestartPC);

                DataBindString AnswerLockPC = new DataBindString();
                AnswerLockPC.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Lock.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerLockPC.Name = "Lock my PC";
                Answers.Add(AnswerLockPC);

                DataBindString messageResult = await Popup_Show_MessageBox("Would you like to close CtrlUI or shutdown your PC?", "If you have DirectXInput running and a controller connected you can launch CtrlUI by pressing on the 'Guide' button.", "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerCloseCtrlUI)
                    {
                        await Notification_Show_Status("AppClose", "Closing CtrlUI");
                        await Exit();
                    }
                    else if (messageResult == AnswerRestartCtrlUI)
                    {
                        await Notification_Show_Status("AppRestart", "Restarting CtrlUI");
                        await Restart();
                    }
                    else if (messageResult == AnswerRestartPC)
                    {
                        await Notification_Show_Status("Restart", "Restarting your PC");

                        //Restart the PC
                        AVProcess.Launch_ShellExecute(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/r /f /t 0", true);

                        //Close CtrlUI
                        await Exit();
                    }
                    else if (messageResult == AnswerShutdownPC)
                    {
                        await Notification_Show_Status("Shutdown", "Shutting down your PC");

                        //Shutdown the PC
                        AVProcess.Launch_ShellExecute(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/s /f /t 0", true);

                        //Close CtrlUI
                        await Exit();
                    }
                    else if (messageResult == AnswerLockPC)
                    {
                        await Notification_Show_Status("Lock", "Locking your PC");

                        //Lock the PC
                        LockWorkStation();
                    }
                }
            }
            catch { }
        }

        //Restart application
        public async Task Restart()
        {
            try
            {
                AVProcess.Launch_ShellExecute("CtrlUI.exe", "", "-restart", true);
                await Exit();
            }
            catch { }
        }

        //Close application
        public async Task Exit()
        {
            try
            {
                Debug.WriteLine("Exiting application.");

                //Disable application window
                AppWindowDisable("Closing CtrlUI, please wait.");

                //Stop the background tasks
                await TasksBackgroundStop();

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