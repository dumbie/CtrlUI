using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        public static AVTaskDetails vTask_UpdateWindowStatus = new AVTaskDetails();
        public static AVTaskDetails vTask_ControllerMonitor = new AVTaskDetails();
        public static AVTaskDetails vTask_ControllerTimeout = new AVTaskDetails();
        public static AVTaskDetails vTask_ControllerBattery = new AVTaskDetails();

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                TaskStartLoop(vTaskLoop_UpdateWindowStatus, vTask_UpdateWindowStatus);
                TaskStartLoop(vTaskLoop_ControllerMonitor, vTask_ControllerMonitor);
                TaskStartLoop(vTaskLoop_ControllerTimeout, vTask_ControllerTimeout);
                TaskStartLoop(vTaskLoop_ControllerBattery, vTask_ControllerBattery);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await TaskStopLoop(vTask_UpdateWindowStatus);
                await TaskStopLoop(vTask_ControllerMonitor);
                await TaskStopLoop(vTask_ControllerTimeout);
                await TaskStopLoop(vTask_ControllerBattery);
            }
            catch { }
        }
    }
}