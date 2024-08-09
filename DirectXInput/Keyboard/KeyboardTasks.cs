using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Task variables
        private static AVTaskDetails vTask_UpdateMediaInformation = new AVTaskDetails("vTask_UpdateMediaInformation");
        private static AVTaskDetails vTask_UpdateInterfaceInformation = new AVTaskDetails("vTask_UpdateInterfaceInformation");

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                AVActions.TaskStartLoop(vTaskLoop_UpdateMediaInformation, vTask_UpdateMediaInformation);
                AVActions.TaskStartLoop(vTaskLoop_UpdateInterfaceInformation, vTask_UpdateInterfaceInformation);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await AVActions.TaskStopLoop(vTask_UpdateMediaInformation, 5000);
                await AVActions.TaskStopLoop(vTask_UpdateInterfaceInformation, 5000);
            }
            catch { }
        }

        async Task vTaskLoop_UpdateMediaInformation()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateMediaInformation, 500))
                {
                    UpdateCurrentVolumeInformation();
                    await UpdateCurrentMediaInformation();
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateInterfaceInformation()
        {
            try
            {
                while (await TaskCheckLoop(vTask_UpdateInterfaceInformation, 1000))
                {
                    UpdateClockTime();
                    UpdateBatteryStatus();
                    UpdateActiveController();
                }
            }
            catch { }
        }
    }
}