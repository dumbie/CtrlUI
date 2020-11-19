using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput.MediaCode
{
    partial class WindowMedia
    {
        private static AVTaskDetails vTask_UpdateMediaInformation = new AVTaskDetails();

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
    }
}