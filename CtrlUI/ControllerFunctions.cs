﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVSettings;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update the controller status icons
        async Task UpdateControllerConnected()
        {
            try
            {
                //Check if application is activated
                if (!vAppActivated)
                {
                    return;
                }

                //Check if process DirectXInput is running
                if (vProcessDirectXInput == null)
                {
                    //Debug.WriteLine("DirectXInput is not running, skipping controller check.");
                    DispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Request controller status from DirectXInput
                await RequestControllerStatus();
            }
            catch { }
        }

        //Update the controller color
        void UpdateControllerColor()
        {
            try
            {
                DispatcherInvoke(delegate
                {
                    SolidColorBrush ControllerColor0Brush = new BrushConverter().ConvertFrom(vController0.Color.ToString()) as SolidColorBrush;
                    border_Menu_Controller0.Background = ControllerColor0Brush;

                    SolidColorBrush ControllerColor1Brush = new BrushConverter().ConvertFrom(vController1.Color.ToString()) as SolidColorBrush;
                    border_Menu_Controller1.Background = ControllerColor1Brush;

                    SolidColorBrush ControllerColor2Brush = new BrushConverter().ConvertFrom(vController2.Color.ToString()) as SolidColorBrush;
                    border_Menu_Controller2.Background = ControllerColor2Brush;

                    SolidColorBrush ControllerColor3Brush = new BrushConverter().ConvertFrom(vController3.Color.ToString()) as SolidColorBrush;
                    border_Menu_Controller3.Background = ControllerColor3Brush;
                });
            }
            catch { }
        }

        //Request the controller status from DirectXInput
        async Task RequestControllerStatus()
        {
            try
            {
                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vSocketServerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vSocketServerPort;
                socketSend.SetObject("ControllerStatusSummaryList");

                //Request controller status
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(vArnoldVinkSockets.vSocketServerIp), 26760);
                await vArnoldVinkSockets.UdpClientSendBytesServer(ipEndPoint, SerializedData, vArnoldVinkSockets.vSocketTimeout);
            }
            catch { }
        }

        //Update the controller status from DirectXInput
        void UpdateControllerStatus(List<ControllerStatusDetails> controllerStatusSummaryList)
        {
            try
            {
                //Update the controller status
                foreach (ControllerStatusDetails controllerStatusNew in controllerStatusSummaryList)
                {
                    //Get current controller status
                    Image controllerStatusImage = null;
                    StackPanel controllerStatusStackpanel = null;
                    ControllerStatusDetails controllerStatusOld = null;
                    if (controllerStatusNew.NumberId == 0)
                    {
                        controllerStatusStackpanel = stackpanel_Menu_Controller0;
                        controllerStatusImage = img_Menu_Controller0;
                        controllerStatusOld = vController0;
                    }
                    else if (controllerStatusNew.NumberId == 1)
                    {
                        controllerStatusStackpanel = stackpanel_Menu_Controller1;
                        controllerStatusImage = img_Menu_Controller1;
                        controllerStatusOld = vController1;
                    }
                    else if (controllerStatusNew.NumberId == 2)
                    {
                        controllerStatusStackpanel = stackpanel_Menu_Controller2;
                        controllerStatusImage = img_Menu_Controller2;
                        controllerStatusOld = vController2;
                    }
                    else if (controllerStatusNew.NumberId == 3)
                    {
                        controllerStatusStackpanel = stackpanel_Menu_Controller3;
                        controllerStatusImage = img_Menu_Controller3;
                        controllerStatusOld = vController3;
                    }

                    //Check if controller is active controller
                    if (controllerStatusNew.Activated && vControllerActiveId != controllerStatusNew.NumberId)
                    {
                        ActivateController(controllerStatusNew.NumberId);
                    }

                    //Update battery icons and level
                    if (controllerStatusNew.Activated)
                    {
                        UpdateBatteryStatus(controllerStatusNew.BatteryCurrent);
                    }

                    //Update controller status for comparison
                    if (controllerStatusNew.NumberId == 0)
                    {
                        vController0 = controllerStatusNew;
                    }
                    else if (controllerStatusNew.NumberId == 1)
                    {
                        vController1 = controllerStatusNew;
                    }
                    else if (controllerStatusNew.NumberId == 2)
                    {
                        vController2 = controllerStatusNew;
                    }
                    else if (controllerStatusNew.NumberId == 3)
                    {
                        vController3 = controllerStatusNew;
                    }

                    //Update controller help
                    UpdateControllerHelp();

                    //Update controller colors
                    UpdateControllerColor();

                    //Show controller connection popup and update the controller menu image
                    if (controllerStatusOld.Connected != controllerStatusNew.Connected)
                    {
                        if (controllerStatusNew.Connected)
                        {
                            DispatcherInvoke(delegate { controllerStatusStackpanel.Opacity = 1.00; });
                            string ControllerIdDisplay = controllerStatusNew.NumberDisplay().ToString();
                        }
                        else
                        {
                            DispatcherInvoke(delegate { controllerStatusStackpanel.Opacity = 0.40; });
                            string ControllerIdDisplay = controllerStatusNew.NumberDisplay().ToString();

                            //Hide the battery status
                            if (vControllerActiveId == controllerStatusNew.NumberId)
                            {
                                HideBatteryStatus(true);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        //Check if battery setting is enabled
        bool HideBatteryStatus(bool ForceHide)
        {
            try
            {
                if (ForceHide || SettingLoad(vConfigurationCtrlUI, "HideBatteryLevel", typeof(bool)))
                {
                    DispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                    });
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Update the battery icons and level
        void UpdateBatteryStatus(ControllerBattery controllerBattery)
        {
            try
            {
                //Check if the battery setting is enabled
                if (HideBatteryStatus(false)) { return; }
                //Debug.WriteLine("Updating battery level of controller.");

                //Check if battery level is available
                if (controllerBattery.BatteryStatus == BatteryStatus.Unknown)
                {
                    DispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Check if battery is charging
                if (controllerBattery.BatteryStatus == BatteryStatus.Charging)
                {
                    DispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Battery/BatteryVerCharge.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Main_Battery.Visibility = Visibility.Visible;
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
                DispatcherInvoke(delegate
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
                        img_Main_Battery.Source = FileToBitmapImage(new string[] { updatedImage }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    }

                    //Show the battery image and clock
                    txt_Main_Battery.Visibility = Visibility.Visible;
                    img_Main_Battery.Visibility = Visibility.Visible;
                });
            }
            catch
            {
                DispatcherInvoke(delegate
                {
                    txt_Main_Battery.Visibility = Visibility.Collapsed;
                    img_Main_Battery.Visibility = Visibility.Collapsed;
                });
            }
        }

        //Set a controller as the active controller
        void ActivateController(int controllerId)
        {
            try
            {
                if (controllerId == 0)
                {
                    vControllerActiveId = controllerId;
                    DispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller-Accent.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    });
                }
                else if (controllerId == 1)
                {
                    vControllerActiveId = controllerId;
                    DispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller-Accent.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    });
                }
                else if (controllerId == 2)
                {
                    vControllerActiveId = controllerId;
                    DispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller-Accent.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    });
                }
                else if (controllerId == 3)
                {
                    vControllerActiveId = controllerId;
                    DispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "Assets/Default/Icons/Controller-Accent.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    });
                }
            }
            catch { }
        }
    }
}