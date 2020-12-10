using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVClassConverters;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Read controller battery level
        void ControllerReadBatteryLevel(ControllerStatus Controller)
        {
            try
            {
                //Check which controller is connected
                if (Controller.SupportedCurrent.CodeName == "SonyDualSense5" && Controller.Details.Wireless)
                {
                    //Bluetooth - DualSense 5
                    int batteryLevelOffset = 52 + Controller.InputHeaderOffsetByte + Controller.InputButtonOffsetByte;
                    byte batteryLevelReport = Controller.InputReport[batteryLevelOffset];
                    int batteryStatusOffset = 53 + Controller.InputHeaderOffsetByte + Controller.InputButtonOffsetByte;
                    byte batteryStatusReport = Controller.InputReport[batteryStatusOffset];

                    bool batteryCharging = batteryStatusReport != 0;
                    if (batteryCharging)
                    {
                        Controller.BatteryCurrent.BatteryPercentage = -1;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                    }
                    else
                    {
                        int batteryPercentage = TranslateByte_0x0F(0, batteryLevelReport) * 10 + 10;
                        if (batteryPercentage > 100) { batteryPercentage = 100; }
                        Controller.BatteryCurrent.BatteryPercentage = batteryPercentage;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Normal;
                    }
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualSense5" && !Controller.Details.Wireless)
                {
                    //Wired USB - DualSense 5
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock4" && Controller.Details.Wireless)
                {
                    //Bluetooth - DualShock 4
                    int batteryOffset = 29 + Controller.InputHeaderOffsetByte + Controller.InputButtonOffsetByte;
                    byte batteryReport = Controller.InputReport[batteryOffset];

                    bool batteryCharging = TranslateByte_0x10(0, batteryReport) != 0;
                    if (batteryCharging)
                    {
                        Controller.BatteryCurrent.BatteryPercentage = -1;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                    }
                    else
                    {
                        int batteryPercentage = TranslateByte_0x0F(0, batteryReport) * 10 + 10;
                        if (batteryPercentage > 100) { batteryPercentage = 100; }
                        Controller.BatteryCurrent.BatteryPercentage = batteryPercentage;
                        Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Normal;
                    }
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock4" && !Controller.Details.Wireless)
                {
                    //Wired USB - DualShock 4
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                }
                else if (Controller.SupportedCurrent.CodeName == "SonyDualShock3" && !Controller.Details.Wireless)
                {
                    //Wired USB - DualShock 3
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Charging;
                }
                else
                {
                    //Unknown controller
                    Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Unknown;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to read the battery level: " + ex.Message);
                Controller.BatteryCurrent.BatteryStatus = BatteryStatus.Unknown;
            }
        }

        //Check controller for low battery level
        void ControllerLowBattery(ControllerStatus Controller)
        {
            try
            {
                //Debug.WriteLine("Checking if controller " + Controller.NumberId + " has a low battery level " + Controller.BatteryPercentageCurrent + "/" + Controller.BatteryPercentagePrevious);
                string controllerNumberDisplay = (Controller.NumberId + 1).ToString();

                //Check the controller id
                TextBlock targetControllerTextblock = null;
                StackPanel targetControllerStackpanel = null;
                if (Controller.NumberId == 0)
                {
                    targetControllerTextblock = App.vWindowOverlay.textblock_Battery_Warning_Controller1;
                    targetControllerStackpanel = App.vWindowOverlay.stackpanel_Battery_Warning_Controller1;
                }
                else if (Controller.NumberId == 1)
                {
                    targetControllerTextblock = App.vWindowOverlay.textblock_Battery_Warning_Controller2;
                    targetControllerStackpanel = App.vWindowOverlay.stackpanel_Battery_Warning_Controller2;
                }
                else if (Controller.NumberId == 2)
                {
                    targetControllerTextblock = App.vWindowOverlay.textblock_Battery_Warning_Controller3;
                    targetControllerStackpanel = App.vWindowOverlay.stackpanel_Battery_Warning_Controller3;
                }
                else if (Controller.NumberId == 3)
                {
                    targetControllerTextblock = App.vWindowOverlay.textblock_Battery_Warning_Controller4;
                    targetControllerStackpanel = App.vWindowOverlay.stackpanel_Battery_Warning_Controller4;
                }

                //Check if the controller is connected
                if (Controller == null || !Controller.Connected())
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        targetControllerStackpanel.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Check the current battery level
                bool batteryShowIconLow = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryShowIconLow"));
                bool BatteryShowPercentageLow = Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryShowPercentageLow"));
                bool batteryLevelChanged = Controller.BatteryCurrent.BatteryPercentage != Controller.BatteryPrevious.BatteryPercentage || Controller.BatteryCurrent.BatteryStatus != Controller.BatteryPrevious.BatteryStatus;
                bool batteryLevelLow = Controller.BatteryCurrent.BatteryPercentage <= 20 && Controller.BatteryCurrent.BatteryStatus == BatteryStatus.Normal;

                //Update controller battery led
                if (batteryLevelChanged)
                {
                    Debug.WriteLine("Controller " + Controller.NumberId + " battery level changed, updating led.");
                    ControllerOutput(vController0, false, false);
                    ControllerOutput(vController1, false, false);
                    ControllerOutput(vController2, false, false);
                    ControllerOutput(vController3, false, false);
                }

                //Show or hide battery level overlay
                if (batteryLevelLow && batteryShowIconLow)
                {
                    //Debug.WriteLine("Controller " + Controller.NumberId + " has a low battery level, showing overlay.");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        App.vWindowOverlay.UpdateBatteryPosition();
                        if (BatteryShowPercentageLow)
                        {
                            targetControllerTextblock.Text = Controller.BatteryCurrent.BatteryPercentage + "%";
                            targetControllerTextblock.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            targetControllerTextblock.Visibility = Visibility.Collapsed;
                        }
                        targetControllerStackpanel.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        targetControllerStackpanel.Visibility = Visibility.Collapsed;
                    });
                }

                //Battery level sound and notification
                if (batteryLevelLow && batteryLevelChanged)
                {
                    Debug.WriteLine("Controller " + Controller.NumberId + " has a low battery level, showing notification.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Battery/BatteryVerDis20";
                    notificationDetails.Text = "Controller (" + controllerNumberDisplay + ") battery " + Controller.BatteryCurrent.BatteryPercentage + "%";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);

                    if (Convert.ToBoolean(Setting_Load(vConfigurationDirectXInput, "BatteryPlaySoundLow")))
                    {
                        PlayInterfaceSound(vConfigurationCtrlUI, "BatteryLow", true);
                    }
                }

                //Update the previous battery level
                CloneObjectShallow(Controller.BatteryCurrent, out Controller.BatteryPrevious);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed checking low battery level: " + ex.Message);
            }
        }

        //Check all controllers for low battery level
        void CheckAllControllersLowBattery()
        {
            try
            {
                ControllerLowBattery(vController0);
                ControllerLowBattery(vController1);
                ControllerLowBattery(vController2);
                ControllerLowBattery(vController3);
            }
            catch { }
        }
    }
}