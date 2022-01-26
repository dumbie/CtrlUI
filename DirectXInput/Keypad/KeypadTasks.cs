using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput.KeypadCode
{
    partial class WindowKeypad
    {
        //Task variables
        private static AVTaskDetails vTask_SwitchProfile = new AVTaskDetails();

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                AVActions.TaskStartLoop(vTaskLoop_SwitchProfile, vTask_SwitchProfile);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await AVActions.TaskStopLoop(vTask_SwitchProfile);
            }
            catch { }
        }

        async Task vTaskLoop_SwitchProfile()
        {
            try
            {
                while (TaskCheckLoop(vTask_SwitchProfile))
                {
                    //Switch keypad profile
                    await SwitchKeypadProfile();

                    //Delay the loop task
                    await TaskDelayLoop(1000, vTask_SwitchProfile);
                }
            }
            catch { }
        }
    }
}