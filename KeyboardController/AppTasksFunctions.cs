using System.Threading.Tasks;

namespace KeyboardController
{
    public partial class WindowMain
    {
        async void vTaskAction_UpdateWindowStatus()
        {
            try
            {
                while (IsTaskRunning(vTaskToken_UpdateWindowStatus))
                {
                    await UpdateWindowStatus();
                    await Task.Delay(500);
                }
            }
            catch { }
        }
    }
}