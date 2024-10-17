using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace CtrlUI
{
    public partial class WindowMain
    {
        public static AVTaskDetails vTask_UpdateClock = new AVTaskDetails("vTask_UpdateClock");
        public static AVTaskDetails vTask_UpdateWindowStatus = new AVTaskDetails("vTask_UpdateWindowStatus");
        public static AVTaskDetails vTask_ControllerConnected = new AVTaskDetails("vTask_ControllerConnected");
        public static AVTaskDetails vTask_UpdateGallery = new AVTaskDetails("vTask_UpdateGallery");
        public static AVTaskDetails vTask_UpdateProcesses = new AVTaskDetails("vTask_UpdateProcesses");
        public static AVTaskDetails vTask_UpdateLaunchers = new AVTaskDetails("vTask_UpdateLaunchers");
        public static AVTaskDetails vTask_UpdateShortcuts = new AVTaskDetails("vTask_UpdateShortcuts");
        public static AVTaskDetails vTask_UpdateListStatus = new AVTaskDetails("vTask_UpdateListStatus");
        public static AVTaskDetails vTask_UpdateAppRunningTime = new AVTaskDetails("vTask_UpdateAppRunningTime");
        public static AVTaskDetails vTask_UpdateMediaInformation = new AVTaskDetails("vTask_UpdateMediaInformation");

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                AVActions.TaskStartLoop(vTaskLoop_UpdateClock, vTask_UpdateClock);
                AVActions.TaskStartLoop(vTaskLoop_UpdateWindowStatus, vTask_UpdateWindowStatus);
                AVActions.TaskStartLoop(vTaskLoop_ControllerConnected, vTask_ControllerConnected);
                AVActions.TaskStartLoop(vTaskLoop_UpdateGallery, vTask_UpdateGallery);
                AVActions.TaskStartLoop(vTaskLoop_UpdateProcesses, vTask_UpdateProcesses);
                AVActions.TaskStartLoop(vTaskLoop_UpdateLaunchers, vTask_UpdateLaunchers);
                AVActions.TaskStartLoop(vTaskLoop_UpdateShortcuts, vTask_UpdateShortcuts);
                AVActions.TaskStartLoop(vTaskLoop_UpdateListStatus, vTask_UpdateListStatus);
                AVActions.TaskStartLoop(vTaskLoop_UpdateAppRunningTime, vTask_UpdateAppRunningTime);
                AVActions.TaskStartLoop(vTaskLoop_UpdateMediaInformation, vTask_UpdateMediaInformation);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await AVActions.TaskStopLoop(vTask_UpdateClock, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateWindowStatus, 5000);
                await AVActions.TaskStopLoop(vTask_ControllerConnected, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateGallery, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateProcesses, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateLaunchers, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateShortcuts, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateListStatus, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateAppRunningTime, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateMediaInformation, 5000);
            }
            catch { }
        }
    }
}