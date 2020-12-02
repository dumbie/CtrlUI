using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive and translate DirectInput
        async Task LoopInputWinUsb(ControllerStatus Controller)
        {
            try
            {
                Debug.WriteLine("Receive and Translate Win DirectInput for: " + Controller.Details.DisplayName);

                //Initialize game controller
                ControllerInitialize(Controller);

                //Send default output to controller
                ControllerOutput(Controller, true, false, false);

                //Receive input from the selected controller
                while (!Controller.InputTask.TaskStopRequest && Controller.WinUsbDevice != null && Controller.WinUsbDevice.Connected)
                {
                    try
                    {
                        //Read data from the controller
                        if (!Controller.WinUsbDevice.ReadBytesIntPipe(Controller.InputReport))
                        {
                            Debug.WriteLine("Failed to read input data from win controller: " + Controller.NumberId);
                            continue;
                        }

                        //Update the controller last read time
                        Controller.LastReadTicks = Stopwatch.GetTimestamp();

                        //Offsets for thumb sticks
                        int OffsetThumbLeftX = 6;
                        int OffsetThumbLeftY = 7;
                        int OffsetThumbRightX = 8;
                        int OffsetThumbRightY = 9;

                        //Offsets for DPad and Buttons
                        int OffsetButtonsGroup1 = 2; //D-Pad, Back, Start and ThumbLeftRight
                        int OffsetButtonsGroup2 = 3; //ShoulderLeftRight, TriggerLeftRight and A,B,X,Y
                        int OffsetButtonsGroup3 = 4; //Guide

                        //Offsets for Triggers
                        int OffsetTriggerLeft = 18;
                        int OffsetTriggerRight = 19;

                        //Calculate left thumbs
                        int ThumbLeftX = Controller.InputReport[OffsetThumbLeftX];
                        int ThumbLeftY = Controller.InputReport[OffsetThumbLeftY];
                        ThumbLeftX = (ThumbLeftX * 257) - (int)32767.5;
                        ThumbLeftY = (int)32767.5 - (ThumbLeftY * 257);

                        //Calculate right thumbs
                        int ThumbRightX = Controller.InputReport[OffsetThumbRightX];
                        int ThumbRightY = Controller.InputReport[OffsetThumbRightY];
                        ThumbRightX = (ThumbRightX * 257) - (int)32767.5;
                        ThumbRightY = (int)32767.5 - (ThumbRightY * 257);

                        //Flip the thumbs
                        if (Controller.Details.Profile.ThumbFlipMovement)
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
                        if (Controller.Details.Profile.ThumbFlipAxesLeft)
                        {
                            int CurrentLeftX = ThumbLeftX;
                            int CurrentLeftY = ThumbLeftY;
                            ThumbLeftX = CurrentLeftY;
                            ThumbLeftY = CurrentLeftX;
                        }
                        if (Controller.Details.Profile.ThumbFlipAxesRight)
                        {
                            int CurrentRightX = ThumbRightX;
                            int CurrentRightY = ThumbRightY;
                            ThumbRightX = CurrentRightY;
                            ThumbRightY = CurrentRightX;
                        }

                        //Reverse the thumbs
                        if (Controller.Details.Profile.ThumbReverseAxesLeft)
                        {
                            ThumbLeftX = -ThumbLeftX;
                            ThumbLeftY = -ThumbLeftY;
                        }
                        if (Controller.Details.Profile.ThumbReverseAxesRight)
                        {
                            ThumbRightX = -ThumbRightX;
                            ThumbRightY = -ThumbRightY;
                        }

                        //Check the thumbs deadzone
                        if (Controller.Details.Profile.DeadzoneThumbLeft != 0)
                        {
                            int deadzoneRangeLeft = ((int)32767.5 * Controller.Details.Profile.DeadzoneThumbLeft) / 100;
                            if (Math.Abs(ThumbLeftX) < deadzoneRangeLeft) { ThumbLeftX = 0; }
                            if (Math.Abs(ThumbLeftY) < deadzoneRangeLeft) { ThumbLeftY = 0; }
                        }
                        if (Controller.Details.Profile.DeadzoneThumbRight != 0)
                        {
                            int deadzoneRangeRight = ((int)32767.5 * Controller.Details.Profile.DeadzoneThumbRight) / 100;
                            if (Math.Abs(ThumbRightX) < deadzoneRangeRight) { ThumbRightX = 0; }
                            if (Math.Abs(ThumbRightY) < deadzoneRangeRight) { ThumbRightY = 0; }
                        }

                        //Calculate thumbs sensitivity
                        if (Controller.Details.Profile.SensitivityThumb != 1)
                        {
                            ThumbLeftX = Convert.ToInt32(ThumbLeftX * Controller.Details.Profile.SensitivityThumb);
                            ThumbLeftY = Convert.ToInt32(ThumbLeftY * Controller.Details.Profile.SensitivityThumb);
                            ThumbRightX = Convert.ToInt32(ThumbRightX * Controller.Details.Profile.SensitivityThumb);
                            ThumbRightY = Convert.ToInt32(ThumbRightY * Controller.Details.Profile.SensitivityThumb);
                        }

                        //Check the thumbs range
                        if (ThumbLeftX > 32767) { ThumbLeftX = 32767; }
                        else if (ThumbLeftX < -32767) { ThumbLeftX = -32767; }
                        if (ThumbLeftY > 32767) { ThumbLeftY = 32767; }
                        else if (ThumbLeftY < -32767) { ThumbLeftY = -32767; }
                        if (ThumbRightX > 32767) { ThumbRightX = 32767; }
                        else if (ThumbRightX < -32767) { ThumbRightX = -32767; }
                        if (ThumbRightY > 32767) { ThumbRightY = 32767; }
                        else if (ThumbRightY < -32767) { ThumbRightY = -32767; }

                        //Store the thumbs
                        Controller.InputCurrent.ThumbLeftX = ThumbLeftX;
                        Controller.InputCurrent.ThumbLeftY = ThumbLeftY;
                        Controller.InputCurrent.ThumbRightX = ThumbRightX;
                        Controller.InputCurrent.ThumbRightY = ThumbRightY;

                        //Raw Triggers
                        if (Controller.InputReport.Length >= OffsetTriggerRight)
                        {
                            int triggerLeftBytes = Controller.InputReport[OffsetTriggerLeft];
                            int triggerRightBytes = Controller.InputReport[OffsetTriggerRight];

                            //Check the triggers deadzone
                            if (Controller.Details.Profile.DeadzoneTriggerLeft != 0)
                            {
                                int deadzoneRangeLeft = (255 * Controller.Details.Profile.DeadzoneTriggerLeft) / 100;
                                if (triggerLeftBytes < deadzoneRangeLeft) { triggerLeftBytes = 0; }
                            }
                            if (Controller.Details.Profile.DeadzoneTriggerRight != 0)
                            {
                                int deadzoneRangeRight = (255 * Controller.Details.Profile.DeadzoneTriggerRight) / 100;
                                if (triggerRightBytes < deadzoneRangeRight) { triggerRightBytes = 0; }
                            }

                            //Calculate trigger sensitivity
                            if (Controller.Details.Profile.SensitivityTrigger != 1)
                            {
                                triggerLeftBytes = Convert.ToInt32(triggerLeftBytes * Controller.Details.Profile.SensitivityTrigger);
                                triggerRightBytes = Convert.ToInt32(triggerRightBytes * Controller.Details.Profile.SensitivityTrigger);
                            }

                            //Check the triggers range
                            if (triggerLeftBytes > 255) { triggerLeftBytes = 255; }
                            if (triggerRightBytes > 255) { triggerRightBytes = 255; }

                            Controller.InputCurrent.TriggerLeft = Convert.ToByte(triggerLeftBytes);
                            Controller.InputCurrent.TriggerRight = Convert.ToByte(triggerRightBytes);
                        }
                        else if (!Controller.Details.Profile.UseButtonTriggers)
                        {
                            Debug.WriteLine("Controller without triggers detected.");
                            App.vWindowOverlay.Notification_Show_Status("Controller", "Controller has no triggers");

                            AVActions.ActionDispatcherInvoke(delegate { cb_ControllerUseButtonTriggers.IsChecked = true; });
                            Controller.Details.Profile.UseButtonTriggers = true;

                            //Save changes to Json file
                            JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                        }

                        //Raw Buttons (Group 1)
                        int ButtonOffset1 = 0;
                        int ButtonTotal1 = 0 + Controller.InputButtonCountTotal1;
                        for (int ButtonByte = 0; ButtonByte < ButtonTotal1; ButtonByte++)
                        {
                            Controller.InputCurrent.ButtonPressStatus[ButtonByte] = (Controller.InputReport[OffsetButtonsGroup1] & (1 << ButtonOffset1)) != 0;
                            ButtonOffset1++;
                        }

                        //Raw Buttons (Group 2)
                        int ButtonOffset2 = 0;
                        int ButtonTotal2 = 100 + Controller.InputButtonCountTotal2;
                        for (int ButtonByte = 100; ButtonByte < ButtonTotal2; ButtonByte++)
                        {
                            Controller.InputCurrent.ButtonPressStatus[ButtonByte] = (Controller.InputReport[OffsetButtonsGroup2] & (1 << ButtonOffset2)) != 0;
                            ButtonOffset2++;
                        }

                        //Raw Buttons (Group 3)
                        int ButtonOffset3 = 0;
                        int ButtonTotal3 = 200 + Controller.InputButtonCountTotal3;
                        for (int ButtonByte = 200; ButtonByte < ButtonTotal3; ButtonByte++)
                        {
                            Controller.InputCurrent.ButtonPressStatus[ButtonByte] = (Controller.InputReport[OffsetButtonsGroup3] & (1 << ButtonOffset3)) != 0;
                            ButtonOffset3++;
                        }

                        //Raw DPad (Group 1)
                        Controller.InputCurrent.DPadLeft.PressedRaw = (Controller.InputReport[OffsetButtonsGroup1] & (1 << 5)) != 0;
                        Controller.InputCurrent.DPadUp.PressedRaw = (Controller.InputReport[OffsetButtonsGroup1] & (1 << 7)) != 0;
                        Controller.InputCurrent.DPadRight.PressedRaw = (Controller.InputReport[OffsetButtonsGroup1] & (1 << 4)) != 0;
                        Controller.InputCurrent.DPadDown.PressedRaw = (Controller.InputReport[OffsetButtonsGroup1] & (1 << 6)) != 0;
                        byte DPadState = (byte)(((Controller.InputCurrent.DPadRight.PressedRaw ? 1 : 0) << 0) | ((Controller.InputCurrent.DPadLeft.PressedRaw ? 1 : 0) << 1) | ((Controller.InputCurrent.DPadDown.PressedRaw ? 1 : 0) << 2) | ((Controller.InputCurrent.DPadUp.PressedRaw ? 1 : 0) << 3));
                        if (Controller.Details.Profile.DPadFourWayMovement)
                        {
                            switch (DPadState)
                            {
                                case 1: Controller.InputCurrent.DPadUp.PressedRaw = true; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 2: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 4: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = true; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 8: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = true; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                default: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                            }
                        }
                        else
                        {
                            switch (DPadState)
                            {
                                case 1: Controller.InputCurrent.DPadUp.PressedRaw = true; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 3: Controller.InputCurrent.DPadUp.PressedRaw = true; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 2: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 6: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = true; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 4: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = true; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 12: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = true; Controller.InputCurrent.DPadLeft.PressedRaw = true; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 8: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = true; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 9: Controller.InputCurrent.DPadUp.PressedRaw = true; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = true; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                default: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                            }
                        }

                        //Save controller button mapping
                        if (!ControllerSaveMapping(Controller))
                        {
                            //Set button mapping input data
                            if (Controller.Details.Profile.ButtonA == null) { Controller.InputCurrent.ButtonA.PressedRaw = Controller.InputCurrent.ButtonPressStatus[106]; }
                            else if (Controller.Details.Profile.ButtonA != -1) { Controller.InputCurrent.ButtonA.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonA.Value]; }

                            if (Controller.Details.Profile.ButtonB == null) { Controller.InputCurrent.ButtonB.PressedRaw = Controller.InputCurrent.ButtonPressStatus[105]; }
                            else if (Controller.Details.Profile.ButtonB != -1) { Controller.InputCurrent.ButtonB.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonB.Value]; }

                            if (Controller.Details.Profile.ButtonX == null) { Controller.InputCurrent.ButtonX.PressedRaw = Controller.InputCurrent.ButtonPressStatus[107]; }
                            else if (Controller.Details.Profile.ButtonX != -1) { Controller.InputCurrent.ButtonX.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonX.Value]; }

                            if (Controller.Details.Profile.ButtonY == null) { Controller.InputCurrent.ButtonY.PressedRaw = Controller.InputCurrent.ButtonPressStatus[104]; }
                            else if (Controller.Details.Profile.ButtonY != -1) { Controller.InputCurrent.ButtonY.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonY.Value]; }

                            if (Controller.Details.Profile.ButtonBack == null) { Controller.InputCurrent.ButtonBack.PressedRaw = Controller.InputCurrent.ButtonPressStatus[0]; }
                            else if (Controller.Details.Profile.ButtonBack != -1) { Controller.InputCurrent.ButtonBack.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonBack.Value]; }

                            if (Controller.Details.Profile.ButtonStart == null) { Controller.InputCurrent.ButtonStart.PressedRaw = Controller.InputCurrent.ButtonPressStatus[3]; }
                            else if (Controller.Details.Profile.ButtonStart != -1) { Controller.InputCurrent.ButtonStart.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonStart.Value]; }

                            if (Controller.Details.Profile.ButtonGuide == null) { Controller.InputCurrent.ButtonGuide.PressedRaw = Controller.InputCurrent.ButtonPressStatus[200]; }
                            else if (Controller.Details.Profile.ButtonGuide != -1) { Controller.InputCurrent.ButtonGuide.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonGuide.Value]; }

                            if (Controller.Details.Profile.ButtonTriggerLeft == null) { Controller.InputCurrent.ButtonTriggerLeft.PressedRaw = Controller.InputCurrent.ButtonPressStatus[100]; }
                            else if (Controller.Details.Profile.ButtonTriggerLeft != -1) { Controller.InputCurrent.ButtonTriggerLeft.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonTriggerLeft.Value]; }

                            if (Controller.Details.Profile.ButtonTriggerRight == null) { Controller.InputCurrent.ButtonTriggerRight.PressedRaw = Controller.InputCurrent.ButtonPressStatus[101]; }
                            else if (Controller.Details.Profile.ButtonTriggerRight != -1) { Controller.InputCurrent.ButtonTriggerRight.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonTriggerRight.Value]; }

                            if (Controller.Details.Profile.ButtonShoulderLeft == null) { Controller.InputCurrent.ButtonShoulderLeft.PressedRaw = Controller.InputCurrent.ButtonPressStatus[102]; }
                            else if (Controller.Details.Profile.ButtonShoulderLeft != -1) { Controller.InputCurrent.ButtonShoulderLeft.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonShoulderLeft.Value]; }

                            if (Controller.Details.Profile.ButtonShoulderRight == null) { Controller.InputCurrent.ButtonShoulderRight.PressedRaw = Controller.InputCurrent.ButtonPressStatus[103]; }
                            else if (Controller.Details.Profile.ButtonShoulderRight != -1) { Controller.InputCurrent.ButtonShoulderRight.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonShoulderRight.Value]; }

                            if (Controller.Details.Profile.ButtonThumbLeft == null) { Controller.InputCurrent.ButtonThumbLeft.PressedRaw = Controller.InputCurrent.ButtonPressStatus[1]; }
                            else if (Controller.Details.Profile.ButtonThumbLeft != -1) { Controller.InputCurrent.ButtonThumbLeft.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonThumbLeft.Value]; }

                            if (Controller.Details.Profile.ButtonThumbRight == null) { Controller.InputCurrent.ButtonThumbRight.PressedRaw = Controller.InputCurrent.ButtonPressStatus[2]; }
                            else if (Controller.Details.Profile.ButtonThumbRight != -1) { Controller.InputCurrent.ButtonThumbRight.PressedRaw = Controller.InputCurrent.ButtonPressStatus[Controller.Details.Profile.ButtonThumbRight.Value]; }

                            //Fake Guide button press with Start and Back
                            if (Controller.Details.Profile.FakeGuideButton && Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonBack.PressedRaw)
                            {
                                Controller.InputCurrent.ButtonStart.PressedRaw = false;
                                Controller.InputCurrent.ButtonBack.PressedRaw = false;
                                Controller.InputCurrent.ButtonGuide.PressedRaw = true;
                            }

                            //Update the controller battery level
                            ControllerReadBatteryLevel(Controller);

                            //Send input to the virtual bus driver
                            await VirtualBusInput(Controller);

                            //Receive output from the virtual bus driver
                            VirtualBusOutput(Controller);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Direct input win data report is out of range or empty, skipping.");
                    }
                }
            }
            catch { }
        }
    }
}