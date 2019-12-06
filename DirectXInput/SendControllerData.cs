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
                bool BlockOutput = false;

                //Update the button press times
                UpdateButtonPressTimes(Controller);
                CheckButtonPressTimeGuide(Controller);

                //Check if keyboard controller is running
                if (vProcessKeyboardController != null)
                {
                    if (Controller.Manage)
                    {
                        await OutputAppKeyboardController(Controller);
                    }
                    BlockOutput = true;
                }
                //Check if CtrlUI is running
                else if (vProcessCtrlUI != null)
                {
                    if (Controller.Manage)
                    {
                        await OutputAppCtrlUI(Controller);
                    }
                    if (vProcessCtrlUIActivated)
                    {
                        BlockOutput = true;
                    }
                }

                //Check if output or guide button needs to be blocked
                if (BlockOutput)
                {
                    //Handle empty XOutput device data
                    PrepareXInputData(Controller, true);
                }
                else
                {
                    //Check if guide button is CtrlUI exclusive
                    if (Controller.InputCurrent.ButtonGuide && Convert.ToBoolean(ConfigurationManager.AppSettings["ExclusiveGuide"]))
                    {
                        Controller.InputCurrent.ButtonGuide = false;
                    }

                    //Handle current XOutput device data
                    PrepareXInputData(Controller, false);
                }

                //Handle XOutput device data
                Controller.X360Device.Report(Controller.XInputData, Controller.XOutputData);

                //Handle XInput device data
                SendXRumbleData(Controller, false, false, false);

                //Update interface controller preview
                ControllerPreview(Controller);

                //Check if controller shortcut is pressed
                await ControllerShortcut(Controller);
            }
            catch { }
        }
    }
}