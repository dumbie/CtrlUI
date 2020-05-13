using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace KeyboardController
{
    public partial class WindowMain
    {
        public static AVTaskDetails vTask_UpdateWindowStatus = new AVTaskDetails();

        //Start all the background tasks
        void TasksBackgroundStart()
        {
            try
            {
                TaskStartLoop(vTaskLoop_UpdateWindowStatus, vTask_UpdateWindowStatus);
            }
            catch { }
        }

        //Stop all the background tasks
        public static async Task TasksBackgroundStop()
        {
            try
            {
                await TaskStopLoop(vTask_UpdateWindowStatus);
            }
            catch { }
        }
    }
}