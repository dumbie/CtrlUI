using System;
using System.Diagnostics;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private static bool InputUpdateDirectionalPad(ControllerStatus controller)
        {
            try
            {
                if (controller.SupportedCurrent.OffsetButton.DPadLeft != null)
                {
                    //Set controller header offset
                    int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                    //Get DPad state
                    bool DPadStateLeft = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.DPadLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.DPadLeft.Offset)) != 0;
                    bool DPadStateRight = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.DPadRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.DPadRight.Offset)) != 0;
                    bool DPadStateUp = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.DPadUp.Group] & (1 << controller.SupportedCurrent.OffsetButton.DPadUp.Offset)) != 0;
                    bool DPadStateDown = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.DPadDown.Group] & (1 << controller.SupportedCurrent.OffsetButton.DPadDown.Offset)) != 0;
                    int DPadStateAll = (DPadStateLeft ? 1 : 0) << 0 | (DPadStateRight ? 1 : 0) << 1 | (DPadStateUp ? 1 : 0) << 2 | (DPadStateDown ? 1 : 0) << 3;

                    //Check DPad type
                    if (controller.SupportedCurrent.HasDirectDPad)
                    {
                        if (controller.Details.Profile.DPadFourWayMovement)
                        {
                            switch (DPadStateAll)
                            {
                                case 1: controller.InputCurrent.DPadUp.PressedRaw = true; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 2: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 4: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = true; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 8: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = true; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                default: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                            }
                        }
                        else
                        {
                            switch (DPadStateAll)
                            {
                                case 1: controller.InputCurrent.DPadUp.PressedRaw = true; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 2: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 3: controller.InputCurrent.DPadUp.PressedRaw = true; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 4: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = true; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 6: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = true; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 8: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = true; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 9: controller.InputCurrent.DPadUp.PressedRaw = true; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = true; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 12: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = true; controller.InputCurrent.DPadLeft.PressedRaw = true; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                default: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                            }
                        }
                    }
                    else
                    {
                        if (controller.Details.Profile.DPadFourWayMovement)
                        {
                            switch (DPadStateAll)
                            {
                                case 0: controller.InputCurrent.DPadUp.PressedRaw = true; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 2: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 4: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = true; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 6: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = true; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                default: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                            }
                        }
                        else
                        {
                            switch (DPadStateAll)
                            {
                                case 0: controller.InputCurrent.DPadUp.PressedRaw = true; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 1: controller.InputCurrent.DPadUp.PressedRaw = true; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 2: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 3: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = true; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 4: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = true; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 5: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = true; controller.InputCurrent.DPadLeft.PressedRaw = true; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 6: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = true; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 7: controller.InputCurrent.DPadUp.PressedRaw = true; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = true; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                default: controller.InputCurrent.DPadUp.PressedRaw = false; controller.InputCurrent.DPadDown.PressedRaw = false; controller.InputCurrent.DPadLeft.PressedRaw = false; controller.InputCurrent.DPadRight.PressedRaw = false; break;
                            }
                        }
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