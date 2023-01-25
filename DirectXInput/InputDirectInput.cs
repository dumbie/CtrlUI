using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVJsonFunctions;
using static DirectXInput.ProfileFunctions;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive and translate DirectInput
        async Task LoopInputDirectInput(ControllerStatus controller, ControllerType controllerType)
        {
            try
            {
                Debug.WriteLine("Receive and translate " + controllerType + " DirectInput for: " + controller.Details.DisplayName);

                //Receive input from the selected controller
                while (TaskCheckLoop(controller.InputControllerTask) && controller.Connected())
                {
                    try
                    {
                        //Read data from the controller
                        if (controllerType == ControllerType.HidDevice)
                        {
                            if (!controller.HidDevice.ReadBytesFile(controller.InputReport))
                            {
                                Debug.WriteLine("Failed to read input data from hid controller: " + controller.NumberId);
                                TaskDelayMs(1);
                                continue;
                            }
                        }
                        else
                        {
                            if (!controller.WinUsbDevice.ReadBytesIntPipe(controller.InputReport))
                            {
                                Debug.WriteLine("Failed to read input data from win controller: " + controller.NumberId);
                                TaskDelayMs(1);
                                continue;
                            }
                        }

                        //Update the controller last input time
                        controller.PrevInputTicks = controller.LastInputTicks;
                        controller.LastInputTicks = GetSystemTicksMs();

                        //Set controller header offset
                        int HeaderOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                        //Raw left thumbs
                        int ThumbLeftX = 0;
                        int ThumbLeftY = 0;
                        if (controller.SupportedCurrent.OffsetHeader.ThumbLeftX != null && controller.SupportedCurrent.OffsetHeader.ThumbLeftY != null)
                        {
                            ThumbLeftX = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftX];
                            ThumbLeftY = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftY];
                            ThumbLeftX = (ThumbLeftX * 257) - (int)32767.5;
                            ThumbLeftY = (int)32767.5 - (ThumbLeftY * 257);
                        }

                        //Raw right thumbs
                        int ThumbRightX = 0;
                        int ThumbRightY = 0;
                        if (controller.SupportedCurrent.OffsetHeader.ThumbRightX != null && controller.SupportedCurrent.OffsetHeader.ThumbRightY != null)
                        {
                            ThumbRightX = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightX];
                            ThumbRightY = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightY];
                            ThumbRightX = (ThumbRightX * 257) - (int)32767.5;
                            ThumbRightY = (int)32767.5 - (ThumbRightY * 257);
                        }

                        //Flip thumb movement
                        if (controller.Details.Profile.ThumbFlipMovement)
                        {
                            int CurrentLeftX = ThumbLeftX;
                            int CurrentLeftY = ThumbLeftY;
                            int CurrentRightX = ThumbRightX;
                            int CurrentRightY = ThumbRightY;
                            ThumbLeftX = CurrentRightX;
                            ThumbLeftY = CurrentRightY;
                            ThumbRightX = CurrentLeftX;
                            ThumbRightY = CurrentLeftY;
                        }

                        //Flip thumb axes
                        if (controller.Details.Profile.ThumbFlipAxesLeft)
                        {
                            int CurrentLeftX = ThumbLeftX;
                            int CurrentLeftY = ThumbLeftY;
                            ThumbLeftX = CurrentLeftY;
                            ThumbLeftY = CurrentLeftX;
                        }
                        if (controller.Details.Profile.ThumbFlipAxesRight)
                        {
                            int CurrentRightX = ThumbRightX;
                            int CurrentRightY = ThumbRightY;
                            ThumbRightX = CurrentRightY;
                            ThumbRightY = CurrentRightX;
                        }

                        //Reverse the thumbs
                        if (controller.Details.Profile.ThumbReverseAxesLeft)
                        {
                            ThumbLeftX = -ThumbLeftX;
                            ThumbLeftY = -ThumbLeftY;
                        }
                        if (controller.Details.Profile.ThumbReverseAxesRight)
                        {
                            ThumbRightX = -ThumbRightX;
                            ThumbRightY = -ThumbRightY;
                        }

                        //Check the thumbs deadzone
                        if (controller.Details.Profile.DeadzoneThumbLeft != 0)
                        {
                            int deadzoneRangeLeft = ((int)32767.5 * controller.Details.Profile.DeadzoneThumbLeft) / 100;
                            if (Math.Abs(ThumbLeftX) < deadzoneRangeLeft) { ThumbLeftX = 0; }
                            if (Math.Abs(ThumbLeftY) < deadzoneRangeLeft) { ThumbLeftY = 0; }
                        }
                        if (controller.Details.Profile.DeadzoneThumbRight != 0)
                        {
                            int deadzoneRangeRight = ((int)32767.5 * controller.Details.Profile.DeadzoneThumbRight) / 100;
                            if (Math.Abs(ThumbRightX) < deadzoneRangeRight) { ThumbRightX = 0; }
                            if (Math.Abs(ThumbRightY) < deadzoneRangeRight) { ThumbRightY = 0; }
                        }

                        //Calculate thumbs sensitivity
                        if (controller.Details.Profile.SensitivityThumb != 1)
                        {
                            ThumbLeftX = Convert.ToInt32(ThumbLeftX * controller.Details.Profile.SensitivityThumb);
                            ThumbLeftY = Convert.ToInt32(ThumbLeftY * controller.Details.Profile.SensitivityThumb);
                            ThumbRightX = Convert.ToInt32(ThumbRightX * controller.Details.Profile.SensitivityThumb);
                            ThumbRightY = Convert.ToInt32(ThumbRightY * controller.Details.Profile.SensitivityThumb);
                        }

                        //Check the thumbs range
                        if (ThumbLeftX > 32767) { ThumbLeftX = 32767; } else if (ThumbLeftX < -32767) { ThumbLeftX = -32767; }
                        if (ThumbLeftY > 32767) { ThumbLeftY = 32767; } else if (ThumbLeftY < -32767) { ThumbLeftY = -32767; }
                        if (ThumbRightX > 32767) { ThumbRightX = 32767; } else if (ThumbRightX < -32767) { ThumbRightX = -32767; }
                        if (ThumbRightY > 32767) { ThumbRightY = 32767; } else if (ThumbRightY < -32767) { ThumbRightY = -32767; }

                        //Store the thumbs
                        controller.InputCurrent.ThumbLeftX = ThumbLeftX;
                        controller.InputCurrent.ThumbLeftY = ThumbLeftY;
                        controller.InputCurrent.ThumbRightX = ThumbRightX;
                        controller.InputCurrent.ThumbRightY = ThumbRightY;

                        //Raw Triggers
                        if (controller.SupportedCurrent.OffsetHeader.TriggerLeft != null || controller.SupportedCurrent.OffsetHeader.TriggerRight != null)
                        {
                            if (controller.SupportedCurrent.OffsetHeader.TriggerLeft != null)
                            {
                                int triggerLeftBytes = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerLeft];

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

                            if (controller.SupportedCurrent.OffsetHeader.TriggerRight != null)
                            {
                                int triggerRightBytes = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerRight];

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
                        }
                        else if (!controller.Details.Profile.UseButtonTriggers)
                        {
                            Debug.WriteLine("Controller without triggers detected.");
                            App.vWindowOverlay.Notification_Show_Status("Controller", "Controller has no triggers");

                            AVActions.ActionDispatcherInvoke(delegate { cb_ControllerUseButtonTriggers.IsChecked = true; });
                            controller.Details.Profile.UseButtonTriggers = true;

                            //Save changes to Json file
                            JsonSaveObject(controller.Details.Profile, GenerateJsonNameControllerProfile(controller.Details.Profile));
                        }

                        //Raw Buttons (DPad)
                        if (controller.SupportedCurrent.OffsetButton.DPadLeft != null)
                        {
                            //Get DPad state
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.DPadLeft.Group, controller.SupportedCurrent);
                            bool DPadStateLeft = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.DPadLeft.Offset)) != 0;
                            bool DPadStateRight = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.DPadRight.Offset)) != 0;
                            bool DPadStateUp = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.DPadUp.Offset)) != 0;
                            bool DPadStateDown = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.DPadDown.Offset)) != 0;
                            int DPadStateAll = (DPadStateLeft ? 1 : 0) << 0 | (DPadStateRight ? 1 : 0) << 1 | (DPadStateUp ? 1 : 0) << 2 | (DPadStateDown ? 1 : 0) << 3;

                            //Check DPad type
                            if (controller.SupportedCurrent.OffsetButton.DPadDirect)
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

                        //Raw Buttons (A, B, X, Y)
                        if (controller.SupportedCurrent.OffsetButton.A != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.A.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[0] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.A.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.B != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.B.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[1] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.B.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.X != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.X.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[2] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.X.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.Y != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.Y.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[3] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.Y.Offset)) != 0;
                        }

                        //Raw Buttons (Shoulders, Triggers, Thumbs, Back, Start)
                        if (controller.SupportedCurrent.OffsetButton.ShoulderLeft != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.ShoulderLeft.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[100] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.ShoulderLeft.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.ShoulderRight != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.ShoulderRight.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[101] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.ShoulderRight.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.TriggerLeft != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.TriggerLeft.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[102] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.TriggerLeft.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.TriggerRight != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.TriggerRight.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[103] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.TriggerRight.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.ThumbLeft != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.ThumbLeft.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[104] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.ThumbLeft.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.ThumbRight != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.ThumbRight.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[105] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.ThumbRight.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.Back != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.Back.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[106] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.Back.Offset)) != 0;
                        }
                        if (controller.SupportedCurrent.OffsetButton.Start != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.Start.Group, controller.SupportedCurrent);
                            controller.InputCurrent.ButtonPressStatus[107] = (controller.InputReport[HeaderOffset + ButtonGroupOffset] & (1 << controller.SupportedCurrent.OffsetButton.Start.Offset)) != 0;
                        }

                        //Raw Buttons (Touchpad, Guide, Media)
                        if (controller.SupportedCurrent.OffsetButton.Touchpad != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.Touchpad.Group, controller.SupportedCurrent);
                            byte ButtonInputByte = controller.InputReport[HeaderOffset + ButtonGroupOffset];
                            if (ButtonInputByte < 10)
                            {
                                controller.InputCurrent.ButtonPressStatus[200] = (ButtonInputByte & (1 << controller.SupportedCurrent.OffsetButton.Touchpad.Offset)) != 0;
                            }
                        }
                        if (controller.SupportedCurrent.OffsetButton.Guide != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.Guide.Group, controller.SupportedCurrent);
                            byte ButtonInputByte = controller.InputReport[HeaderOffset + ButtonGroupOffset];
                            if (ButtonInputByte < 10)
                            {
                                controller.InputCurrent.ButtonPressStatus[201] = (ButtonInputByte & (1 << controller.SupportedCurrent.OffsetButton.Guide.Offset)) != 0;
                            }
                        }
                        if (controller.SupportedCurrent.OffsetButton.Media != null)
                        {
                            int ButtonGroupOffset = LookupButtonGroupOffset(controller.SupportedCurrent.OffsetButton.Media.Group, controller.SupportedCurrent);
                            byte ButtonInputByte = controller.InputReport[HeaderOffset + ButtonGroupOffset];
                            if (ButtonInputByte < 10)
                            {
                                controller.InputCurrent.ButtonPressStatus[202] = (ButtonInputByte & (1 << controller.SupportedCurrent.OffsetButton.Media.Offset)) != 0;
                            }
                        }

                        //Raw Touchpad
                        if (controller.SupportedCurrent.OffsetHeader.Touchpad != null)
                        {
                            byte touchByte0 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad];
                            byte touchByte1 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 1];
                            byte touchByte2 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 2];
                            byte touchByte3 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 3];

                            if ((touchByte0 & 0x80) == 0)
                            {
                                controller.InputCurrent.TouchpadActive = 1;
                            }
                            else
                            {
                                controller.InputCurrent.TouchpadActive = 0;
                            }

                            controller.InputCurrent.TouchpadId = (byte)(touchByte0 & 0x7F);
                            controller.InputCurrent.TouchpadX = ((ushort)(touchByte2 & 0x0F) << 8) | touchByte1;
                            controller.InputCurrent.TouchpadY = (touchByte3 << 4) | ((ushort)(touchByte2 & 0xF0) >> 4);
                        }

                        //Raw Gyroscope
                        if (controller.SupportedCurrent.OffsetHeader.Gyroscope != null)
                        {
                            byte gyroByte0 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope];
                            byte gyroByte1 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 1];
                            byte gyroByte2 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 2];
                            byte gyroByte3 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 3];
                            byte gyroByte4 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 4];
                            byte gyroByte5 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 5];

                            short gyroPitch = (short)((ushort)(gyroByte1 << 8) | gyroByte0);
                            short gyroYaw = (short)((ushort)(gyroByte3 << 8) | gyroByte2);
                            short gyroRoll = (short)((ushort)(gyroByte5 << 8) | gyroByte4);

                            controller.InputCurrent.GyroPitch = gyroPitch / (float)16;
                            controller.InputCurrent.GyroYaw = -gyroYaw / (float)16;
                            controller.InputCurrent.GyroRoll = -gyroRoll / (float)16;
                        }

                        //Raw Accelerometer
                        if (controller.SupportedCurrent.OffsetHeader.Accelerometer != null)
                        {
                            byte accelByte0 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer];
                            byte accelByte1 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 1];
                            byte accelByte2 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 2];
                            byte accelByte3 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 3];
                            byte accelByte4 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 4];
                            byte accelByte5 = controller.InputReport[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 5];

                            short accelX = (short)((ushort)(accelByte1 << 8) | accelByte0);
                            short accelY = (short)((ushort)(accelByte3 << 8) | accelByte2);
                            short accelZ = (short)((ushort)(accelByte5 << 8) | accelByte4);

                            controller.InputCurrent.AccelX = -accelX / (float)8192;
                            controller.InputCurrent.AccelY = -accelY / (float)8192;
                            controller.InputCurrent.AccelZ = -accelZ / (float)8192;
                        }

                        //Save controller button mapping
                        if (!ControllerSaveMapping(controller))
                        {
                            //Raw Buttons (A, B, X, Y)
                            if (controller.Details.Profile.ButtonA == null) { controller.InputCurrent.ButtonA.PressedRaw = controller.InputCurrent.ButtonPressStatus[0]; }
                            else if (controller.Details.Profile.ButtonA != -1) { controller.InputCurrent.ButtonA.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonA.Value]; }

                            if (controller.Details.Profile.ButtonB == null) { controller.InputCurrent.ButtonB.PressedRaw = controller.InputCurrent.ButtonPressStatus[1]; }
                            else if (controller.Details.Profile.ButtonB != -1) { controller.InputCurrent.ButtonB.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonB.Value]; }

                            if (controller.Details.Profile.ButtonX == null) { controller.InputCurrent.ButtonX.PressedRaw = controller.InputCurrent.ButtonPressStatus[2]; }
                            else if (controller.Details.Profile.ButtonX != -1) { controller.InputCurrent.ButtonX.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonX.Value]; }

                            if (controller.Details.Profile.ButtonY == null) { controller.InputCurrent.ButtonY.PressedRaw = controller.InputCurrent.ButtonPressStatus[3]; }
                            else if (controller.Details.Profile.ButtonY != -1) { controller.InputCurrent.ButtonY.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonY.Value]; }

                            //Raw Buttons (Shoulders, Triggers, Thumbs, Back, Start)
                            if (controller.Details.Profile.ButtonShoulderLeft == null) { controller.InputCurrent.ButtonShoulderLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[100]; }
                            else if (controller.Details.Profile.ButtonShoulderLeft != -1) { controller.InputCurrent.ButtonShoulderLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonShoulderLeft.Value]; }

                            if (controller.Details.Profile.ButtonShoulderRight == null) { controller.InputCurrent.ButtonShoulderRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[101]; }
                            else if (controller.Details.Profile.ButtonShoulderRight != -1) { controller.InputCurrent.ButtonShoulderRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonShoulderRight.Value]; }

                            if (controller.Details.Profile.ButtonTriggerLeft == null) { controller.InputCurrent.ButtonTriggerLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[102]; }
                            else if (controller.Details.Profile.ButtonTriggerLeft != -1) { controller.InputCurrent.ButtonTriggerLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonTriggerLeft.Value]; }

                            if (controller.Details.Profile.ButtonTriggerRight == null) { controller.InputCurrent.ButtonTriggerRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[103]; }
                            else if (controller.Details.Profile.ButtonTriggerRight != -1) { controller.InputCurrent.ButtonTriggerRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonTriggerRight.Value]; }

                            if (controller.Details.Profile.ButtonThumbLeft == null) { controller.InputCurrent.ButtonThumbLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[104]; }
                            else if (controller.Details.Profile.ButtonThumbLeft != -1) { controller.InputCurrent.ButtonThumbLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonThumbLeft.Value]; }

                            if (controller.Details.Profile.ButtonThumbRight == null) { controller.InputCurrent.ButtonThumbRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[105]; }
                            else if (controller.Details.Profile.ButtonThumbRight != -1) { controller.InputCurrent.ButtonThumbRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonThumbRight.Value]; }

                            if (controller.Details.Profile.ButtonBack == null) { controller.InputCurrent.ButtonBack.PressedRaw = controller.InputCurrent.ButtonPressStatus[106]; }
                            else if (controller.Details.Profile.ButtonBack != -1) { controller.InputCurrent.ButtonBack.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonBack.Value]; }

                            if (controller.Details.Profile.ButtonStart == null) { controller.InputCurrent.ButtonStart.PressedRaw = controller.InputCurrent.ButtonPressStatus[107]; }
                            else if (controller.Details.Profile.ButtonStart != -1) { controller.InputCurrent.ButtonStart.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonStart.Value]; }

                            //Raw Buttons (Touchpad, Guide, Media)
                            if (controller.Details.Profile.ButtonTouchpad == null) { controller.InputCurrent.ButtonTouchpad.PressedRaw = controller.InputCurrent.ButtonPressStatus[200]; }
                            else if (controller.Details.Profile.ButtonTouchpad != -1) { controller.InputCurrent.ButtonTouchpad.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonTouchpad.Value]; }

                            if (controller.Details.Profile.ButtonGuide == null) { controller.InputCurrent.ButtonGuide.PressedRaw = controller.InputCurrent.ButtonPressStatus[201]; }
                            else if (controller.Details.Profile.ButtonGuide != -1) { controller.InputCurrent.ButtonGuide.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonGuide.Value]; }

                            if (controller.Details.Profile.ButtonMedia == null) { controller.InputCurrent.ButtonMedia.PressedRaw = controller.InputCurrent.ButtonPressStatus[202]; }
                            else if (controller.Details.Profile.ButtonMedia != -1) { controller.InputCurrent.ButtonMedia.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonMedia.Value]; }

                            //Fake Guide or Media button press with LB and Back
                            if (controller.Details.Profile.FakeGuideButton && controller.InputCurrent.ButtonShoulderLeft.PressedRaw && controller.InputCurrent.ButtonBack.PressedRaw)
                            {
                                controller.InputCurrent.ButtonShoulderLeft.PressedRaw = false;
                                controller.InputCurrent.ButtonBack.PressedRaw = false;
                                controller.InputCurrent.ButtonGuide.PressedRaw = true;
                            }
                            else if (controller.Details.Profile.FakeMediaButton && controller.InputCurrent.ButtonShoulderLeft.PressedRaw && controller.InputCurrent.ButtonBack.PressedRaw)
                            {
                                controller.InputCurrent.ButtonShoulderLeft.PressedRaw = false;
                                controller.InputCurrent.ButtonBack.PressedRaw = false;
                                controller.InputCurrent.ButtonMedia.PressedRaw = true;
                            }

                            //Fake Touchpad button press with RB and Back
                            if (controller.Details.Profile.FakeTouchpadButton && controller.InputCurrent.ButtonShoulderRight.PressedRaw && controller.InputCurrent.ButtonBack.PressedRaw)
                            {
                                controller.InputCurrent.ButtonShoulderRight.PressedRaw = false;
                                controller.InputCurrent.ButtonBack.PressedRaw = false;
                                controller.InputCurrent.ButtonTouchpad.PressedRaw = true;
                            }

                            //Update the controller battery level
                            ControllerReadBatteryLevel(controller);

                            //Send input to the virtual device
                            await SendInputVirtualController(controller);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Direct input " + controllerType + " data report is out of range or empty, skipping.");
                    }
                }
            }
            catch { }
        }
    }
}