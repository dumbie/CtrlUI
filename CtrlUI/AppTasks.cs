using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace CtrlUI
{
    public partial class WindowMain
    {
        public static Task vTask_ControllerConnected = null;
        public static CancellationTokenSource vTaskToken_ControllerConnected = null;

        public static Task vTask_UpdateClock = null;
        public static CancellationTokenSource vTaskToken_UpdateClock = null;

        public static Task vTask_UpdateWindowStatus = null;
        public static CancellationTokenSource vTaskToken_UpdateWindowStatus = null;

        public static Task vTask_UpdateProcesses = null;
        public static CancellationTokenSource vTaskToken_UpdateProcesses = null;

        public static Task vTask_UpdateShortcuts = null;
        public static CancellationTokenSource vTaskToken_UpdateShortcuts = null;

        public static Task vTask_UpdateListStatus = null;
        public static CancellationTokenSource vTaskToken_UpdateListStatus = null;

        public static Task vTask_UpdateAppRunningTime = null;
        public static CancellationTokenSource vTaskToken_UpdateAppRunningTime = null;

        public static Task vTask_UpdateMediaInformation = null;
        public static CancellationTokenSource vTaskToken_UpdateMediaInformation = null;

        public static Task vTask_ShowHideMouse = null;
        public static CancellationTokenSource vTaskToken_ShowHideMouse = null;

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                vTaskToken_UpdateClock = new CancellationTokenSource();
                vTask_UpdateClock = AVActions.TaskStart(vTaskAction_UpdateClock, vTaskToken_UpdateClock);

                vTaskToken_UpdateWindowStatus = new CancellationTokenSource();
                vTask_UpdateWindowStatus = AVActions.TaskStart(vTaskAction_UpdateWindowStatus, vTaskToken_UpdateWindowStatus);

                vTaskToken_ControllerConnected = new CancellationTokenSource();
                vTask_ControllerConnected = AVActions.TaskStart(vTaskAction_ControllerConnected, vTaskToken_ControllerConnected);

                vTaskToken_UpdateProcesses = new CancellationTokenSource();
                vTask_UpdateProcesses = AVActions.TaskStart(vTaskAction_UpdateProcesses, vTaskToken_UpdateProcesses);

                vTaskToken_UpdateShortcuts = new CancellationTokenSource();
                vTask_UpdateShortcuts = AVActions.TaskStart(vTaskAction_UpdateShortcuts, vTaskToken_UpdateShortcuts);

                vTaskToken_UpdateListStatus = new CancellationTokenSource();
                vTask_UpdateListStatus = AVActions.TaskStart(vTaskAction_UpdateListStatus, vTaskToken_UpdateListStatus);

                vTaskToken_UpdateAppRunningTime = new CancellationTokenSource();
                vTask_UpdateAppRunningTime = AVActions.TaskStart(vTaskAction_UpdateAppRunningTime, vTaskToken_UpdateAppRunningTime);

                vTaskToken_UpdateMediaInformation = new CancellationTokenSource();
                vTask_UpdateMediaInformation = AVActions.TaskStart(vTaskAction_UpdateMediaInformation, vTaskToken_UpdateMediaInformation);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["HideMouseCursor"]))
                {
                    TaskStart_ShowHideMouseCursor();
                }
            }
            catch { }
        }

        //Stop all the background tasks
        public static void TasksBackgroundStop()
        {
            try
            {
                vTaskToken_ControllerConnected.Cancel();
                vTaskToken_UpdateClock.Cancel();
                vTaskToken_UpdateWindowStatus.Cancel();
                vTaskToken_UpdateProcesses.Cancel();
                vTaskToken_UpdateShortcuts.Cancel();
                vTaskToken_UpdateListStatus.Cancel();
                vTaskToken_UpdateAppRunningTime.Cancel();
                vTaskToken_UpdateMediaInformation.Cancel();
                vTaskToken_ShowHideMouse.Cancel();
            }
            catch { }
        }

        void TaskStart_ShowHideMouseCursor()
        {
            try
            {
                vTaskToken_ShowHideMouse = new CancellationTokenSource();
                vTask_ShowHideMouse = AVActions.TaskStart(vTaskAction_ShowHideMouse, vTaskToken_ShowHideMouse);
            }
            catch { }
        }
    }
}