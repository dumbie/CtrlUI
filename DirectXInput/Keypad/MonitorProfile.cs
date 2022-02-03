using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;

namespace DirectXInput.KeypadCode
{
    partial class WindowKeypad
    {
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