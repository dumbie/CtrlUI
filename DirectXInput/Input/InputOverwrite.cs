using System.Linq;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Check and overwrite controller button presses
        private static void CheckControllerButtonOverwrite(ControllerStatus controller)
        {
            try
            {
                //Check if alt tab is active and buttons need to be blocked
                if (vAltTabDownStatus)
                {
                    ShortcutTriggerController shortcutTrigger = vShortcutsController.FirstOrDefault(x => x.Name == "AltTab");
                    if (shortcutTrigger != null)
                    {
                        foreach (ControllerButtons button in shortcutTrigger.Trigger)
                        {
                            controller.InputCurrent.Buttons[(byte)button].PressedRaw = false;
                        }
                    }
                }

                //Check if guide button is exclusive and needs to be blocked
                //Fix HasInputOnDemand button press time
                if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Guide].PressedRaw && SettingLoad(vConfigurationDirectXInput, "ExclusiveGuide", typeof(bool)))
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.Guide].PressedRaw = false;
                }
            }
            catch { }
        }
    }
}