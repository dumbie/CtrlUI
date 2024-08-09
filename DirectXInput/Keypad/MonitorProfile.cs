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
                while (await TaskCheckLoop(vTask_SwitchProfile, 1000))
                {
                    //Switch keypad profile
                    await SwitchKeypadProfile();
                }
            }
            catch { }
        }
    }
}