using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        async Task<bool> CheckControllerBlockInteraction(ControllerStatus controller)
        {
            try
            {
                //Ignore controller timeout
                controller.TimeoutIgnore = true;

                //Check if controller is currently disconnecting
                if (controller.Disconnecting)
                {
                    return true;
                }

                //Check if controller output needs to be blocked
                if (vAppActivated && (vShowControllerDebug || vShowControllerPreview))
                {
                    return true;
                }

                //Check if controller shortcut is pressed
                if (await ControllerShortcut(controller))
                {
                    return true;
                }

                //Check if controller output needs to be forwarded
                if (await ControllerOutputForward(controller))
                {
                    return true;
                }

                //Return result
                return false;
            }
            catch
            {
                //Return result
                return false;
            }
            finally
            {
                //Allow controller timeout 
                controller.TimeoutIgnore = false;
            }
        }
    }
}