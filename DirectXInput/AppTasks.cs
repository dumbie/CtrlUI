using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        public static AVTaskDetails vTask_UpdateWindowStatus = new AVTaskDetails("vTask_UpdateWindowStatus");
        public static AVTaskDetails vTask_ControllerMonitor = new AVTaskDetails("vTask_ControllerMonitor");
        public static AVTaskDetails vTask_ControllerTimeout = new AVTaskDetails("vTask_ControllerTimeout");
        public static AVTaskDetails vTask_ControllerLedColor = new AVTaskDetails("vTask_ControllerLedColor");
        public static AVTaskDetails vTask_ControllerBattery = new AVTaskDetails("vTask_ControllerBattery");
        public static AVTaskDetails vTask_ControllerInformation = new AVTaskDetails("vTask_ControllerInformation");

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                TaskStartLoop(vTaskLoop_UpdateWindowStatus, vTask_UpdateWindowStatus);
                TaskStartLoop(vTaskLoop_ControllerMonitor, vTask_ControllerMonitor);
                TaskStartLoop(vTaskLoop_ControllerTimeout, vTask_ControllerTimeout);
                TaskStartLoop(vTaskLoop_ControllerLedColor, vTask_ControllerLedColor);
                TaskStartLoop(vTaskLoop_ControllerBattery, vTask_ControllerBattery);
                TaskStartLoop(vTaskLoop_ControllerInformation, vTask_ControllerInformation);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await TaskStopLoop(vTask_UpdateWindowStatus, 5000);
                await TaskStopLoop(vTask_ControllerMonitor, 5000);
                await TaskStopLoop(vTask_ControllerTimeout, 5000);
                await TaskStopLoop(vTask_ControllerLedColor, 5000);
                await TaskStopLoop(vTask_ControllerBattery, 5000);
                await TaskStopLoop(vTask_ControllerInformation, 5000);
            }
            catch { }
        }
    }
}