using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Task variables
        private static AVTaskDetails vTask_UpdateMediaInformation = new AVTaskDetails();
        private static AVTaskDetails vTask_UpdateInterfaceInformation = new AVTaskDetails();

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
                while (TaskCheckLoop(vTask_UpdateMediaInformation))
                {
                    UpdateCurrentVolumeInformation();
                    await UpdateCurrentMediaInformation();

                    //Delay the loop task
                    await TaskDelayLoop(500, vTask_UpdateMediaInformation);
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateInterfaceInformation()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateInterfaceInformation))
                {
                    UpdateClockTime();
                    UpdateBatteryStatus();
                    UpdateActiveController();

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_UpdateInterfaceInformation);
                }
            }
            catch { }
        }
    }
}