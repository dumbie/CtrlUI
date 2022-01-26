using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Task variables
        private static AVTaskDetails vTask_UpdateWindowStyle = new AVTaskDetails();

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                AVActions.TaskStartLoop(vTaskLoop_UpdateWindowStyle, vTask_UpdateWindowStyle);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await AVActions.TaskStopLoop(vTask_UpdateWindowStyle);
            }
            catch { }
        }
        async Task vTaskLoop_UpdateWindowStyle()
        {
            try
            {
                while (TaskCheckLoop(vTask_UpdateWindowStyle))
                {
                    //Update the window style
                    if (vWindowVisible)
                    {
                        await UpdateWindowStyleVisible();
                    }

                    //Delay the loop task
                    await TaskDelayLoop(100, vTask_UpdateWindowStyle);
                }
            }
            catch { }
        }
    }
}