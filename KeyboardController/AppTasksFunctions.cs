using static ArnoldVinkCode.AVActions;

namespace KeyboardController
{
    public partial class WindowMain
    {
        async void vTaskLoop_UpdateWindowStatus()
        {
            try
            {
                while (vTask_UpdateWindowStatus.Status == AVTaskStatus.Running)
                {
                    UpdateWindowStatus();

                    //Delay the loop task
                    await TaskDelayLoop(500, vTask_UpdateWindowStatus);
                }
            }
            catch { }
            finally
            {
                vTask_UpdateWindowStatus.Status = AVTaskStatus.Stopped;
            }
        }
    }
}