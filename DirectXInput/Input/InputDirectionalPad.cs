using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private static bool InputUpdateDirectionalPad(ControllerStatus controller)
        {
            try
            {
                //Set controller header offset
                int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                //Get left DPad state
                if (controller.SupportedCurrent.OffsetHeader.DPadLeft != null)
                {
                    int dPadLeftRaw = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.DPadLeft];
                    if (dPadLeftRaw == controller.SupportedCurrent.OffsetDPad.DPadN)
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = false;
                    }
                    else if (!controller.Details.Profile.DPadFourWayMovement && dPadLeftRaw == controller.SupportedCurrent.OffsetDPad.DPadNE)
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = false;
                    }
                    else if (dPadLeftRaw == controller.SupportedCurrent.OffsetDPad.DPadE)
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = false;
                    }
                    else if (!controller.Details.Profile.DPadFourWayMovement && dPadLeftRaw == controller.SupportedCurrent.OffsetDPad.DPadSE)
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = false;
                    }
                    else if (dPadLeftRaw == controller.SupportedCurrent.OffsetDPad.DPadS)
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = false;
                    }
                    else if (!controller.Details.Profile.DPadFourWayMovement && dPadLeftRaw == controller.SupportedCurrent.OffsetDPad.DPadSW)
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = true;
                    }
                    else if (dPadLeftRaw == controller.SupportedCurrent.OffsetDPad.DPadW)
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = true;
                    }
                    else if (!controller.Details.Profile.DPadFourWayMovement && dPadLeftRaw == controller.SupportedCurrent.OffsetDPad.DPadNW)
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = true;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = true;
                    }
                    else
                    {
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw = false;
                        controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw = false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update dpad input: " + ex.Message);
                return false;
            }
        }
    }
}