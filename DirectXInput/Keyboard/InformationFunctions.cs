﻿using ArnoldVinkCode;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Update the user interface clock style
        public void UpdateClockStyle()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    string clockStyle = SettingLoad(AppVariables.vConfigurationCtrlUI, "InterfaceClockStyleName", typeof(string));
                    string clockPath = "Assets/Default/Clocks/" + clockStyle;
                    if (Directory.Exists("Assets/User/Clocks/" + clockStyle))
                    {
                        clockPath = "Assets/User/Clocks/" + clockStyle;
                    }

                    img_Main_Time_Face.Source = FileToBitmapImage(new string[] { clockPath + "/Face.png" }, null, vImageBackupSource, 40, 0, IntPtr.Zero, 0);
                    img_Main_Time_Hour.Source = FileToBitmapImage(new string[] { clockPath + "/Hour.png" }, null, vImageBackupSource, 40, 0, IntPtr.Zero, 0);
                    img_Main_Time_Minute.Source = FileToBitmapImage(new string[] { clockPath + "/Minute.png" }, null, vImageBackupSource, 40, 0, IntPtr.Zero, 0);
                    img_Main_Time_Center.Source = FileToBitmapImage(new string[] { clockPath + "/Center.png" }, null, vImageBackupSource, 40, 0, IntPtr.Zero, 0);
                });
            }
            catch { }
        }

        //Update the user interface clock time
        void UpdateClockTime()
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    //Rotate the clock images
                    int clockSecond = DateTime.Now.Second;
                    int clockMinute = DateTime.Now.Minute;
                    int clockHour = DateTime.Now.Hour;
                    img_Main_Time_Minute.LayoutTransform = new RotateTransform((clockMinute * 360 / 60) + (clockSecond / 60 * 6));
                    img_Main_Time_Hour.LayoutTransform = new RotateTransform((clockHour * 360 / 12) + (clockMinute / 2));

                    //Change the time format
                    txt_Main_Time.Text = DateTime.Now.ToShortTimeString();
                });
            }
            catch { }
        }

        //Update the active controller
        void UpdateActiveController()
        {
            try
            {
                //Debug.WriteLine("Updating active controller.");
                ControllerStatus activeController = AppVariables.vActiveController();
                if (activeController == null)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        stackpanel_ControllerActive.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                AVActions.DispatcherInvoke(delegate
                {
                    stackpanel_ControllerActive.Visibility = Visibility.Visible;
                    border_ControllerActive.Background = new SolidColorBrush((Color)activeController.Color);
                });
            }
            catch { }
        }

        //Update the battery icons and level
        void UpdateBatteryStatus()
        {
            try
            {
                //Debug.WriteLine("Updating battery level of controller.");
                ControllerStatus activeController = AppVariables.vActiveController();
                if (activeController == null)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                        grid_Main_Time.Visibility = Visibility.Collapsed;
                    });
                    return;
                }
                ControllerBattery controllerBattery = activeController.BatteryCurrent;

                //Check if battery level is available
                if (controllerBattery.BatteryStatus == BatteryStatus.Unknown)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                        grid_Main_Time.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Check if battery is charging
                if (controllerBattery.BatteryStatus == BatteryStatus.Charging)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;

                        //Set the used battery status icon
                        string currentImage = string.Empty;
                        if (img_Main_Battery.Source != null)
                        {
                            currentImage = img_Main_Battery.Source.ToString();
                        }
                        string updatedImage = "Assets/Default/Icons/Battery/BatteryVerCharge.png";
                        if (currentImage.ToLower() != updatedImage.ToLower())
                        {
                            img_Main_Battery.Source = FileToBitmapImage(new string[] { updatedImage }, null, vImageBackupSource, 0, 0, IntPtr.Zero, 0);
                        }

                        img_Main_Battery.Visibility = Visibility.Visible;
                        grid_Main_Time.Visibility = Visibility.Visible;
                    });
                    return;
                }

                //Check the battery percentage
                string percentageNumber = "100";
                if (controllerBattery.BatteryPercentage <= 10) { percentageNumber = "10"; }
                else if (controllerBattery.BatteryPercentage <= 20) { percentageNumber = "20"; }
                else if (controllerBattery.BatteryPercentage <= 30) { percentageNumber = "30"; }
                else if (controllerBattery.BatteryPercentage <= 40) { percentageNumber = "40"; }
                else if (controllerBattery.BatteryPercentage <= 50) { percentageNumber = "50"; }
                else if (controllerBattery.BatteryPercentage <= 60) { percentageNumber = "60"; }
                else if (controllerBattery.BatteryPercentage <= 70) { percentageNumber = "70"; }
                else if (controllerBattery.BatteryPercentage <= 80) { percentageNumber = "80"; }
                else if (controllerBattery.BatteryPercentage <= 90) { percentageNumber = "90"; }

                //Set the battery percentage
                AVActions.DispatcherInvoke(delegate
                {
                    //Set the used battery percentage text
                    txt_Main_Battery.Text = Convert.ToString(controllerBattery.BatteryPercentage) + "%";

                    //Set the used battery status icon
                    string currentImage = string.Empty;
                    if (img_Main_Battery.Source != null)
                    {
                        currentImage = img_Main_Battery.Source.ToString();
                    }
                    string updatedImage = "Assets/Default/Icons/Battery/BatteryVerDis" + percentageNumber + ".png";
                    if (currentImage.ToLower() != updatedImage.ToLower())
                    {
                        img_Main_Battery.Source = FileToBitmapImage(new string[] { updatedImage }, null, vImageBackupSource, 0, 0, IntPtr.Zero, 0);
                    }

                    //Show the battery image and clock
                    txt_Main_Battery.Visibility = Visibility.Visible;
                    img_Main_Battery.Visibility = Visibility.Visible;
                    grid_Main_Time.Visibility = Visibility.Visible;
                });
            }
            catch
            {
                AVActions.DispatcherInvoke(delegate
                {
                    txt_Main_Battery.Visibility = Visibility.Collapsed;
                    img_Main_Battery.Visibility = Visibility.Collapsed;
                    grid_Main_Time.Visibility = Visibility.Collapsed;
                });
            }
        }
    }
}