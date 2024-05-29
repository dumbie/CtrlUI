using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static LibraryShared.Classes;
using static LibraryShared.ControllerTimings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private static bool InputUpdateThumbsticks(ControllerStatus controller)
        {
            try
            {
                //Set controller header offset
                int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                //Check thumb type
                int ThumbLeftX = 0;
                int ThumbLeftY = 0;
                int ThumbRightX = 0;
                int ThumbRightY = 0;
                if (controller.SupportedCurrent.OffsetHeader.ThumbLeftZ != null)
                {
                    //Raw left thumbs
                    if (controller.SupportedCurrent.OffsetHeader.ThumbLeftX != null)
                    {
                        int readLeftX = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftX];
                        int readLeftY = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftY];
                        int readLeftZ = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftZ];
                        ThumbLeftX = readLeftX | ((readLeftY & 0x0F) << 8);
                        ThumbLeftY = (readLeftY >> 4) | (readLeftZ << 4);
                        ThumbLeftX -= 2048;
                        ThumbLeftX *= 16;
                        ThumbLeftY -= 2048;
                        ThumbLeftY *= 16;
                    }

                    //Raw right thumbs
                    if (controller.SupportedCurrent.OffsetHeader.ThumbRightX != null)
                    {
                        int readRightX = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightX];
                        int readRightY = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightY];
                        int readRightZ = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightZ];
                        ThumbRightX = readRightX | ((readRightY & 0x0F) << 8);
                        ThumbRightY = (readRightY >> 4) | (readRightZ << 4);
                        ThumbRightX -= 2048;
                        ThumbRightX *= 16;
                        ThumbRightY -= 2048;
                        ThumbRightY *= 16;
                    }
                }
                else
                {
                    //Raw left thumbs
                    if (controller.SupportedCurrent.OffsetHeader.ThumbLeftX != null)
                    {
                        ThumbLeftX = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftX];
                        ThumbLeftY = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbLeftY];
                        ThumbLeftX = (ThumbLeftX * 257) - (int)32767.5;
                        ThumbLeftY = (int)32767.5 - (ThumbLeftY * 257);
                    }

                    //Raw right thumbs
                    if (controller.SupportedCurrent.OffsetHeader.ThumbRightX != null)
                    {
                        ThumbRightX = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightX];
                        ThumbRightY = controller.ControllerDataInput[headerOffset + (int)controller.SupportedCurrent.OffsetHeader.ThumbRightY];
                        ThumbRightX = (ThumbRightX * 257) - (int)32767.5;
                        ThumbRightY = (int)32767.5 - (ThumbRightY * 257);
                    }
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

                //Reverse thumb axes
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

                //Check thumb deadzone
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

                //Check thumbs range
                if (ThumbLeftX > 32767) { ThumbLeftX = 32767; } else if (ThumbLeftX < -32767) { ThumbLeftX = -32767; }
                if (ThumbLeftY > 32767) { ThumbLeftY = 32767; } else if (ThumbLeftY < -32767) { ThumbLeftY = -32767; }
                if (ThumbRightX > 32767) { ThumbRightX = 32767; } else if (ThumbRightX < -32767) { ThumbRightX = -32767; }
                if (ThumbRightY > 32767) { ThumbRightY = 32767; } else if (ThumbRightY < -32767) { ThumbRightY = -32767; }

                //Store thumbs
                controller.InputCurrent.ThumbLeftX = ThumbLeftX;
                controller.InputCurrent.ThumbLeftY = ThumbLeftY;
                controller.InputCurrent.ThumbRightX = ThumbRightX;
                controller.InputCurrent.ThumbRightY = ThumbRightY;

                //Check left thumb stick movement
                if (controller.InputCurrent.ThumbLeftX < -vControllerThumbOffset7500)
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftLeft].PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftLeft].PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbLeftY > vControllerThumbOffset7500)
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftUp].PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftUp].PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbLeftX > vControllerThumbOffset7500)
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftRight].PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftRight].PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbLeftY < -vControllerThumbOffset7500)
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftDown].PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeftDown].PressedRaw = false;
                }

                //Check right thumb stick movement
                if (controller.InputCurrent.ThumbRightX < -vControllerThumbOffset7500)
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightLeft].PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightLeft].PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbRightY > vControllerThumbOffset7500)
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightUp].PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightUp].PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbRightX > vControllerThumbOffset7500)
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightRight].PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightRight].PressedRaw = false;
                }

                if (controller.InputCurrent.ThumbRightY < -vControllerThumbOffset7500)
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightDown].PressedRaw = true;
                }
                else
                {
                    controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRightDown].PressedRaw = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update thumbsticks input: " + ex.Message);
                return false;
            }
        }
    }
}