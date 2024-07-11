using ArnoldVinkCode;
using System;
using System.Windows;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Update the controller interface settings
        public void ControllerUpdateSettingsInterface(ControllerStatus Controller)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Enable controller tab
                    grid_Controller.IsEnabled = true;

                    //Check if controller supports rumble mode
                    if (Controller.SupportedCurrent.HasRumbleMode)
                    {
                        stackpanel_ControllerRumbleMode.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_ControllerRumbleMode.Visibility = Visibility.Collapsed;
                    }

                    //Check if controller supports rumble trigger
                    if (Controller.SupportedCurrent.HasRumbleTrigger)
                    {
                        stackpanel_TriggerRumbleSettings.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_TriggerRumbleSettings.Visibility = Visibility.Collapsed;
                    }

                    //Check if controller supports led status
                    if (Controller.SupportedCurrent.HasLedStatus)
                    {
                        stackpanel_StatusLed.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_StatusLed.Visibility = Visibility.Collapsed;
                    }

                    //Check if controller supports led player
                    if (Controller.SupportedCurrent.HasLedPlayer)
                    {
                        stackpanel_PlayerLed.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_PlayerLed.Visibility = Visibility.Collapsed;
                    }

                    //Check if controller supports led media
                    if (Controller.SupportedCurrent.HasLedMedia)
                    {
                        stackpanel_MediaLed.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stackpanel_MediaLed.Visibility = Visibility.Collapsed;
                    }

                    cb_ControllerUseButtonTriggers.IsChecked = Controller.Details.Profile.UseButtonTriggers;
                    textblock_ControllerDeadzoneTriggerLeft.Text = textblock_ControllerDeadzoneTriggerLeft.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneTriggerLeft) + "%";
                    slider_ControllerDeadzoneTriggerLeft.Value = Controller.Details.Profile.DeadzoneTriggerLeft;
                    textblock_ControllerDeadzoneTriggerRight.Text = textblock_ControllerDeadzoneTriggerRight.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneTriggerRight) + "%";
                    slider_ControllerDeadzoneTriggerRight.Value = Controller.Details.Profile.DeadzoneTriggerRight;
                    textblock_ControllerSensitivityTriggerLeft.Text = textblock_ControllerSensitivityTriggerLeft.Tag.ToString() + Controller.Details.Profile.SensitivityTriggerLeft.ToString("0.00");
                    slider_ControllerSensitivityTriggerLeft.Value = Controller.Details.Profile.SensitivityTriggerLeft;
                    textblock_ControllerSensitivityTriggerRight.Text = textblock_ControllerSensitivityTriggerRight.Tag.ToString() + Controller.Details.Profile.SensitivityTriggerRight.ToString("0.00");
                    slider_ControllerSensitivityTriggerRight.Value = Controller.Details.Profile.SensitivityTriggerRight;

                    cb_ControllerDPadFourWayMovement.IsChecked = Controller.Details.Profile.DPadFourWayMovement;

                    cb_ControllerThumbFlipMovement.IsChecked = Controller.Details.Profile.ThumbFlipMovement;
                    cb_ControllerThumbFlipAxesLeft.IsChecked = Controller.Details.Profile.ThumbFlipAxesLeft;
                    cb_ControllerThumbFlipAxesRight.IsChecked = Controller.Details.Profile.ThumbFlipAxesRight;
                    cb_ControllerThumbReverseAxesLeft.IsChecked = Controller.Details.Profile.ThumbReverseAxesLeft;
                    cb_ControllerThumbReverseAxesRight.IsChecked = Controller.Details.Profile.ThumbReverseAxesRight;

                    //Thumb deadzone
                    textblock_ControllerDeadzoneThumbLeft.Text = textblock_ControllerDeadzoneThumbLeft.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneThumbLeft) + "%";
                    slider_ControllerDeadzoneThumbLeft.Value = Controller.Details.Profile.DeadzoneThumbLeft;
                    textblock_ControllerDeadzoneThumbRight.Text = textblock_ControllerDeadzoneThumbRight.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.DeadzoneThumbRight) + "%";
                    slider_ControllerDeadzoneThumbRight.Value = Controller.Details.Profile.DeadzoneThumbRight;

                    //Thumb sensitivity
                    textblock_ControllerSensitivityThumbLeft.Text = textblock_ControllerSensitivityThumbLeft.Tag.ToString() + Controller.Details.Profile.SensitivityThumbLeft.ToString("0.00");
                    slider_ControllerSensitivityThumbLeft.Value = Controller.Details.Profile.SensitivityThumbLeft;
                    textblock_ControllerSensitivityThumbRight.Text = textblock_ControllerSensitivityThumbRight.Tag.ToString() + Controller.Details.Profile.SensitivityThumbRight.ToString("0.00");
                    slider_ControllerSensitivityThumbRight.Value = Controller.Details.Profile.SensitivityThumbRight;

                    cb_ControllerRumbleEnabled.IsChecked = Controller.Details.Profile.ControllerRumbleEnabled;
                    combobox_ControllerRumbleMode.SelectedIndex = Controller.Details.Profile.ControllerRumbleMode;
                    if (Controller.Details.Profile.ControllerRumbleEnabled)
                    {
                        combobox_ControllerRumbleMode.IsEnabled = true;
                        slider_ControllerRumbleStrength.IsEnabled = true;
                        slider_ControllerRumbleLimit.IsEnabled = true;
                    }
                    else
                    {
                        combobox_ControllerRumbleMode.IsEnabled = false;
                        slider_ControllerRumbleStrength.IsEnabled = false;
                        slider_ControllerRumbleLimit.IsEnabled = false;
                    }

                    textblock_ControllerRumbleLimit.Text = textblock_ControllerRumbleLimit.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.ControllerRumbleLimit) + "%";
                    slider_ControllerRumbleLimit.Value = Controller.Details.Profile.ControllerRumbleLimit;

                    textblock_ControllerRumbleStrength.Text = textblock_ControllerRumbleStrength.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.ControllerRumbleStrength) + "%";
                    slider_ControllerRumbleStrength.Value = Controller.Details.Profile.ControllerRumbleStrength;

                    cb_TriggerRumbleEnabled.IsChecked = Controller.Details.Profile.TriggerRumbleEnabled;
                    if (Controller.Details.Profile.TriggerRumbleEnabled)
                    {
                        slider_TriggerRumbleStrengthLeft.IsEnabled = true;
                        slider_TriggerRumbleStrengthRight.IsEnabled = true;
                        slider_TriggerRumbleLimit.IsEnabled = true;
                    }
                    else
                    {
                        slider_TriggerRumbleStrengthLeft.IsEnabled = false;
                        slider_TriggerRumbleStrengthRight.IsEnabled = false;
                        slider_TriggerRumbleLimit.IsEnabled = false;
                    }

                    textblock_TriggerRumbleLimit.Text = textblock_TriggerRumbleLimit.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.TriggerRumbleLimit) + "%";
                    slider_TriggerRumbleLimit.Value = Controller.Details.Profile.TriggerRumbleLimit;

                    textblock_TriggerRumbleStrengthLeft.Text = textblock_TriggerRumbleStrengthLeft.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.TriggerRumbleStrengthLeft) + "%";
                    slider_TriggerRumbleStrengthLeft.Value = Controller.Details.Profile.TriggerRumbleStrengthLeft;

                    textblock_TriggerRumbleStrengthRight.Text = textblock_TriggerRumbleStrengthRight.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.TriggerRumbleStrengthRight) + "%";
                    slider_TriggerRumbleStrengthRight.Value = Controller.Details.Profile.TriggerRumbleStrengthRight;

                    textblock_ControllerLedBrightness.Text = textblock_ControllerLedBrightness.Tag.ToString() + Convert.ToInt32(Controller.Details.Profile.LedBrightness) + "%";
                    slider_ControllerLedBrightness.Value = Controller.Details.Profile.LedBrightness;

                    cb_PlayerLedEnabled.IsChecked = Controller.Details.Profile.PlayerLedEnabled;
                });
            }
            catch { }
        }
    }
}