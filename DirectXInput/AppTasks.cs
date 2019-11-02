using ArnoldVinkCode;
using System.Threading;
using System.Threading.Tasks;

namespace DirectXInput
{
    public partial class WindowMain
    {
        public static bool IsTaskRunning(CancellationTokenSource TaskToken) { return TaskToken != null && !TaskToken.IsCancellationRequested; }

        public static Task vTask_UpdateWindowStatus = null;
        public static CancellationTokenSource vTaskToken_UpdateWindowStatus = null;

        public static Task vTask_ControllerMonitor = null;
        public static CancellationTokenSource vTaskToken_ControllerMonitor = null;

        public static Task vTask_ControllerTimeout = null;
        public static CancellationTokenSource vTaskToken_ControllerTimeout = null;

        public static Task vTask_ControllerBattery = null;
        public static CancellationTokenSource vTaskToken_ControllerBattery = null;

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                vTaskToken_UpdateWindowStatus = new CancellationTokenSource();
                vTask_UpdateWindowStatus = AVActions.TaskStart(vTaskAction_UpdateWindowStatus, vTaskToken_UpdateWindowStatus);

                vTaskToken_ControllerMonitor = new CancellationTokenSource();
                vTask_ControllerMonitor = AVActions.TaskStart(vTaskAction_ControllerMonitor, vTaskToken_ControllerMonitor);

                vTaskToken_ControllerTimeout = new CancellationTokenSource();
                vTask_ControllerTimeout = AVActions.TaskStart(vTaskAction_ControllerTimeout, vTaskToken_ControllerTimeout);

                vTaskToken_ControllerBattery = new CancellationTokenSource();
                vTask_ControllerBattery = AVActions.TaskStart(vTaskAction_ControllerBattery, vTaskToken_ControllerBattery);
            }
            catch { }
        }

        //Stop all the background tasks
        public static void TasksBackgroundStop()
        {
            try
            {
                vTaskToken_UpdateWindowStatus.Cancel();
                vTaskToken_ControllerMonitor.Cancel();
                vTaskToken_ControllerTimeout.Cancel();
                vTaskToken_ControllerBattery.Cancel();
            }
            catch { }
        }
    }
}