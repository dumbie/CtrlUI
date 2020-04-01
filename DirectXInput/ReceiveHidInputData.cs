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
        async Task ReceiveHidInputData(ControllerStatus Controller)
        {
            try
            {
                Debug.WriteLine("Receive and Translate Hid DirectInput for: " + Controller.Details.DisplayName);

                //Send output to activate controller
                SendXRumbleData(Controller, true, false, false);

                //Receive input from the selected controller
                while (Controller.InputTask != null && Controller.HidDevice != null)
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

                        //Update the last controller active time
                        Controller.LastActive = Environment.TickCount;

                        //Detect controller header offset
                        if (Controller.InputReport[0 + Controller.InputHeaderByteOffset] > 4)
                        {
                            Controller.InputHeaderByteOffset++;
                            Debug.WriteLine("Adjusted the controller header offset to: " + Controller.InputHeaderByteOffset);
                            continue;
                        }

                        //Offsets for thumb sticks
                        int OffsetThumbLeftX = 1 + Controller.InputHeaderByteOffset;
                        int OffsetThumbLeftY = 2 + Controller.InputHeaderByteOffset;
                        int OffsetThumbRightX = 3 + Controller.InputHeaderByteOffset;
                        int OffsetThumbRightY = 4 + Controller.InputHeaderByteOffset;

                        //Offsets for DPad and Buttons
                        int OffsetButtonsGroup1 = 5 + Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset; //D-Pad and A,B,X,Y
                        int OffsetButtonsGroup2 = 6 + Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset; //ShoulderLeftRight, TriggerLeftRight, ThumbLeftRight, Back, Start
                        int OffsetButtonsGroup3 = 7 + Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset; //Guide, Touchpad

                        //Offsets for Triggers
                        int OffsetTriggerLeft = 8 + Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset;
                        int OffsetTriggerRight = 9 + Controller.InputHeaderByteOffset + Controller.InputButtonByteOffset;

                        //Detect controller button offset
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
                        for (int ButtonByte = 40; ButtonByte < 42; ButtonByte++)
                        {
                            Controller.InputCurrent.RawBytes[ButtonByte] = ((byte)Controller.InputReport[OffsetButtonsGroup3] & (1 << ButtonIdGroup3)) != 0;
                            ButtonIdGroup3++;
                        }

                        //Raw DPad (Group 1)
                        Controller.InputCurrent.DPadLeft = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 1)) != 0;
                        Controller.InputCurrent.DPadUp = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 3)) != 0;
                        Controller.InputCurrent.DPadRight = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 0)) != 0;
                        Controller.InputCurrent.DPadDown = ((byte)Controller.InputReport[OffsetButtonsGroup1] & (1 << 2)) != 0;
                        byte DPadState = (byte)(((Controller.InputCurrent.DPadRight ? 1 : 0) << 0) | ((Controller.InputCurrent.DPadLeft ? 1 : 0) << 1) | ((Controller.InputCurrent.DPadDown ? 1 : 0) << 2) | ((Controller.InputCurrent.DPadUp ? 1 : 0) << 3));
                        if (Controller.Details.Profile.DPadFourWayMovement)
                        {
                            switch (DPadState)
                            {
                                case 0: Controller.InputCurrent.DPadUp = true; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = false; break;
                                case 2: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = true; break;
                                case 4: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = true; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = false; break;
                                case 6: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = true; Controller.InputCurrent.DPadRight = false; break;
                                default: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = false; break;
                            }
                        }
                        else
                        {
                            switch (DPadState)
                            {
                                case 0: Controller.InputCurrent.DPadUp = true; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = false; break;
                                case 1: Controller.InputCurrent.DPadUp = true; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = true; break;
                                case 2: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = true; break;
                                case 3: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = true; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = true; break;
                                case 4: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = true; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = false; break;
                                case 5: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = true; Controller.InputCurrent.DPadLeft = true; Controller.InputCurrent.DPadRight = false; break;
                                case 6: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = true; Controller.InputCurrent.DPadRight = false; break;
                                case 7: Controller.InputCurrent.DPadUp = true; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = true; Controller.InputCurrent.DPadRight = false; break;
                                default: Controller.InputCurrent.DPadUp = false; Controller.InputCurrent.DPadDown = false; Controller.InputCurrent.DPadLeft = false; Controller.InputCurrent.DPadRight = false; break;
                            }
                        }

                        //Check if controller mapping is enabled
                        if (Controller.Mapping[0] == "Map")
                        {
                            //Store new button mapping in Json controller
                            int ButtonMapId = Array.FindIndex(Controller.InputCurrent.RawBytes, ButtonPressed => ButtonPressed);
                            if (ButtonMapId != -1)
                            {
                                Debug.WriteLine("Mapped button " + Controller.Mapping[1] + " to: " + ButtonMapId);
                                if (Controller.Mapping[1] == "Button A") { Controller.Details.Profile.ButtonA = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button B") { Controller.Details.Profile.ButtonB = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button X") { Controller.Details.Profile.ButtonX = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button Y") { Controller.Details.Profile.ButtonY = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button LB") { Controller.Details.Profile.ButtonShoulderLeft = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button RB") { Controller.Details.Profile.ButtonShoulderRight = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button Back") { Controller.Details.Profile.ButtonBack = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button Start") { Controller.Details.Profile.ButtonStart = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button Guide") { Controller.Details.Profile.ButtonGuide = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button Thumb Left") { Controller.Details.Profile.ButtonThumbLeft = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button Thumb Right") { Controller.Details.Profile.ButtonThumbRight = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button Trigger Left") { Controller.Details.Profile.ButtonTriggerLeft = ButtonMapId; }
                                if (Controller.Mapping[1] == "Button Trigger Right") { Controller.Details.Profile.ButtonTriggerRight = ButtonMapId; }

                                //Reset controller button mapping
                                Controller.Mapping[0] = "Done";
                                Controller.Mapping[1] = "None";

                                //Save changes to Json file
                                JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");
                            }
                        }
                        else
                        {
                            //Set button mapping input data
                            if (Controller.Details.Profile.ButtonA == null) { Controller.InputCurrent.ButtonA = Controller.InputCurrent.RawBytes[1]; }
                            else if (Controller.Details.Profile.ButtonA != -1) { Controller.InputCurrent.ButtonA = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonA.Value]; }

                            if (Controller.Details.Profile.ButtonB == null) { Controller.InputCurrent.ButtonB = Controller.InputCurrent.RawBytes[2]; }
                            else if (Controller.Details.Profile.ButtonB != -1) { Controller.InputCurrent.ButtonB = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonB.Value]; }

                            if (Controller.Details.Profile.ButtonX == null) { Controller.InputCurrent.ButtonX = Controller.InputCurrent.RawBytes[0]; }
                            else if (Controller.Details.Profile.ButtonX != -1) { Controller.InputCurrent.ButtonX = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonX.Value]; }

                            if (Controller.Details.Profile.ButtonY == null) { Controller.InputCurrent.ButtonY = Controller.InputCurrent.RawBytes[3]; }
                            else if (Controller.Details.Profile.ButtonY != -1) { Controller.InputCurrent.ButtonY = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonY.Value]; }

                            if (Controller.Details.Profile.ButtonBack == null) { Controller.InputCurrent.ButtonBack = Controller.InputCurrent.RawBytes[24]; }
                            else if (Controller.Details.Profile.ButtonBack != -1) { Controller.InputCurrent.ButtonBack = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonBack.Value]; }

                            if (Controller.Details.Profile.ButtonStart == null) { Controller.InputCurrent.ButtonStart = Controller.InputCurrent.RawBytes[25]; }
                            else if (Controller.Details.Profile.ButtonStart != -1) { Controller.InputCurrent.ButtonStart = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonStart.Value]; }

                            if (Controller.Details.Profile.ButtonGuide == null) { Controller.InputCurrent.ButtonGuide = Controller.InputCurrent.RawBytes[40]; }
                            else if (Controller.Details.Profile.ButtonGuide != -1) { Controller.InputCurrent.ButtonGuide = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonGuide.Value]; }

                            if (Controller.Details.Profile.ButtonTriggerLeft == null) { Controller.InputCurrent.ButtonTriggerLeft = Controller.InputCurrent.RawBytes[22]; }
                            else if (Controller.Details.Profile.ButtonTriggerLeft != -1) { Controller.InputCurrent.ButtonTriggerLeft = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonTriggerLeft.Value]; }

                            if (Controller.Details.Profile.ButtonTriggerRight == null) { Controller.InputCurrent.ButtonTriggerRight = Controller.InputCurrent.RawBytes[23]; }
                            else if (Controller.Details.Profile.ButtonTriggerRight != -1) { Controller.InputCurrent.ButtonTriggerRight = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonTriggerRight.Value]; }

                            if (Controller.Details.Profile.ButtonShoulderLeft == null) { Controller.InputCurrent.ButtonShoulderLeft = Controller.InputCurrent.RawBytes[20]; }
                            else if (Controller.Details.Profile.ButtonShoulderLeft != -1) { Controller.InputCurrent.ButtonShoulderLeft = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonShoulderLeft.Value]; }

                            if (Controller.Details.Profile.ButtonShoulderRight == null) { Controller.InputCurrent.ButtonShoulderRight = Controller.InputCurrent.RawBytes[21]; }
                            else if (Controller.Details.Profile.ButtonShoulderRight != -1) { Controller.InputCurrent.ButtonShoulderRight = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonShoulderRight.Value]; }

                            if (Controller.Details.Profile.ButtonThumbLeft == null) { Controller.InputCurrent.ButtonThumbLeft = Controller.InputCurrent.RawBytes[26]; }
                            else if (Controller.Details.Profile.ButtonThumbLeft != -1) { Controller.InputCurrent.ButtonThumbLeft = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonThumbLeft.Value]; }

                            if (Controller.Details.Profile.ButtonThumbRight == null) { Controller.InputCurrent.ButtonThumbRight = Controller.InputCurrent.RawBytes[27]; }
                            else if (Controller.Details.Profile.ButtonThumbRight != -1) { Controller.InputCurrent.ButtonThumbRight = Controller.InputCurrent.RawBytes[Controller.Details.Profile.ButtonThumbRight.Value]; }

                            //Fake Guide button press with Start and Back
                            if (Controller.Details.Profile.FakeGuideButton && Controller.InputCurrent.ButtonStart && Controller.InputCurrent.ButtonBack)
                            {
                                Controller.InputCurrent.ButtonStart = false;
                                Controller.InputCurrent.ButtonBack = false;
                                Controller.InputCurrent.ButtonGuide = true;
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