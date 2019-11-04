using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace KeyboardController
{
    public partial class WindowMain
    {
        async void vTaskAction_UpdateWindowStatus()
        {
            try
            {
                while (TaskRunningCheck(vTaskToken_UpdateWindowStatus))
                {
                    await UpdateWindowStatus();
                    await Task.Delay(500);
                }
            }
            catch { }
        }
    }
}