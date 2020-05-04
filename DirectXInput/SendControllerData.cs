using System;
using System.Configuration;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Send the prepared controller data
        async Task SendControllerData(ControllerStatus Controller)
        {
            try
            {
                //Update the button press times
                UpdateButtonPressTimes(Controller.InputCurrent.ButtonGuide);
                CheckButtonPressTimes(Controller.InputCurrent.ButtonGuide);

                //Update interface controller preview
                ControllerPreview(Controller);

                //Check if CtrlUI or Keyboard controller is running
                bool BlockOutputApplication = false;
                if (vProcessKeyboardController != null)
                {
                    if (Controller.Manage)
                    {
                        await OutputAppKeyboardController(Controller);
                    }
                    BlockOutputApplication = true;
                }
                else if (vProcessCtrlUI != null && vProcessCtrlUIActivated)
                {
                    if (Controller.Manage)
                    {
                        await OutputAppCtrlUI(Controller);
                    }
                    BlockOutputApplication = true;
                }

                //Check if controller shortcut is pressed
                bool BlockOutputShortcut = await ControllerShortcut(Controller);

                //Check if output or guide button needs to be blocked
                if (BlockOutputApplication || BlockOutputShortcut)
                {
                    //Prepare empty XOutput device data
                    PrepareXInputData(Controller, true);
                }
                else
                {
                    //Check if guide button is CtrlUI exclusive
                    if (Controller.InputCurrent.ButtonGuide.PressedRaw && Convert.ToBoolean(ConfigurationManager.AppSettings["ExclusiveGuide"]))
                    {
                        Controller.InputCurrent.ButtonGuide.PressedRaw = false;
                    }

                    //Prepare current XOutput device data
                    PrepareXInputData(Controller, false);
                }

                //Send XOutput device data
                Controller.X360Device.Send(Controller.XInputData, Controller.XOutputData);

                //Send XInput device data
                SendXRumbleData(Controller, false, false, false);
            }
            catch { }
        }
    }
}