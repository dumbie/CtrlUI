using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace KeyboardController
{
    public partial class WindowMain
    {
        async Task vTaskLoop_UpdateWindowStatus()
        {
            try
            {
                while (!vTask_UpdateWindowStatus.TaskStopRequest)
                {
                    UpdateWindowStatus();

                    //Delay the loop task
                    await TaskDelayLoop(500, vTask_UpdateWindowStatus);
                }
            }
            catch { }
        }
    }
}