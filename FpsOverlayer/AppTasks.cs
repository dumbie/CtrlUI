using System.Threading;
using System.Threading.Tasks;

namespace FpsOverlayer
{
    public class AppTasks
    {
        public static bool IsTaskRunning(CancellationTokenSource TaskToken) { return TaskToken != null && !TaskToken.IsCancellationRequested; }

        public static Task vTask_TraceEventProcess = null;
        public static CancellationTokenSource vTaskToken_TraceEventProcess = null;

        public static Task vTask_TraceEventOutput = null;
        public static CancellationTokenSource vTaskToken_TraceEventOutput = null;

        public static Task vTask_MonitorHardware = null;
        public static CancellationTokenSource vTaskToken_MonitorHardware = null;

        public static Task vTask_MonitorProcess = null;
        public static CancellationTokenSource vTaskToken_MonitorProcess = null;

        //Stop all the background tasks
        public static void TasksBackgroundStop()
        {
            try
            {
                vTaskToken_TraceEventProcess.Cancel();
                vTaskToken_TraceEventOutput.Cancel();
                vTaskToken_MonitorHardware.Cancel();
                vTaskToken_MonitorProcess.Cancel();
            }
            catch { }
        }
    }
}