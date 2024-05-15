using System;
using System.Diagnostics;
using static LibraryShared.Classes;

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
                    controller.InputCurrent.ButtonPressStatus[0] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.A.Group] & (1 << controller.SupportedCurrent.OffsetButton.A.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.B != null)
                {
                    controller.InputCurrent.ButtonPressStatus[1] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.B.Group] & (1 << controller.SupportedCurrent.OffsetButton.B.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.X != null)
                {
                    controller.InputCurrent.ButtonPressStatus[2] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.X.Group] & (1 << controller.SupportedCurrent.OffsetButton.X.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Y != null)
                {
                    controller.InputCurrent.ButtonPressStatus[3] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Y.Group] & (1 << controller.SupportedCurrent.OffsetButton.Y.Offset)) != 0;
                }

                //Buttons (Shoulders, Triggers, Thumbs, Back, Start)
                if (controller.SupportedCurrent.OffsetButton.ShoulderLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[100] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.ShoulderLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.ShoulderLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ShoulderRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[101] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.ShoulderRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.ShoulderRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.TriggerLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[102] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.TriggerLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.TriggerLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.TriggerRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[103] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.TriggerRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.TriggerRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ThumbLeft != null)
                {
                    controller.InputCurrent.ButtonPressStatus[104] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.ThumbLeft.Group] & (1 << controller.SupportedCurrent.OffsetButton.ThumbLeft.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.ThumbRight != null)
                {
                    controller.InputCurrent.ButtonPressStatus[105] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.ThumbRight.Group] & (1 << controller.SupportedCurrent.OffsetButton.ThumbRight.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Back != null)
                {
                    controller.InputCurrent.ButtonPressStatus[106] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Back.Group] & (1 << controller.SupportedCurrent.OffsetButton.Back.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Start != null)
                {
                    controller.InputCurrent.ButtonPressStatus[107] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Start.Group] & (1 << controller.SupportedCurrent.OffsetButton.Start.Offset)) != 0;
                }

                //Buttons (Guide and others)
                if (controller.SupportedCurrent.OffsetButton.Guide != null)
                {
                    controller.InputCurrent.ButtonPressStatus[200] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Guide.Group] & (1 << controller.SupportedCurrent.OffsetButton.Guide.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Touchpad != null)
                {
                    controller.InputCurrent.ButtonPressStatus[201] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Touchpad.Group] & (1 << controller.SupportedCurrent.OffsetButton.Touchpad.Offset)) != 0;
                }
                if (controller.SupportedCurrent.OffsetButton.Media != null)
                {
                    controller.InputCurrent.ButtonPressStatus[202] = (controller.ControllerDataInput[headerOffset + controller.SupportedCurrent.OffsetButton.Media.Group] & (1 << controller.SupportedCurrent.OffsetButton.Media.Offset)) != 0;
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