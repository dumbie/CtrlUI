using ArnoldVinkCode;
using System.Threading;
using System.Threading.Tasks;

namespace KeyboardController
{
    public partial class WindowMain
    {
        public static Task vTask_UpdateWindowStatus = null;
        public static CancellationTokenSource vTaskToken_UpdateWindowStatus = null;

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                vTaskToken_UpdateWindowStatus = new CancellationTokenSource();
                vTask_UpdateWindowStatus = AVActions.TaskStart(vTaskAction_UpdateWindowStatus, vTaskToken_UpdateWindowStatus);
            }
            catch { }
        }

        //Stop all the background tasks
        public static void TasksBackgroundStop()
        {
            try
            {
                vTaskToken_UpdateWindowStatus.Cancel();
            }
            catch { }
        }
    }
}