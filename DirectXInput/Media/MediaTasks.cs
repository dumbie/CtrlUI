using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput.MediaCode
{
    partial class WindowMedia
    {
        //Task variables
        private static AVTaskDetails vTask_UpdateMediaInformation = new AVTaskDetails();
        private static AVTaskDetails vTask_UpdateTimeBatteryInformation = new AVTaskDetails();
        private static AVTaskDetails vTask_UpdateWindowStyle = new AVTaskDetails();

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                AVActions.TaskStartLoop(vTaskLoop_UpdateMediaInformation, vTask_UpdateMediaInformation);
                AVActions.TaskStartLoop(vTaskLoop_UpdateTimeBatteryInformation, vTask_UpdateTimeBatteryInformation);
                AVActions.TaskStartLoop(vTaskLoop_UpdateWindowStyle, vTask_UpdateWindowStyle);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await AVActions.TaskStopLoop(vTask_UpdateMediaInformation);
                await AVActions.TaskStopLoop(vTask_UpdateTimeBatteryInformation);
                await AVActions.TaskStopLoop(vTask_UpdateWindowStyle);
            }
            catch { }
        }

        async Task vTaskLoop_UpdateMediaInformation()
        {
            try
            {
                while (!vTask_UpdateMediaInformation.TaskStopRequest)
                {
                    await UpdateCurrentMediaInformation();

                    //Delay the loop task
                    await TaskDelayLoop(250, vTask_UpdateMediaInformation);
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateTimeBatteryInformation()
        {
            try
            {
                while (!vTask_UpdateTimeBatteryInformation.TaskStopRequest)
                {
                    UpdateClockTime();
                    UpdateBatteryStatus();
                    UpdateTriggerRumbleButton();

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_UpdateTimeBatteryInformation);
                }
            }
            catch { }
        }

        async Task vTaskLoop_UpdateWindowStyle()
        {
            try
            {
                while (!vTask_UpdateWindowStyle.TaskStopRequest)
                {
                    //Update the window style
                    if (vWindowVisible)
                    {
                        await UpdateWindowStyleVisible();
                    }

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_UpdateWindowStyle);
                }
            }
            catch { }
        }
    }
}