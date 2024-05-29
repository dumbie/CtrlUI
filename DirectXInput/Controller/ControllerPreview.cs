using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Update controller information
        void UpdateControllerInformation()
        {
            try
            {
                //Update the interface when window is active
                if (vAppActivated && !vAppMinimized)
                {
                    ControllerStatus activeController = vActiveController();
                    if (activeController != null && activeController.Connected())
                    {
                        //Update controller information
                        UpdateControllerInformation(activeController);

                        //Update controller preview information
                        if (vShowControllerPreview)
                        {
                            UpdateControllerPreviewInformation(activeController);
                            UpdateControllerPreviewRumble(activeController);
                        }

                        //Update controller debug information
                        if (vShowControllerDebug)
                        {
                            UpdateControllerDebugInformation(activeController);
                        }
                    }
                }
            }
            catch { }
        }

        //Update interface controller rumble preview
        void UpdateControllerPreviewRumble(ControllerStatus Controller)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    if (Controller.RumbleCurrentHeavy > 0 || Controller.RumbleCurrentLight > 0)
                    {
                        img_ControllerPreview_ControllerRumble.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        img_ControllerPreview_ControllerRumble.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch { }
        }

        //Update interface controller information
        void UpdateControllerInformation(ControllerStatus Controller)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Update name and type
                    txt_ActiveControllerType.Text = Controller.Details.Wireless ? "Wireless" : "Wired";
                    txt_ActiveControllerName.Text = Controller.Details.DisplayName;
                    txt_ActiveControllerName.Foreground = new SolidColorBrush((Color)Controller.Color);

                    //Update latency
                    long latencyMs = Controller.TicksInputLast - Controller.TicksInputPrev;
                    txt_ActiveControllerLatency.Text = "Latency " + latencyMs + "ms";

                    //Update battery
                    if (Controller.BatteryCurrent.BatteryStatus == BatteryStatus.Charging)
                    {
                        txt_ActiveControllerBattery.Text = "Battery charging";
                    }
                    else if (Controller.BatteryCurrent.BatteryStatus == BatteryStatus.Unknown)
                    {
                        txt_ActiveControllerBattery.Text = "Battery unknown";
                    }
                    else
                    {
                        txt_ActiveControllerBattery.Text = "Battery is at " + Controller.BatteryCurrent.BatteryPercentage + "%";
                    }
                });
            }
            catch { }
        }

        //Update interface controller buttons preview
        void UpdateControllerPreviewInformation(ControllerStatus controller)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //DPad
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw) { img_ControllerPreview_DPadLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadLeft.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw) { img_ControllerPreview_DPadUp.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadUp.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw) { img_ControllerPreview_DPadRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadRight.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw) { img_ControllerPreview_DPadDown.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadDown.Visibility = Visibility.Collapsed; }

                    //Buttons
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.A].PressedRaw) { img_ControllerPreview_ButtonA.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonA.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.B].PressedRaw) { img_ControllerPreview_ButtonB.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonB.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.X].PressedRaw) { img_ControllerPreview_ButtonX.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonX.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Y].PressedRaw) { img_ControllerPreview_ButtonY.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonY.Visibility = Visibility.Collapsed; }

                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Back].PressedRaw) { img_ControllerPreview_ButtonBack.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonBack.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Start].PressedRaw) { img_ControllerPreview_ButtonStart.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonStart.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Guide].PressedRaw) { img_ControllerPreview_ButtonGuide.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonGuide.Visibility = Visibility.Collapsed; }

                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.One].PressedRaw) { img_ControllerPreview_ButtonOne.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonOne.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Two].PressedRaw) { img_ControllerPreview_ButtonTwo.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonTwo.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Three].PressedRaw) { img_ControllerPreview_ButtonThree.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonThree.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Four].PressedRaw) { img_ControllerPreview_ButtonFour.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonFour.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Five].PressedRaw) { img_ControllerPreview_ButtonFive.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonFive.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.Six].PressedRaw) { img_ControllerPreview_ButtonSix.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonSix.Visibility = Visibility.Collapsed; }

                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ShoulderLeft].PressedRaw) { img_ControllerPreview_ButtonShoulderLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonShoulderLeft.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ShoulderRight].PressedRaw) { img_ControllerPreview_ButtonShoulderRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonShoulderRight.Visibility = Visibility.Collapsed; }

                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbLeft].PressedRaw) { img_ControllerPreview_ButtonThumbLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonThumbLeft.Visibility = Visibility.Collapsed; }
                    if (controller.InputCurrent.Buttons[(byte)ControllerButtons.ThumbRight].PressedRaw) { img_ControllerPreview_ButtonThumbRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonThumbRight.Visibility = Visibility.Collapsed; }

                    //Triggers
                    textblock_Trigger_Left.Text = controller.InputCurrent.TriggerLeft.ToString();
                    textblock_Trigger_Right.Text = controller.InputCurrent.TriggerRight.ToString();
                    img_ControllerPreview_TriggerLeft.Opacity = (double)(controller.InputCurrent.TriggerLeft * 257) / 65535;
                    img_ControllerPreview_TriggerRight.Opacity = (double)(controller.InputCurrent.TriggerRight * 257) / 65535;

                    //Thumb Left and Right Image
                    int LeftX = controller.InputCurrent.ThumbLeftX / 1000;
                    int LeftY = -controller.InputCurrent.ThumbLeftY / 1000;
                    int RightX = controller.InputCurrent.ThumbRightX / 1000;
                    int RightY = -controller.InputCurrent.ThumbRightY / 1000;
                    img_ControllerPreview_LeftAxe.Margin = new Thickness(LeftX, LeftY, 0, 0);
                    img_ControllerPreview_RightAxe.Margin = new Thickness(RightX, RightY, 0, 0);

                    //Thumb Left and Right Percentage
                    int PercentageLeftYUp = (controller.InputCurrent.ThumbLeftY * 257) / 65535;
                    if (PercentageLeftYUp < 0) { PercentageLeftYUp = 0; }
                    int PercentageLeftYDown = (-controller.InputCurrent.ThumbLeftY * 257) / 65535;
                    if (PercentageLeftYDown < 0) { PercentageLeftYDown = 0; }
                    int PercentageLeftXLeft = (-controller.InputCurrent.ThumbLeftX * 257) / 65535;
                    if (PercentageLeftXLeft < 0) { PercentageLeftXLeft = 0; }
                    int PercentageLeftXRight = (controller.InputCurrent.ThumbLeftX * 257) / 65535;
                    if (PercentageLeftXRight < 0) { PercentageLeftXRight = 0; }
                    textblock_Thumb_Left_Y_Up.Text = PercentageLeftYUp.ToString();
                    textblock_Thumb_Left_Y_Down.Text = PercentageLeftYDown.ToString();
                    textblock_Thumb_Left_X_Left.Text = PercentageLeftXLeft.ToString();
                    textblock_Thumb_Left_X_Right.Text = PercentageLeftXRight.ToString();
                    int PercentageRightYUp = (controller.InputCurrent.ThumbRightY * 257) / 65535;
                    if (PercentageRightYUp < 0) { PercentageRightYUp = 0; }
                    int PercentageRightYDown = (-controller.InputCurrent.ThumbRightY * 257) / 65535;
                    if (PercentageRightYDown < 0) { PercentageRightYDown = 0; }
                    int PercentageRightXLeft = (-controller.InputCurrent.ThumbRightX * 257) / 65535;
                    if (PercentageRightXLeft < 0) { PercentageRightXLeft = 0; }
                    int PercentageRightXRight = (controller.InputCurrent.ThumbRightX * 257) / 65535;
                    if (PercentageRightXRight < 0) { PercentageRightXRight = 0; }
                    textblock_Thumb_Right_Y_Up.Text = PercentageRightYUp.ToString();
                    textblock_Thumb_Right_Y_Down.Text = PercentageRightYDown.ToString();
                    textblock_Thumb_Right_X_Left.Text = PercentageRightXLeft.ToString();
                    textblock_Thumb_Right_X_Right.Text = PercentageRightXRight.ToString();
                });
            }
            catch { }
        }
    }
}