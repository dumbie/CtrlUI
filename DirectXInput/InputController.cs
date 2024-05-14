using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVJsonFunctions;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
using static LibraryShared.Classes;
using static LibraryShared.ControllerTimings;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private async Task ControllerInputSend(ControllerStatus controller)
        {
            try
            {
                //Read data from the controller
                if (controller.Details.Type == ControllerType.HidDevice)
                {
                    if (!controller.HidDevice.ReadBytesFile(controller.ControllerDataInput))
                    {
                        Debug.WriteLine("Failed to read input data from hid controller: " + controller.NumberId);
                        AVHighResDelay.Delay(0.1F);
                        return;
                    }
                }
                else
                {
                    if (!controller.WinUsbDevice.ReadBytesIntPipe(controller.ControllerDataInput))
                    {
                        Debug.WriteLine("Failed to read input data from win controller: " + controller.NumberId);
                        AVHighResDelay.Delay(0.1F);
                        return;
                    }
                }

                //Validate controller input data
                if (!ControllerValidateInputData(controller))
                {
                    Debug.WriteLine("Invalid input data read from controller: " + controller.NumberId);
                    AVHighResDelay.Delay(0.1F);
                    return;
                }

                //Set controller header offset
                int HeaderOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                //Raw left thumbs
                int ThumbLeftX = 0;
                int ThumbLeftY = 0;
                if (controller.SupportedCurrent.OffsetHeader.ThumbLeftX != null && controller.SupportedCurrent.OffsetHeader.ThumbLeftY != null)
                {
                    ThumbLeftX = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftX];
                    ThumbLeftY = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftY];
                    ThumbLeftX = (ThumbLeftX * 257) - (int)32767.5;
                    ThumbLeftY = (int)32767.5 - (ThumbLeftY * 257);
                }

                //Raw right thumbs
                int ThumbRightX = 0;
                int ThumbRightY = 0;
                if (controller.SupportedCurrent.OffsetHeader.ThumbRightX != null && controller.SupportedCurrent.OffsetHeader.ThumbRightY != null)
                {
                    ThumbRightX = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightX];
                    ThumbRightY = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightY];
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
                if (controller.Details.Profile.SensitivityThumbLeft != 1)
                {
                    ThumbLeftX = Convert.ToInt32(ThumbLeftX * controller.Details.Profile.SensitivityThumbLeft);
                    ThumbLeftY = Convert.ToInt32(ThumbLeftY * controller.Details.Profile.SensitivityThumbLeft);
                }
                if (controller.Details.Profile.SensitivityThumbRight != 1)
                {
                    ThumbRightX = Convert.ToInt32(ThumbRightX * controller.Details.Profile.SensitivityThumbRight);
                    ThumbRightY = Convert.ToInt32(ThumbRightY * controller.Details.Profile.SensitivityThumbRight);
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

                //Check left thumb stick movement
                int absThumbLeftY = Math.Abs(controller.InputCurrent.ThumbLeftY);
                int absThumbLeftX = Math.Abs(controller.InputCurrent.ThumbLeftX);
                if (controller.InputCurrent.ThumbLeftX < -vControllerThumbOffset7500)
                {
                    controller.InputCurrent.ButtonThumbLeftLeft.PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.ButtonThumbLeftLeft.PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbLeftY > vControllerThumbOffset7500)
                {
                    controller.InputCurrent.ButtonThumbLeftUp.PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.ButtonThumbLeftUp.PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbLeftX > vControllerThumbOffset7500)
                {
                    controller.InputCurrent.ButtonThumbLeftRight.PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.ButtonThumbLeftRight.PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbLeftY < -vControllerThumbOffset7500)
                {
                    controller.InputCurrent.ButtonThumbLeftDown.PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.ButtonThumbLeftDown.PressedRaw = false;
                }

                //Check right thumb stick movement
                int absThumbRightY = Math.Abs(controller.InputCurrent.ThumbRightY);
                int absThumbRightX = Math.Abs(controller.InputCurrent.ThumbRightX);
                if (controller.InputCurrent.ThumbRightX < -vControllerThumbOffset7500)
                {
                    controller.InputCurrent.ButtonThumbRightLeft.PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.ButtonThumbRightLeft.PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbRightY > vControllerThumbOffset7500)
                {
                    controller.InputCurrent.ButtonThumbRightUp.PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.ButtonThumbRightUp.PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbRightX > vControllerThumbOffset7500)
                {
                    controller.InputCurrent.ButtonThumbRightRight.PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.ButtonThumbRightRight.PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbRightY < -vControllerThumbOffset7500)
                {
                    controller.InputCurrent.ButtonThumbRightDown.PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.ButtonThumbRightDown.PressedRaw = false;
                }

                //Raw Triggers
                if (controller.SupportedCurrent.OffsetHeader.TriggerLeft != null || controller.SupportedCurrent.OffsetHeader.TriggerRight != null)
                {
                    if (controller.SupportedCurrent.OffsetHeader.TriggerLeft != null)
                    {
                        int triggerLeftBytes = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerLeft];

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
                        int triggerRightBytes = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.TriggerRight];

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
                    vWindowOverlay.Notification_Show_Status("Controller", "Controller has no triggers");

                    AVActions.DispatcherInvoke(delegate { cb_ControllerUseButtonTriggers.IsChecked = true; });
                    controller.Details.Profile.UseButtonTriggers = true;

                    //Save changes to Json file
                    JsonSaveObject(controller.Details.Profile, GenerateJsonNameControllerProfile(controller.Details.Profile));
                }

                //Raw Buttons (DPad)
                if (controller.SupportedCurrent.OffsetButton.DPadLeft != null)
                {
                    //Get DPad state
                    bool DPadStateLeft = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.DPadLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.DPadLeft.Offset)) != 0;
                    bool DPadStateRight = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.DPadRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.DPadRight.Offset)) != 0;
                    bool DPadStateUp = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.DPadUp.Group] & (1 << controller.SupportedCurrent.OffsetButton.DPadUp.Offset)) != 0;
                    bool DPadStateDown = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.DPadDown.Group] & (1 << controller.SupportedCurrent.OffsetButton.DPadDown.Offset)) != 0;
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

                //Raw Buttons (A, B, X, Y)
                if (controller.SupportedCurrent.OffsetButton.A != null)
                {
                    controller.InputCurrent.ButtonPressStatus[0] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.A.Group] & (1 << controller.SupportedCurrent.OffsetButton.A.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.B != null)
                {
                    controller.InputCurrent.ButtonPressStatus[1] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.B.Group] & (1 << controller.SupportedCurrent.OffsetButton.B.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.X != null)
                {
                    controller.InputCurrent.ButtonPressStatus[2] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.X.Group] & (1 << controller.SupportedCurrent.OffsetButton.X.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Y != null)
                {
                    controller.InputCurrent.ButtonPressStatus[3] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.Y.Group] & (1 << controller.SupportedCurrent.OffsetButton.Y.Offset)) != 0;
                }

                //Raw Buttons (Shoulders, Triggers, Thumbs, Back, Start)
                if (controller.SupportedCurrent.OffsetButton.ShoulderLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[100] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.ShoulderLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.ShoulderLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ShoulderRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[101] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.ShoulderRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.ShoulderRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.TriggerLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[102] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.TriggerLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.TriggerLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.TriggerRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[103] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.TriggerRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.TriggerRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ThumbLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[104] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.ThumbLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.ThumbLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ThumbRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[105] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.ThumbRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.ThumbRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Back != null)
                {
                    controller.InputCurrent.ButtonPressStatus[106] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.Back.Group] & (1 << controller.SupportedCurrent.OffsetButton.Back.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Start != null)
                {
                    controller.InputCurrent.ButtonPressStatus[107] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.Start.Group] & (1 << controller.SupportedCurrent.OffsetButton.Start.Offset)) != 0;
                }

                //Raw Buttons (Guide and others)
                if (controller.SupportedCurrent.OffsetButton.Guide != null)
                {
                    controller.InputCurrent.ButtonPressStatus[200] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.Guide.Group] & (1 << controller.SupportedCurrent.OffsetButton.Guide.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Touchpad != null)
                {
                    controller.InputCurrent.ButtonPressStatus[201] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.Touchpad.Group] & (1 << controller.SupportedCurrent.OffsetButton.Touchpad.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Media != null)
                {
                    controller.InputCurrent.ButtonPressStatus[202] = (controller.ControllerDataInput[HeaderOffset + controller.SupportedCurrent.OffsetButton.Media.Group] & (1 << controller.SupportedCurrent.OffsetButton.Media.Offset)) != 0;
                }

                //Raw Touchpad
                if (controller.SupportedCurrent.OffsetHeader.Touchpad != null)
                {
                    byte touchByte0 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad];
                    byte touchByte1 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 1];
                    byte touchByte2 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 2];
                    byte touchByte3 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Touchpad + 3];

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
                    byte gyroByte0 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope];
                    byte gyroByte1 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 1];
                    byte gyroByte2 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 2];
                    byte gyroByte3 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 3];
                    byte gyroByte4 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 4];
                    byte gyroByte5 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Gyroscope + 5];

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
                    byte accelByte0 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer];
                    byte accelByte1 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 1];
                    byte accelByte2 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 2];
                    byte accelByte3 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 3];
                    byte accelByte4 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 4];
                    byte accelByte5 = controller.ControllerDataInput[HeaderOffset + (int)controller.SupportedCurrent.OffsetHeader.Accelerometer + 5];

                    short accelX = (short)((ushort)(accelByte1 << 8) | accelByte0);
                    short accelY = (short)((ushort)(accelByte3 << 8) | accelByte2);
                    short accelZ = (short)((ushort)(accelByte5 << 8) | accelByte4);

                    controller.InputCurrent.AccelX = -accelX / (float)8192;
                    controller.InputCurrent.AccelY = -accelY / (float)8192;
                    controller.InputCurrent.AccelZ = -accelZ / (float)8192;
                }

                //Save controller button mapping
                if (ControllerSaveMapping(controller))
                {
                    AVHighResDelay.Delay(0.1F);
                    return;
                }

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

                //Raw Buttons (Guide and others)
                if (controller.Details.Profile.ButtonGuide == null) { controller.InputCurrent.ButtonGuide.PressedRaw = controller.InputCurrent.ButtonPressStatus[200]; }
                else if (controller.Details.Profile.ButtonGuide != -1) { controller.InputCurrent.ButtonGuide.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonGuide.Value]; }

                if (controller.Details.Profile.ButtonTouchpad == null) { controller.InputCurrent.ButtonTouchpad.PressedRaw = controller.InputCurrent.ButtonPressStatus[201]; }
                else if (controller.Details.Profile.ButtonTouchpad != -1) { controller.InputCurrent.ButtonTouchpad.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonTouchpad.Value]; }

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

                //Check if alt tab is active and buttons need to be blocked
                if (vAltTabDownStatus)
                {
                    controller.InputCurrent.ButtonStart.PressedRaw = false;
                    controller.InputCurrent.ButtonShoulderLeft.PressedRaw = false;
                }

                //Update and check button press times
                UpdateCheckButtonPressTimes(controller);

                //Ignore controller timeout
                controller.TimeoutIgnore = true;

                //Check if controller output needs to be blocked
                bool blockOutputPreview = vAppActivated && (vShowControllerDebug || vShowControllerPreview);

                //Check if controller is currently disconnecting
                bool blockOutputDisconnecting = controller.Disconnecting;

                //Check if controller shortcut is pressed
                bool blockOutputShortcut = await ControllerShortcut(controller);

                //Check if controller output needs to be forwarded
                bool blockOutputApplication = await ControllerOutputApps(controller);

                //Allow controller timeout 
                controller.TimeoutIgnore = false;

                //Check if guide button is exclusive and needs to be blocked
                if (controller.InputCurrent.ButtonGuide.PressedRaw && SettingLoad(vConfigurationDirectXInput, "ExclusiveGuide", typeof(bool)))
                {
                    controller.InputCurrent.ButtonGuide.PressedRaw = false;
                }

                //Check if controller output needs to be blocked
                if (blockOutputPreview || blockOutputDisconnecting || blockOutputShortcut || blockOutputApplication)
                {
                    //Update and prepare virtual input data
                    PrepareVirtualInputDataEmpty(controller);
                }
                else
                {
                    //Update and prepare virtual input data
                    PrepareVirtualInputDataCurrent(controller);
                }

                //Update controller input time
                long ticksSystem = GetSystemTicksMs();
                controller.TicksInputPrev = controller.TicksInputLast;
                controller.TicksInputLast = ticksSystem;

                //Check if controller is idle and update active time
                if (!CheckControllerIdlePress(controller))
                {
                    controller.TicksActiveLast = ticksSystem;
                }

                //Send input to virtual device
                vVirtualBusDevice.VirtualReadWrite(ref controller);

                //Update controller rumble status
                if (controller.VirtualDataOutput[1] == 0x08)
                {
                    controller.RumbleCurrentHeavy = controller.VirtualDataOutput[3];
                    controller.RumbleCurrentLight = controller.VirtualDataOutput[4];
                }
            }
            catch
            {
                Debug.WriteLine("DirectInput " + controller.Details.Type + " data report is out of range or empty, skipping.");
            }
        }
    }
}