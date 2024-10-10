using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private static bool InputUpdateTriggers(ControllerStatus controller)
        {
            try
            {
                //Set controller header offset
                int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                //Read analog triggers
                if (!controller.Details.Profile.UseButtonTriggers && controller.SupportedCurrent.OffsetHeader.TriggerLeft != null)
                {
                    //Check Microsoft Xbox controller
                    int triggerLeftBytes = 0;
                    if (controller.SupportedCurrent.CodeName == "MicrosoftXboxOneS")
                    {
                        int triggerLeftRaw = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerLeft];
                        int triggerLeftRange = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerLeft + 1];
                        triggerLeftBytes = ((triggerLeftRange * 255) + triggerLeftRaw) / 4;
                    }
                    else
                    {
                        triggerLeftBytes = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerLeft];
                    }

                    //Check the triggers deadzone
                    if (controller.Details.Profile.DeadzoneTriggerLeft != 0)
                    {
                        int deadzoneRangeLeft = (255 * controller.Details.Profile.DeadzoneTriggerLeft) / 100;
                        if (triggerLeftBytes < deadzoneRangeLeft) { triggerLeftBytes = 0; }
                    }

                    //Calculate trigger sensitivity
                    if (controller.Details.Profile.SensitivityTriggerLeft != 1)
                    {
                        triggerLeftBytes = Convert.ToInt32(triggerLeftBytes * controller.Details.Profile.SensitivityTriggerLeft);
                    }

                    //Check the triggers range
                    if (triggerLeftBytes > 255) { triggerLeftBytes = 255; } else if (triggerLeftBytes < 0) { triggerLeftBytes = 0; }

                    //Store the triggers
                    controller.InputCurrent.TriggerLeft = Convert.ToByte(triggerLeftBytes);
                }

                if (!controller.Details.Profile.UseButtonTriggers && controller.SupportedCurrent.OffsetHeader.TriggerRight != null)
                {
                    //Check Microsoft Xbox controller
                    int triggerRightBytes = 0;
                    if (controller.SupportedCurrent.CodeName == "MicrosoftXboxOneS")
                    {
                        int triggerRightRaw = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerRight];
                        int triggerRightRange = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerRight + 1];
                        triggerRightBytes = ((triggerRightRange * 255) + triggerRightRaw) / 4;
                    }
                    else
                    {
                        triggerRightBytes = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerRight];
                    }

                    //Check the triggers deadzone
                    if (controller.Details.Profile.DeadzoneTriggerRight != 0)
                    {
                        int deadzoneRangeRight = (255 * controller.Details.Profile.DeadzoneTriggerRight) / 100;
                        if (triggerRightBytes < deadzoneRangeRight) { triggerRightBytes = 0; }
                    }

                    //Calculate trigger sensitivity
                    if (controller.Details.Profile.SensitivityTriggerRight != 1)
                    {
                        triggerRightBytes = Convert.ToInt32(triggerRightBytes * controller.Details.Profile.SensitivityTriggerRight);
                    }

                    //Check the triggers range
                    if (triggerRightBytes > 255) { triggerRightBytes = 255; } else if (triggerRightBytes < 0) { triggerRightBytes = 0; }

                    //Store the triggers
                    controller.InputCurrent.TriggerRight = Convert.ToByte(triggerRightBytes);
                }

                //Read digital triggers
                if (controller.Details.Profile.UseButtonTriggers || controller.SupportedCurrent.OffsetHeader.TriggerLeft == null || controller.InputCurrent.TriggerLeft == 0)
                {
                    if (controller.InputCurrent.Buttons[(int)ControllerButtons.TriggerLeft].PressedRaw) { controller.InputCurrent.TriggerLeft = 255; } else { controller.InputCurrent.TriggerLeft = 0; }
                }

                if (controller.Details.Profile.UseButtonTriggers || controller.SupportedCurrent.OffsetHeader.TriggerRight == null || controller.InputCurrent.TriggerRight == 0)
                {
                    if (controller.InputCurrent.Buttons[(int)ControllerButtons.TriggerRight].PressedRaw) { controller.InputCurrent.TriggerRight = 255; } else { controller.InputCurrent.TriggerRight = 0; }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update triggers input: " + ex.Message);
                return false;
            }
        }
    }
}