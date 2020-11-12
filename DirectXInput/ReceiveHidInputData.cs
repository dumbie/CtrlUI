using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive and Translate DirectInput Controller
        async Task LoopReceiveHidInputData(ControllerStatus Controller)
        {
            try
            {
                Debug.WriteLine("Receive and Translate Hid DirectInput for: " + Controller.Details.DisplayName);

                //Send output to activate controller
                SendXRumbleData(Controller, true, false, false);

                //Receive input from the selected controller
                while (!Controller.InputTask.TaskStopRequest && Controller.HidDevice != null)
                {
                    try
                    {
                        //Read data from the controller
                        bool ReadFile = NativeMethods_Hid.ReadFile(Controller.HidDevice.DeviceHandle, Controller.InputReport, (uint)Controller.InputReport.Length, out uint bytesRead, IntPtr.Zero);

                        //Check if there is data from the controller
                        if (!ReadFile)
                        {
                            Debug.WriteLine("Failed to read input data from hid controller: " + Controller.NumberId);
                            continue;
                        }

                        //Update the controller last read time
                        Controller.LastReadTicks = Stopwatch.GetTimestamp();

                        //Detect and adjust controller header offset
                        if (!Controller.InputHeaderOffsetFinished)
                        {
                            if (Controller.InputReport[Controller.InputHeaderByteOffset] > 4)
                            {
                                Controller.InputHeaderByteOffset++;
                                Debug.WriteLine("Adjusted the controller header offset to: " + Controller.InputHeaderByteOffset);
                                continue;
                            }
                            else
                            {
                                Controller.InputHeaderOffsetFinished = true;
                                Debug.WriteLine("Finished adjusting controller header offset to: " + Controller.InputHeaderByteOffset);
                            }
                        }

                        //Set controller read offfsets
                        int OffsetThumbLeftX = Controller.InputHeaderByteOffset;
                        int OffsetThumbLeftY = Controller.InputHeaderByteOffset;
                        int OffsetThumbRightX = Controller.InputHeaderByteOffset;
                        int OffsetThumbRightY = Controller.InputHeaderByteOffset;
                        int OffsetButtonsGroup1 = Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset; //D-Pad and A,B,X,Y
                        int OffsetButtonsGroup2 = Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset; //ShoulderLeftRight, TriggerLeftRight, ThumbLeftRight, Back, Start
                        int OffsetButtonsGroup3 = Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset; //Guide, Touchpad, Mute
                        int OffsetTriggerLeft = Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset;
                        int OffsetTriggerRight = Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset;
                        if (Controller.Details.Wireless)
                        {
                            OffsetThumbLeftX += Controller.SupportedCurrent.OffsetWireless.ThumbLeftX;
                            OffsetThumbLeftY += Controller.SupportedCurrent.OffsetWireless.ThumbLeftY;
                            OffsetThumbRightX += Controller.SupportedCurrent.OffsetWireless.ThumbRightX;
                            OffsetThumbRightY += Controller.SupportedCurrent.OffsetWireless.ThumbRightY;
                            OffsetButtonsGroup1 += Controller.SupportedCurrent.OffsetWireless.ButtonsGroup1;
                            OffsetButtonsGroup2 += Controller.SupportedCurrent.OffsetWireless.ButtonsGroup2;
                            OffsetButtonsGroup3 += Controller.SupportedCurrent.OffsetWireless.ButtonsGroup3;
                            OffsetTriggerLeft += Controller.SupportedCurrent.OffsetWireless.TriggerLeft;
                            OffsetTriggerRight += Controller.SupportedCurrent.OffsetWireless.TriggerRight;
                        }
                        else
                        {
                            OffsetThumbLeftX += Controller.SupportedCurrent.OffsetUsb.ThumbLeftX;
                            OffsetThumbLeftY += Controller.SupportedCurrent.OffsetUsb.ThumbLeftY;
                            OffsetThumbRightX += Controller.SupportedCurrent.OffsetUsb.ThumbRightX;
                            OffsetThumbRightY += Controller.SupportedCurrent.OffsetUsb.ThumbRightY;
                            OffsetButtonsGroup1 += Controller.SupportedCurrent.OffsetUsb.ButtonsGroup1;
                            OffsetButtonsGroup2 += Controller.SupportedCurrent.OffsetUsb.ButtonsGroup2;
                            OffsetButtonsGroup3 += Controller.SupportedCurrent.OffsetUsb.ButtonsGroup3;
                            OffsetTriggerLeft += Controller.SupportedCurrent.OffsetUsb.TriggerLeft;
                            OffsetTriggerRight += Controller.SupportedCurrent.OffsetUsb.TriggerRight;
                        }

                        //Detect and adjust controller button offset
                        if (Controller.InputReport[OffsetButtonsGroup1] == 255)
                        {
                            Controller.InputButtonByteOffset++;
                            Debug.WriteLine("Adjusted the controller button offset to: " + Controller.InputButtonByteOffset);
                            continue;
                        }

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

                            AVActions.ActionDispatcherInvoke(delegate { cb_ControllerUseButtonTriggers.IsChecked = true; });
                            Controller.Details.Profile.UseButtonTriggers = true;

                            //Save changes to Json file
                            JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                        }

                        //Raw Buttons (Group 1)
                        int ButtonIdGroup1 = 4;
                        for (int ButtonByte = 0; ButtonByte < 20; ButtonByte++)
                        {
                            Controller.InputCurrent.RawBytes[ButtonByte] = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << ButtonIdGroup1)) != 0;
                            ButtonIdGroup1++;
                        }

                        //Raw Buttons (Group 2)
                        int ButtonIdGroup2 = 0;
                        for (int ButtonByte = 20; ButtonByte < 40; ButtonByte++)
                        {
                            Controller.InputCurrent.RawBytes[ButtonByte] = ((byte)Controller.InputReport[OffsetButtonsGroup2] & (1 << ButtonIdGroup2)) != 0;
                            ButtonIdGroup2++;
                        }

                        //Raw Buttons (Group 3)
                        int ButtonIdGroup3 = 0;
                        for (int ButtonByte = 40; ButtonByte < 45; ButtonByte++)
                        {
                            Controller.InputCurrent.RawBytes[ButtonByte] = ((byte)Controller.InputReport[OffsetButtonsGroup3] & (1 << ButtonIdGroup3)) != 0;
                            ButtonIdGroup3++;
                        }

                        //Raw DPad (Group 1)
                        Controller.InputCurrent.DPadLeft.PressedRaw = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 1)) != 0;
                        Controller.InputCurrent.DPadUp.PressedRaw = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 3)) != 0;
                        Controller.InputCurrent.DPadRight.PressedRaw = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 0)) != 0;
                        Controller.InputCurrent.DPadDown.PressedRaw = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 2)) != 0;
                        byte DPadState = (byte)(((Controller.InputCurrent.DPadRight.PressedRaw ? 1 : 0) << 0) | ((Controller.InputCurrent.DPadLeft.PressedRaw ? 1 : 0) << 1) | ((Controller.InputCurrent.DPadDown.PressedRaw ? 1 : 0) << 2) | ((Controller.InputCurrent.DPadUp.PressedRaw ? 1 : 0) << 3));
                        if (Controller.Details.Profile.DPadFourWayMovement)
                        {
                            switch (DPadState)
                            {
                                case 0: Controller.InputCurrent.DPadUp.PressedRaw = true; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 2: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 4: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = true; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 6: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = true; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                default: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                            }
                        }
                        else
                        {
                            switch (DPadState)
                            {
                                case 0: Controller.InputCurrent.DPadUp.PressedRaw = true; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 1: Controller.InputCurrent.DPadUp.PressedRaw = true; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 2: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 3: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = true; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = true; break;
                                case 4: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = true; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 5: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = true; Controller.InputCurrent.DPadLeft.PressedRaw = true; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 6: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = true; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                case 7: Controller.InputCurrent.DPadUp.PressedRaw = true; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = true; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                                default: Controller.InputCurrent.DPadUp.PressedRaw = false; Controller.InputCurrent.DPadDown.PressedRaw = false; Controller.InputCurrent.DPadLeft.PressedRaw = false; Controller.InputCurrent.DPadRight.PressedRaw = false; break;
                            }
                        }

                        //Save controller button mapping
                        bool controllerMapping = ControllerSaveMapping(Controller);
                        if (!controllerMapping)
                        {
                            //Set button mapping input data
                            if (Controller.Details.Profile.ButtonA == null) { Controller.InputCurrent.ButtonA.PressedRaw = Controller.InputCurrent.RawBytes[1]; }
                            else if (Controller.Details.Profile.ButtonA != -1) { Controller.InputCurrent.ButtonA.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonA.Value]; }

                            if (Controller.Details.Profile.ButtonB == null) { Controller.InputCurrent.ButtonB.PressedRaw = Controller.InputCurrent.RawBytes[2]; }
                            else if (Controller.Details.Profile.ButtonB != -1) { Controller.InputCurrent.ButtonB.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonB.Value]; }

                            if (Controller.Details.Profile.ButtonX == null) { Controller.InputCurrent.ButtonX.PressedRaw = Controller.InputCurrent.RawBytes[0]; }
                            else if (Controller.Details.Profile.ButtonX != -1) { Controller.InputCurrent.ButtonX.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonX.Value]; }

                            if (Controller.Details.Profile.ButtonY == null) { Controller.InputCurrent.ButtonY.PressedRaw = Controller.InputCurrent.RawBytes[3]; }
                            else if (Controller.Details.Profile.ButtonY != -1) { Controller.InputCurrent.ButtonY.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonY.Value]; }

                            if (Controller.Details.Profile.ButtonBack == null) { Controller.InputCurrent.ButtonBack.PressedRaw = Controller.InputCurrent.RawBytes[24]; }
                            else if (Controller.Details.Profile.ButtonBack != -1) { Controller.InputCurrent.ButtonBack.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonBack.Value]; }

                            if (Controller.Details.Profile.ButtonStart == null) { Controller.InputCurrent.ButtonStart.PressedRaw = Controller.InputCurrent.RawBytes[25]; }
                            else if (Controller.Details.Profile.ButtonStart != -1) { Controller.InputCurrent.ButtonStart.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonStart.Value]; }

                            if (Controller.Details.Profile.ButtonGuide == null) { Controller.InputCurrent.ButtonGuide.PressedRaw = Controller.InputCurrent.RawBytes[40]; }
                            else if (Controller.Details.Profile.ButtonGuide != -1) { Controller.InputCurrent.ButtonGuide.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonGuide.Value]; }

                            if (Controller.Details.Profile.ButtonTriggerLeft == null) { Controller.InputCurrent.ButtonTriggerLeft.PressedRaw = Controller.InputCurrent.RawBytes[22]; }
                            else if (Controller.Details.Profile.ButtonTriggerLeft != -1) { Controller.InputCurrent.ButtonTriggerLeft.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonTriggerLeft.Value]; }

                            if (Controller.Details.Profile.ButtonTriggerRight == null) { Controller.InputCurrent.ButtonTriggerRight.PressedRaw = Controller.InputCurrent.RawBytes[23]; }
                            else if (Controller.Details.Profile.ButtonTriggerRight != -1) { Controller.InputCurrent.ButtonTriggerRight.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonTriggerRight.Value]; }

                            if (Controller.Details.Profile.ButtonShoulderLeft == null) { Controller.InputCurrent.ButtonShoulderLeft.PressedRaw = Controller.InputCurrent.RawBytes[20]; }
                            else if (Controller.Details.Profile.ButtonShoulderLeft != -1) { Controller.InputCurrent.ButtonShoulderLeft.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonShoulderLeft.Value]; }

                            if (Controller.Details.Profile.ButtonShoulderRight == null) { Controller.InputCurrent.ButtonShoulderRight.PressedRaw = Controller.InputCurrent.RawBytes[21]; }
                            else if (Controller.Details.Profile.ButtonShoulderRight != -1) { Controller.InputCurrent.ButtonShoulderRight.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonShoulderRight.Value]; }

                            if (Controller.Details.Profile.ButtonThumbLeft == null) { Controller.InputCurrent.ButtonThumbLeft.PressedRaw = Controller.InputCurrent.RawBytes[26]; }
                            else if (Controller.Details.Profile.ButtonThumbLeft != -1) { Controller.InputCurrent.ButtonThumbLeft.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonThumbLeft.Value]; }

                            if (Controller.Details.Profile.ButtonThumbRight == null) { Controller.InputCurrent.ButtonThumbRight.PressedRaw = Controller.InputCurrent.RawBytes[27]; }
                            else if (Controller.Details.Profile.ButtonThumbRight != -1) { Controller.InputCurrent.ButtonThumbRight.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonThumbRight.Value]; }

                            //Fake Guide button press with Start and Back
                            if (Controller.Details.Profile.FakeGuideButton && Controller.InputCurrent.ButtonStart.PressedRaw && Controller.InputCurrent.ButtonBack.PressedRaw)
                            {
                                Controller.InputCurrent.ButtonStart.PressedRaw = false;
                                Controller.InputCurrent.ButtonBack.PressedRaw = false;
                                Controller.InputCurrent.ButtonGuide.PressedRaw = true;
                            }

                            //Update the controller battery level
                            ControllerUpdateBatteryLevel(Controller);

                            //Send the prepared controller data
                            await SendControllerData(Controller);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Direct input hid data report is out of range or empty, skipping.");
                    }
                }
            }
            catch { }
        }
    }
}