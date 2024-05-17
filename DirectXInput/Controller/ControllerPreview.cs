﻿using ArnoldVinkCode;
using System.Windows;
using System.Windows.Media;
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
        void UpdateControllerPreviewInformation(ControllerStatus Controller)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //DPad
                    if (Controller.InputCurrent.DPadLeft.PressedRaw) { img_ControllerPreview_DPadLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadLeft.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.DPadUp.PressedRaw) { img_ControllerPreview_DPadUp.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadUp.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.DPadRight.PressedRaw) { img_ControllerPreview_DPadRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadRight.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.DPadDown.PressedRaw) { img_ControllerPreview_DPadDown.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadDown.Visibility = Visibility.Collapsed; }

                    //Buttons
                    if (Controller.InputCurrent.ButtonA.PressedRaw) { img_ControllerPreview_ButtonA.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonA.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonB.PressedRaw) { img_ControllerPreview_ButtonB.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonB.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonX.PressedRaw) { img_ControllerPreview_ButtonX.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonX.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonY.PressedRaw) { img_ControllerPreview_ButtonY.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonY.Visibility = Visibility.Collapsed; }

                    if (Controller.InputCurrent.ButtonBack.PressedRaw) { img_ControllerPreview_ButtonBack.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonBack.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonStart.PressedRaw) { img_ControllerPreview_ButtonStart.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonStart.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonGuide.PressedRaw) { img_ControllerPreview_ButtonGuide.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonGuide.Visibility = Visibility.Collapsed; }

                    if (Controller.InputCurrent.ButtonOne.PressedRaw) { img_ControllerPreview_ButtonOne.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonOne.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonTwo.PressedRaw) { img_ControllerPreview_ButtonTwo.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonTwo.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonThree.PressedRaw) { img_ControllerPreview_ButtonThree.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonThree.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonFour.PressedRaw) { img_ControllerPreview_ButtonFour.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonFour.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonFive.PressedRaw) { img_ControllerPreview_ButtonFive.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonFive.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonSix.PressedRaw) { img_ControllerPreview_ButtonSix.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonSix.Visibility = Visibility.Collapsed; }

                    if (Controller.InputCurrent.ButtonShoulderLeft.PressedRaw) { img_ControllerPreview_ButtonShoulderLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonShoulderLeft.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonShoulderRight.PressedRaw) { img_ControllerPreview_ButtonShoulderRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonShoulderRight.Visibility = Visibility.Collapsed; }

                    if (Controller.InputCurrent.ButtonThumbLeft.PressedRaw) { img_ControllerPreview_ButtonThumbLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonThumbLeft.Visibility = Visibility.Collapsed; }
                    if (Controller.InputCurrent.ButtonThumbRight.PressedRaw) { img_ControllerPreview_ButtonThumbRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonThumbRight.Visibility = Visibility.Collapsed; }

                    //Triggers
                    if (!Controller.Details.Profile.UseButtonTriggers)
                    {
                        textblock_Trigger_Left.Text = Controller.InputCurrent.TriggerLeft.ToString();
                        textblock_Trigger_Right.Text = Controller.InputCurrent.TriggerRight.ToString();
                        img_ControllerPreview_TriggerLeft.Opacity = (double)(Controller.InputCurrent.TriggerLeft * 257) / 65535;
                        img_ControllerPreview_TriggerRight.Opacity = (double)(Controller.InputCurrent.TriggerRight * 257) / 65535;
                    }
                    else
                    {
                        if (Controller.InputCurrent.ButtonTriggerLeft.PressedRaw)
                        {
                            textblock_Trigger_Left.Text = "255";
                            img_ControllerPreview_TriggerLeft.Opacity = 1.00;
                        }
                        else
                        {
                            textblock_Trigger_Left.Text = "0";
                            img_ControllerPreview_TriggerLeft.Opacity = 0.00;
                        }
                        if (Controller.InputCurrent.ButtonTriggerRight.PressedRaw)
                        {
                            textblock_Trigger_Right.Text = "255";
                            img_ControllerPreview_TriggerRight.Opacity = 1.00;
                        }
                        else
                        {
                            textblock_Trigger_Right.Text = "0";
                            img_ControllerPreview_TriggerRight.Opacity = 0.00;
                        }
                    }

                    //Thumb Left and Right Image
                    int LeftX = Controller.InputCurrent.ThumbLeftX / 1000;
                    int LeftY = -Controller.InputCurrent.ThumbLeftY / 1000;
                    int RightX = Controller.InputCurrent.ThumbRightX / 1000;
                    int RightY = -Controller.InputCurrent.ThumbRightY / 1000;
                    img_ControllerPreview_LeftAxe.Margin = new Thickness(LeftX, LeftY, 0, 0);
                    img_ControllerPreview_RightAxe.Margin = new Thickness(RightX, RightY, 0, 0);

                    //Thumb Left and Right Percentage
                    int PercentageLeftYUp = (Controller.InputCurrent.ThumbLeftY * 257) / 65535;
                    if (PercentageLeftYUp < 0) { PercentageLeftYUp = 0; }
                    int PercentageLeftYDown = (-Controller.InputCurrent.ThumbLeftY * 257) / 65535;
                    if (PercentageLeftYDown < 0) { PercentageLeftYDown = 0; }
                    int PercentageLeftXLeft = (-Controller.InputCurrent.ThumbLeftX * 257) / 65535;
                    if (PercentageLeftXLeft < 0) { PercentageLeftXLeft = 0; }
                    int PercentageLeftXRight = (Controller.InputCurrent.ThumbLeftX * 257) / 65535;
                    if (PercentageLeftXRight < 0) { PercentageLeftXRight = 0; }
                    textblock_Thumb_Left_Y_Up.Text = PercentageLeftYUp.ToString();
                    textblock_Thumb_Left_Y_Down.Text = PercentageLeftYDown.ToString();
                    textblock_Thumb_Left_X_Left.Text = PercentageLeftXLeft.ToString();
                    textblock_Thumb_Left_X_Right.Text = PercentageLeftXRight.ToString();
                    int PercentageRightYUp = (Controller.InputCurrent.ThumbRightY * 257) / 65535;
                    if (PercentageRightYUp < 0) { PercentageRightYUp = 0; }
                    int PercentageRightYDown = (-Controller.InputCurrent.ThumbRightY * 257) / 65535;
                    if (PercentageRightYDown < 0) { PercentageRightYDown = 0; }
                    int PercentageRightXLeft = (-Controller.InputCurrent.ThumbRightX * 257) / 65535;
                    if (PercentageRightXLeft < 0) { PercentageRightXLeft = 0; }
                    int PercentageRightXRight = (Controller.InputCurrent.ThumbRightX * 257) / 65535;
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