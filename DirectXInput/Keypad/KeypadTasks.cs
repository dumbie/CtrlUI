using ArnoldVinkCode;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput.KeypadCode
{
    partial class WindowKeypad
    {
        //Task variables
        private static AVTaskDetails vTask_SwitchProfile = new AVTaskDetails();
        private static AVTaskDetails vTask_MonitorTaskbar = new AVTaskDetails();

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                TaskStartLoop(vTaskLoop_SwitchProfile, vTask_SwitchProfile);
                TaskStartLoop(vTaskLoop_MonitorTaskbar, vTask_MonitorTaskbar);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await TaskStopLoop(vTask_SwitchProfile);
                await TaskStopLoop(vTask_MonitorTaskbar);
            }
            catch { }
        }
    }
}