using ArnoldVinkCode;
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

        public static Task vTask_UpdateAppRunningStatus = null;
        public static CancellationTokenSource vTaskToken_UpdateAppRunningStatus = null;

        public static Task vTask_UpdateApplications = null;
        public static CancellationTokenSource vTaskToken_UpdateApplications = null;

        public static Task vTask_UpdateAppRunningTime = null;
        public static CancellationTokenSource vTaskToken_UpdateAppRunningTime = null;

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

                vTaskToken_UpdateAppRunningStatus = new CancellationTokenSource();
                vTask_UpdateAppRunningStatus = AVActions.TaskStart(vTaskAction_UpdateAppRunningStatus, vTaskToken_UpdateAppRunningStatus);

                vTaskToken_UpdateAppRunningTime = new CancellationTokenSource();
                vTask_UpdateAppRunningTime = AVActions.TaskStart(vTaskAction_UpdateAppRunningTime, vTaskToken_UpdateAppRunningTime);

                vTaskToken_UpdateApplications = new CancellationTokenSource();
                vTask_UpdateApplications = AVActions.TaskStart(vTaskAction_UpdateApplications, vTaskToken_UpdateApplications);

                if (ConfigurationManager.AppSettings["HideMouseCursor"] == "True")
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
                vTaskToken_UpdateAppRunningStatus.Cancel();
                vTaskToken_UpdateApplications.Cancel();
                vTaskToken_UpdateAppRunningTime.Cancel();
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