using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update the controller status icons
        async Task UpdateControllerConnected()
        {
            try
            {
                //Check if process DirectXInput is running
                if (vProcessDirectXInput == null)
                {
                    //Debug.WriteLine("DirectXInput is not running, skipping controller check.");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                        grid_Main_Time.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Request controller status from DirectXInput
                await RequestControllerStatus();
            }
            catch { }
        }

        //Request the controller status from DirectXInput
        async Task RequestControllerStatus()
        {
            try
            {
                //Debug.WriteLine("Requesting controller status information.");

                //Check if socket server is running
                if (vArnoldVinkSockets == null)
                {
                    Debug.WriteLine("The socket server is not running.");
                    return;
                }

                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vArnoldVinkSockets.vTcpListenerIp;
                socketSend.SourcePort = vArnoldVinkSockets.vTcpListenerPort;
                socketSend.Object = "ControllerStatusSummaryList";

                //Request controller status
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient tcpClient = await vArnoldVinkSockets.TcpClientCheckCreateConnect(vArnoldVinkSockets.vTcpListenerIp, vArnoldVinkSockets.vTcpListenerPort + 1, vArnoldVinkSockets.vTcpClientTimeout);
                await vArnoldVinkSockets.TcpClientSendBytes(tcpClient, SerializedData, vArnoldVinkSockets.vTcpClientTimeout, false);
            }
            catch { }
        }

        //Update the controller status from DirectXInput
        async Task UpdateControllerStatus(List<ControllerStatusSummary> controllerStatusSummaryList)
        {
            try
            {
                //Update the controller status
                foreach (ControllerStatusSummary controllerStatusNew in controllerStatusSummaryList)
                {
                    //Get current controller status
                    Image controllerStatusImage = null;
                    ControllerStatusSummary controllerStatusOld = null;
                    if (controllerStatusNew.NumberId == 0)
                    {
                        controllerStatusImage = img_Menu_Controller0;
                        controllerStatusOld = vController0;
                    }
                    else if (controllerStatusNew.NumberId == 1)
                    {
                        controllerStatusImage = img_Menu_Controller1;
                        controllerStatusOld = vController1;
                    }
                    else if (controllerStatusNew.NumberId == 2)
                    {
                        controllerStatusImage = img_Menu_Controller2;
                        controllerStatusOld = vController2;
                    }
                    else if (controllerStatusNew.NumberId == 3)
                    {
                        controllerStatusImage = img_Menu_Controller3;
                        controllerStatusOld = vController3;
                    }

                    //Check if the controller is manage controller
                    if (controllerStatusNew.Manage && vControllerActiveId != controllerStatusNew.NumberId)
                    {
                        ActivateController(controllerStatusNew.NumberId);
                    }

                    //Update the battery icons and level
                    if (controllerStatusNew.Manage)
                    {
                        UpdateBatteryStatus(controllerStatusNew.BatteryPercentageCurrent);
                    }

                    //Update the controller status for comparison
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

                    //Show controller connection popup and update the controller menu image
                    if (controllerStatusOld.Connected != controllerStatusNew.Connected)
                    {
                        if (controllerStatusNew.Connected)
                        {
                            AVActions.ActionDispatcherInvoke(delegate { controllerStatusImage.Opacity = 1.00; });
                            string ControllerIdDisplay = Convert.ToString(controllerStatusNew.NumberId + 1);
                            Popup_Show_Status("Controller", "Connected (" + ControllerIdDisplay + ")");
                            //Hide the mouse cursor
                            await MouseCursorHide();
                        }
                        else
                        {
                            AVActions.ActionDispatcherInvoke(delegate { controllerStatusImage.Opacity = 0.40; });
                            string ControllerIdDisplay = Convert.ToString(controllerStatusNew.NumberId + 1);
                            Popup_Show_Status("Controller", "Disconnected (" + ControllerIdDisplay + ")");
                            if (vControllerActiveId == controllerStatusNew.NumberId) { HideBatteryStatus(true); }
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
                if (ForceHide || ConfigurationManager.AppSettings["HideBatteryLevel"] == "True")
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                        grid_Main_Time.Visibility = Visibility.Collapsed;
                    });
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Update the battery icons and level
        void UpdateBatteryStatus(int batteryPercentageCurrent)
        {
            try
            {
                //Check if the battery setting is enabled
                if (HideBatteryStatus(false)) { return; }
                //Debug.WriteLine("Updating battery level of controller: " + ControllerId);

                //Check if battery level is available
                if (batteryPercentageCurrent == -1)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                        grid_Main_Time.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Check if battery is charging
                if (batteryPercentageCurrent == -2)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerCharge.png" }, IntPtr.Zero, -1, 0);
                        img_Main_Battery.Visibility = Visibility.Visible;
                        grid_Main_Time.Visibility = Visibility.Visible;
                    });
                    return;
                }

                //Check the battery percentage
                string percentageNumber = "100";
                if (batteryPercentageCurrent <= 10) { percentageNumber = "10"; }
                else if (batteryPercentageCurrent <= 20) { percentageNumber = "20"; }
                else if (batteryPercentageCurrent <= 30) { percentageNumber = "30"; }
                else if (batteryPercentageCurrent <= 40) { percentageNumber = "40"; }
                else if (batteryPercentageCurrent <= 50) { percentageNumber = "50"; }
                else if (batteryPercentageCurrent <= 60) { percentageNumber = "60"; }
                else if (batteryPercentageCurrent <= 70) { percentageNumber = "70"; }
                else if (batteryPercentageCurrent <= 80) { percentageNumber = "80"; }
                else if (batteryPercentageCurrent <= 90) { percentageNumber = "90"; }

                //Set the battery percentage
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Set the used battery percentage text
                    txt_Main_Battery.Text = Convert.ToString(batteryPercentageCurrent) + "%";

                    //Set the used battery status icon
                    string currentImage = img_Main_Battery.Source.ToString();
                    string updatedImage = "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis" + percentageNumber + ".png";
                    if (currentImage.ToLower() != updatedImage.ToLower())
                    {
                        img_Main_Battery.Source = FileToBitmapImage(new string[] { updatedImage }, IntPtr.Zero, -1, 0);
                    }

                    //Show the battery image and clock
                    txt_Main_Battery.Visibility = Visibility.Visible;
                    img_Main_Battery.Visibility = Visibility.Visible;
                    grid_Main_Time.Visibility = Visibility.Visible;
                });
            }
            catch
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    txt_Main_Battery.Visibility = Visibility.Collapsed;
                    img_Main_Battery.Visibility = Visibility.Collapsed;
                    grid_Main_Time.Visibility = Visibility.Collapsed;
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
                    Popup_Show_Status("Controller", "Activated (1)");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller-Accent.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                    });
                }
                else if (controllerId == 1)
                {
                    vControllerActiveId = controllerId;
                    Popup_Show_Status("Controller", "Activated (2)");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller-Accent.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                    });
                }
                else if (controllerId == 2)
                {
                    vControllerActiveId = controllerId;
                    Popup_Show_Status("Controller", "Activated (3)");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller-Accent.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                    });
                }
                else if (controllerId == 3)
                {
                    vControllerActiveId = controllerId;
                    Popup_Show_Status("Controller", "Activated (4)");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1, 0);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller-Accent.png" }, IntPtr.Zero, -1, 0);
                    });
                }
            }
            catch { }
        }
    }
}