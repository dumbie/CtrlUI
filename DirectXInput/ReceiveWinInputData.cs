using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive and Translate DirectInput Controller
        async Task LoopReceiveWinInputData(ControllerStatus Controller)
        {
            try
            {
                Debug.WriteLine("Receive and Translate Win DirectInput for: " + Controller.Details.DisplayName);

                //Wake up USB controllers
                IEnumerable<ControllerSupported> TargetController = vDirectControllersSupported.Where(x => x.ProductIDs.Any(z => z.ToLower() == Controller.Details.Profile.ProductID.ToLower() && x.VendorID.ToLower() == Controller.Details.Profile.VendorID.ToLower()));
                if (TargetController.Any(x => x.CodeName == "SonyDualShock3"))
                {
                    Debug.WriteLine("Waking up USB controller: SonyDualShock3");
                    byte[] EnableBytes = { 0x42, 0x0C, 0x00, 0x00 };
                    int Transferred = 0;
                    Controller.WinUsbDevice.SendTransfer(0x21, 0x09, 0x3F4, EnableBytes, ref Transferred);
                }
                else if (TargetController.Any(x => x.CodeName == "SonyMoveNavigation3"))
                {
                    Debug.WriteLine("Waking up USB controller: SonyMoveNavigation3");
                    byte[] EnableBytes = { 0x42, 0x0C, 0x00, 0x00 };
                    int Transferred = 0;
                    Controller.WinUsbDevice.SendTransfer(0x21, 0x09, 0x3F4, EnableBytes, ref Transferred);
                }

                //Send output to activate controller
                SendXRumbleData(Controller, true, false, false);

                //Receive input from the selected controller
                while (!Controller.InputTask.TaskStopRequest && Controller.WinUsbDevice != null && Controller.WinUsbDevice.IsActive)
                {
                    try
                    {
                        //Read data from the controller
                        int Transferred = 0;
                        bool Readed = Controller.WinUsbDevice.ReadIntPipe(Controller.InputReport, Controller.InputReport.Length, ref Transferred) && Transferred > 0;

                        //Check if there is data from the controller
                        if (!Readed)
                        {
                            Debug.WriteLine("Failed to read input data from win controller: " + Controller.NumberId);
                            continue;
                        }

                        //Update the last controller active time
                        Controller.LastActive = Environment.TickCount;

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

                        //Check the thumbs range
                        if (ThumbLeftX > 32767) { ThumbLeftX = 32767; }
                        if (ThumbLeftY > 32767) { ThumbLeftY = 32767; }
                        if (ThumbRightX > 32767) { ThumbRightX = 32767; }
                        if (ThumbRightY > 32767) { ThumbRightY = 32767; }

                        //Store the thumbs
                        Controller.InputCurrent.ThumbLeftX = ThumbLeftX;
                        Controller.InputCurrent.ThumbLeftY = ThumbLeftY;
                        Controller.InputCurrent.ThumbRightX = ThumbRightX;
                        Controller.InputCurrent.ThumbRightY = ThumbRightY;

                        //Raw Triggers
                        if (Controller.InputReport.Length >= OffsetTriggerRight)
                        {
                            Controller.InputCurrent.TriggerLeft = Controller.InputReport[OffsetTriggerLeft];
                            Controller.InputCurrent.TriggerRight = Controller.InputReport[OffsetTriggerRight];
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
                        int ButtonIdGroup1 = 0;
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
                        for (int ButtonByte = 40; ButtonByte < 42; ButtonByte++)
                        {
                            Controller.InputCurrent.RawBytes[ButtonByte] = ((byte)Controller.InputReport[OffsetButtonsGroup3] & (1 << ButtonIdGroup3)) != 0;
                            ButtonIdGroup3++;
                        }

                        //Raw DPad (Group 1)
                        Controller.InputCurrent.DPadLeft.PressedRaw = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 5)) != 0;
                        Controller.InputCurrent.DPadUp.PressedRaw = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 7)) != 0;
                        Controller.InputCurrent.DPadRight.PressedRaw = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 4)) != 0;
                        Controller.InputCurrent.DPadDown.PressedRaw = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 6)) != 0;
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
                        bool controllerMapping = ControllerSaveMapping(Controller);
                        if (!controllerMapping)
                        {
                            //Set button mapping input data
                            if (Controller.Details.Profile.ButtonA == null) { Controller.InputCurrent.ButtonA.PressedRaw = Controller.InputCurrent.RawBytes[26]; }
                            else if (Controller.Details.Profile.ButtonA != -1) { Controller.InputCurrent.ButtonA.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonA.Value]; }

                            if (Controller.Details.Profile.ButtonB == null) { Controller.InputCurrent.ButtonB.PressedRaw = Controller.InputCurrent.RawBytes[25]; }
                            else if (Controller.Details.Profile.ButtonB != -1) { Controller.InputCurrent.ButtonB.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonB.Value]; }

                            if (Controller.Details.Profile.ButtonX == null) { Controller.InputCurrent.ButtonX.PressedRaw = Controller.InputCurrent.RawBytes[27]; }
                            else if (Controller.Details.Profile.ButtonX != -1) { Controller.InputCurrent.ButtonX.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonX.Value]; }

                            if (Controller.Details.Profile.ButtonY == null) { Controller.InputCurrent.ButtonY.PressedRaw = Controller.InputCurrent.RawBytes[24]; }
                            else if (Controller.Details.Profile.ButtonY != -1) { Controller.InputCurrent.ButtonY.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonY.Value]; }

                            if (Controller.Details.Profile.ButtonBack == null) { Controller.InputCurrent.ButtonBack.PressedRaw = Controller.InputCurrent.RawBytes[0]; }
                            else if (Controller.Details.Profile.ButtonBack != -1) { Controller.InputCurrent.ButtonBack.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonBack.Value]; }

                            if (Controller.Details.Profile.ButtonStart == null) { Controller.InputCurrent.ButtonStart.PressedRaw = Controller.InputCurrent.RawBytes[3]; }
                            else if (Controller.Details.Profile.ButtonStart != -1) { Controller.InputCurrent.ButtonStart.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonStart.Value]; }

                            if (Controller.Details.Profile.ButtonGuide == null) { Controller.InputCurrent.ButtonGuide.PressedRaw = Controller.InputCurrent.RawBytes[40]; }
                            else if (Controller.Details.Profile.ButtonGuide != -1) { Controller.InputCurrent.ButtonGuide.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonGuide.Value]; }

                            if (Controller.Details.Profile.ButtonTriggerLeft == null) { Controller.InputCurrent.ButtonTriggerLeft.PressedRaw = Controller.InputCurrent.RawBytes[20]; }
                            else if (Controller.Details.Profile.ButtonTriggerLeft != -1) { Controller.InputCurrent.ButtonTriggerLeft.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonTriggerLeft.Value]; }

                            if (Controller.Details.Profile.ButtonTriggerRight == null) { Controller.InputCurrent.ButtonTriggerRight.PressedRaw = Controller.InputCurrent.RawBytes[21]; }
                            else if (Controller.Details.Profile.ButtonTriggerRight != -1) { Controller.InputCurrent.ButtonTriggerRight.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonTriggerRight.Value]; }

                            if (Controller.Details.Profile.ButtonShoulderLeft == null) { Controller.InputCurrent.ButtonShoulderLeft.PressedRaw = Controller.InputCurrent.RawBytes[22]; }
                            else if (Controller.Details.Profile.ButtonShoulderLeft != -1) { Controller.InputCurrent.ButtonShoulderLeft.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonShoulderLeft.Value]; }

                            if (Controller.Details.Profile.ButtonShoulderRight == null) { Controller.InputCurrent.ButtonShoulderRight.PressedRaw = Controller.InputCurrent.RawBytes[23]; }
                            else if (Controller.Details.Profile.ButtonShoulderRight != -1) { Controller.InputCurrent.ButtonShoulderRight.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonShoulderRight.Value]; }

                            if (Controller.Details.Profile.ButtonThumbLeft == null) { Controller.InputCurrent.ButtonThumbLeft.PressedRaw = Controller.InputCurrent.RawBytes[1]; }
                            else if (Controller.Details.Profile.ButtonThumbLeft != -1) { Controller.InputCurrent.ButtonThumbLeft.PressedRaw = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonThumbLeft.Value]; }

                            if (Controller.Details.Profile.ButtonThumbRight == null) { Controller.InputCurrent.ButtonThumbRight.PressedRaw = Controller.InputCurrent.RawBytes[2]; }
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
                        Debug.WriteLine("Direct input data report is out of range or empty, skipping.");
                    }
                }
            }
            catch { }
        }
    }
}