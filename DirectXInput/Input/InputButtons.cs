using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Classes.ControllerSupported;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Read controller button data switch
        private static ButtonPressStatus ReadButtonDataSwitch(ControllerStatus controller, ControllerButtons controllerButton)
        {
            try
            {
                switch (controllerButton)
                {
                    //Buttons (A, B, X, Y)
                    case ControllerButtons.A:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.A);
                    case ControllerButtons.B:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.B);
                    case ControllerButtons.X:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.X);
                    case ControllerButtons.Y:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Y);
                    //Buttons (Shoulders, Triggers, Thumbs)
                    case ControllerButtons.ShoulderLeft:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.ShoulderLeft);
                    case ControllerButtons.ShoulderRight:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.ShoulderRight);
                    case ControllerButtons.TriggerLeft:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.TriggerLeft);
                    case ControllerButtons.TriggerRight:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.TriggerRight);
                    case ControllerButtons.ThumbLeft:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.ThumbLeft);
                    case ControllerButtons.ThumbRight:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.ThumbRight);
                    //Buttons (Back, Start, Guide)
                    case ControllerButtons.Back:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Back);
                    case ControllerButtons.Start:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Start);
                    case ControllerButtons.Guide:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Guide);
                    //Buttons (Others)
                    case ControllerButtons.One:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.One);
                    case ControllerButtons.Two:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Two);
                    case ControllerButtons.Three:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Three);
                    case ControllerButtons.Four:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Four);
                    case ControllerButtons.Five:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Five);
                    case ControllerButtons.Six:
                        return ReadButtonDataRaw(controller, controller.SupportedCurrent.OffsetButton.Six);
                }
            }
            catch { }
            return ButtonPressStatus.Ignore;
        }

        //Read controller button data raw
        private static ButtonPressStatus ReadButtonDataRaw(ControllerStatus controller, ClassButtonDetails button)
        {
            try
            {
                if (button != null)
                {
                    //Check if controller header matches
                    if (button.Header != null)
                    {
                        //Fix change to byte array check (SequenceEqual)
                        byte headerBytes = controller.ControllerDataInput[0];
                        if (headerBytes != button.Header)
                        {
                            return ButtonPressStatus.Ignore;
                        }
                    }

                    //Set controller header offset
                    int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                    //Check if controller button is pressed
                    if ((controller.ControllerDataInput[headerOffset + (int)button.Group] & (1 << button.Offset)) != 0)
                    {
                        return ButtonPressStatus.Pressed;
                    }
                    else
                    {
                        return ButtonPressStatus.Released;
                    }
                }
            }
            catch { }
            return ButtonPressStatus.Ignore;
        }

        private static void UpdateButtonData(ControllerStatus controller, ControllerButtons controllerButton, ControllerButtons? controllerMapping)
        {
            try
            {
                //Check controller mapping status
                if (vMappingControllerStatus == MappingStatus.Mapping || controllerMapping == null)
                {
                    ButtonPressStatus readButtonData = ReadButtonDataSwitch(controller, controllerButton);
                    if (readButtonData != ButtonPressStatus.Ignore)
                    {
                        controller.InputCurrent.Buttons[(byte)controllerButton].PressedRaw = readButtonData == ButtonPressStatus.Pressed;
                    }
                }
                else
                {
                    ButtonPressStatus readButtonData = ReadButtonDataSwitch(controller, (ControllerButtons)controllerMapping);
                    if (readButtonData != ButtonPressStatus.Ignore)
                    {
                        controller.InputCurrent.Buttons[(byte)controllerButton].PressedRaw = readButtonData == ButtonPressStatus.Pressed;
                    }
                }
            }
            catch { }
        }

        private static bool InputUpdateButtons(ControllerStatus controller)
        {
            try
            {
                //Buttons (A, B, X, Y)
                UpdateButtonData(controller, ControllerButtons.A, controller.Details.Profile.ButtonA);
                UpdateButtonData(controller, ControllerButtons.B, controller.Details.Profile.ButtonB);
                UpdateButtonData(controller, ControllerButtons.X, controller.Details.Profile.ButtonX);
                UpdateButtonData(controller, ControllerButtons.Y, controller.Details.Profile.ButtonY);

                //Buttons (Shoulders, Triggers, Thumbs)
                UpdateButtonData(controller, ControllerButtons.ShoulderLeft, controller.Details.Profile.ButtonShoulderLeft);
                UpdateButtonData(controller, ControllerButtons.ShoulderRight, controller.Details.Profile.ButtonShoulderRight);
                UpdateButtonData(controller, ControllerButtons.TriggerLeft, controller.Details.Profile.ButtonTriggerLeft);
                UpdateButtonData(controller, ControllerButtons.TriggerRight, controller.Details.Profile.ButtonTriggerRight);
                UpdateButtonData(controller, ControllerButtons.ThumbLeft, controller.Details.Profile.ButtonThumbLeft);
                UpdateButtonData(controller, ControllerButtons.ThumbRight, controller.Details.Profile.ButtonThumbRight);

                //Buttons (Back, Start, Guide)
                UpdateButtonData(controller, ControllerButtons.Back, controller.Details.Profile.ButtonBack);
                UpdateButtonData(controller, ControllerButtons.Start, controller.Details.Profile.ButtonStart);
                UpdateButtonData(controller, ControllerButtons.Guide, controller.Details.Profile.ButtonGuide);

                //Buttons (Others)
                UpdateButtonData(controller, ControllerButtons.One, controller.Details.Profile.ButtonOne);
                UpdateButtonData(controller, ControllerButtons.Two, controller.Details.Profile.ButtonTwo);
                UpdateButtonData(controller, ControllerButtons.Three, controller.Details.Profile.ButtonThree);
                UpdateButtonData(controller, ControllerButtons.Four, controller.Details.Profile.ButtonFour);
                UpdateButtonData(controller, ControllerButtons.Five, controller.Details.Profile.ButtonFive);
                UpdateButtonData(controller, ControllerButtons.Six, controller.Details.Profile.ButtonSix);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update buttons input: " + ex.Message);
                return false;
            }
        }
    }
}