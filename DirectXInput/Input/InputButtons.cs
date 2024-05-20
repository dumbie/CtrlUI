using System;
using System.Diagnostics;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private static bool InputUpdateButtons(ControllerStatus controller)
        {
            try
            {
                //Set controller header offset
                int headerOffset = controller.Details.Wireless ? controller.SupportedCurrent.OffsetWireless : controller.SupportedCurrent.OffsetWired;

                //Buttons (A, B, X, Y)
                if (controller.SupportedCurrent.OffsetButton.A != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.A] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.A.Group] & (1 << controller.SupportedCurrent.OffsetButton.A.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.B != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.B] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.B.Group] & (1 << controller.SupportedCurrent.OffsetButton.B.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.X != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.X] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.X.Group] & (1 << controller.SupportedCurrent.OffsetButton.X.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Y != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Y] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Y.Group] & (1 << controller.SupportedCurrent.OffsetButton.Y.Offset)) != 0;
                }

                //Buttons (Shoulders, Triggers, Thumbs, Back, Start)
                if (controller.SupportedCurrent.OffsetButton.ShoulderLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.ShoulderLeft] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.ShoulderLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.ShoulderLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ShoulderRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.ShoulderRight] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.ShoulderRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.ShoulderRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.TriggerLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.TriggerLeft] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.TriggerLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.TriggerLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.TriggerRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.TriggerRight] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.TriggerRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.TriggerRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ThumbLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.ThumbLeft] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.ThumbLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.ThumbLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ThumbRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.ThumbRight] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.ThumbRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.ThumbRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Back != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Back] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Back.Group] & (1 << controller.SupportedCurrent.OffsetButton.Back.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Start != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Start] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Start.Group] & (1 << controller.SupportedCurrent.OffsetButton.Start.Offset)) != 0;
                }

                //Buttons (Guide and others)
                if (controller.SupportedCurrent.OffsetButton.Guide != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Guide] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Guide.Group] & (1 << controller.SupportedCurrent.OffsetButton.Guide.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.One != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.One] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.One.Group] & (1 << controller.SupportedCurrent.OffsetButton.One.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Two != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Two] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Two.Group] & (1 << controller.SupportedCurrent.OffsetButton.Two.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Three != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Three] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Three.Group] & (1 << controller.SupportedCurrent.OffsetButton.Three.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Four != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Four] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Four.Group] & (1 << controller.SupportedCurrent.OffsetButton.Four.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Five != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Five] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Five.Group] & (1 << controller.SupportedCurrent.OffsetButton.Five.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Six != null)
                {
                    controller.InputCurrent.ButtonPressStatus[(int)ControllerButtonIds.Six] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Six.Group] & (1 << controller.SupportedCurrent.OffsetButton.Six.Offset)) != 0;
                }

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